<template>
  <div class="relative">
    <button
      @click="toggleDropdown"
      class="relative p-2 text-gray-600 hover:text-gray-900 focus:outline-none"
    >
      <svg
        class="w-6 h-6"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
        />
      </svg>
      <span
        v-if="notificationStore.unreadCount > 0"
        class="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/2 -translate-y-1/2 bg-red-600 rounded-full"
      >
        {{ notificationStore.unreadCount > 99 ? '99+' : notificationStore.unreadCount }}
      </span>
    </button>

    <!-- Dropdown -->
    <div
      v-if="isOpen"
      class="absolute right-0 mt-2 w-80 bg-white rounded-lg shadow-xl z-50 border"
    >
      <div class="p-4 border-b flex justify-between items-center">
        <h3 class="font-semibold">Thông báo</h3>
        <button
          v-if="notificationStore.unreadCount > 0"
          @click="markAllAsRead"
          class="text-xs text-blue-600 hover:underline"
        >
          Đánh dấu tất cả đã đọc
        </button>
      </div>

      <div class="max-h-96 overflow-y-auto">
        <div v-if="notificationStore.loading" class="p-4 text-center text-gray-500">
          Đang tải...
        </div>

        <div
          v-else-if="notificationStore.notifications.length === 0"
          class="p-4 text-center text-gray-500"
        >
          Không có thông báo
        </div>

        <div v-else>
          <div
            v-for="notification in notificationStore.notifications"
            :key="notification.id"
            @click="handleNotificationClick(notification)"
            :class="[
              'p-4 border-b hover:bg-gray-50 cursor-pointer transition-colors',
              !notification.isRead ? 'bg-blue-50' : ''
            ]"
          >
            <div class="flex items-start">
              <div
                :class="[
                  'w-2 h-2 mt-2 rounded-full mr-3',
                  !notification.isRead ? 'bg-blue-600' : 'bg-gray-300'
                ]"
              ></div>
              <div class="flex-1">
                <p class="font-medium text-sm">{{ notification.title }}</p>
                <p class="text-xs text-gray-600 mt-1">{{ notification.message }}</p>
                <p class="text-xs text-gray-400 mt-1">
                  {{ formatDate(notification.createdAt) }}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="p-2 border-t text-center">
        <button
          @click="viewAll"
          class="text-sm text-blue-600 hover:underline"
        >
          Xem tất cả
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useNotificationStore } from '@/stores/notification'
import { useRouter } from 'vue-router'

const notificationStore = useNotificationStore()
const router = useRouter()
const isOpen = ref(false)

onMounted(() => {
  notificationStore.fetchNotifications()
  notificationStore.requestNotificationPermission()
  
  // Close dropdown when clicking outside
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

const toggleDropdown = (event) => {
  event.stopPropagation()
  isOpen.value = !isOpen.value
}

const handleClickOutside = (event) => {
  const dropdown = event.target.closest('.relative')
  if (!dropdown) {
    isOpen.value = false
  }
}

const handleNotificationClick = async (notification) => {
  if (!notification.isRead) {
    await notificationStore.markAsRead(notification.id)
  }
  
  // Navigate based on notification type
  if (notification.relatedEntityId) {
    switch (notification.type) {
      case 'Booking':
        router.push('/bookings')
        break
      case 'Tournament':
        router.push(`/tournaments/${notification.relatedEntityId}`)
        break
      case 'Match':
        router.push('/referee')
        break
      case 'Transaction':
        router.push('/wallet')
        break
      default:
        break
    }
  }
  
  isOpen.value = false
}

const markAllAsRead = async () => {
  await notificationStore.markAllAsRead()
}

const viewAll = () => {
  isOpen.value = false
  router.push('/notifications')
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  const now = new Date()
  const diff = Math.floor((now - date) / 1000) // seconds

  if (diff < 60) return 'Vừa xong'
  if (diff < 3600) return `${Math.floor(diff / 60)} phút trước`
  if (diff < 86400) return `${Math.floor(diff / 3600)} giờ trước`
  if (diff < 604800) return `${Math.floor(diff / 86400)} ngày trước`
  
  return date.toLocaleDateString('vi-VN')
}
</script>
