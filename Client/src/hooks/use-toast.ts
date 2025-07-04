// toast.tsx
import { toast as sonnerToast } from "sonner"

type ToastOptions = {
  title: string
  description?: string
  action?: {
    label: string
    onClick: () => void
  }
}

export function toast({ title, description, action }: ToastOptions) {
  return sonnerToast(title, {
    description,
    action,
  })
}
