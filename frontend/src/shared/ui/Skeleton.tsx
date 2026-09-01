import './Skeleton.css'

interface SkeletonProps {
  width?: string
  height?: string
  className?: string
}

/** Bileşen Envanteri — Skeleton, yükleme durumu için (bkz. docs/frontend.md §Üç Durum Kuralı). */
export function Skeleton({ width = '100%', height = '1rem', className }: SkeletonProps) {
  const classes = ['skeleton', className].filter(Boolean).join(' ')
  // Ekran okuyucular için görsel gürültü değil — gerçek içerik yerine geçtiği için gizli.
  return <div className={classes} style={{ width, height }} aria-hidden="true" />
}
