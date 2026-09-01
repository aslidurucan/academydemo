import type { ReactNode } from 'react'
import './EmptyState.css'

interface EmptyStateProps {
  title: string
  description?: string
  action?: ReactNode
}

/** Bileşen Envanteri — EmptyState (bkz. docs/frontend.md §Üç Durum Kuralı). */
export function EmptyState({ title, description, action }: EmptyStateProps) {
  return (
    <div className="empty-state" role="status">
      <p className="empty-state__title">{title}</p>
      {description && <p className="empty-state__description">{description}</p>}
      {action && <div className="empty-state__action">{action}</div>}
    </div>
  )
}
