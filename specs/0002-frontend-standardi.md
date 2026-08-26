# Spec 0002 — Frontend Profesyonel Standardı

| Alan | Değer |
|---|---|
| Spec No | 0002 |
| Durum | Onaylandı |
| Modül(ler) | Cross-cutting (docs/süreç) — `frontend/` henüz kod içermiyor |
| Branch | feature/0002-frontend-standardi |
| Sahip | Belirlenecek (Takım Yöneticisi atar) |

## Intent

`frontend/` henüz yazılmadan önce, ileride yazılacak her ekranın hangi kurallara göre denetleneceği net olsun isteniyor — bunu isteyen Takım Yöneticisi. Kod yazılmaya başlandığında "bu nasıl yapılmalı" tartışmasının her seferinde yeniden açılmaması, tek bir dosyaya bakılıp karar verilmesi hedefleniyor. Başarı: `docs/frontend.md`, dil/yapı, API disiplini, tasarım sistemi, üç durum kuralı, erişilebilirlik, kalite kapıları ve süreç konularının her birinde net, denetlenebilir bir kural içeriyor; bundan sonraki her frontend PR'ı bu dosyaya göre değerlendiriliyor — kişiye göre değil.

## Requirements

- TypeScript zorunlu kılınır; klasör yapısı (`features/<feature>`, `shared/ui`, `shared/api`) tanımlanır.
- Tüm API isteklerinin tek client'tan geçmesi, DTO'ların backend sözleşmesinden türetilmesi ve endpoint URL'inin bileşen içine yazılamayacağı kural olarak yazılır.
- Tasarım token'ları (renk/aralık/tipografi/köşe) tek dosyada toplanır; gömülü sabit değer (hex/px) yasaklanır; bileşen envanteri (`Button`, `Card`, `Badge`, `EmptyState`, `ErrorState`, `Skeleton`) tanımlanır.
- Her ekranın yükleme/boş/hata durumunu tasarlaması ve hata mesajlarının Türkçe + eylem önerili olması kural olarak yazılır.
- Erişilebilirlik tabanı (semantik HTML, etiketli form, klavye erişimi, kontrast) tanımlanır.
- Kalite kapıları (typecheck, ESLint, Prettier, kritik akış smoke test) ve bunların `package.json` komutlarına bağlanacağı yazılır.
- Sürecin mini-spec (`specs/frontend-XXX`) ve `docs/git.md` ile aynı disiplinde ilerleyeceği, görsel kanıtın VERIFY'ın parçası olduğu yazılır.
- `AGENTS.md`, frontend işinin artık bu dosyaya göre denetlendiğini işaret eder.

## Constraints

- Kapsam dışı: gerçek bir React/Vite iskelet projesinin oluşturulması — bu spec yalnızca kural dokümanını kapsar, kod değil.
- Kapsam dışı: tasarım token değerlerinin (gerçek hex/px sayıları) belirlenmesi — token dosyasının varlığı ve gömülü sabit değer yasağı kural olarak yazılır; somut palet ilk frontend diliminde (`specs/frontend-XXX`) gelir.
- Kapsam dışı: CI pipeline'ının gerçek kurulumu (ör. GitHub Actions) — bu spec yalnızca "hangi kapılar zorunlu" kararını yazıya döker.

## Context

- İlgili: [docs/architecture.md](../docs/architecture.md) (frontend, backend modülleriyle bire bir eşlenmez), [docs/git.md](../docs/git.md) (aynı branch/commit disiplini), [docs/testing.md](../docs/testing.md) (smoke test kuralı), [docs/roles/qa.md](../docs/roles/qa.md) (VERIFY'da görsel kanıt).
- Önceki spec: [specs/0001-urun-listeleme.md](0001-urun-listeleme.md) — backend/API'ye odaklıydı, frontend'e değinmedi; bu spec o boşluğu dolduruyor.

## Acceptance Criteria

- [x] `docs/frontend.md` "Dil ve Yapı" bölümünde TypeScript zorunluluğunu ve `features/`/`shared` klasör yapısını tanımlıyor.
- [x] `docs/frontend.md` "API Disiplini" bölümünde tek client kuralını, DTO türetimini ve endpoint URL'inin bileşen içine yazılamayacağını tanımlıyor.
- [x] `docs/frontend.md` "Tasarım Sistemi" bölümünde token zorunluluğunu, gömülü sabit değer yasağını ve 6 bileşenlik envanteri (`Button`, `Card`, `Badge`, `EmptyState`, `ErrorState`, `Skeleton`) listeliyor.
- [x] `docs/frontend.md` "Üç Durum Kuralı" bölümünde yükleme/boş/hata durumlarını ve Türkçe + eylem önerili hata mesajı kuralını tanımlıyor.
- [x] `docs/frontend.md` "Erişilebilirlik Tabanı" bölümünde semantik HTML, etiketli form, klavye erişimi ve kontrast kurallarını tanımlıyor.
- [x] `docs/frontend.md` "Kalite Kapıları" bölümünde typecheck/ESLint/Prettier zorunluluğunu ve kritik akış smoke test kuralını (Vitest + Testing Library) tablo halinde tanımlıyor.
- [x] `docs/frontend.md` "Süreç" bölümünde mini-spec akışını, görsel kanıtın VERIFY'ın parçası olduğunu ve `docs/git.md` ile aynı branch/commit disiplinini tanımlıyor.
- [x] `AGENTS.md`, Çalışma Disiplini altında frontend işinin `docs/frontend.md` standardına göre denetlendiğini belirten bir satır içeriyor.

## Definition of Done

- [x] Tüm Acceptance Criteria karşılandı — doğrulama code review ile yapıldı (bu spec docs-only olduğu için otomatik teste bağlanacak kod yok, bkz. docs/testing.md)
- [ ] Frontend ise: her AC için görsel kanıt eklendi — N/A: bu spec bir kural dokümanı, henüz ekran yok
- [x] Commit'ler docs/git.md formatına uygun, plan atıflı — `feature/0002-frontend-standardi` branch'inde 3 commit, `[plan 0002/N]` atıflı
- [ ] PR şablonu dolduruldu, CI yeşil — branch henüz push edilmedi (push yetkisi Takım Yöneticisi'nde, bkz. docs/git.md §AI Kuralı)
- [ ] Bu spec specs/done/ klasörüne taşındı — PR merge sonrası yapılacak

## Scorecard

| Metrik | Değer |
|---|---|
| Spec revizyon sayısı | 1 — spec bu depoya doğrudan "Onaylandı" durumunda giriyor; içerik önceki oturumda Takım Yöneticisi'yle birebir konuşularak netleşti. |
| Düzeltme turu sayısı | 0 |
| Bulgu gerçek/gürültü oranı | - (QA turu henüz yapılmadı) |
| Regresyon sayısı | 0 |
| Kaçan hata (production'da bulunan) | - (henüz merge/deploy edilmedi) |
