import type { ReactNode } from 'react'
import './AppShell.css'

interface AppShellProps {
  children: ReactNode
}

/**
 * Uygulama kabuğu — üst bar (marka + gezinme) + içerik alanı (specs/frontend-001-urun-listesi.md).
 * Sonraki ekran dilimleri `children` olarak buraya yerleşir; kabuk değişmeden yeniden kullanılır.
 */
export function AppShell({ children }: AppShellProps) {
  return (
    <div className="app-shell">
      <header className="app-shell__topbar">
        <span className="app-shell__brand">Academy</span>
        <nav className="app-shell__nav" aria-label="Ana gezinme">
          <a className="app-shell__nav-link" href="/" aria-current="page">
            Kurslar
          </a>
        </nav>
      </header>
      <main className="app-shell__content">{children}</main>
    </div>
  )
}
