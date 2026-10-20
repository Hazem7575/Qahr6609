<?php
// Offline check of this PHP install + QahrApi.php against the game server's known-answer vector
// (the same vector is printed by the server console command "webapi test"). No connection is made.
//   php selftest.php
if (PHP_SAPI !== 'cli') { http_response_code(404); exit; }
require __DIR__ . '/QahrApi.php';

$fails = 0;
function check($ok, $what) { global $fails; echo ($ok ? 'OK   ' : 'FAIL ') . $what . PHP_EOL; if (!$ok) $fails++; }

// key = bytes 0..31, ts = 1700000000, nonce = 16 x 0xA5, iv = 12 x 0x11, inner = {"op":"inventory","player":"Test"}
$api = new QahrApi('127.0.0.1', 5900, base64_encode(implode('', array_map('chr', range(0, 31)))));
$expected = '{"v":1,"ts":1700000000,"nonce":"paWlpaWlpaWlpaWlpaWlpQ==","iv":"ERERERERERERERER",'
          . '"ct":"1WmQXoo3vPiF/7FWg0394Ox/lCsJ89BJAQ/xAi3bkr05kg==","tag":"LZj+n1o5EhQoVgidTrwjAw==",'
          . '"code":"aa0f01ece589aa71b206c06bbca67675"}';
$nonce = str_repeat("\xA5", 16);
$line = $api->buildEnvelope(array('op' => 'inventory', 'player' => 'Test'), 1700000000, $nonce, str_repeat("\x11", 12));
check($line === $expected, 'request envelope == server vector (HKDF, rotating code, AES-256-GCM, AAD)');
if ($line !== $expected) echo '  got      ', $line, PHP_EOL, '  expected ', $expected, PHP_EOL;

// a reply made the way the server makes it, then the client checks
$key = implode('', array_map('chr', range(0, 31)));
$enc = hash_hkdf('sha256', $key, 32, 'qahr-webapi-enc', '');
$codeKey = hash_hkdf('sha256', $key, 32, 'qahr-webapi-code', '');
$ts = 1700000005; $iv = random_bytes(12); $tag = '';
$nb = base64_encode($nonce);
$ct = openssl_encrypt('{"ok":true,"op":"inventory"}', 'aes-256-gcm', $enc, OPENSSL_RAW_DATA, $iv, $tag, 'v1r|' . $ts . '|' . $nb, 16);
$reply = array('v' => 1, 'ts' => $ts, 'nonce' => $nb, 'iv' => base64_encode($iv), 'ct' => base64_encode($ct), 'tag' => base64_encode($tag),
               'code' => QahrApi::rotatingCode($codeKey, $ts, $nonce, true));
$r = $api->openReply(json_encode($reply), $nonce, $ts);
check(!empty($r['ok']) && $r['verified'] === true, 'server reply verified and decrypted');
$bad = $reply; $bad['code'] = str_repeat('0', 32);
try { $api->openReply(json_encode($bad), $nonce, $ts); check(false, 'wrong server code rejected'); } catch (QahrApiException $e) { check(true, 'wrong server code rejected'); }
$bad = $reply; $c = base64_decode($bad['ct']); $c[0] = chr(ord($c[0]) ^ 1); $bad['ct'] = base64_encode($c);
try { $api->openReply(json_encode($bad), $nonce, $ts); check(false, 'changed reply rejected'); } catch (QahrApiException $e) { check(true, 'changed reply rejected'); }
try { $api->openReply(json_encode($reply), random_bytes(16), $ts); check(false, 'reply to another request rejected'); } catch (QahrApiException $e) { check(true, 'reply to another request rejected'); }

echo $fails === 0 ? 'ALL PASSED' . PHP_EOL : $fails . ' FAILED' . PHP_EOL;
exit($fails === 0 ? 0 : 1);
