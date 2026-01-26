import { defineStore } from 'pinia'
import axiosClient from '@/api/axiosClient'

export const useNotificationStore = defineStore('notification', {
  state: () => ({
    notifications: [],
    unreadCount: 0,
    loading: false,
    error: null,
    connection: null
  }),

  actions: {
    async fetchNotifications() {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.get('/notifications')
        if (response.data.success) {
          this.notifications = response.data.data
          this.unreadCount = this.notifications.filter(n => !n.isRead).length
        } else {
          this.error = response.data.message
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tải thông báo'
        console.error('Error fetching notifications:', error)
      } finally {
        this.loading = false
      }
    },

    async markAsRead(id) {
      try {
        const response = await axiosClient.put(`/notifications/${id}/read`)
        if (response.data.success) {
          const notification = this.notifications.find(n => n.id === id)
          if (notification) {
            notification.isRead = true
            this.unreadCount = Math.max(0, this.unreadCount - 1)
          }
          return { success: true }
        } else {
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        console.error('Error marking notification as read:', error)
        return { success: false, message: error.response?.data?.message }
      }
    },

    async markAllAsRead() {
      try {
        const response = await axiosClient.put('/notifications/read-all')
        if (response.data.success) {
          this.notifications.forEach(n => n.isRead = true)
          this.unreadCount = 0
          return { success: true }
        } else {
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        console.error('Error marking all as read:', error)
        return { success: false, message: error.response?.data?.message }
      }
    },

    addNotification(notification) {
      this.notifications.unshift(notification)
      if (!notification.isRead) {
        this.unreadCount++
      }
    },

    // SignalR connection methods
    initializeSignalR(hubConnection) {
      this.connection = hubConnection
      
      this.connection.on('ReceiveNotification', (notification) => {
        this.addNotification(notification)
        // Show browser notification if permitted
        if ('Notification' in window && Notification.permission === 'granted') {
          new Notification(notification.title, {
            body: notification.message,
            icon: '/favicon.ico'
          })
        }
      })
    },

    async requestNotificationPermission() {
      if ('Notification' in window && Notification.permission === 'default') {
        await Notification.requestPermission()
      }
    }
  }
})
