<?php
// Send CPs to a player.
//   CLI: php send_cps.php <player name or uid> <amount> [order ref]
//   Web: only with 'web_examples' => true in config.php (protect the page with your admin login first).
// The ref must be unique per payment: the game server refuses a ref it already executed (no double delivery).
require __DIR__ . '/_common.php';

if (qahr_is_cli()) {
    if ($argc < 3) qahr_die('usage: php send_cps.php <player> <amount> [ref]');
    $ref = isset($argv[3]) ? $argv[3] : qahr_new_ref('cps');
    try {
        qahr_show_result(qahr_api()->sendCps($argv[1], $argv[2], $ref));   // QahrApi checks name / amount / ref
    } catch (QahrApiException $e) { qahr_die('error: ' . $e->getMessage()); }
    exit;
}

qahr_web_begin('Send CPs');
if (qahr_csrf_ok()) {
    try {
        // raw strings on purpose: QahrApi::checkInt refuses "1e9", "12abc", -5, ... instead of (int) silently changing them
        qahr_show_result(qahr_api()->sendCps(qahr_post('player'), qahr_post('amount'), qahr_post('ref')));
    } catch (QahrApiException $e) { echo '<p>error: ' . h($e->getMessage()) . '</p>'; }
}
?>
<form method="post">
  <?= qahr_csrf_field() ?>
  <p>Player (name or uid): <input name="player" required maxlength="32"></p>
  <p>Amount: <input name="amount" type="number" min="1" max="2000000000" required></p>
  <p>Order ref (unique): <input name="ref" value="<?= h(qahr_new_ref('cps')) ?>" required maxlength="64"></p>
  <button>Send</button>
</form>
