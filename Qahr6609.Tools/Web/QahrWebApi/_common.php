<?php
// Shared by the example pages. Loads config.php, makes the QahrApi client, escapes output.
// The examples work from the command line (php send_cps.php ...) and, only if config.php has
// 'web_examples' => true, as simple forms. Put them behind YOUR site's admin login before enabling that.
//
// Where config.php is read from (the first that exists):
//   1) the path in the environment variable QAHR_WEBAPI_CONFIG   (recommended: a folder OUTSIDE the web root, e.g.
//      Apache: SetEnv QAHR_WEBAPI_CONFIG /home/site/private/qahr_config.php   nginx: fastcgi_param QAHR_WEBAPI_CONFIG ...)
//   2) ../qahr_config.php  (the folder above this one: with this folder at the top of the web root it is outside it)
//   3) config.php next to this file (works, but then the included .htaccess / web.config MUST be active to deny it)
if (PHP_SAPI !== 'cli' && isset($_SERVER['SCRIPT_FILENAME']) && realpath($_SERVER['SCRIPT_FILENAME']) === __FILE__) { http_response_code(404); exit; }

if (PHP_SAPI !== 'cli') {
    // a web page never shows PHP errors (they can contain paths); they go to the server's error log
    ini_set('display_errors', '0');
    ini_set('log_errors', '1');
}

require __DIR__ . '/QahrApi.php';

function qahr_config_path()
{
    $env = getenv('QAHR_WEBAPI_CONFIG');
    if (is_string($env) && $env !== '' && is_file($env)) return $env;
    if (is_file(dirname(__DIR__) . '/qahr_config.php')) return dirname(__DIR__) . '/qahr_config.php';
    return __DIR__ . '/config.php';
}

function qahr_config()
{
    static $c = null;
    if ($c !== null) return $c;
    $f = qahr_config_path();
    if (!is_file($f)) qahr_die('config.php is missing: copy config.example.php to config.php (better: outside the web root) and fill in the key.');
    $c = require $f;
    if (!is_array($c) || empty($c['key'])) qahr_die('config.php: key is empty.');
    return $c;
}

function qahr_api()
{
    $c = qahr_config();
    return new QahrApi($c['host'], $c['port'], $c['key'], isset($c['timeout']) ? $c['timeout'] : 10);
}

function qahr_is_cli() { return PHP_SAPI === 'cli'; }

function h($s) { return htmlspecialchars((string)$s, ENT_QUOTES | ENT_SUBSTITUTE, 'UTF-8'); }

function qahr_die($msg)
{
    if (qahr_is_cli()) { fwrite(STDERR, $msg . PHP_EOL); exit(1); }
    http_response_code(500);
    if (!headers_sent()) header('Content-Type: text/html; charset=utf-8');
    echo '<p>' . h($msg) . '</p>';
    exit;
}

function qahr_is_https()
{
    return (!empty($_SERVER['HTTPS']) && strtolower((string)$_SERVER['HTTPS']) !== 'off')
        || (isset($_SERVER['SERVER_PORT']) && (int)$_SERVER['SERVER_PORT'] === 443);
}

// web mode (needs PHP 7.3+): off unless config.php says 'web_examples' => true, and then only for the IPs in
// 'web_allow_ips' (default: this machine only). POST forms carry a per-session CSRF token; the session cookie is
// HttpOnly + SameSite=Strict (+ Secure on HTTPS) and the session id is renewed when the session starts (no fixation).
function qahr_web_begin($title)
{
    // security headers first (they also cover the refusals below)
    header('X-Frame-Options: DENY');                                    // clickjacking (old browsers)
    header("Content-Security-Policy: default-src 'none'; style-src 'unsafe-inline'; form-action 'self'; frame-ancestors 'none'; base-uri 'none'");
    header('X-Content-Type-Options: nosniff');
    header('Referrer-Policy: no-referrer');
    header('Cache-Control: no-store');

    $c = qahr_config();
    if (empty($c['web_examples'])) { http_response_code(403); exit('Disabled. Run from the command line, or set web_examples=true in config.php AFTER protecting this folder with your admin login.'); }

    $allow = isset($c['web_allow_ips']) && is_array($c['web_allow_ips']) ? $c['web_allow_ips'] : array('127.0.0.1', '::1');
    $ip = isset($_SERVER['REMOTE_ADDR']) ? (string)$_SERVER['REMOTE_ADDR'] : '';
    if (stripos($ip, '::ffff:') === 0 && filter_var(substr($ip, 7), FILTER_VALIDATE_IP, FILTER_FLAG_IPV4)) $ip = substr($ip, 7);   // IPv4-mapped IPv6
    if (!in_array($ip, $allow, true)) { http_response_code(403); exit('Forbidden (your IP is not in web_allow_ips).'); }

    $method = isset($_SERVER['REQUEST_METHOD']) ? $_SERVER['REQUEST_METHOD'] : '';
    if ($method !== 'GET' && $method !== 'POST') { http_response_code(405); header('Allow: GET, POST'); exit; }

    if (session_status() !== PHP_SESSION_ACTIVE) {
        ini_set('session.use_strict_mode', '1');                          // ids the server never issued are refused
        ini_set('session.use_only_cookies', '1');                         // never an id from the URL
        ini_set('session.use_trans_sid', '0');
        session_name('qahr_webapi');
        session_set_cookie_params(array('lifetime' => 0, 'path' => '/', 'secure' => qahr_is_https(), 'httponly' => true, 'samesite' => 'Strict'));
        session_start();
    }
    if (empty($_SESSION['qahr_csrf']) || !is_string($_SESSION['qahr_csrf'])) {
        session_regenerate_id(true);                                      // a new session always gets a fresh id
        $_SESSION['qahr_csrf'] = bin2hex(random_bytes(32));
    }
    header('Content-Type: text/html; charset=utf-8');
    echo '<!doctype html><meta charset="utf-8"><title>' . h($title) . '</title><h2>' . h($title) . '</h2>';
    if (!qahr_is_https() && $ip !== '127.0.0.1' && $ip !== '::1') echo '<p><b>Warning: this page is not on HTTPS.</b></p>';
}

function qahr_csrf_field() { return '<input type="hidden" name="csrf" value="' . h($_SESSION['qahr_csrf']) . '">'; }

// true only for a POST from our own form: the session token matches, and the browser's Origin (when it sends one) is this host
function qahr_csrf_ok()
{
    if (!isset($_SERVER['REQUEST_METHOD']) || $_SERVER['REQUEST_METHOD'] !== 'POST') return false;
    if (!empty($_SERVER['HTTP_ORIGIN']) && isset($_SERVER['HTTP_HOST'])
        && parse_url((string)$_SERVER['HTTP_ORIGIN'], PHP_URL_HOST) !== parse_url('http://' . $_SERVER['HTTP_HOST'], PHP_URL_HOST)) return false;
    return isset($_POST['csrf'], $_SESSION['qahr_csrf']) && is_string($_POST['csrf']) && is_string($_SESSION['qahr_csrf'])
        && hash_equals($_SESSION['qahr_csrf'], $_POST['csrf']);
}

function qahr_post($k, $def = '') { return isset($_POST[$k]) && is_string($_POST[$k]) ? trim($_POST[$k]) : $def; }

function qahr_new_ref($prefix) { return $prefix . '-' . date('Ymd-His') . '-' . bin2hex(random_bytes(4)); }

function qahr_json($r) { return json_encode($r, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_INVALID_UTF8_SUBSTITUTE); }

function qahr_show_result(array $r)
{
    if (qahr_is_cli()) { echo qahr_json($r), PHP_EOL; return; }
    echo '<pre>' . h(qahr_json($r)) . '</pre>';                          // the JSON is escaped too (a name like </pre><script> stays text)
}
