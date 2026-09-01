# Spec frontend-001 — Ürün Listesi Ekranı (Kabuk + Kart Grid'i)

| Alan | Değer |
|---|---|
| Spec No | frontend-001 |
| Durum | Onaylandı |
| Modül(ler) | Frontend (Catalog ekranı) — bkz. docs/frontend.md |
| Branch | feature/frontend-001-urun-listesi |
| Sahip | Belirlenecek (Takım Yöneticisi atar) |

## Intent

Öğrenci/ziyaretçinin katalogdaki kursları görebilmesi için bir ekran gerekiyor; bu dilim, [specs/0001-urun-listeleme.md](0001-urun-listeleme.md)'de tanımlanan sayfalama ve pasif ürün görünmezliği davranışlarının ekran karşılığını verir. Aynı zamanda sonraki tüm ekran dilimlerinin üzerine oturacağı uygulama kabuğunu (üst bar + içerik alanı) kurar. Başarı: kullanıcı ürünleri kartlar halinde sayfalar arasında gezinerek görebiliyor; katalog boşken veya istek hata verdiğinde ekran sessiz kalmıyor, kullanıcıya ne olduğunu ve ne yapabileceğini net biçimde anlatıyor. Kategoriye göre daraltma etkileşimi (filtre kontrolü) bilinçli olarak bu dilimin kapsamı dışında — kart üzerinde kategori bilgisi yalnızca görüntülenir, henüz filtrelenemez.

## Requirements

- Uygulama kabuğu: üst bar (marka + gezinme) ve içerik alanından oluşur; sonraki ekran dilimleri içerik alanına yerleşir, kabuk değişmeden yeniden kullanılır.
- Ürün listesi, içerik alanında kart grid'i olarak görüntülenir; her kart görsel alanı, ürün adı, kategori rozeti ve fiyat bilgisini birlikte gösterir.
- Sayfalama kontrolü ekranda görünür ve çalışır — kullanıcı sayfalar arasında geçiş yapabilir; ekran, [specs/0001-urun-listeleme.md](0001-urun-listeleme.md)'deki "parametresiz istek/varsayılan sayfa" davranışıyla varsayılan (ilk) sayfa açık gelecek şekilde başlar.
- Yalnızca yayınlanmış (aktif) ürünler kart olarak görünür; [specs/0001-urun-listeleme.md](0001-urun-listeleme.md)'deki pasif ürün görünmezliği kuralının ekran karşılığı olarak, taslak/pasif ürün hiçbir sayfada render edilmez.
- Ekran üç durumu da tasarlar (bkz. docs/frontend.md §Üç Durum Kuralı):
  - **Yükleme:** veri gelene kadar skeleton görünümü gösterilir.
  - **Boş katalog:** [specs/0001-urun-listeleme.md](0001-urun-listeleme.md)'deki `totalCount: 0` durumunun ekran karşılığı — "henüz ürün yok" mesajı ve kullanıcıyı yönlendiren bir eylem içeren boş durum gösterilir.
  - **Hata:** istek başarısız olduğunda Türkçe, eylem önerili bir mesaj ("... tekrar dene") gösterilir; teknik detay (stack trace, HTTP durum kodu, exception mesajı) ekrana hiçbir şekilde sızmaz.

## Constraints

- Ekranda kullanılan bileşenler proje Bileşen Envanteri'nden gelir (`Button`, `Card`, `Badge`, `EmptyState`, `ErrorState`, `Skeleton` — bkz. docs/frontend.md §Tasarım Sistemi); envanterde karşılığı olmayan yeni bir bileşen ihtiyacı doğarsa sessizce türetilmez, önerilir (Öneri Kuralı — karar Takım Yöneticisi'nde).
- Renk/aralık/tipografi gömülü sabit değer (hex/px) olarak yazılmaz, tasarım tokenlarından gelir.
- KAPSAM DIŞI: arama.
- KAPSAM DIŞI: filtre tasarımı — kategori rozeti kartta görüntülenir ama filtreleme etkileşimi bu dilimde yok.
- KAPSAM DIŞI: tema değiştirme.

## Context

- Ana spec: [specs/0001-urun-listeleme.md](0001-urun-listeleme.md) — bu ekran, o spec'in şu Acceptance Criteria'larının görsel/etkileşim karşılığıdır: "Parametresiz istek gönderildiğinde varsayılan sayfa ile sonuç döner", "Katalogda hiç kurs yokken... boş bir liste ve totalCount: 0 ile döner", "Yayınlanmamış (taslak) durumdaki hiçbir kurs... listede görünmez".
- Standart: docs/frontend.md — Dil/Yapı, Tasarım Sistemi, Üç Durum Kuralı, Kalite Kapıları bu spec'e birebir uygulanır.
- Süreç: docs/frontend.md §Süreç uyarınca bu bir mini-spec'tir; görsel kanıt VERIFY'ın parçasıdır (bkz. docs/roles/qa.md).

## Acceptance Criteria

- [x] Ürünler yüklenirken ekranda skeleton (yükleme) görünümü gösterilir. — Otomatik test kanıtı: `CatalogPage.test.tsx` "shows skeleton while loading..." (ekran görüntüsü değil — yükleme çok hızlı geçtiği için canlı yakalamak güvenilir değildi, bkz. DoD notu).
- [x] Katalogda ürün yokken (`totalCount: 0`) kart grid'i boş/sessiz kalmaz; "henüz ürün yok" mesajı ve yönlendirici bir eylem içeren boş durum (EmptyState) gösterilir. — Otomatik test kanıtı: `CatalogPage.test.tsx` "shows empty state..." (ekran görüntüsü yok, bkz. DoD notu).
- [x] API isteği hata döndüğünde Türkçe, eylem önerili bir hata mesajı ve "tekrar dene" eylemi içeren hata durumu (ErrorState) gösterilir; teknik hata metni ekrana yansımaz. — Canlı ekran görüntüsü + otomatik test (`CatalogPage.test.tsx` "shows error state without leaking technical details...").
- [x] Ekran ilk açıldığında parametre verilmeden varsayılan (ilk) sayfa gösterilir. — Canlı ekran görüntüsü (gerçek Postgres'e karşı çalışan backend, "Sayfa 1 / 1").
- [x] Kullanıcı sayfalama kontrolüyle sonraki/önceki sayfaya geçebilir; grid, geçilen sayfanın ürünleriyle güncellenir. — Otomatik test kanıtı: `CatalogPage.test.tsx` "navigates to the next page...". Canlı ekran görüntüsünde yalnızca buton disabled/enabled sınır durumu görüldü (6 tohum kursu tek sayfaya sığdığından gerçek sayfa geçişi canlı yakalanmadı).
- [x] Listede yalnızca yayınlanmış (aktif) ürünler kart olarak görünür; taslak/pasif ürün hiçbir sayfada görünmez. — Canlı ekran görüntüsü: 7 tohumlanan kurstan (1 taslak) yalnızca 6 yayınlanmış olan listelendi.
- [x] Her ürün kartında görsel alanı, ürün adı, kategori rozeti ve fiyat bilgisi birlikte görünür. — Canlı ekran görüntüsü (görsel alanı yapısal olarak var; `picsum.photos` kapak URL'leri bu sandboxed ortamda internet erişimi olmadığı için yüklenmedi — uygulama hatası değil).
- [x] Üst bar marka alanını ve gezinmeyi gösterir; içerik alanı ürün listesini barındırır. — Canlı ekran görüntüsü.

## Definition of Done

- [ ] Her Acceptance Criteria için ekran görüntüsü kanıtı eklendi. — **Kısmen**: 5/8 AC için canlı ekran görüntüsü var (hata durumu, varsayılan sayfa, taslak gizliliği, kart alanları, kabuk); skeleton/boş durum/sayfa-geçişi için yalnızca otomatik test kanıtı var, ayrı ekran görüntüsü yok (skeleton çok hızlı geçiyor, boş durum için tohum veriyi geçici silmek gerekirdi — zaman/kapsam nedeniyle atlandı). İstenirse tamamlanır.
- [x] Tip kontrolü (`tsc --noEmit`) temiz.
- [x] ESLint temiz.
- [x] Prettier (kontrol modu) temiz.
- [x] Kritik akış smoke testi (Vitest + Testing Library) yeşil — 4/4 test.
- [x] Commit'ler docs/git.md formatına uygun, plan atıflı — `feature/frontend-001-urun-listesi` branch'inde 7 commit.
- [x] PR şablonu dolduruldu, CI yeşil — Takım Yöneticisi talimatıyla doğrudan `master`'a merge edildi (GitHub PR akışı bu spec için atlandı); repoda henüz otomatik CI pipeline'ı yok, yeşillik elle doğrulandı.
- [x] Bu spec specs/done/ klasörüne taşındı

## Scorecard

| Metrik | Değer |
|---|---|
| Spec revizyon sayısı | 1 — Durum alanı Taslak → Onaylandı (bu oturumda); Requirements/AC içeriği değiştirilmedi. |
| Düzeltme turu sayısı | 0 — QA review turu henüz yapılmadı. |
| Bulgu gerçek/gürültü oranı | - (QA turu henüz yapılmadı) |
| Regresyon sayısı | 0 |
| Kaçan hata (production'da bulunan) | - (henüz merge/deploy edilmedi) |
