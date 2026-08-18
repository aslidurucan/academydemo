# Git Disiplini

> Bkz. [AGENTS.md](../AGENTS.md) altın kural 5. Spec süreci için [specs/TEMPLATE.md](../specs/TEMPLATE.md).

## Branch Stratejisi

- Desen: `feature/<spec-no>-<kısa-ad>` — örn. `feature/0001-kurs-sayfalama`.
- **SPEC'SİZ BRANCH AÇILMAZ.** `specs/` altında karşılık gelen, onaylanmış bir spec dosyası olmadan branch açılmaz.
- `main` her zaman deploy edilebilir durumda kalır.
- Acil prod düzeltmesi (spec'e bağlı olmayan, nadir durum): `hotfix/<kısa-ad>` — yalnızca Takım Yöneticisi onayıyla.
- Spec'e bağlı hata düzeltmesi: `fix/<spec-no>-<kısa-ad>`.

## Commit Formatı

**Conventional Commits** + modül scope + plan atıfı:

```
<tür>(<modül>): <özet> [plan <spec-no>/<adım-no>]
```

Türler: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `ci`.

Örnekler:
```
feat(catalog): sayfalama endpoint'i [plan 0001/3]
fix(ordering): kupon indirim hesaplama hatası [plan 0007/1]
test(enrollment): iade sonrası erişim iptali testi [plan 0003/2]
```

Plan atfı zorunlu: her commit'in hangi spec'in hangi adımını karşıladığı izlenebilir olmalı — code review'da ve sonradan "bu neden yapıldı" sorusuna cevap verir.

## Yasaklar

- `main`'e doğrudan commit — her değişiklik PR üzerinden gelir.
- Force push (`git push --force`) — paylaşılan branch'te asla.
- Geçmiş silme/rewrite (`git filter-branch`, paylaşılan branch'te `rebase -i`) — **geri alma her zaman `git revert` ile yapılır**, geçmiş asla silinmez.
- Squash dışında bir merge stratejisi kullanılması (bkz. altta).

## PR Şartları

- PR şablonu doldurulur: bağlı spec no'su, değişiklik özeti, test kanıtı (backend test çıktısı / frontend ekran görüntüsü — bkz. [docs/testing.md](testing.md)).
- CI pipeline yeşil olmadan merge edilmez (lint + build + test).
- En az 1 onay (Takım Yöneticisi ya da atanan reviewer).
- PR, spec'in Acceptance Criteria'sına birebir karşılık gelir; kapsam dışı değişiklik aynı PR'a eklenmez.

## Squash-Merge

Tüm PR'lar `main`'e **squash-merge** ile girer. Branch üzerindeki ara commit geçmişi kaybolur; PR başlığı tek commit mesajı olarak `main`'e düşer — bu yüzden **PR başlığı da Conventional Commits formatında** yazılır (`feat(catalog): sayfalama endpoint'i [plan 0001/3]`).

## AI Kuralı

- Ajan (AI ekip üyesi) commit attığında yukarıdaki standartlara **birebir** uyar; format atlanmaz/basitleştirilmez.
- Commit mesajını ajan yazar; commit'i onaylayan ve push eden her zaman Takım Yöneticisi'dir — ajan kendi başına `main`'e veya paylaşılan branch'e push etmez.
- Commit mesajının sonuna, kullanılan aracın kimliğiyle bir `Co-Authored-By` satırı eklenir.
