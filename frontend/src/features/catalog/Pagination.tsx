import { Button } from '../../shared/ui/Button'
import './Pagination.css'

interface PaginationProps {
  page: number
  totalPages: number
  onPrevious: () => void
  onNext: () => void
}

export function Pagination({ page, totalPages, onPrevious, onNext }: PaginationProps) {
  return (
    <nav className="pagination" aria-label="Sayfalama">
      <Button variant="secondary" onClick={onPrevious} disabled={page <= 1}>
        Önceki
      </Button>
      <span className="pagination__status" aria-live="polite">
        Sayfa {page} / {totalPages}
      </span>
      <Button variant="secondary" onClick={onNext} disabled={page >= totalPages}>
        Sonraki
      </Button>
    </nav>
  )
}
