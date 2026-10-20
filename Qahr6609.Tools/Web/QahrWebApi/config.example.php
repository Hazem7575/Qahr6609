<?php
// Copy this file to config.php and fill it in. config.php is in .gitignore: NEVER commit or upload the real key.
// The key must be the SAME as key= in the game server's ServerDataBase\webapi.ini.
// BEST: keep the real file OUTSIDE the web root and point to it with the environment variable QAHR_WEBAPI_CONFIG
// (or name it qahr_config.php in the folder above this one) - see _common.php. If it stays here, the included
// .htaccess (Apache) / web.config (IIS) deny it; nginx needs:  location ~ /QahrWebApi/ { deny all; }  or the same rules.
if (PHP_SAPI !== 'cli' && isset($_SERVER['SCRIPT_FILENAME']) && realpath($_SERVER['SCRIPT_FILENAME']) === __FILE__) { http_response_code(404); exit; }
return array(
    'host'    => '127.0.0.1',   // the game server (same machine = 127.0.0.1; otherwise a trusted private network only)
    'port'    => 5900,          // webapi.ini port=
    'key'     => '',            // webapi.ini key=  (base64 of 32 random bytes: php gen_key.php  or server console: webapi genkey)
    'timeout' => 10,            // seconds
    // true = send_cps.php / give_item.php / inventory.php also work as web forms. Only after the folder is behind
    // YOUR site's admin login (and HTTPS). false = command line only.
    'web_examples' => false,
    // web mode only: the visitor IPs that may open the forms (default: this machine only). Your admin login is still needed.
    'web_allow_ips' => array('127.0.0.1', '::1'),
);
