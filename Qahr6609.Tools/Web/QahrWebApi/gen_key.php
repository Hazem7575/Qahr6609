<?php
// Prints a new random key (base64 of 32 bytes). Run:  php gen_key.php
// Put the same value in the game server's ServerDataBase\webapi.ini (key=...) and in config.php ('key' => '...').
if (PHP_SAPI !== 'cli') { http_response_code(404); exit; }   // never generate keys from a web page
echo base64_encode(random_bytes(32)), PHP_EOL;
