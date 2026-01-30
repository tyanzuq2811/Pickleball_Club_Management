import { ref, reactive, h, render } from 'vue'
import { useToast } from 'vue-toastification'

// Confirm dialog state - global singleton
const confirmState = reactive({
  isOpen: false,
  title: 'Xác nhận',
  message: '',
  type: 'question',
  confirmText: 'Xác nhận',
  cancelText: 'Hủy',
  resolve: null
})

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

export function useConfirmDialog() {
  const toast = useToast()

  return {
    confirmState,
    handleConfirm,
    handleCancel,
    
    // Shorthand methods
    confirm: (message, options = {}) => showConfirm({
      message,
      type: 'question',
      ...options
    }),
    
    confirmDelete: (message, options = {}) => showConfirm({
      title: 'Xác nhận xóa',
      message,
      type: 'danger',
      confirmText: 'Xóa',
      ...options
    }),
    
    confirmWarning: (message, options = {}) => showConfirm({
      title: 'Cảnh báo',
      message,
      type: 'warning',
      ...options
    }),
    
    confirmInfo: (message, options = {}) => showConfirm({
      title: 'Thông tin',
      message,
      type: 'info',
      ...options
    }),

    // Toast wrappers (using vue-toastification)
    success: (message) => toast.success(message),
    error: (message) => toast.error(message),
    warning: (message) => toast.warning(message),
    info: (message) => toast.info(message)
  }
}
