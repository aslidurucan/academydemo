import type { ButtonHTMLAttributes } from 'react'
import './Button.css'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary'
}

/** Bileşen Envanteri — Button (bkz. docs/frontend.md §Tasarım Sistemi). */
export function Button({ variant = 'primary', className, type = 'button', ...rest }: ButtonProps) {
  const classes = ['btn', `btn--${variant}`, className].filter(Boolean).join(' ')
  return <button type={type} className={classes} {...rest} />
}
