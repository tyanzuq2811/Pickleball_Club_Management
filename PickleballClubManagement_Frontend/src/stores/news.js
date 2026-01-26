import { defineStore } from 'pinia'
import axiosClient from '@/api/axiosClient'

export const useNewsStore = defineStore('news', {
  state: () => ({
    newsList: [],
    pinnedNews: [],
    currentNews: null,
    totalPages: 0,
    currentPage: 1,
    loading: false,
    error: null
  }),

  actions: {
    async fetchNews(pageNumber = 1, pageSize = 10) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.get('/news', {
          params: { pageNumber, pageSize }
        })
        if (response.data.success) {
          this.newsList = response.data.data.items
          this.totalPages = response.data.data.totalPages
          this.currentPage = response.data.data.currentPage
        } else {
          this.error = response.data.message
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tải tin tức'
        console.error('Error fetching news:', error)
      } finally {
        this.loading = false
      }
    },

    async fetchPinnedNews() {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.get('/news/pinned')
        if (response.data.success) {
          this.pinnedNews = response.data.data
        } else {
          this.error = response.data.message
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tải tin ghim'
        console.error('Error fetching pinned news:', error)
      } finally {
        this.loading = false
      }
    },

    async fetchNewsById(id) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.get(`/news/${id}`)
        if (response.data.success) {
          this.currentNews = response.data.data
        } else {
          this.error = response.data.message
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tải tin tức'
        console.error('Error fetching news:', error)
      } finally {
        this.loading = false
      }
    },

    async createNews(newsData) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.post('/news', newsData)
        if (response.data.success) {
          await this.fetchNews(this.currentPage)
          return { success: true, data: response.data.data }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi tạo tin tức'
        console.error('Error creating news:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    },

    async updateNews(id, newsData) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.put(`/news/${id}`, newsData)
        if (response.data.success) {
          await this.fetchNews(this.currentPage)
          return { success: true, data: response.data.data }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi cập nhật tin tức'
        console.error('Error updating news:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    },

    async deleteNews(id) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.delete(`/news/${id}`)
        if (response.data.success) {
          await this.fetchNews(this.currentPage)
          return { success: true }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi xóa tin tức'
        console.error('Error deleting news:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    },

    async pinNews(id) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.post(`/news/${id}/pin`)
        if (response.data.success) {
          await this.fetchNews(this.currentPage)
          await this.fetchPinnedNews()
          return { success: true }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi ghim tin tức'
        console.error('Error pinning news:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    },

    async unpinNews(id) {
      this.loading = true
      this.error = null
      try {
        const response = await axiosClient.post(`/news/${id}/unpin`)
        if (response.data.success) {
          await this.fetchNews(this.currentPage)
          await this.fetchPinnedNews()
          return { success: true }
        } else {
          this.error = response.data.message
          return { success: false, message: response.data.message }
        }
      } catch (error) {
        this.error = error.response?.data?.message || 'Lỗi khi bỏ ghim tin tức'
        console.error('Error unpinning news:', error)
        return { success: false, message: this.error }
      } finally {
        this.loading = false
      }
    }
  }
})
