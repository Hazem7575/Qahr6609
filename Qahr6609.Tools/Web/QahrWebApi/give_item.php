<?php
// Give an item to an ONLINE player (needs free bag space).
//   CLI: php give_item.php <player name or uid> <item id> <count> [order ref] [plus] [bound 0|1]
//   Web: only with 'web_examples' => true in config.php (protect the page with your admin login first).
require __DIR__ . '/_common.php';

if (qahr_is_cli()) {
    if ($argc < 4) qahr_die('usage: php give_item.php <player> <item id> <count> [ref] [plus] [bound 0|1]');
    $ref = isset($argv[4]) && $argv[4] !== '' ? $argv[4] : qahr_new_ref('item');
    $plus = isset($argv[5]) ? $argv[5] : 0;
    $bound = isset($argv[6]) && $argv[6] === '1';
    try {
        qahr_show_result(qahr_api()->giveItem($argv[1], $argv[2], $argv[3], $ref, $plus, $bound));   // QahrApi checks the numbers
    } catch (QahrApiException $e) { qahr_die('error: ' . $e->getMessage()); }
    exit;
}

qahr_web_begin('Give item');
if (qahr_csrf_ok()) {
    try {
        // raw strings on purpose: QahrApi::checkInt refuses "1e9", "12abc", -5, ... instead of (int) silently changing them
        qahr_show_result(qahr_api()->giveItem(qahr_post('player'), qahr_post('item'), qahr_post('count', '1'),
            qahr_post('ref'), qahr_post('plus', '0'), qahr_post('bound') === '1'));
    } catch (QahrApiException $e) { echo '<p>error: ' . h($e->getMessage()) . '</p>'; }
}
?>
<form method="post">
  <?= qahr_csrf_field() ?>
  <p>Player (name or uid): <input name="player" required maxlength="32"></p>
  <p>Item id: <input name="item" type="number" min="1" required></p>
  <p>Count: <input name="count" type="number" min="1" value="1" required></p>
  <p>Plus: <input name="plus" type="number" min="0" max="12" value="0"></p>
  <p><label><input type="checkbox" name="bound" value="1"> Bound</label></p>
  <p>Order ref (unique): <input name="ref" value="<?= h(qahr_new_ref('item')) ?>" required maxlength="64"></p>
  <button>Give</button>
</form>
