<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Quản lý Sân</h1>
      <button
        v-if="isAdmin"
        @click="showCreateModal = true"
        class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
      >
        Thêm Sân Mới
      </button>
    </div>

    <!-- Loading -->
    <div v-if="courtStore.loading" class="text-center py-8">
      <p>Đang tải...</p>
    </div>

    <!-- Error -->
    <div v-else-if="courtStore.error" class="bg-red-100 text-red-700 p-4 rounded mb-4">
      {{ courtStore.error }}
    </div>

    <!-- Court List -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="court in courtStore.courts"
        :key="court.id"
        class="bg-white p-4 rounded-lg shadow hover:shadow-lg transition-shadow"
      >
        <div class="flex justify-between items-start mb-2">
          <h3 class="text-lg font-semibold">{{ court.name }}</h3>
          <span
            :class="[
              'px-2 py-1 rounded text-xs',
              court.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
            ]"
          >
            {{ court.isAvailable ? 'Hoạt động' : 'Không hoạt động' }}
          </span>
        </div>
        <p class="text-gray-600 text-sm mb-2">{{ court.description }}</p>
        <p class="text-gray-800 font-medium">
          Giá: {{ formatCurrency(court.pricePerHour) }}/giờ
        </p>
        
        <div v-if="isAdmin" class="flex gap-2 mt-4">
          <button
            @click="editCourt(court)"
            class="flex-1 bg-yellow-500 text-white px-3 py-1 rounded hover:bg-yellow-600 text-sm"
          >
            Sửa
          </button>
          <button
            @click="confirmDelete(court)"
            class="flex-1 bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600 text-sm"
          >
            Xóa
          </button>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal || showEditModal"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
    >
      <div class="bg-white p-6 rounded-lg w-full max-w-md">
        <h2 class="text-xl font-bold mb-4">
          {{ showCreateModal ? 'Thêm Sân Mới' : 'Chỉnh Sửa Sân' }}
        </h2>
        <form @submit.prevent="handleSubmit">
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Tên sân</label>
            <input
              v-model="formData.name"
              type="text"
              required
              class="w-full border rounded px-3 py-2"
            />
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Mô tả</label>
            <textarea
              v-model="formData.description"
              class="w-full border rounded px-3 py-2"
              rows="3"
            ></textarea>
          </div>
          <div class="mb-4">
            <label class="block text-sm font-medium mb-1">Giá/giờ</label>
            <input
              v-model.number="formData.pricePerHour"
              type="number"
              required
              min="0"
              class="w-full border rounded px-3 py-2"
            />
          </div>
          <div class="mb-4">
            <label class="flex items-center">
              <input
                v-model="formData.isAvailable"
                type="checkbox"
                class="mr-2"
              />
              <span class="text-sm font-medium">Hoạt động</span>
            </label>
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
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useCourtStore } from '@/stores/court'
import { useAuthStore } from '@/stores/auth'

const courtStore = useCourtStore()
const authStore = useAuthStore()

const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedCourt = ref(null)

const formData = ref({
  name: '',
  description: '',
  pricePerHour: 0,
  isAvailable: true
})

const isAdmin = computed(() => authStore.user?.role === 'Admin')

onMounted(() => {
  courtStore.fetchCourts()
})

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount)
}

const editCourt = (court) => {
  selectedCourt.value = court
  formData.value = {
    name: court.name,
    description: court.description,
    pricePerHour: court.pricePerHour,
    isAvailable: court.isAvailable
  }
  showEditModal.value = true
}

const confirmDelete = async (court) => {
  if (confirm(`Bạn có chắc muốn xóa sân "${court.name}"?`)) {
    const result = await courtStore.deleteCourt(court.id)
    if (result.success) {
      alert('Xóa sân thành công!')
    } else {
      alert(`Lỗi: ${result.message}`)
    }
  }
}

const handleSubmit = async () => {
  if (showCreateModal.value) {
    const result = await courtStore.createCourt(formData.value)
    if (result.success) {
      alert('Thêm sân thành công!')
      closeModal()
    } else {
      alert(`Lỗi: ${result.message}`)
    }
  } else {
    const result = await courtStore.updateCourt(selectedCourt.value.id, formData.value)
    if (result.success) {
      alert('Cập nhật sân thành công!')
      closeModal()
    } else {
      alert(`Lỗi: ${result.message}`)
    }
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  selectedCourt.value = null
  formData.value = {
    name: '',
    description: '',
    pricePerHour: 0,
    isAvailable: true
  }
}
</script>
