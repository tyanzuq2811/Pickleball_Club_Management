<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Nhật ký hoạt động hệ thống</h2>
      <button @click="refreshLogs" 
              :disabled="loading"
              class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 font-medium text-sm flex items-center gap-2">
        <svg class="w-4 h-4" :class="{ 'animate-spin': loading }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
        </svg>
        Làm mới
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- User Filter -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Người dùng</label>
          <input v-model="filters.userName" 
                 type="text" 
                 placeholder="Tên hoặc email..."
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                 @input="debounceSearch">
        </div>

        <!-- Action Type Filter -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Loại hành động</label>
          <select v-model="filters.actionType" 
                  @change="applyFilters"
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option value="">Tất cả</option>
            <option value="Create">Tạo mới</option>
            <option value="Update">Cập nhật</option>
            <option value="Delete">Xóa</option>
            <option value="Login">Đăng nhập</option>
            <option value="Logout">Đăng xuất</option>
            <option value="Approve">Phê duyệt</option>
            <option value="Reject">Từ chối</option>
            <option value="Payment">Thanh toán</option>
          </select>
        </div>

        <!-- Entity Filter -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Đối tượng</label>
          <select v-model="filters.entityType" 
                  @change="applyFilters"
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent">
            <option value="">Tất cả</option>
            <option value="Booking">Đặt sân</option>
            <option value="Transaction">Giao dịch</option>
            <option value="Member">Hội viên</option>
            <option value="Court">Sân</option>
            <option value="Match">Trận đấu</option>
            <option value="Tournament">Giải đấu</option>
            <option value="News">Tin tức</option>
          </select>
        </div>

        <!-- Date Range -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Ngày</label>
          <input v-model="filters.date" 
                 type="date" 
                 @change="applyFilters"
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent">
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mt-6 pt-6 border-t border-slate-200">
        <div class="text-center">
          <div class="text-3xl font-bold text-blue-600">{{ stats.totalToday }}</div>
          <div class="text-sm text-slate-600">Hoạt động hôm nay</div>
        </div>
        <div class="text-center">
          <div class="text-3xl font-bold text-green-600">{{ stats.totalLogins }}</div>
          <div class="text-sm text-slate-600">Đăng nhập</div>
        </div>
        <div class="text-center">
          <div class="text-3xl font-bold text-purple-600">{{ stats.totalChanges }}</div>
          <div class="text-sm text-slate-600">Thay đổi dữ liệu</div>
        </div>
        <div class="text-center">
          <div class="text-3xl font-bold text-orange-600">{{ stats.totalErrors }}</div>
          <div class="text-sm text-slate-600">Lỗi</div>
        </div>
      </div>
    </div>

    <!-- Activity Logs Table -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Thời gian</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Người dùng</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Hành động</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Đối tượng</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">Chi tiết</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">IP</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-200">
            <tr v-if="loading" class="text-center">
              <td colspan="6" class="px-6 py-12">
                <div class="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
                <p class="mt-2 text-slate-500">Đang tải...</p>
              </td>
            </tr>
            <tr v-else-if="filteredLogs.length === 0" class="text-center">
              <td colspan="6" class="px-6 py-12 text-slate-500">
                Không tìm thấy nhật ký nào
              </td>
            </tr>
            <tr v-else v-for="log in paginatedLogs" :key="log.id" 
                class="hover:bg-slate-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <div class="font-medium text-slate-900">{{ formatDateTime(log.timestamp) }}</div>
                <div class="text-xs text-slate-500">{{ formatTimeAgo(log.timestamp) }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-8 w-8 bg-blue-100 rounded-full flex items-center justify-center">
                    <span class="text-blue-600 font-bold text-xs">{{ getInitials(log.userName) }}</span>
                  </div>
                  <div class="ml-3">
                    <div class="text-sm font-medium text-slate-900">{{ log.userName }}</div>
                    <div class="text-xs text-slate-500">{{ log.userEmail }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-3 py-1 text-xs font-semibold rounded-full"
                      :class="getActionBadgeClass(log.actionType)">
                  {{ translateAction(log.actionType) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <div class="font-medium text-slate-900">{{ translateEntity(log.entityType) }}</div>
                <div v-if="log.entityId" class="text-xs text-slate-500">ID: {{ log.entityId }}</div>
              </td>
              <td class="px-6 py-4 text-sm text-slate-700 max-w-xs truncate">
                <button @click="showDetails(log)" 
                        class="text-blue-600 hover:text-blue-700 hover:underline">
                  {{ log.details || 'Xem chi tiết' }}
                </button>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                {{ log.ipAddress }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="px-6 py-4 border-t border-slate-200 flex items-center justify-between">
        <div class="text-sm text-slate-700">
          Hiển thị <span class="font-medium">{{ startIndex + 1 }}</span> đến <span class="font-medium">{{ endIndex }}</span> trong tổng số <span class="font-medium">{{ filteredLogs.length }}</span> kết quả
        </div>
        <div class="flex gap-2">
          <button @click="currentPage--" 
                  :disabled="currentPage === 1"
                  class="px-4 py-2 border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed">
            Trước
          </button>
          <button v-for="page in visiblePages" :key="page"
                  @click="currentPage = page"
                  class="px-4 py-2 border rounded-lg"
                  :class="page === currentPage ? 'bg-blue-600 text-white border-blue-600' : 'border-slate-300 hover:bg-slate-50'">
            {{ page }}
          </button>
          <button @click="currentPage++" 
                  :disabled="currentPage === totalPages"
                  class="px-4 py-2 border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed">
            Sau
          </button>
        </div>
      </div>
    </div>

    <!-- Detail Modal -->
    <div v-if="showDetailModal && selectedLog" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6 border-b border-slate-200 flex justify-between items-center">
          <h3 class="text-xl font-bold text-slate-800">Chi tiết hoạt động</h3>
          <button @click="showDetailModal = false" class="text-slate-400 hover:text-slate-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>
        <div class="p-6 space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="text-sm font-medium text-slate-500">Người thực hiện</label>
              <div class="text-slate-900">{{ selectedLog.userName }}</div>
              <div class="text-sm text-slate-500">{{ selectedLog.userEmail }}</div>
            </div>
            <div>
              <label class="text-sm font-medium text-slate-500">Thời gian</label>
              <div class="text-slate-900">{{ formatDateTime(selectedLog.timestamp) }}</div>
            </div>
            <div>
              <label class="text-sm font-medium text-slate-500">Hành động</label>
              <div>
                <span class="px-3 py-1 text-xs font-semibold rounded-full"
                      :class="getActionBadgeClass(selectedLog.actionType)">
                  {{ translateAction(selectedLog.actionType) }}
                </span>
              </div>
            </div>
            <div>
              <label class="text-sm font-medium text-slate-500">Đối tượng</label>
              <div class="text-slate-900">{{ translateEntity(selectedLog.entityType) }} (ID: {{ selectedLog.entityId }})</div>
            </div>
            <div>
              <label class="text-sm font-medium text-slate-500">IP Address</label>
              <div class="text-slate-900">{{ selectedLog.ipAddress }}</div>
            </div>
            <div>
              <label class="text-sm font-medium text-slate-500">User Agent</label>
              <div class="text-sm text-slate-700 break-all">{{ selectedLog.userAgent || 'N/A' }}</div>
            </div>
          </div>
          
          <div v-if="selectedLog.details">
            <label class="text-sm font-medium text-slate-500">Chi tiết</label>
            <div class="mt-2 p-4 bg-slate-50 rounded-lg text-sm text-slate-700 whitespace-pre-wrap">
              {{ selectedLog.details }}
            </div>
          </div>

          <div v-if="selectedLog.oldValue || selectedLog.newValue" class="grid grid-cols-2 gap-4">
            <div v-if="selectedLog.oldValue">
              <label class="text-sm font-medium text-slate-500">Giá trị cũ</label>
              <div class="mt-2 p-4 bg-red-50 rounded-lg text-sm text-slate-700 whitespace-pre-wrap max-h-40 overflow-y-auto">
                {{ selectedLog.oldValue }}
              </div>
            </div>
            <div v-if="selectedLog.newValue">
              <label class="text-sm font-medium text-slate-500">Giá trị mới</label>
              <div class="mt-2 p-4 bg-green-50 rounded-lg text-sm text-slate-700 whitespace-pre-wrap max-h-40 overflow-y-auto">
                {{ selectedLog.newValue }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { format, formatDistanceToNow, parseISO } from 'date-fns';
import { vi } from 'date-fns/locale';
import axiosClient from '@/api/axiosClient';

const logs = ref([]);
const loading = ref(true);
const currentPage = ref(1);
const pageSize = 20;
const showDetailModal = ref(false);
const selectedLog = ref(null);

const filters = ref({
  userName: '',
  actionType: '',
  entityType: '',
  date: ''
});

const stats = ref({
  totalToday: 0,
  totalLogins: 0,
  totalChanges: 0,
  totalErrors: 0
});

let searchTimeout = null;

onMounted(async () => {
  await fetchLogs();
  await fetchStats();
});

const fetchLogs = async () => {
  try {
    loading.value = true;
    const response = await axiosClient.get('/activity-logs', {
      params: {
        userName: filters.value.userName || undefined,
        actionType: filters.value.actionType || undefined,
        entityType: filters.value.entityType || undefined,
        date: filters.value.date || undefined
      }
    });
    if (response.data.success) {
      logs.value = response.data.data || [];
    }
  } catch (error) {
    console.error('Error fetching logs:', error);
  } finally {
    loading.value = false;
  }
};

const fetchStats = async () => {
  try {
    const response = await axiosClient.get('/activity-logs/stats');
    if (response.data.success) {
      stats.value = response.data.data;
    }
  } catch (error) {
    console.error('Error fetching stats:', error);
  }
};

const filteredLogs = computed(() => logs.value);

const totalPages = computed(() => Math.ceil(filteredLogs.value.length / pageSize));

const startIndex = computed(() => (currentPage.value - 1) * pageSize);
const endIndex = computed(() => Math.min(startIndex.value + pageSize, filteredLogs.value.length));

const paginatedLogs = computed(() => {
  return filteredLogs.value.slice(startIndex.value, endIndex.value);
});

const visiblePages = computed(() => {
  const pages = [];
  const start = Math.max(1, currentPage.value - 2);
  const end = Math.min(totalPages.value, currentPage.value + 2);
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
});

const debounceSearch = () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    applyFilters();
  }, 500);
};

const applyFilters = () => {
  currentPage.value = 1;
  fetchLogs();
};

const refreshLogs = async () => {
  await fetchLogs();
  await fetchStats();
};

const showDetails = (log) => {
  selectedLog.value = log;
  showDetailModal.value = true;
};

const getInitials = (name) => {
  if (!name) return '?';
  return name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
};

const getActionBadgeClass = (action) => {
  const classes = {
    Create: 'bg-green-100 text-green-800',
    Update: 'bg-blue-100 text-blue-800',
    Delete: 'bg-red-100 text-red-800',
    Login: 'bg-purple-100 text-purple-800',
    Logout: 'bg-slate-100 text-slate-800',
    Approve: 'bg-green-100 text-green-800',
    Reject: 'bg-red-100 text-red-800',
    Payment: 'bg-yellow-100 text-yellow-800'
  };
  return classes[action] || 'bg-slate-100 text-slate-800';
};

const translateAction = (action) => {
  const translations = {
    Create: 'Tạo mới',
    Update: 'Cập nhật',
    Delete: 'Xóa',
    Login: 'Đăng nhập',
    Logout: 'Đăng xuất',
    Approve: 'Phê duyệt',
    Reject: 'Từ chối',
    Payment: 'Thanh toán'
  };
  return translations[action] || action;
};

const translateEntity = (entity) => {
  const translations = {
    Booking: 'Đặt sân',
    Transaction: 'Giao dịch',
    Member: 'Hội viên',
    Court: 'Sân',
    Match: 'Trận đấu',
    Tournament: 'Giải đấu',
    News: 'Tin tức'
  };
  return translations[entity] || entity;
};

const formatDateTime = (date) => format(parseISO(date), 'HH:mm:ss - dd/MM/yyyy');
const formatTimeAgo = (date) => formatDistanceToNow(parseISO(date), { addSuffix: true, locale: vi });
</script>
