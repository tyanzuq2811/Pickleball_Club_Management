<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="confirmState.isOpen" class="fixed inset-0 z-[9999] flex items-center justify-center p-4">
        <!-- Backdrop -->
        <div 
          class="absolute inset-0 bg-black/60 backdrop-blur-sm"
          @click="handleCancel"
        ></div>
        
        <!-- Dialog -->
        <div class="relative bg-white dark:bg-gray-800 rounded-2xl shadow-2xl w-full max-w-md transform transition-all animate-modal-in">
          <!-- Icon -->
          <div class="pt-8 pb-4 flex justify-center">
            <div :class="iconContainerClass">
              <!-- Warning Icon -->
              <svg v-if="confirmState.type === 'warning'" class="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
              <!-- Danger Icon -->
              <svg v-else-if="confirmState.type === 'danger'" class="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              <!-- Info Icon -->
              <svg v-else-if="confirmState.type === 'info'" class="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <!-- Success Icon -->
              <svg v-else-if="confirmState.type === 'success'" class="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <!-- Question Icon (default) -->
              <svg v-else class="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
          
          <!-- Content -->
          <div class="px-8 pb-6 text-center">
            <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-3">
              {{ confirmState.title }}
            </h3>
            <p class="text-gray-500 dark:text-gray-400 text-base leading-relaxed">
              {{ confirmState.message }}
            </p>
          </div>
          
          <!-- Actions -->
          <div class="px-8 pb-8 flex gap-4 justify-center">
            <button
              @click="handleCancel"
              class="flex-1 px-6 py-3 text-sm font-semibold text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 rounded-xl transition-all duration-200 hover:shadow-md"
            >
              {{ confirmState.cancelText }}
            </button>
            <button
              @click="handleConfirm"
              :class="confirmButtonClass"
              class="flex-1 px-6 py-3 text-sm font-semibold text-white rounded-xl transition-all duration-200 hover:shadow-lg hover:scale-[1.02]"
            >
              {{ confirmState.confirmText }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { computed } from 'vue'
import { useConfirmDialog } from '@/composables/useConfirmDialog'

const { confirmState, handleConfirm, handleCancel } = useConfirmDialog()

const iconContainerClass = computed(() => {
  const base = 'w-20 h-20 rounded-full flex items-center justify-center'
  switch (confirmState.type) {
    case 'danger':
      return `${base} bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400`
    case 'warning':
      return `${base} bg-amber-100 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400`
    case 'info':
      return `${base} bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400`
    case 'success':
      return `${base} bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400`
    default:
      return `${base} bg-sky-100 dark:bg-sky-900/30 text-sky-600 dark:text-sky-400`
  }
})

const confirmButtonClass = computed(() => {
  switch (confirmState.type) {
    case 'danger':
      return 'bg-red-600 hover:bg-red-700'
    case 'warning':
      return 'bg-amber-600 hover:bg-amber-700'
    case 'info':
      return 'bg-blue-600 hover:bg-blue-700'
    case 'success':
      return 'bg-green-600 hover:bg-green-700'
    default:
      return 'bg-sky-600 hover:bg-sky-700'
  }
})
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .relative,
.modal-leave-to .relative {
  transform: scale(0.9) translateY(10px);
}

@keyframes modal-in {
  0% {
    opacity: 0;
    transform: scale(0.9) translateY(10px);
  }
  100% {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

.animate-modal-in {
  animation: modal-in 0.3s ease-out;
}
</style>
