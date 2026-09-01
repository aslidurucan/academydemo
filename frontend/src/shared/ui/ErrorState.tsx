import { Button } from './Button'
import './ErrorState.css'

interface ErrorStateProps {
  /**
   * Yalnızca sabit, iş diliyle yazılmış Türkçe metin geçilir — teknik hata/istisna mesajı,
   * HTTP durum kodu ASLA buraya taşınmaz (docs/frontend.md §Üç Durum Kuralı).
   */
  message?: string
  onRetry: () => void
}

const DEFAULT_MESSAGE = 'Kurslar yüklenemedi. Lütfen tekrar deneyin.'

/** Bileşen Envanteri — ErrorState (bkz. docs/frontend.md §Üç Durum Kuralı). */
export function ErrorState({ message = DEFAULT_MESSAGE, onRetry }: ErrorStateProps) {
  return (
    <div className="error-state" role="alert">
      <p className="error-state__message">{message}</p>
      <Button variant="secondary" onClick={onRetry}>
        Tekrar dene
      </Button>
    </div>
  )
}
