<template>
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="$emit('close')">
    <div class="bg-white rounded-xl shadow-xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto">
      <!-- Header -->
      <div class="p-6 border-b border-slate-200 flex justify-between items-center">
        <h3 class="text-2xl font-bold text-slate-800">🔍 Tìm Kiếm Nâng Cao</h3>
        <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
          </svg>
        </button>
      </div>

      <!-- Search Tabs -->
      <div class="p-6">
        <div class="flex gap-2 mb-6 border-b border-slate-200">
          <button v-for="tab in tabs" :key="tab.value"
                  @click="activeTab = tab.value"
                  class="px-4 py-2 font-medium transition-colors relative"
                  :class="activeTab === tab.value 
                    ? 'text-blue-600 border-b-2 border-blue-600' 
                    : 'text-slate-600 hover:text-slate-800'">
            {{ tab.label }}
          </button>
        </div>

        <!-- Bookings Search -->
        <div v-if="activeTab === 'bookings'" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Từ khóa</label>
              <input v-model="bookingFilters.keyword" type="text" placeholder="Tên hội viên, tên sân..."
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
              <select v-model="bookingFilters.status" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option value="0">Chờ thanh toán</option>
                <option value="1">Đã xác nhận</option>
                <option value="2">Đã hủy</option>
                <option value="3">Hoàn thành</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Từ ngày</label>
              <input v-model="bookingFilters.startDate" type="date"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Đến ngày</label>
              <input v-model="bookingFilters.endDate" type="date"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Sân</label>
              <select v-model="bookingFilters.courtId" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option v-for="court in courts" :key="court.id" :value="court.id">{{ court.name }}</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Giá từ - đến</label>
              <div class="flex gap-2">
                <input v-model.number="bookingFilters.minPrice" type="number" placeholder="Từ"
                       class="w-1/2 px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <input v-model.number="bookingFilters.maxPrice" type="number" placeholder="Đến"
                       class="w-1/2 px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
              </div>
            </div>
          </div>
        </div>

        <!-- Members Search -->
        <div v-if="activeTab === 'members'" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Họ tên</label>
              <input v-model="memberFilters.name" type="text" placeholder="Nhập tên hội viên..."
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Email</label>
              <input v-model="memberFilters.email" type="email" placeholder="Nhập email..."
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số điện thoại</label>
              <input v-model="memberFilters.phone" type="tel" placeholder="Nhập số điện thoại..."
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
              <select v-model="memberFilters.isActive" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option value="true">Hoạt động</option>
                <option value="false">Không hoạt động</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số dư ví từ</label>
              <input v-model.number="memberFilters.minBalance" type="number" placeholder="Từ"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số dư ví đến</label>
              <input v-model.number="memberFilters.maxBalance" type="number" placeholder="Đến"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
          </div>
        </div>

        <!-- Transactions Search -->
        <div v-if="activeTab === 'transactions'" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Loại giao dịch</label>
              <select v-model="transactionFilters.type" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option value="0">Thu</option>
                <option value="1">Chi</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Danh mục</label>
              <select v-model="transactionFilters.categoryId" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Từ ngày</label>
              <input v-model="transactionFilters.startDate" type="date"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Đến ngày</label>
              <input v-model="transactionFilters.endDate" type="date"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số tiền từ</label>
              <input v-model.number="transactionFilters.minAmount" type="number" placeholder="Từ"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số tiền đến</label>
              <input v-model.number="transactionFilters.maxAmount" type="number" placeholder="Đến"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
          </div>
        </div>

        <!-- Tournaments Search -->
        <div v-if="activeTab === 'tournaments'" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Tên giải đấu</label>
              <input v-model="tournamentFilters.name" type="text" placeholder="Nhập tên giải..."
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Loại giải</label>
              <select v-model="tournamentFilters.type" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option value="0">Đấu loại</option>
                <option value="1">Vòng tròn</option>
                <option value="2">Chiến Đội</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
              <select v-model="tournamentFilters.status" class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
                <option value="">Tất cả</option>
                <option value="0">Sắp diễn ra</option>
                <option value="1">Đang diễn ra</option>
                <option value="2">Đã kết thúc</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Từ ngày</label>
              <input v-model="tournamentFilters.startDate" type="date"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex gap-3 mt-6 pt-6 border-t border-slate-200">
          <button @click="clearFilters" 
                  class="flex-1 px-6 py-3 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium transition-colors">
            Xóa bộ lọc
          </button>
          <button @click="search" 
                  class="flex-1 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-medium transition-colors flex items-center justify-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
            </svg>
            Tìm kiếm
          </button>
        </div>

        <!-- Results -->
        <div v-if="searchResults.length > 0" class="mt-6">
          <h4 class="font-bold text-slate-800 mb-3">Kết quả tìm kiếm ({{ searchResults.length }})</h4>
          <div class="space-y-2 max-h-64 overflow-y-auto">
            <div v-for="result in searchResults" :key="result.id"
                 class="p-4 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors cursor-pointer"
                 @click="$emit('select', result)">
              <div class="font-medium text-slate-800">{{ result.title }}</div>
              <div class="text-sm text-slate-600">{{ result.description }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axiosClient from '@/api/axiosClient';
import { useToast } from 'vue-toastification';

const emit = defineEmits(['close', 'select']);
const toast = useToast();

const activeTab = ref('bookings');
const searchResults = ref([]);
const courts = ref([]);
const categories = ref([]);

const tabs = [
  { value: 'bookings', label: 'Booking' },
  { value: 'members', label: 'Hội viên' },
  { value: 'transactions', label: 'Giao dịch' },
  { value: 'tournaments', label: 'Giải đấu' }
];

const bookingFilters = ref({
  keyword: '',
  status: '',
  startDate: '',
  endDate: '',
  courtId: '',
  minPrice: null,
  maxPrice: null
});

const memberFilters = ref({
  name: '',
  email: '',
  phone: '',
  isActive: '',
  minBalance: null,
  maxBalance: null
});

const transactionFilters = ref({
  type: '',
  categoryId: '',
  startDate: '',
  endDate: '',
  minAmount: null,
  maxAmount: null
});

const tournamentFilters = ref({
  name: '',
  type: '',
  status: '',
  startDate: ''
});

onMounted(async () => {
  // Load courts and categories for filters
  try {
    const [courtsRes, catsRes] = await Promise.all([
      axiosClient.get('/courts'),
      axiosClient.get('/transaction-categories')
    ]);
    courts.value = courtsRes.data.data?.items || [];
    categories.value = catsRes.data.data || [];
  } catch (error) {
    console.error('Error loading filter data:', error);
  }
});

const search = async () => {
  try {
    let endpoint = '';
    let params = {};

    switch (activeTab.value) {
      case 'bookings':
        endpoint = '/search/bookings';
        params = bookingFilters.value;
        break;
      case 'members':
        endpoint = '/search/members';
        params = memberFilters.value;
        break;
      case 'transactions':
        endpoint = '/search/transactions';
        params = transactionFilters.value;
        break;
      case 'tournaments':
        endpoint = '/search/tournaments';
        params = tournamentFilters.value;
        break;
    }

    const response = await axiosClient.get(endpoint, { params });
    if (response.data.success) {
      searchResults.value = response.data.data || [];
      if (searchResults.value.length === 0) {
        toast.info('Không tìm thấy kết quả phù hợp');
      }
    }
  } catch (error) {
    console.error('Search error:', error);
    toast.error('Có lỗi xảy ra khi tìm kiếm');
  }
};

const clearFilters = () => {
  bookingFilters.value = { keyword: '', status: '', startDate: '', endDate: '', courtId: '', minPrice: null, maxPrice: null };
  memberFilters.value = { name: '', email: '', phone: '', isActive: '', minBalance: null, maxBalance: null };
  transactionFilters.value = { type: '', categoryId: '', startDate: '', endDate: '', minAmount: null, maxAmount: null };
  tournamentFilters.value = { name: '', type: '', status: '', startDate: '' };
  searchResults.value = [];
};
</script>
