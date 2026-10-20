<?php
/**
 * أبسط مثال: الـ 3 عمليات في ملف واحد.
 * Simplest example: the 3 operations in one file.
 *
 *   php example.php
 *
 * قبل ما تشغّله:
 *   1) السيرفر: ServerDataBase\webapi.ini  →  enabled=1  و  key=<المفتاح>
 *   2) هنا:     انسخ config.example.php باسم config.php وحط نفس المفتاح
 *
 * من سطر الأوامر بس: لو اتفتح من المتصفح بيرجّع 404 (وإلا أي حد يفتح الرابط كان هيبعت CPs وآيتم).
 * Command line only: from a browser it answers 404 (otherwise anyone opening the URL would send CPs + an item).
 */
if (PHP_SAPI !== 'cli') { http_response_code(404); exit; }
require __DIR__ . '/_common.php';   // بيقرا config.php (أو المسار اللي في QAHR_WEBAPI_CONFIG) ويعمل QahrApi

// ----------------------------------------------------------------- الاتصال
$api = qahr_api();

$player = 'mohamed';   // اسم اللاعب أو الـ UID

try {
    // ------------------------------------------------------------- 1) إرسال CPs
    // رقم الأوردر لازم يكون جديد لكل دفعة، عشان السيرفر مايدفعش مرتين
    $orderRef = 'order-' . date('YmdHis') . '-' . bin2hex(random_bytes(4));
    $result = $api->sendCps($player, 1000, $orderRef);
    echo "CPs:       ", json_encode($result), PHP_EOL;

    // ------------------------------------------------------------- 2) إرسال آيتم
    // 1088001 = Meteor ، العدد 5 (اللاعب لازم يكون أونلاين وفي شنطته مكان)
    $itemRef = 'item-' . date('YmdHis') . '-' . bin2hex(random_bytes(4));
    $result = $api->giveItem($player, 1088001, 5, $itemRef);
    echo "Item:      ", json_encode($result), PHP_EOL;

    // ------------------------------------------------------------- 3) عرض الشنطة
    $result = $api->inventory($player);
    echo "Inventory: ", json_encode($result, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE), PHP_EOL;

} catch (QahrApiException $e) {
    // أي مشكلة (السيرفر مقفول، المفتاح غلط، الرد اتغير ...) بتيجي هنا برسالة واضحة
    echo "خطأ: ", $e->getMessage(), PHP_EOL;
}
