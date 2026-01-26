import { defineStore } from 'pinia'
import axiosClient from '@/api/axiosClient'

export const useCourtStore = defineStore('court', {
  state: () => ({
    courts: [],
    currentCourt: null,
    loading: false,
    error: null
  }),

  getters: {
    availableCourts: (state) => state.courts.filter(court => court.isAvailable)
  },

  actions: {
    async fetchCourts() {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.get('/courts')
        if (response.data.success) {
          this.courts = response.data.data
        } else {
          this.error = response.data.message
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tải danh sách sân'
        console.error('Error fetching courts:', error)
      } finally {
        this.loading = false
      }
    },

    async fetchCourtById(id) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.get(`/courts/${id}`)
        if (response.data.success) {
          this.currentCourt = response.data.data
        } else {
          this.error = response.data.message
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tải thông tin sân'
        console.error('Error fetching court:', error)
      } finally {
        this.loading = false
      }
    },

    async createCourt(courtData) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.post('/courts', courtData)
        if (response.data.success) {
          this.courts.push(response.data.data)
          return { success: true, data: response.data.data }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tạo sân'
        console.error('Error creating court:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    },

    async updateCourt(id, courtData) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.put(`/courts/${id}`, courtData)
        if (response.data.success) {
          const index = this.courts.findIndex(c => c.id === id)
          if (index !== -1) {
            this.courts[index] = response.data.data
          }
          return { success: true, data: response.data.data }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi cập nhật sân'
        console.error('Error updating court:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    },

    async deleteCourt(id) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.delete(`/courts/${id}`)
        if (response.data.success) {
          this.courts = this.courts.filter(c => c.id !== id)
          return { success: true }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi xóa sân'
        console.error('Error deleting court:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    }
  }
})
