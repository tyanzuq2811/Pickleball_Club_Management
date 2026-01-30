<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Tin tức CLB</h1>
      <button
        v-if="isAdmin"
        @click="showCreateModal = true"
        class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
      >
        Thêm Tin Mới
      </button>
    </div>

    <!-- Search Bar -->
    <div class="mb-6">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Tìm kiếm tin tức theo tiêu đề hoặc tóm tắt..."
        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500"
      />
    </div>

    <!-- Pinned News -->
    <div v-if="filteredPinnedNews.length > 0" class="mb-8">
      <h2 class="text-xl font-semibold mb-4">📌 Tin Ghim</h2>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div
          v-for="news in filteredPinnedNews"
          :key="news.id"
          class="bg-yellow-50 border-l-4 border-yellow-400 p-4 rounded-lg shadow"
        >
          <h3 class="font-semibold text-lg mb-2">{{ news.title }}</h3>
          <p class="text-gray-600 text-sm mb-2">{{ news.summary }}</p>
          <div class="flex justify-between items-center text-xs text-gray-500">
            <span>{{ formatDate(news.createdAt) }}</span>
            <div class="flex gap-2">
              <button
                @click="viewNews(news)"
                class="inline-flex items-center gap-1 px-3 py-1.5 bg-blue-50 text-blue-600 hover:bg-blue-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                title="Xem chi tiết"
              >
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                </svg>
                Xem chi tiết
              </button>
              <button
                v-if="isAdmin"
                @click="unpinNews(news.id)"
                class="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                title="Bỏ ghim"
              >
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
                Bỏ ghim
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="newsStore.loading" class="text-center py-8">
      <p>Đang tải...</p>
    </div>

    <!-- Error -->
    <div v-else-if="newsStore.error" class="bg-red-100 text-red-700 p-4 rounded mb-4">
      {{ newsStore.error }}
    </div>

    <!-- News List -->
    <div v-else>
      <h2 class="text-xl font-semibold mb-4">Tất cả tin tức</h2>
      <div class="space-y-4">
        <div
          v-for="news in filteredNewsList"
          :key="news.id"
          class="bg-white p-4 rounded-lg shadow hover:shadow-lg transition-shadow"
        >
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <h3 class="font-semibold text-lg mb-2">{{ news.title }}</h3>
              <p class="text-gray-600 text-sm mb-2">{{ news.summary }}</p>
              <div class="flex gap-4 text-xs text-gray-500">
                <span>{{ formatDate(news.createdAt) }}</span>
                <span>Bởi: {{ news.createdBy }}</span>
              </div>
            </div>
            <div class="flex flex-col gap-2 ml-4">
              <button
                @click="viewNews(news)"
                class="inline-flex items-center justify-center gap-1 px-3 py-1.5 bg-blue-50 text-blue-600 hover:bg-blue-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                title="Xem chi tiết"
              >
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                </svg>
                Xem
              </button>
              <button
                v-if="isAdmin && !news.isPinned"
                @click="pinNews(news.id)"
                class="inline-flex items-center justify-center gap-1 px-3 py-1.5 bg-purple-50 text-purple-600 hover:bg-purple-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                title="Ghim tin này"
              >
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 5a2 2 0 012-2h10a2 2 0 012 2v16l-7-3.5L5 21V5z"/>
                </svg>
                Ghim
              </button>
              <button
                v-if="isAdmin"
                @click="editNews(news)"
                class="inline-flex items-center justify-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                title="Chỉnh sửa"
              >
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                </svg>
                Sửa
              </button>
              <button
                v-if="isAdmin"
                @click="confirmDelete(news)"
                class="inline-flex items-center justify-center gap-1 px-3 py-1.5 bg-red-50 text-red-600 hover:bg-red-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                title="Xóa tin này"
              >
                Xóa
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="newsStore.totalPages > 1" class="flex justify-center gap-2 mt-6">
        <button
          v-for="page in newsStore.totalPages"
          :key="page"
          @click="newsStore.fetchNews(page)"
          :class="[
            'px-3 py-1 rounded',
            page === newsStore.currentPage
              ? 'bg-blue-500 text-white'
              : 'bg-gray-200 hover:bg-gray-300'
          ]"
        >
          {{ page }}
        </button>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || showEditModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 overflow-y-auto"
    >
      <div class="bg-white p-6 rounded-lg w-full max-w-2xl m-4">
        <h2 class="text-xl font-bold mb-4">
          {{ showCreateModal ? 'Thêm Tin Mới' : 'Chỉnh Sửa Tin' }}
        </h2>
        <form @submit.prevent="handleSubmit">
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Tiêu đề</label>
            <input
              v-model="formData.title"
              type="text"
              required
              class="w-full border rounded px-3 py-2"
            />
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Tóm tắt</label>
            <textarea
              v-model="formData.summary"
              class="w-full border rounded px-3 py-2"
              rows="2"
            ></textarea>
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Nội dung</label>
            <textarea
              v-model="formData.content"
              required
              class="w-full border rounded px-3 py-2"
              rows="10"
            ></textarea>
          </div>
          <div class="flex gap-2">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 bg-gray-300 text-gray-700 px-4 py-2 rounded hover:bg-gray-400"
            >
              Hủy
            </button>
            <button
              type="submit"
              class="flex-1 bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
            >
              {{ showCreateModal ? 'Thêm' : 'Cập nhật' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- View Modal -->
    <div
      v-if="showViewModal && newsStore.currentNews"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 overflow-y-auto"
    >
      <div class="bg-white p-6 rounded-lg w-full max-w-3xl m-4">
        <div class="flex justify-between items-start mb-4">
          <h2 class="text-2xl font-bold">{{ newsStore.currentNews.title }}</h2>
          <button
            @click="showViewModal = false"
            class="text-gray-500 hover:text-gray-700"
          >
            ✕
          </button>
        </div>
        <div class="text-sm text-gray-500 mb-4">
          {{ formatDate(newsStore.currentNews.createdAt) }} - 
          Bởi: {{ newsStore.currentNews.createdBy }}
        </div>
        <div class="prose max-w-none">
          <p class="whitespace-pre-wrap">{{ newsStore.currentNews.content }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useNewsStore } from '@/stores/news'
import { useAuthStore } from '@/stores/auth'
import { useConfirmDialog } from '@/composables/useConfirmDialog'
import { useToast } from 'vue-toastification'

const newsStore = useNewsStore()
const authStore = useAuthStore()
const { confirmDelete: confirmDeleteDialog } = useConfirmDialog()
const toast = useToast()

const showCreateModal = ref(false)
const showEditModal = ref(false)
const showViewModal = ref(false)
const selectedNews = ref(null)
const searchQuery = ref('')

const formData = ref({
  title: '',
  summary: '',
  content: ''
})

const isAdmin = computed(() => authStore.user?.role === 'Admin')

const filteredPinnedNews = computed(() => {
  if (!searchQuery.value) {
    return newsStore.pinnedNews;
  }
  const query = searchQuery.value.toLowerCase();
  return newsStore.pinnedNews.filter(news => 
    news.title.toLowerCase().includes(query) ||
    news.summary?.toLowerCase().includes(query)
  );
})

const filteredNewsList = computed(() => {
  if (!searchQuery.value) {
    return newsStore.newsList;
  }
  const query = searchQuery.value.toLowerCase();
  return newsStore.newsList.filter(news => 
    news.title.toLowerCase().includes(query) ||
    news.summary?.toLowerCase().includes(query)
  );
})

onMounted(() => {
  newsStore.fetchNews()
  newsStore.fetchPinnedNews()
})

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const viewNews = async (news) => {
  await newsStore.fetchNewsById(news.id)
  showViewModal.value = true
}

const editNews = (news) => {
  selectedNews.value = news
  formData.value = {
    title: news.title,
    summary: news.summary,
    content: news.content
  }
  showEditModal.value = true
}

const pinNews = async (id) => {
  const result = await newsStore.pinNews(id)
  if (result.success) {
    toast.success('Ghim tin thành công!')
  } else {
    toast.error(`Lỗi: ${result.message}`)
  }
}

const unpinNews = async (id) => {
  const result = await newsStore.unpinNews(id)
  if (result.success) {
    toast.success('Bỏ ghim thành công!')
  } else {
    toast.error(`Lỗi: ${result.message}`)
  }
}

const confirmDelete = async (news) => {
  const confirmed = await confirmDeleteDialog(`Bạn có chắc muốn xóa tin "${news.title}"?`)
  if (confirmed) {
    const result = await newsStore.deleteNews(news.id)
    if (result.success) {
      toast.success('Xóa tin thành công!')
    } else {
      toast.error(`Lỗi: ${result.message}`)
    }
  }
}

const handleSubmit = async () => {
  if (showCreateModal.value) {
    const result = await newsStore.createNews(formData.value)
    if (result.success) {
      toast.success('Thêm tin thành công!')
      closeModal()
    } else {
      toast.error(`Lỗi: ${result.message}`)
    }
  } else {
    const result = await newsStore.updateNews(selectedNews.value.id, formData.value)
    if (result.success) {
      toast.success('Cập nhật tin thành công!')
      closeModal()
    } else {
      toast.error(`Lỗi: ${result.message}`)
    }
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  selectedNews.value = null
  formData.value = {
    title: '',
    summary: '',
    content: ''
  }
}
</script>
