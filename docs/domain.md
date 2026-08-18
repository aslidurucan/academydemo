# Domain — Ortak Dil ve İş Kuralları

> Modül sahiplikleri için [docs/architecture.md](architecture.md). Bu belge, hangi terimin kod ve konuşmada ne anlama geldiğini ve sipariş/kupon/iade akışlarının kurallarını sabitler. Bir iş kuralı netleşmedikçe buradaki varsayılan geçerlidir; değişiklik PR ile bu dosyaya işlenir.

## Ortak Dil (Ubiquitous Language)

Kodda yalnızca "Kod Karşılığı" sütunundaki isim kullanılır — eş anlamlı isimlerin (Video/Ders/Lecture gibi) karışması yasak.

| Terim (TR) | Kod Karşılığı (EN) | Tanım |
|---|---|---|
| Kurs | `Course` | Satılan ana ürün; bölümlerden oluşur |
| Bölüm | `Section` | Kursun alt başlığı; dersleri gruplar |
| Ders | `Lecture` | İzlenebilir en küçük birim (video) |
| Eğitmen | `Instructor` | Kurs üreten/yayınlayan kullanıcı |
| Öğrenci | `Student` | Kurs satın alan/izleyen kullanıcı |
| Sepet | `Cart` | Ödeme öncesi, serbestçe düzenlenebilir kalem listesi |
| Sipariş | `Order` | Sepetten oluşturulan, fiyatı donmuş, değiştirilemez kayıt |
| Sipariş Kalemi | `OrderItem` | Siparişteki tek bir kurs satırı + o anki fiyat |
| Kupon | `Coupon` | Kod karşılığında indirim sağlayan tanım |
| Kampanya | `Promotion` | Kuponun bağlı olduğu kural seti (tarih, kısıt) |
| Ödeme | `Payment` | Sipariş için yapılan tahsilat işlemi |
| İade | `Refund` | Bir ödemenin kısmen/tamamen geri alınması |
| Erişim Hakkı / Kayıt | `Enrollment` | Bir kullanıcının bir kursa erişim yetkisi — kaynağı ne olursa olsun (satın alma, ücretsiz, admin) tek isim: `Enrollment` |
| İlerleme | `Progress` | Bir dersin/kursun izlenme durumu |
| Değerlendirme | `Review` | Kurs yorumu + puanı |
| Fiyat Anlık Görüntüsü | `PriceSnapshot` | Sipariş anında donmuş fiyat/indirim tutarı |
| Yayın Durumu | `PublicationStatus` | Kursun `Draft` / `Published` durumu |

## İş Kuralları

### Sipariş (Order)

- Durum makinesi: `PendingPayment` → `Paid` → (`Refunded` | `Cancelled`).
- Sipariş oluşturulduğunda sepetteki her kalem için kurs fiyatı o anki haliyle **donar** (`PriceSnapshot`) — kurs fiyatı sonradan değişse bile mevcut sipariş kalemi etkilenmez.
- Sipariş yalnızca `Paid` durumuna geçtiğinde (`PaymentCompleted` event'i) Enrollment'a erişim hakkı talebi düşer — bkz. [docs/architecture.md](architecture.md) İletişim Kuralları.
- Sepet, ödeme tamamlanana kadar serbestçe düzenlenebilir; sipariş oluştuktan sonra sipariş kalemleri değişmez (immutable).
- Bir sepetten yalnızca bir aktif (`PendingPayment`) sipariş türetilebilir; ikinci deneme öncekini geçersiz kılar.

### Kupon (Coupon)

- Bir kupon; geçerlilik tarihi aralığı, opsiyonel kurs/kategori kısıtı ve opsiyonel kullanıcı başına kullanım limiti taşır.
- **V1 varsayımı:** sipariş başına yalnızca **1 kupon** uygulanabilir (stacking yok). Netleşmemiş bir varsayımdır, ihtiyaç doğarsa Takım Yöneticisi onayıyla değişir.
- Kupon indirimi de sipariş kalemine `PriceSnapshot` ile birlikte donmuş tutar olarak yazılır — kupon sonradan iptal edilse bile geçmiş siparişi etkilemez.
- Süresi geçmiş veya kullanım limiti dolmuş kupon, sepette uygulanma anında reddedilir (sipariş oluşturma anında değil — kullanıcıya erken geri bildirim).

### İade (Refund)

- **V1 varsayımı:** satın alma tarihinden itibaren **14 gün** iade penceresi (sektör pratiğine dayalı varsayım, netleşmemişse değiştirilebilir).
- İade akışı: Payments iade kaydı oluşturur → `RefundCompleted` event → Enrollment erişimi iptal eder (`Revoked`) → Ordering siparişi `Refunded` olarak işaretler.
- V1'de kısmi iade yok — ders kısmen izlenmiş olsa da ya tam iade ya hiç.
- İade edilen bir sipariş için Enrollment kaydı `Revoked` durumuna geçer ama silinmez (denetim izi/geçmiş için saklanır).
