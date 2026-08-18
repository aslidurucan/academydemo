# Mimari — Modül Haritası

> Bkz. [AGENTS.md](../AGENTS.md) altın kural 2. Kod düzeyinde karşılığı için [docs/conventions.md](conventions.md) §Klasör Yapısı.

Tek deployable, tek veritabanı sunucusu — ama her modül kendi EF Core `DbContext`'ine ve kendi SQL şemasına sahip. Modüller birbirini yalnızca `Contracts` katmanındaki public arayüzler üzerinden görür; bu sınır bir klasör anlaşması değil, derleme zamanında ve şema düzeyinde uygulanan bir sınırdır.

## Modüller

Her modül tek bir iş kararının otoritesidir. "Sahip olunan veri" tablo değil, kavram düzeyinde: o modül dışında hiçbir yerde o kavramın birincil kopyası tutulmaz.

| # | Modül | Şema | Sorumluluk | Sahip olduğu veri |
|---|---|---|---|---|
| M01 | Identity & Access | `identity` | Kimlik doğrulama, parola/oturum yönetimi, rol ataması (Öğrenci / Eğitmen / Admin) | Hesap, kimlik bilgisi, rol ataması, oturum/refresh token |
| M02 | Instructor Management | `instructor` | Eğitmen başvurusu, onay iş akışı, eğitmen profili | Eğitmen profili, başvuru durumu, onay geçmişi |
| M03 | Catalog | `catalog` | Kurs/bölüm/ders meta verisi, kategori taksonomisi, fiyat tanımı, yayın durumu iş akışı | Kurs, bölüm, ders (başlık/süre/sıra), kategori, liste fiyatı |
| M04 | Content Delivery | `content` | Video/materyal dosyalarının depolanması, işlenme (transcode) durumu, imzalı oynatma URL'i üretimi | Medya varlığı, işlenme durumu, depolama referansı, altyazı dosyası |
| M05 | Cart & Ordering | `ordering` | Sepet yönetimi, sipariş oluşturma ve durum makinesi, sipariş anındaki fiyat/indirim tutarının kaydı | Sepet, sepet kalemi, sipariş, sipariş kalemi (fiyat anlık görüntüsüyle) |
| M06 | Promotions | `ordering` (alt-bileşen) | Kupon kodu tanımı ve kullanım kısıtları | Kupon, kampanya kuralı, kullanım kaydı |
| M07 | Payments | `payments` | Ödeme sağlayıcı entegrasyonu, ödeme işlemi yaşam döngüsü, iade, fatura/makbuz üretimi | Ödeme işlemi, sağlayıcı referans kimliği, iade kaydı, fatura |
| M08 | Enrollment & Entitlements | `enrollment` | "Bu kullanıcı bu kursa erişebilir mi?" sorusunun tek otoritesi | Erişim kaydı (kullanıcı, kurs, kaynak, durum, tarih) |
| M09 | Learning Progress | `progress` | Ders bazında izleme ilerlemesi, tamamlanma işareti, kurs bitirme durumu | Ders izleme kaydı (izlenen süre/yüzde, tamamlandı mı) |
| M10 | Reviews & Ratings | `reviews` | Kurs yorumu ve puanlama, moderasyon durumu | Yorum, puan, moderasyon durumu |
| M11 | Notifications | `notifications` | Diğer modüllerin event'lerini dinleyip e-posta bildirimi gönderen, tamamen event-driven tüketici | Gönderim log'u, şablon tanımı |

Promotions (M06), V1'de Ordering'in alt-bileşeni olarak yaşar (aynı şema, ayrı iç namespace) — kapsamı büyürse bağımsız modüle çıkarılır.

## İletişim Kuralları

İki iletişim biçimi var: **senkron çağrı** (in-process, public arayüz üzerinden, aynı istek içinde cevap) ve **domain event** (asenkron, her modül kendi verisini kendi günceller — transactional outbox ile taşınır, bkz. Karar Günlüğü).

1. **Senkron çağrı yalnızca okuma içindir.** Cevap veren modül o verinin tek otoritesiyse ve sorgu ucuzsa (örn. "bu kullanıcı bu kursa erişebilir mi?"), doğrudan public arayüz üzerinden çağrılır.
2. **Başka modülün verisini değiştirmek asla senkron komutla yapılmaz.** İhtiyaç doğduğunda kaynak modül bir domain event yayınlar, hedef modül kendi kararıyla kendi verisini günceller.
3. **Domain event bir işin bittiğini duyurur — bir sonuç talep etmez.** Yayınlayan taraf kimin dinlediğini bilmez, dinleyicinin cevabını beklemez (fire-and-forget, eventual consistency).
4. **Event handler'lar idempotent olmalı.** Aynı event iki kez teslim edilirse (retry/outbox) sonuç değişmemeli.
5. **Yazma işlemi her zaman kendi şeması içinde, kendi transaction sınırında kalır.** İki şemayı aynı transaction'da güncellemek yasak (bkz. Yasak Liste).
6. **Sık listelenen "yabancı" alanlar** (ör. sipariş ekranında kurs başlığı) event ile beslenen bir salt-okunur kopyaya düşürülür, her istekte senkron çapraz çağrıya değil.

### Somut Örnekler

| Kaynak | Hedef | Tür | Amaç |
|---|---|---|---|
| Ordering | Payments | senkron | Ödeme işlemini başlat, sonucu bekle |
| Payments | Ordering | event | `PaymentCompleted` → siparişi "ödendi" işaretle |
| Payments | Enrollment | event | `PaymentCompleted` → erişim hakkı oluştur |
| Payments | Notifications | event | `PaymentCompleted` → satın alma e-postası |
| Content | Enrollment | senkron | Oynatma URL'i üretmeden önce erişim kontrolü |
| Learning Progress | Enrollment | senkron | İlerleme kaydından önce kayıt kontrolü |
| Reviews | Enrollment | senkron | Yorum yapmadan önce enrolled mı kontrolü |
| Catalog | Instructor Mgmt | senkron | Kurs yayınlarken eğitmen onaylı mı kontrolü |
| Ordering | Catalog | senkron | Sepete eklerken güncel fiyat/durum bilgisi |
| Enrollment | Notifications | event | `AccessGranted` / `AccessRevoked` → bilgilendirme |

## Yasak Liste

İhlali code review'da otomatik red sebebi:

- Bir modülün başka modülün EF Core `DbContext`'ine veya entity sınıflarına doğrudan referans/erişimi.
- Şemalar arası veritabanı foreign key veya JOIN — id'ler yalnızca zayıf referans (Guid) olarak taşınır.
- Bir modülün başka modülün "internal" (public olmayan) namespace'indeki servis/repository sınıflarına referansı — yalnızca `Contracts` katmanı görünür.
- Ortak bir "Users" tablosunun/entity'sinin her modülce doğrudan sorgulanması — her modül ihtiyacı olan minimal kullanıcı verisini Identity'nin public arayüzünden alır veya event ile kendi kopyasını tutar.
- Modüller arası dağıtık transaction — iki şemayı aynı transaction'da güncellemek. Çoklu modül tutarlılığı yalnızca event + eventual consistency ile sağlanır.
- Bir modülün, yayınladığı event'in dinleyicisinden senkron bir cevap bekleyip iş akışını buna bağlaması (event'i gizli bir komut gibi kullanmak).
- Paylaşılan `Shared` katmanına iş kuralı (business logic) yazılması — yalnızca gerçekten teknik, alan-bağımsız yardımcılar (ör. `Result<T>`, event bus arayüzü) paylaşılabilir.
- Bir modülün başka modülün konfigürasyon/secret'larına (ör. ödeme sağlayıcı API anahtarı) doğrudan erişimi.

## V1 Kapsam Dışı Kararlar

Bilinçli olarak ertelenen kapsam — modül sınırları bunları sonradan eklemeye izin verecek şekilde tasarlandı, ama V1'de inşa edilmiyor:

- Eğitmen hakedişi (revenue share) otomasyonu — V1'de kayıt tutulur, hesaplama manuel yapılır.
- Full-text arama motoru (Elasticsearch vb.) — V1'de SQL tabanlı basit arama yeterli.
- Kendi video transcoding altyapısı — V1'de üçüncü parti servise (ör. Mux, Cloudflare Stream) delege edilir.
- Sertifika üretimi (PDF) — sonraki sürüme bırakıldı.
- Çoklu para birimi / bölgesel vergi motoru — V1'de tek para birimi, sabit KDV oranı.
- Quiz / ödev / sınav sistemi — V1 yalnızca video izlemeyi kapsıyor.
- Abonelik modeli (Udemy Business tarzı) — V1'de yalnızca tekil kurs satışı.
- Çok kiracılı (multi-tenant) mimari — V1 tek kiracı.
- Soru-cevap forumu / öğrenciler arası mesajlaşma — V1'de yalnızca yorum/puan var.
- İnce taneli (fine-grained) yetkilendirme — V1'de yalnızca 3 sabit rol (Öğrenci/Eğitmen/Admin).
- Notifications için SMS/push kanalı — V1'de yalnızca e-posta.

## Karar Günlüğü

17-18 Ağustos 2026 tarihli mimari taslakta açık bırakılan 9 nokta, Takım Yöneticisi tarafından onaylandı:

1. Content Delivery, Catalog'dan ayrı modül (M04) — video farklı ölçek/depolama profiline sahip.
2. Ordering ve Payments ayrı modül (M05/M07) — ödeme sağlayıcı entegrasyonu bağımsız evrilmeli.
3. Promotions V1'de Ordering'in alt-bileşeni (M06) — kapsam tek başına modülü haklı çıkarmıyor, iç sınır yine de net.
4. Domain event iletimi: transactional outbox + arka planda işleyen consumer (aynı process, ayrı deployment değil).
5. Instructor Management, Identity'den ayrı modül (M02) — Identity ince tutulur, yalnızca kimlik doğrulama + rol.
6. Enrollment'ın bağımsız `Grant`/`Revoke` arayüzü var — ücretsiz kurs/admin ataması sahte sipariş yerine doğrudan bu arayüzü kullanır.
7. Şema izolasyonu katı uygulanır: her modülün kendi `DbContext`'i, kendi migration'ları, kendi SQL şeması.
8. Sık listelenen çapraz-modül alanlar için event ile beslenen denormalize kopya (ör. `CourseSnapshot`); detay ekranlarında senkron çağrı.
9. V1'de kurs yayınında moderasyon/admin onayı yok — eğitmen kendi kursunu doğrudan yayına alır (self-service).
