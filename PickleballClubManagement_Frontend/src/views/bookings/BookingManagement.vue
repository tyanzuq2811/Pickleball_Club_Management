<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Quản Lý Đặt Sân</h2>
      <button v-if="authStore.isAdmin || authStore.isTreasurer" @click="showCreateModal = true" 
              class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors font-medium text-sm flex items-center gap-2">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
        </svg>
        Tạo đặt sân mới
      </button>
    </div>

    <!-- Search & Filter -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Tìm kiếm</label>
          <input v-model="searchQuery" type="text" placeholder="Tên hội viên..."
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Sân</label>
          <select v-model="filters.courtId" class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            <option :value="null">Tất cả</option>
            <option v-for="court in courts" :key="court.id" :value="court.id">{{ court.name }}</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
          <select v-model="filters.status" class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            <option :value="null">Tất cả</option>
            <option :value="0">Chờ</option>
            <option :value="1">Đã xác nhận</option>
            <option :value="2">Đã hủy</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Ngày</label>
          <input v-model="filters.date" type="date" 
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg">
        </div>
      </div>
    </div>

    <!-- Bookings Table -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 overflow-hidden">
      <table class="min-w-full divide-y divide-slate-200">
        <thead class="bg-slate-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">ID</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Hội viên</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Sân</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Thời gian</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Giá</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Trạng thái</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Hành động</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-slate-200">
          <tr v-for="booking in filteredBookings" :key="booking.id" class="hover:bg-slate-50">
            <td class="px-6 py-4 text-sm text-slate-900">#{{ booking.id }}</td>
            <td class="px-6 py-4 text-sm text-slate-900">{{ getMemberName(booking) }}</td>
            <td class="px-6 py-4 text-sm text-slate-900">{{ getCourtName(booking) }}</td>
            <td class="px-6 py-4 text-sm text-slate-900">
              {{ formatDateTime(booking.startTime) }} - {{ formatTime(booking.endTime) }}
            </td>
            <td class="px-6 py-4 text-sm text-slate-900">{{ formatCurrency(booking.totalPrice) }}</td>
            <td class="px-6 py-4">
              <span class="px-2 py-1 text-xs font-semibold rounded-full"
                    :class="getStatusClass(booking.status)">
                {{ getStatusText(booking.status) }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm">
              <div class="flex items-center justify-center gap-2">
                <button v-if="authStore.isAdmin || authStore.isTreasurer" @click="editBooking(booking)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Chỉnh sửa">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                  </svg>
                  Sửa
                </button>
                <button v-if="authStore.isAdmin || authStore.isTreasurer || booking.memberId === authStore.user?.memberId" 
                        @click="confirmDelete(booking.id)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-red-50 text-red-600 hover:bg-red-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Hủy đặt sân">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                  </svg>
                  Hủy
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Edit Booking Modal -->
    <div v-if="showEditModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Chỉnh Sửa Đặt Sân</h3>
          
          <form @submit.prevent="handleUpdate" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Sân</label>
              <select v-model="editForm.courtId" required 
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg">
                <option v-for="court in courts" :key="court.id" :value="court.id">{{ court.name }}</option>
              </select>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày</label>
                <input v-model="editForm.date" type="date" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Giờ bắt đầu</label>
                <input v-model="editForm.startTime" type="time" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg">
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Thời gian (giờ)</label>
              <input v-model.number="editForm.duration" type="number" min="1" max="8" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Ghi chú</label>
              <textarea v-model="editForm.notes" rows="3"
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg"></textarea>
            </div>

            <div class="flex space-x-3 pt-4">
              <button type="button" @click="closeEditModal"
                      class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
                Hủy
              </button>
              <button type="submit" 
                      class="flex-1 px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 font-medium">
                Cập nhật
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Create Booking Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Tạo Đặt Sân Mới</h3>
          
          <form @submit.prevent="handleCreate" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Hội viên</label>
              <select v-model="createForm.memberId" required 
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                <option value="">-- Chọn hội viên --</option>
                <option v-for="member in members" :key="member.id" :value="member.id">{{ member.fullName }}</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Sân</label>
              <select v-model="createForm.courtId" required 
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                <option value="">-- Chọn sân --</option>
                <option v-for="court in courts" :key="court.id" :value="court.id">{{ court.name }} - {{ formatCurrency(court.pricePerHour) }}/giờ</option>
              </select>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày</label>
                <input v-model="createForm.date" type="date" required
                       :min="minDate"
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Giờ bắt đầu</label>
                <select v-model="createForm.startTime" required
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                  <option value="">-- Chọn giờ --</option>
                  <option v-for="hour in availableHours" :key="hour" :value="hour">{{ hour }}</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Thời gian (giờ)</label>
              <select v-model.number="createForm.duration" required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                <option :value="1">1 giờ</option>
                <option :value="2">2 giờ</option>
                <option :value="3">3 giờ</option>
                <option :value="4">4 giờ</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Ghi chú</label>
              <textarea v-model="createForm.notes" rows="2" placeholder="Ghi chú thêm (nếu có)"
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500"></textarea>
            </div>

            <!-- Estimated Price -->
            <div v-if="estimatedPrice > 0" class="p-3 bg-sky-50 rounded-lg">
              <p class="text-sm text-sky-800">Dự tính giá: <strong class="text-lg">{{ formatCurrency(estimatedPrice) }}</strong></p>
            </div>

            <div class="flex space-x-3 pt-4">
              <button type="button" @click="closeCreateModal"
                      class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
                Hủy
              </button>
              <button type="submit" :disabled="creating"
                      class="flex-1 px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 font-medium disabled:opacity-50">
                {{ creating ? 'Đang tạo...' : 'Tạo đặt sân' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useBookingStore } from '@/stores/booking';
import { useAuthStore } from '@/stores/auth';
import { format, parseISO } from 'date-fns';
import { useToast } from 'vue-toastification';
import axiosClient from '@/api/axiosClient';
import { useConfirmDialog } from '@/composables/useConfirmDialog';

const bookingStore = useBookingStore();
const authStore = useAuthStore();
const toast = useToast();
const { confirmDelete: confirmDeleteDialog } = useConfirmDialog();

const searchQuery = ref('');
const filters = ref({
  courtId: null,
  status: null,
  date: ''
});

const courts = ref([]);
const bookings = ref([]);
const members = ref([]);
const showEditModal = ref(false);
const showCreateModal = ref(false);
const creating = ref(false);
const selectedBooking = ref(null);

const editForm = ref({
  courtId: null,
  date: '',
  startTime: '',
  duration: 1,
  notes: ''
});

const createForm = ref({
  memberId: '',
  courtId: '',
  date: '',
  startTime: '',
  duration: 1,
  notes: ''
});

const minDate = computed(() => format(new Date(), 'yyyy-MM-dd'));

const availableHours = computed(() => {
  const hours = [];
  for (let h = 6; h <= 22; h++) {
    hours.push(`${h.toString().padStart(2, '0')}:00`);
  }
  return hours;
});

const estimatedPrice = computed(() => {
  if (!createForm.value.courtId || !createForm.value.duration) return 0;
  const court = courts.value.find(c => c.id === createForm.value.courtId);
  return court ? court.pricePerHour * createForm.value.duration : 0;
});

const getMemberName = (booking) => {
  if (booking.memberName) return booking.memberName;
  const member = members.value.find(m => m.id === booking.memberId);
  return member?.fullName || 'N/A';
};

const getCourtName = (booking) => {
  if (booking.courtName) return booking.courtName;
  const court = courts.value.find(c => c.id === booking.courtId);
  return court?.name || 'N/A';
};

const filteredBookings = computed(() => {
  let result = bookings.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(b => {
      const memberName = getMemberName(b);
      const courtName = getCourtName(b);
      return memberName?.toLowerCase().includes(query) ||
             courtName?.toLowerCase().includes(query);
    });
  }

  if (filters.value.courtId !== null) {
    result = result.filter(b => b.courtId === filters.value.courtId);
  }

  if (filters.value.status !== null) {
    result = result.filter(b => b.status === filters.value.status);
  }

  if (filters.value.date) {
    result = result.filter(b => {
      const bookingDate = format(parseISO(b.startTime), 'yyyy-MM-dd');
      return bookingDate === filters.value.date;
    });
  }

  return result;
});

const fetchBookings = async () => {
  try {
    const response = await axiosClient.get('/bookings?pageSize=100');
    if (response.data.success) {
      bookings.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching bookings:', error);
  }
};

const fetchCourts = async () => {
  try {
    const response = await axiosClient.get('/courts');
    if (response.data.success) {
      courts.value = response.data.data || [];
    }
  } catch (error) {
    console.error('Error fetching courts:', error);
  }
};

const fetchMembers = async () => {
  try {
    const response = await axiosClient.get('/members?pageSize=200');
    if (response.data.success) {
      members.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching members:', error);
  }
};

onMounted(() => {
  fetchBookings();
  fetchCourts();
  fetchMembers(); // Always fetch members to display names
});

const handleCreate = async () => {
  try {
    creating.value = true;
    const startDateTime = new Date(`${createForm.value.date}T${createForm.value.startTime}`);
    const endDateTime = new Date(startDateTime.getTime() + createForm.value.duration * 60 * 60 * 1000);

    const bookingData = {
      memberId: createForm.value.memberId,
      courtId: createForm.value.courtId,
      startTime: startDateTime.toISOString(),
      endTime: endDateTime.toISOString(),
      notes: createForm.value.notes
    };

    const response = await axiosClient.post('/bookings', bookingData);
    if (response.data.success) {
      toast.success('Tạo đặt sân thành công!');
      closeCreateModal();
      fetchBookings();
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Lỗi khi tạo đặt sân');
  } finally {
    creating.value = false;
  }
};

const closeCreateModal = () => {
  showCreateModal.value = false;
  createForm.value = {
    memberId: '',
    courtId: '',
    date: '',
    startTime: '',
    duration: 1,
    notes: ''
  };
};

const editBooking = (booking) => {
  selectedBooking.value = booking;
  const startTime = parseISO(booking.startTime);
  const endTime = parseISO(booking.endTime);
  const duration = Math.round((endTime - startTime) / (1000 * 60 * 60));
  
  editForm.value = {
    courtId: booking.courtId,
    date: format(startTime, 'yyyy-MM-dd'),
    startTime: format(startTime, 'HH:mm'),
    duration: duration,
    notes: booking.notes || ''
  };
  showEditModal.value = true;
};

const handleUpdate = async () => {
  try {
    const startDateTime = new Date(`${editForm.value.date}T${editForm.value.startTime}`);
    const endDateTime = new Date(startDateTime.getTime() + editForm.value.duration * 60 * 60 * 1000);

    const updateData = {
      courtId: editForm.value.courtId,
      startTime: startDateTime.toISOString(),
      endTime: endDateTime.toISOString(),
      notes: editForm.value.notes
    };

    const response = await axiosClient.put(`/bookings/${selectedBooking.value.id}`, updateData);
    if (response.data.success) {
      toast.success('Cập nhật đặt sân thành công!');
      closeEditModal();
      fetchBookings();
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Lỗi khi cập nhật đặt sân');
  }
};

const confirmDelete = async (bookingId) => {
  const confirmed = await confirmDeleteDialog('Bạn có chắc chắn muốn hủy đặt sân này?', {
    title: 'Xác nhận hủy đặt sân',
    confirmText: 'Hủy đặt sân'
  });
  if (confirmed) {
    try {
      const response = await axiosClient.delete(`/bookings/${bookingId}`);
      if (response.data.success) {
        toast.success('Hủy đặt sân thành công!');
        fetchBookings();
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Lỗi khi hủy đặt sân');
    }
  }
};

const closeEditModal = () => {
  showEditModal.value = false;
  selectedBooking.value = null;
};

const formatDateTime = (dateStr) => {
  try {
    return format(parseISO(dateStr), 'dd/MM/yyyy HH:mm');
  } catch {
    return dateStr;
  }
};

const formatTime = (dateStr) => {
  try {
    return format(parseISO(dateStr), 'HH:mm');
  } catch {
    return dateStr;
  }
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { 
  style: 'currency', 
  currency: 'VND' 
}).format(val || 0);

const getStatusText = (status) => ['Chờ', 'Đã xác nhận', 'Đã hủy'][status] || 'Unknown';
const getStatusClass = (status) => [
  'bg-yellow-100 text-yellow-800',
  'bg-green-100 text-green-800',
  'bg-red-100 text-red-800'
][status];
</script>
