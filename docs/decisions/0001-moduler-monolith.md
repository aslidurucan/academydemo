# ADR 0001 — Modüler Monolith mi, Mikroservisler mi?

| Alan | Değer |
|---|---|
| Durum | Kabul |
| Tarih | 18 Ağustos 2026 |
| Karar Verici | Takım Yöneticisi |
| İlgili | [docs/architecture.md](../architecture.md) |

## Bağlam

- Tek ekip, tek ürün, hızlı iterasyon önceliği var.
- Buna rağmen modüller arası sınırların net kalması gerekiyor (bkz. [docs/architecture.md](../architecture.md)) — mimari, olası büyümeye karşı kendini kilitlememeli.
- Mikroservisler bağımsız ölçekleme, ayrı ekip sahipliği ve teknoloji çeşitliliği kazandırır; ama bunların hiçbiri şu an gerçek bir ihtiyaç değil. Buna karşılık ağ sınırı, dağıtık transaction yönetimi, servisler arası sürüm uyumu ve orkestrasyon gibi kalıcı maliyetler getirir — tek kişilik/tek ekiplik bir ürünün hızını doğrudan keser.
- Sınırsız (modülsüz) klasik bir monolith ise kısa vadede en hızlı yol gibi görünür; ama sınır disiplini kod dışına (derleme zamanı, şema) dışsallaştırılmadığı sürece birkaç ay içinde modüller arası bağımlılık "çamur"a döner — hem bugünkü testi hem de gelecekteki olası bölünmeyi pahalılaştırır.

### Karşılaştırma

| | Sınırsız Monolith | Modüler Monolith | Mikroservisler |
|---|---|---|---|
| Dağıtım karmaşıklığı | Basit (tek deploy) | Basit (tek deploy) | Yüksek (N deploy + orkestrasyon) |
| Sınır disiplini | Zayıf, zamanla erir | Net — derleme + şema zorunlu kılar | Net — ağ sınırı zorunlu kılar |
| Tek ekip için uygunluk | Uygun | Uygun, büyümeye de hazır | Gereksiz karmaşıklık |
| İterasyon hızı (bugünkü ölçek) | Yüksek | Yüksek | Düşük (ağ, sürüm uyumu, dağıtık debug) |
| Bağımsız ölçekleme | Yok | Yok (ama sınır sayesinde sonradan mümkün) | Var — asıl kazanç burada |
| Operasyonel yük | Düşük | Düşük | Yüksek (izleme, log toplama, ağ hataları) |
| Yanlış çizilmiş sınırı düzeltme maliyeti | Yüksek (sınır zaten yok) | Düşük — kod içi refactor, ağ sınırı yok | Çok yüksek — servisler arası taşıma |

En pahalı hata, sınırı ağ seviyesinde yanlış çizmektir; modüler monolith bu hatayı ucuza (kod içi refactor ile) düzeltme imkânı tanır çünkü henüz bir ağ sınırı yoktur.

## Karar

**Modüler monolith** ile ilerlenir: tek deployable, tek veritabanı sunucusu — ama her modül kendi şeması ve kendi `Contracts` yüzeyiyle bugünden net sınırlıdır (bkz. [docs/architecture.md](../architecture.md)).

Bölünme ihtiyacı bugün yok. Bölünme, aşağıdaki sinyallerden biri **gerçek ve ölçülebilir** hale geldiğinde değerlendirilir — ve yapıldığında var olan modül sınırından yapılır; yeni bir sınır icat edilmez, mevcut olan ağ sınırına terfi ettirilir.

### Bölünmeyi Tetikleyecek Sinyaller

1. **Bağımsız ölçekleme ihtiyacı** — bir modülün kaynak tüketimi diğerlerinden kopuk şekilde büyüyor (ör. Content Delivery'nin video işleme yükü tüm uygulamayı CPU/bellek açısından domine ediyor) ve geri kalanı gereksiz yere onunla birlikte scale ediliyor.
2. **Ayrı ekip sahipliği** — bir modülü sürekli geliştiren, kendi release ritmine ihtiyaç duyan ayrı bir ekip oluştu.
3. **Deploy sıklığı çakışması** — bir modül günde onlarca kez deploy edilmek isterken bir başkası ayda bir; ortak pipeline ikisini de yavaşlatıyor.
4. **Farklı runtime/teknoloji ihtiyacı** — bir modül (ör. video transcoding) .NET dışında bir runtime/ekosistem gerektiriyor.
5. **Blast radius** — bir modüldeki hata veya yük artışı (memory leak, uzun sorgu) tüm uygulamayı aşağı çekiyor; izolasyon ihtiyacı uptime baskısı yaratıyor.
6. **Uyum/güvenlik izolasyonu** — bir modülün (ör. Payments) ayrı bir güvenlik/uyum sınırına (PCI kapsamı, ayrı ağ segmenti) girmesi gerekiyor.
7. **Sınırın fiilen delinmeye başlanması** — geliştiriciler sürekli "sadece bu sefer" diyerek [Yasak Liste](../architecture.md#yasak-liste)'yi ihlal etmek zorunda kalıyor; bu, sınırın yanlış çizildiğinin ya da modülün gerçekten ayrılması gerektiğinin işaretidir.
8. **Build/test süresi** — tek deploy edilebilir birimin derleme + test süresi iterasyonu görünür şekilde yavaşlatıyor.

## Sonuçlar

**İyi**
- Tek deploy, tek DB → düşük operasyonel yük, tek ekip profiline uygun hızlı iterasyon.
- Modül sınırı bugünden var (Contracts, ayrı şema, Yasak Liste) → bölünme gerektiğinde mimari yeniden tasarım değil, mekanik taşıma işi olur.
- Sınır ihlalleri code review'da yakalanabilir (derleme zamanı + şema) — disiplin kişiye değil sisteme dışsallaştırılmış.

**Bedel**
- Bağımsız ölçekleme yok — bir modülün yük artışı tüm uygulamayı etkileyebilir. Kabul edilen risk: bugünkü ölçekte gerçek değil, sinyal 1 ve 5 izleniyor.
- Tek DB sunucusu tüm modüller için tek nokta — kapasite büyüdükçe izlenmeli.
- "Sınır kağıt üzerinde kalır, kimse uymaz" riski var → bu risk [docs/architecture.md](../architecture.md) Yasak Liste'si ve code review disipliniyle karşılanıyor; sinyal 7 bu riskin fiilen gerçekleştiğinin göstergesi.

## Değerlendirilen Alternatifler

1. **Sınırsız monolith (modülsüz)** — Reddedildi. En hızlı başlangıç gibi görünür, ama sınır disiplini yoksa birkaç ay içinde bağımlılıklar birbirine geçer; hem bugünkü test hem de gelecekteki olası bölünme maliyeti katlanarak büyür.
2. **Baştan mikroservisler** — Reddedildi. Tek ekip + tek ürün + hızlı iterasyon profiliyle uyuşmuyor; ağ sınırı, dağıtık transaction, orkestrasyon ve sürüm uyumu maliyeti bugünkü ölçekteki kazanımından fazla. Ayrıca sınırlar henüz kod içi refactor ile ucuza düzeltilebilecekken, servisler arası yanlış çizilmiş bir sınırı sonradan taşımak çok daha pahalıya mal olur.
3. **Modüler monolith** — Seçildi. Bugünün hızını korur, yarının bölünmesini ucuzlaştırır.
