# frontend

React + Vite + TypeScript. Konvansiyonlar için [docs/frontend.md](../docs/frontend.md).

## Komutlar

```bash
npm install
npm run dev          # geliştirme sunucusu (http://localhost:5173)
npm run build         # production build (önce tsc -b, sonra vite build)
npm run typecheck     # tsc -b --noEmit
npm run lint           # ESLint
npm run format:check   # Prettier (kontrol modu)
npm run test            # Vitest (Testing Library)
```

`npm run dev` sırasında `/api` altındaki istekler `vite.config.ts`'deki proxy ile
`http://localhost:5080`'e (Catalog API / `Academy.Host`) yönlendirilir — bkz.
[src/shared/api/client.ts](src/shared/api/client.ts).

## Yapı

```
src/
  features/<feature>/   ekranlar — o feature'a özel bileşen/hook/durum
  shared/
    ui/                  ortak bileşen envanteri (Button, Card, Badge, EmptyState, ErrorState, Skeleton) + tasarım token'ları
    api/                 TEK API client (client.ts) + sözleşmeden türetilen DTO tipleri (types.ts)
```
