<?php
/**
 * =====================================================================================================
 *  QahrApi — كلاس الموقع اللي بيكلّم سيرفر اللعبة
 *  QahrApi — the website's client for the game server's WebApi
 * =====================================================================================================
 *
 *  بيعمل 3 حاجات بس:
 *      sendCps($player, $amount, $ref)              يبعت CPs لللاعب
 *      giveItem($player, $item, $count, $ref, ...)  يبعت آيتم لللاعب
 *      inventory($player)                           يجيب شنطة اللاعب وعتاده
 *
 *  الكود المقابل في السيرفر:  Qahr6609.GameServer\Infrastructure\Threading\WebApi.cs
 *  إعدادات السيرفر:            ServerDataBase\webapi.ini   (مقفول لحد ما تكتب enabled=1)
 *
 *  -----------------------------------------------------------------------------------------------
 *  إزاي الطلب بيتأمّن (بالترتيب):
 *
 *   1) المفتاح السري (key) واحد عند الموقع وعند السيرفر، ومابيتبعتش أبداً على الشبكة.
 *      منه بنطلّع مفتاحين (HKDF-SHA256):
 *          encKey  = للتشفير
 *          codeKey = للرمز المتغير
 *
 *   2) الطلب نفسه (op / player / amount ...) بيتشفّر AES-256-GCM بـ encKey.
 *      اللي يشوف الطلب على الشبكة مش هيفهم منه حاجة، ولو غيّر فيه حرف السيرفر بيرفضه.
 *
 *   3) الرمز المتغير (code): بيتغير كل 30 ثانية.
 *          code = HMAC-SHA256(codeKey, رقم الـ 30 ثانية الحالية + رقم عشوائي) — أول 32 حرف
 *      اللي ماعندوش المفتاح مايقدرش يطلّعه، والسيرفر بيرفض أي طلب عمره أكتر من 30 ثانية.
 *
 *   4) رقم عشوائي (nonce) لكل طلب: السيرفر بيفتكره دقيقتين، فنفس الطلب لو اتبعت تاني بيترفض.
 *
 *   5) رقم الأوردر (ref): السيرفر مابينفّذش نفس الـ ref مرتين — يعني مفيش دفع مرتين.
 *
 *   6) الرد: السيرفر بيرد بنفس الطريقة (مشفّر + رمز). الكلاس بيتأكد من الرمز قبل ما يصدّق الرد،
 *      يعني محدش يقدر يعمل نفسه السيرفر.
 *
 *  شكل الطلب على الشبكة (سطر JSON واحد على TCP):
 *      {"v":1, "ts":وقت, "nonce":عشوائي, "iv":عشوائي, "ct":الطلب_مشفّر, "tag":ختم_التشفير, "code":الرمز_المتغير}
 *
 *  محتاج: PHP 7.1.2 أو أحدث (64-bit) + إضافة openssl.
 * =====================================================================================================
 */

// الملف ده مكتبة بس: لو اتفتح مباشرة من المتصفح يرجّع 404 / a library only: opened directly from a browser it answers 404
if (PHP_SAPI !== 'cli' && isset($_SERVER['SCRIPT_FILENAME']) && realpath($_SERVER['SCRIPT_FILENAME']) === __FILE__) { http_response_code(404); exit; }

class QahrApiException extends Exception {}

class QahrApi
{
    const PROTOCOL_VERSION = 1;
    const CODE_WINDOW_SECONDS = 30;   // الرمز بيتغير كل 30 ثانية

    private $host;      // عنوان سيرفر اللعبة
    private $port;      // البورت (5900)
    private $timeout;   // ثواني انتظار الاتصال والرد
    private $encKey;    // مفتاح التشفير (متطلّع من المفتاح السري)
    private $codeKey;   // مفتاح الرمز المتغير (متطلّع من المفتاح السري)

    /**
     * @param string $host     عنوان سيرفر اللعبة — 127.0.0.1 لو الموقع على نفس الجهاز
     * @param int    $port     نفس port= اللي في webapi.ini (افتراضي 5900)
     * @param string $secretKey نفس key= اللي في webapi.ini (base64 لـ 32 بايت عشوائي)
     * @param int    $timeout  ثواني
     */
    public function __construct($host, $port, $secretKey, $timeout = 10)
    {
        // التأكد إن PHP فيه اللي محتاجينه
        if (PHP_INT_SIZE < 8) {
            throw new QahrApiException('محتاج PHP 64-bit');
        }
        if (!function_exists('hash_hkdf') || !in_array('aes-256-gcm', openssl_get_cipher_methods(), true)) {
            throw new QahrApiException('محتاج PHP 7.1.2 أو أحدث مع إضافة openssl');
        }

        // فك المفتاح السري
        $key = base64_decode((string)$secretKey, true);
        if ($key === false || strlen($key) < 32) {
            throw new QahrApiException('المفتاح غلط: لازم يكون base64 لـ 32 بايت عشوائي (php gen_key.php)');
        }

        $this->host    = (string)$host;
        $this->port    = (int)$port;
        $this->timeout = max(1, (int)$timeout);

        // نطلّع من المفتاح السري مفتاحين منفصلين (نفس اللي السيرفر بيعمله بالظبط)
        $this->encKey  = hash_hkdf('sha256', $key, 32, 'qahr-webapi-enc', '');
        $this->codeKey = hash_hkdf('sha256', $key, 32, 'qahr-webapi-code', '');
    }

    // =================================================================================================
    //  العمليات التلاتة
    // =================================================================================================

    /**
     * يبعت CPs لللاعب (إضافة).
     * @param string|int $player اسم اللاعب أو الـ UID
     * @param int        $amount عدد الـ CPs
     * @param string     $ref    رقم الأوردر — لازم يكون مختلف لكل دفعة (السيرفر يرفض نفس الرقم مرتين)
     */
    public function sendCps($player, $amount, $ref)
    {
        return $this->sendRequest(array(
            'op'     => 'cps',
            'player' => self::checkPlayer($player),
            'mode'   => 'add',
            'amount' => self::checkInt($amount, 1, 2000000000, 'amount'),
            'ref'    => self::checkRef($ref),
        ));
    }

    /**
     * يبعت آيتم لللاعب. اللاعب لازم يكون أونلاين وفي شنطته مكان.
     * @param int  $item  رقم الآيتم
     * @param int  $count العدد
     * @param int  $plus  الـ + (0 لحد 12)
     * @param bool $bound مربوط ولا لأ
     */
    public function giveItem($player, $item, $count, $ref, $plus = 0, $bound = false)
    {
        return $this->sendRequest(array(
            'op'     => 'giveitem',
            'player' => self::checkPlayer($player),
            'item'   => self::checkInt($item, 1, 4294967295, 'item'),
            'count'  => self::checkInt($count, 1, 65535, 'count'),
            'plus'   => self::checkInt($plus, 0, 12, 'plus'),
            'bound'  => $bound === true || $bound === 1 || $bound === '1',
            'ref'    => self::checkRef($ref),
        ));
    }

    // =================================================================================================
    //  التحقق من المدخلات قبل الإرسال (السيرفر بيتحقق تاني، ده عشان الغلط يبان بدري وبرسالة واضحة)
    //  Input checks before sending (the server checks everything again; this only fails early and clearly)
    // =================================================================================================

    /** اسم اللاعب أو الـ UID: 1-32 حرف UTF-8، من غير أحرف تحكم/سطر جديد/| = [ ]  */
    public static function checkPlayer($player)
    {
        if (is_int($player)) $player = (string)$player;
        if (!is_string($player)) throw new QahrApiException('player: لازم يكون نص أو رقم');
        $player = trim($player);
        $len = preg_match('//u', $player) ? preg_match_all('/./us', $player) : -1;   // عدد الحروف، -1 = مش UTF-8 سليم
        if ($len < 1 || $len > 32 || preg_match('/[\p{C}\p{Zl}\p{Zp}\x{FFFD}\x{10000}-\x{10FFFF}|=\[\]]/u', $player)) {
            throw new QahrApiException('player: اسم غير صالح (1-32 حرف، من غير رموز تحكم)');
        }
        return $player;
    }

    /** رقم الأوردر: 1-64 من A-Z a-z 0-9 _ . : -  (نفس شرط السيرفر) */
    public static function checkRef($ref)
    {
        if (!is_string($ref) || !preg_match('/\A[A-Za-z0-9_.:\-]{1,64}\z/', $ref)) {
            throw new QahrApiException('ref: لازم 1-64 حرف من A-Z a-z 0-9 _ . : -');
        }
        return $ref;
    }

    /** رقم صحيح بين min و max: int أو نص أرقام بس (مفيش "1e3" ولا "12abc" ولا كسور ولا أرقام ضخمة) */
    public static function checkInt($v, $min, $max, $what)
    {
        if (is_string($v) && preg_match('/\A-?[0-9]{1,10}\z/', trim($v))) $v = (int)trim($v);
        if (!is_int($v) || $v < $min || $v > $max) {
            throw new QahrApiException($what . ': لازم رقم صحيح من ' . $min . ' لـ ' . $max);
        }
        return $v;
    }

    /** يجيب شنطة اللاعب وعتاده (لو أوفلاين: الاسم والليفل والـ CPs بس). */
    public function inventory($player)
    {
        return $this->sendRequest(array(
            'op'     => 'inventory',
            'player' => self::checkPlayer($player),
        ));
    }

    // =================================================================================================
    //  الإرسال والاستقبال
    // =================================================================================================

    /** الخطوات: نجهّز الطلب المشفّر ← نبعته ← نستلم الرد ← نتأكد منه ونفك تشفيره. */
    private function sendRequest(array $request)
    {
        $nonce = random_bytes(16);   // رقم عشوائي جديد لكل طلب
        $iv    = random_bytes(12);   // رقم عشوائي للتشفير
        $line  = $this->buildEnvelope($request, time(), $nonce, $iv) . "\n";

        $reply = $this->exchangeLine($line);

        return $this->openReply($reply, $nonce, time());
    }

    /** يفتح اتصال TCP بالسيرفر، يبعت سطر، ويستنى سطر الرد. */
    private function exchangeLine($line)
    {
        $errno = 0;
        $errstr = '';
        $socket = @stream_socket_client('tcp://' . $this->host . ':' . $this->port, $errno, $errstr, $this->timeout);
        if (!$socket) {
            throw new QahrApiException('مش قادر أوصل لسيرفر اللعبة (هل WebApi مفعّل؟ enabled=1 في webapi.ini)');
        }
        stream_set_timeout($socket, $this->timeout);

        try {
            // إرسال السطر كله
            for ($sent = 0; $sent < strlen($line); $sent += $written) {
                $written = fwrite($socket, substr($line, $sent));
                if ($written === false || $written === 0) {
                    throw new QahrApiException('فشل الإرسال');
                }
            }
            // استلام سطر الرد
            $reply = fgets($socket, 65536);
            $meta  = stream_get_meta_data($socket);
            if ($reply === false || !empty($meta['timed_out'])) {
                throw new QahrApiException('السيرفر مارّدش');
            }
        } finally {
            fclose($socket);
        }

        return rtrim($reply, "\r\n");
    }

    // =================================================================================================
    //  التشفير (لازم يفضل مطابق لـ WebApi.cs حرف بحرف)
    // =================================================================================================

    /**
     * الرمز المتغير.
     *   window = رقم الـ 30 ثانية الحالية = floor(ts / 30)
     *   code   = أول 32 حرف hex من HMAC-SHA256(codeKey, ["srv|" لو رد السيرفر] + window (8 بايت) + nonce)
     */
    public static function rotatingCode($codeKey, $ts, $nonce, $fromServer)
    {
        $window = intdiv((int)$ts, self::CODE_WINDOW_SECONDS);
        $data   = ($fromServer ? 'srv|' : '') . pack('J', $window) . $nonce;   // J = 8 بايت big-endian
        return substr(hash_hmac('sha256', $data, $codeKey), 0, 32);
    }

    /** يجهّز سطر الطلب: يشفّر الطلب ويحط معاه الرمز المتغير. (عام عشان selftest.php) */
    public function buildEnvelope(array $request, $ts, $nonce, $iv)
    {
        $ts = (int)$ts;   // نفس الرقم بالظبط في الـ AAD وفي الـ JSON
        if (!is_string($nonce) || strlen($nonce) !== 16 || !is_string($iv) || strlen($iv) !== 12) {
            throw new QahrApiException('nonce لازم 16 بايت و iv لازم 12 بايت (عشوائي جديد لكل طلب)');
        }
        $json = json_encode($request, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);
        if ($json === false) {
            throw new QahrApiException('json_encode فشل');
        }

        $nonceB64 = base64_encode($nonce);
        $aad      = 'v1|' . $ts . '|' . $nonceB64;   // بيانات مربوطة بالتشفير: لو اتغيرت، فك التشفير بيفشل
        $tag      = '';
        $cipher   = openssl_encrypt($json, 'aes-256-gcm', $this->encKey, OPENSSL_RAW_DATA, $iv, $tag, $aad, 16);
        if ($cipher === false) {
            throw new QahrApiException('التشفير فشل');
        }

        return json_encode(array(
            'v'     => self::PROTOCOL_VERSION,
            'ts'    => (int)$ts,
            'nonce' => $nonceB64,
            'iv'    => base64_encode($iv),
            'ct'    => base64_encode($cipher),
            'tag'   => base64_encode($tag),
            'code'  => self::rotatingCode($this->codeKey, $ts, $nonce, false),
        ), JSON_UNESCAPED_SLASHES);
    }

    /**
     * يتأكد من رد السيرفر ويفك تشفيره. (عام عشان selftest.php)
     * بيتأكد بالترتيب: الرد تبع طلبنا ← وقته مظبوط ← رمز السيرفر صح ← التشفير سليم.
     */
    public function openReply($line, $nonce, $now)
    {
        $reply = json_decode((string)$line, true);
        if (!is_array($reply)) {
            throw new QahrApiException('رد غير مفهوم');
        }

        // رد مش مشفّر = السيرفر رفض الطلب (unauthorized / rate_limited) — مفيهوش حاجة سرية
        // (مش موثّق: أي حد في النص ممكن يبعته، فبناخد بس كود خطأ قصير ومش بنصدّق أي حاجة تانية فيه)
        // (unverified: only a short error code is taken from it, it can never say ok)
        if (!isset($reply['ct'])) {
            $err = isset($reply['error']) && is_string($reply['error']) && preg_match('/\A[a-z_]{1,32}\z/', $reply['error'])
                ? $reply['error'] : 'unknown';
            return array(
                'ok'       => false,
                'error'    => $err,
                'verified' => false,
            );
        }

        foreach (array('v', 'ts', 'nonce', 'iv', 'tag', 'code') as $field) {
            if (!isset($reply[$field])) {
                throw new QahrApiException('رد ناقص');
            }
        }

        // 1) الرد تبع طلبنا (نفس الـ nonce)
        $nonceB64 = base64_encode($nonce);
        if ($reply['v'] !== self::PROTOCOL_VERSION || !is_int($reply['ts']) || !is_string($reply['nonce'])
            || !hash_equals($nonceB64, $reply['nonce'])) {
            throw new QahrApiException('الرد مش تبع الطلب ده');
        }

        // 2) الوقت مظبوط (ساعة الجهازين)
        if (abs($now - $reply['ts']) > 60) {
            throw new QahrApiException('وقت الرد مختلف (اتأكد من ساعة الجهازين)');
        }

        // 3) رمز السيرفر صح = ده سيرفرنا ومعاه نفس المفتاح
        $expected = self::rotatingCode($this->codeKey, $reply['ts'], $nonce, true);
        if (!is_string($reply['code']) || !hash_equals($expected, $reply['code'])) {
            throw new QahrApiException('رمز السيرفر غلط: ده مش سيرفرنا أو المفتاح مختلف');
        }

        // 4) فك التشفير (لو حد غيّر حرف في الطريق، الفك بيفشل)
        $iv     = base64_decode((string)$reply['iv'], true);
        $tag    = base64_decode((string)$reply['tag'], true);
        $cipher = base64_decode((string)$reply['ct'], true);
        if ($iv === false || $tag === false || $cipher === false || strlen($iv) !== 12 || strlen($tag) !== 16) {
            throw new QahrApiException('رد غير سليم');
        }
        $aad   = 'v1r|' . $reply['ts'] . '|' . $nonceB64;
        $plain = openssl_decrypt($cipher, 'aes-256-gcm', $this->encKey, OPENSSL_RAW_DATA, $iv, $tag, $aad);
        if ($plain === false) {
            throw new QahrApiException('الرد اتغير في الطريق (فشل التحقق)');
        }

        $data = json_decode($plain, true);
        if (!is_array($data)) {
            throw new QahrApiException('رد غير مفهوم');
        }
        $data['verified'] = true;   // الرد ده مؤكد إنه من سيرفرنا
        return $data;
    }
}
