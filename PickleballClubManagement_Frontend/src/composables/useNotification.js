import { ref, reactive } from 'vue'

// Toast state
const toasts = ref([])
let toastId = 0

// Confirm dialog state
const confirmState = reactive({
  isOpen: false,
  title: 'Xác nhận',
  message: '',
  type: 'question',
  confirmText: 'Xác nhận',
  cancelText: 'Hủy',
  resolve: null
})

// Toast functions
const addToast = (options) => {
  const id = ++toastId
  const toast = {
    id,
    type: options.type || 'info',
    title: options.title || '',
    message: options.message || '',
    duration: options.duration ?? 3000
  }
  
  toasts.value.push(toast)
  
  if (toast.duration > 0) {
    setTimeout(() => {
      removeToast(id)
    }, toast.duration)
  }
  
  return id
}

const removeToast = (id) => {
  const index = toasts.value.findIndex(t => t.id === id)
  if (index > -1) {
    toasts.value.splice(index, 1)
  }
}

// Confirm dialog function
const showConfirm = (options) => {
  return new Promise((resolve) => {
    confirmState.isOpen = true
    confirmState.title = options.title || 'Xác nhận'
    confirmState.message = options.message || 'Bạn có chắc chắn muốn thực hiện hành động này?'
    confirmState.type = options.type || 'question'
    confirmState.confirmText = options.confirmText || 'Xác nhận'
    confirmState.cancelText = options.cancelText || 'Hủy'
    confirmState.resolve = resolve
  })
}

const handleConfirm = () => {
  confirmState.isOpen = false
  if (confirmState.resolve) {
    confirmState.resolve(true)
    confirmState.resolve = null
  }
}

const handleCancel = () => {
  confirmState.isOpen = false
  if (confirmState.resolve) {
    confirmState.resolve(false)
    confirmState.resolve = null
  }
}

export function useToast() {
  return {
    toasts,
    success: (message, title = '') => addToast({ type: 'success', message, title }),
    error: (message, title = '') => addToast({ type: 'error', message, title }),
    warning: (message, title = '') => addToast({ type: 'warning', message, title }),
    info: (message, title = '') => addToast({ type: 'info', message, title }),
    remove: removeToast
  }
}

export function useConfirm() {
  return {
    confirmState,
    confirm: showConfirm,
    handleConfirm,
    handleCancel
  }
}
