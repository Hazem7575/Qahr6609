<?php
// Show a player's bag (inventory) and equipment. The bag is only readable while the player is online.
//   CLI: php inventory.php <player name or uid>
//   Web: only with 'web_examples' => true in config.php (protect the page with your admin login first).
require __DIR__ . '/_common.php';

if (qahr_is_cli()) {
    if ($argc < 2) qahr_die('usage: php inventory.php <player>');
    try {
        $r = qahr_api()->inventory($argv[1]);
    } catch (QahrApiException $e) { qahr_die('error: ' . $e->getMessage()); }
    if (empty($r['ok'])) { qahr_show_result($r); exit(1); }
    $r += array('name' => '?', 'uid' => '?', 'level' => '?', 'cps' => '?', 'online' => false);
    echo $r['name'], ' (', $r['uid'], ') level ', $r['level'], ', CPs ', $r['cps'], $r['online'] ? '' : ' - offline', PHP_EOL;
    foreach (array('inventory' => 'Bag', 'equipment' => 'Equipment') as $k => $title) {
        if (empty($r[$k]) || !is_array($r[$k])) continue;
        echo $title, ':', PHP_EOL;
        foreach ($r[$k] as $it) {
            $it = (is_array($it) ? $it : array()) + array('id' => 0, 'name' => '?', 'stack' => 1, 'plus' => 0, 'bound' => false);
            printf("  [%d] %s x%d%s%s\n", $it['id'], $it['name'], max(1, (int)$it['stack']), $it['plus'] ? ' +' . (int)$it['plus'] : '', $it['bound'] ? ' (bound)' : '');
        }
    }
    exit;
}

qahr_web_begin('Player bag');
?>
<form method="post">
  <?= qahr_csrf_field() ?>
  <p>Player (name or uid): <input name="player" required maxlength="32" value="<?= h(qahr_post('player')) ?>"> <button>Show</button></p>
</form>
<?php
if (qahr_csrf_ok()) {
    try {
        $r = qahr_api()->inventory(qahr_post('player'));
    } catch (QahrApiException $e) { echo '<p>error: ' . h($e->getMessage()) . '</p>'; exit; }
    if (empty($r['ok'])) { qahr_show_result($r); exit; }
    // every value from the server is escaped with h() (a player or item name is data, never HTML)
    $r += array('name' => '?', 'uid' => '?', 'level' => '?', 'cps' => '?', 'online' => false, 'message' => '');
    echo '<p>' . h($r['name']) . ' (' . h($r['uid']) . ') level ' . h($r['level']) . ', CPs ' . h($r['cps']) . ($r['online'] ? '' : ' - offline: ' . h($r['message'])) . '</p>';
    foreach (array('inventory' => 'Bag', 'equipment' => 'Equipment') as $k => $title) {
        if (empty($r[$k]) || !is_array($r[$k])) continue;
        echo '<h3>' . h($title) . '</h3><table border="1" cellpadding="4"><tr><th>Id</th><th>Name</th><th>Count</th><th>Plus</th><th>Bound</th></tr>';
        foreach ($r[$k] as $it) {
            $it = (is_array($it) ? $it : array()) + array('id' => 0, 'name' => '?', 'stack' => 1, 'plus' => 0, 'bound' => false);
            echo '<tr><td>' . h($it['id']) . '</td><td>' . h($it['name']) . '</td><td>' . h(max(1, (int)$it['stack'])) . '</td><td>' . h($it['plus']) . '</td><td>' . ($it['bound'] ? 'yes' : '') . '</td></tr>';
        }
        echo '</table>';
    }
}
