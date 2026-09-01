# Spec 0001 — Katalogda Kurs Listeleme ve Kategoriye Göre Daraltma

| Alan | Değer |
|---|---|
| Spec No | 0001 |
| Durum | Onaylandı |
| Modül(ler) | Catalog (M03) — bkz. docs/architecture.md |
| Branch | feature/0001-urun-listeleme |
| Sahip | Belirlenecek (Takım Yöneticisi atar) |

## Intent

Katalogda kayıtlı kurs sayısı arttıkça öğrenci/ziyaretçi, düz bir liste içinden ilgilendiği kursu bulmakta zorlanıyor; sayfalar halinde görüntüleme ve kategoriye göre daraltma bu keşif sürecini hızlandırıyor. Bu ihtiyacı kursu keşfetmek isteyen öğrenci ve giriş yapmamış ziyaretçi duyuyor — katalog tarama satın alma öncesi bir keşif adımı olduğundan herkese açık. Başarı: kullanıcı yalnızca yayınlanmış (aktif) kursları sayfalar halinde görebiliyor ve istediği kategoriyi seçtiğinde liste yalnızca o kategoriye ait kurslarla daralıyor; kategori boşsa kullanıcı net bir "kurs yok" bilgisi görüyor, hata almıyor. Bilinçli olarak bu feature'ın kapsamı dışında: serbest metin arama, fiyata/puana göre sıralama ve birden fazla kategoriyi aynı anda seçme — bunlar ayrı bir ihtiyaç olarak değerlendirilecek.

## Requirements

- Kullanıcı, katalogdaki yayınlanmış (aktif) kursların listesini sayfalar halinde görüntüleyebilir.
- Kullanıcı, bir kategori seçtiğinde liste yalnızca o kategoriye ait kurslarla daralır.
- Yayınlanmamış (taslak) kurslar hiçbir koşulda listede görünmez.
- Katalog tarama işlemi giriş yapmamış ziyaretçiler dahil herkese açıktır.
- Seçilen kategoride kurs bulunmuyorsa kullanıcıya kurs bulunmadığı açıkça bildirilir; bu bir hata durumu değildir.
- Sayfa numarası, mevcut sonuç aralığının dışında istenirse (ör. toplam sayfa sayısından büyük) kullanıcı hata almaz; boş bir sonuç kümesi ve doğru toplam sayı bilgisiyle karşılaşır.
- Geçersiz sayfalama değeri (ör. negatif, sıfır veya sayısal olmayan sayfa/sayfa boyutu) verildiğinde istek reddedilmez; sistem varsayılan değerlere döner. *(V1 varsayımı — önceden netleşmemiş, ihtiyaç doğarsa değiştirilebilir.)*
- `page` ve `pageSize` birbirinden bağımsız doğrulanır: yalnızca geçersiz olan parametre kendi varsayılanına döner, diğeri kullanıcının verdiği geçerli değerde kalır. *(Netleştirme: 2026-08-21.)*
- `pageSize` için alt sınır 1'dir; 0, negatif veya sayısal olmayan `pageSize` değeri geçersiz sayılır ve `page` ile aynı kurala tabi olarak varsayılana (20) döner. *(Netleştirme: 2026-08-21.)*
- Geçersiz formatlı `categoryId` (ör. GUID olmayan bir değer) verildiğinde istek reddedilmez; sistem bunu "kategoride kurs yok" durumuyla aynı şekilde ele alır ve boş liste + `totalCount: 0` döner. *(Netleştirme: 2026-08-21.)*
- Liste, yayın tarihine göre en yeniden eskiye sıralanır (`PublishedAt DESC`); bu sıralama sayfalar arası tutarlılığın (aynı kursun tekrar/kayıp görünmemesi) temelidir. *(Netleştirme: 2026-08-21.)*
- Liste öğesinde en az şu alanlar döner: kurs kimliği, başlık, liste fiyatı, kategori adı, eğitmen adı, kapak görseli URL'i. Başka modüle ait alanlar (ör. Reviews'a ait puan/yorum sayısı) V1 kapsamı dışındadır. *(Netleştirme: 2026-08-21.)*
- Kategori taksonomisi V1'de tek seviyelidir ve bir kurs tek bir kategoriye aittir. *(V1 varsayımı, bkz. docs/domain.md kategori taksonomisi.)*

## Constraints

- Sayfa boyutu varsayılanı 20, üst sınırı 100, alt sınırı 1'dir.
- p95 yanıt süresi hedefi < 200ms; bu hedef referans veri hacminde ölçülür: 10.000 kurs, 50 kategori. *(Netleştirme: 2026-08-21.)*
- N+1 sorgu deseni yasaktır (bkz. docs/architecture.md).
- Uygulama minimal API yaklaşımıyla gerçekleştirilir.
- Kapsam dışı (V1): metin arama, fiyat filtresi, kullanıcının seçebileceği sıralama seçenekleri (sistemin sabit varsayılan sıralaması — yayın tarihi — Requirements'ta tanımlıdır, kullanıcı tarafından değiştirilemez).
- Kapsam dışı (V1): aynı anda birden fazla kategori seçimi, kategori hiyerarşisi (alt kategori).

## Context

- Modül: Catalog (M03) — kurs, kategori taksonomisi ve yayın durumu iş akışının tek otoritesi (bkz. docs/architecture.md).
- Yayın Durumu (`PublicationStatus`) kavramı için bkz. docs/domain.md — yalnızca `Published` durumundaki kurslar bu listede yer alır.
- Önceki/bağlı spec yok — bu, specs/ altındaki ilk spec.

## Acceptance Criteria

- [x] Parametresiz istek gönderildiğinde varsayılan sayfa (1) ve varsayılan sayfa boyutu (20) ile sonuç döner. — `CourseCatalogQueryTests.Create_NoParams_DefaultsToPage1AndPageSize20`, `CourseCatalogQueriesTests.GetPublishedCourses_NoParams_ReturnsFirstDefaultPage`
- [x] `page` ve `pageSize` birlikte verildiğinde belirtilen sayfa/boyuta göre sonuç döner. — `CourseCatalogQueriesTests.GetPublishedCourses_ValidPageAndPageSize_ReturnsRequestedSlice`
- [x] `pageSize` üst sınır olan 100'den büyük istenirse sonuç 100 ile sınırlandırılır, istek reddedilmez. — `CourseCatalogQueryTests.Create_PageSizeAboveMax_ClampsTo100`, `CourseCatalogQueriesTests.GetPublishedCourses_PageSizeAbove100_LimitsTo100Items`
- [x] Katalogda hiç kurs yokken yapılan istek, boş bir liste ve `totalCount: 0` ile döner (hata değil). — `CourseCatalogQueriesTests.GetPublishedCourses_EmptyCatalog_ReturnsEmptyResultWithZeroTotal`
- [x] `categoryId` ile filtre uygulandığında sonuç listesinde yalnızca o kategoriye ait kurslar yer alır. — `CourseCatalogQueriesTests.GetPublishedCourses_WithCategoryFilter_ReturnsOnlyMatchingCategory`
- [x] Var olmayan veya o kategoride hiç kurs olmayan bir `categoryId` verildiğinde boş liste ve `totalCount: 0` döner (hata değil). — `CourseCatalogQueriesTests.GetPublishedCourses_CategoryWithNoCourses_ReturnsEmptyResult`
- [x] Geçersiz sayfa parametresi (ör. `page=0`, `page=-1`, sayısal olmayan değer) verildiğinde istek reddedilmez, varsayılan değerlere dönülür. — `CourseCatalogQueryTests.Create_InvalidPage_DefaultsToPage1`, `CatalogEndpointsTests.Get_InvalidPage_RequestNotRejected_DefaultsToPage1`
- [x] Geçersiz `pageSize` parametresi (ör. `pageSize=0`, `pageSize=-1`, sayısal olmayan değer) verildiğinde istek reddedilmez, `pageSize` varsayılan değere (20) döner. — `CourseCatalogQueryTests.Create_InvalidPageSize_DefaultsToPageSize20`, `CatalogEndpointsTests.Get_InvalidPageSize_RequestNotRejected_DefaultsToPageSize20`
- [x] `page` geçerli ama `pageSize` geçersiz verildiğinde (veya tersi), yalnızca geçersiz olan parametre varsayılana döner; diğeri kullanıcının verdiği geçerli değerde kalır. — `CourseCatalogQueryTests.Create_OnlyPageInvalid_KeepsProvidedValidPageSize`, `Create_OnlyPageSizeInvalid_KeepsProvidedValidPage`
- [x] Toplam sonuç sayısından büyük bir sayfa numarası istendiğinde (aralık dışı sayfa) hata dönmez; boş liste ve doğru `totalCount` ile sonuç döner. — `CourseCatalogQueriesTests.GetPublishedCourses_PageBeyondRange_ReturnsEmptyItemsWithCorrectTotalCount`
- [x] Geçersiz formatlı `categoryId` (GUID olmayan bir değer) verildiğinde istek reddedilmez; boş liste ve `totalCount: 0` döner (hata değil). — `CourseCatalogQueryTests.Create_CategoryIdInvalidFormat_MarksFilterInvalid`, `CourseCatalogQueriesTests.GetPublishedCourses_InvalidCategoryIdFormat_ReturnsEmptyResult`, `CatalogEndpointsTests.Get_InvalidCategoryIdFormat_RequestNotRejected_ReturnsEmptyResult`
- [x] Yayınlanmamış (taslak) durumdaki hiçbir kurs, hiçbir sayfada ve hiçbir kategori filtresinde listede görünmez. — `CourseCatalogQueriesTests.GetPublishedCourses_DraftCourses_NeverAppearInResults`
- [x] Aynı filtre/sayfalama parametreleriyle art arda yapılan istekler, listeyi her seferinde yayın tarihine göre en yeniden eskiye aynı sırada döner (sayfalar arası kayma/tekrar olmaz). — `CourseCatalogQueriesTests.GetPublishedCourses_ConsecutiveRequests_ReturnConsistentOrderAcrossPages`
- [x] Liste öğesi en az şu alanları içerir: kurs kimliği, başlık, liste fiyatı, kategori adı, eğitmen adı, kapak görseli URL'i. — `CourseCatalogQueriesTests.GetPublishedCourses_ReturnsRequiredFieldsPerItem`

## Definition of Done

- [x] Tüm Acceptance Criteria karşılandı ve teste bağlandı (bkz. docs/testing.md) — 37/37 test yeşil (`dotnet test`)
- [x] Frontend ise: her AC için görsel kanıt eklendi — N/A: bu spec yalnızca backend/API (bkz. plan onayı, madde 4); AC'ler API sözleşmesi dilinde yazılı, ekran içermiyor
- [x] Commit'ler docs/git.md formatına uygun, plan atıflı — `feature/0001-urun-listeleme` branch'inde 10 commit, `[plan 0001/N]` atıflı
- [x] PR şablonu dolduruldu, CI yeşil — PR #1 Takım Yöneticisi tarafından GitHub'da merge edildi (`d7e02e1`). Repoda henüz otomatik CI pipeline'ı yok; yeşillik `dotnet build`/`dotnet test`'in elle çalıştırılmasıyla doğrulandı.
- [x] Bu spec specs/done/ klasörüne taşındı

## Scorecard

| Metrik | Değer |
|---|---|
| Spec revizyon sayısı | 1 — spec bu depoya doğrudan "Onaylandı" durumunda girdi; git geçmişinde daha önceki taslak revizyonu yok. Spec metnindeki tek görünür kanıt: 2026-08-21 tarihli bir netleştirme turu (Requirements'ta 4, Constraints'te 1 madde — toplam 5 satır, tek tarih). Daha erken revizyon olup olmadığı bu depodan doğrulanamaz. |
| Düzeltme turu sayısı | 1 — QA review sonrası düzeltme turu (`[plan 0001/9]`, 3 commit: `a28589f`, `9d1fc78`, `7f04ac3`) |
| Bulgu gerçek/gürültü oranı | 3/3 gerçek, 0 gürültü (yanlış çıkan bulgu yok). Ayrım: 2/3 reprodüksiyonla doğrulandı (page taşması: `page=int.MaxValue` repro'su + regresyon testi; pageSize=1 kapsama boşluğu). 1/3 (orta öncelik, index önerisi) QA'nın kendisinin de belirttiği gibi ölçülmedi — kod incelemesiyle geçerli kabul edilip önlem alındı, ama referans hacimde (10k kurs/50 kategori, gerçek Postgres) EXPLAIN ANALYZE ile doğrulanamadı (bu ortamda çalışan Postgres yok). |
| Regresyon sayısı | 0 — düzeltme turu sonrası tam test paketi (37/37) yeşil; düzeltmeler öncesinde geçen hiçbir test kırılmadı. |
| Kaçan hata (production'da bulunan) | - (henüz merge/deploy edilmedi) |
