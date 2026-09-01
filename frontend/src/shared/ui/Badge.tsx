import type { HTMLAttributes } from 'react'
import './Badge.css'

/** Bileşen Envanteri — Badge (bkz. docs/frontend.md §Tasarım Sistemi). */
export function Badge({ className, ...rest }: HTMLAttributes<HTMLSpanElement>) {
  const classes = ['badge', className].filter(Boolean).join(' ')
  return <span className={classes} {...rest} />
}
