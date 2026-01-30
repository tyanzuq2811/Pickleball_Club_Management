<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Booking của tôi</h2>
      <router-link to="/bookings" class="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium text-sm">
        + Đặt sân mới
      </router-link>
    </div>

    <!-- Pending Bookings với Countdown -->
    <div v-if="pendingBookings.length > 0" class="bg-white rounded-xl shadow-sm border border-yellow-300 overflow-hidden">
      <div class="p-6 bg-yellow-50 border-b border-yellow-200">
        <h3 class="text-lg font-bold text-yellow-900 flex items-center gap-2">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
          </svg>
          Chờ thanh toán - Vui lòng hoàn tất trong 15 phút
        </h3>
      </div>
      
      <div class="divide-y divide-yellow-100">
        <div v-for="booking in pendingBookings" :key="booking.id" 
             class="p-4 hover:bg-yellow-50/50 transition-colors">
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h4 class="font-bold text-slate-800">{{ booking.courtName }}</h4>
                <span class="px-2 py-1 bg-yellow-100 text-yellow-800 text-xs font-semibold rounded-full">
                  Chờ thanh toán
                </span>
              </div>
              <div class="text-sm text-slate-600 space-y-1">
                <div>📅 {{ formatDate(booking.startTime) }}</div>
                <div>🕐 {{ formatTime(booking.startTime) }} - {{ formatTime(booking.endTime) }}</div>
                <div>💰 {{ formatCurrency(booking.totalPrice) }}</div>
              </div>
            </div>
            
            <!-- Countdown Timer -->
            <div class="text-center ml-4">
              <div class="text-3xl font-bold mb-1" 
                   :class="getCountdownColor(booking.id)">
                {{ getCountdown(booking.id) }}
              </div>
              <div class="text-xs text-slate-500">còn lại</div>
              <button @click="payNow(booking)" 
                      class="mt-3 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 font-medium text-sm transition-colors">
                Thanh toán ngay
              </button>
              <button @click="cancelBooking(booking.id)" 
                      class="mt-2 px-4 py-2 bg-slate-200 text-slate-700 rounded-lg hover:bg-slate-300 font-medium text-sm transition-colors">
                Hủy
              </button>
            </div>
          </div>

          <!-- Warning khi < 5 phút -->
          <div v-if="getMinutesLeft(booking.id) < 5 && getMinutesLeft(booking.id) > 0" 
               class="mt-3 p-3 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-sm text-red-800">
            <svg class="w-5 h-5 text-red-600" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clip-rule="evenodd"/>
            </svg>
            <strong>Cảnh báo:</strong> Booking sẽ tự động hủy sau {{ getMinutesLeft(booking.id) }} phút!
          </div>
        </div>
      </div>
    </div>

    <!-- Confirmed Bookings -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200">
      <div class="p-6 border-b border-slate-100">
        <h3 class="text-lg font-bold text-slate-800">Booking đã xác nhận</h3>
      </div>
      
      <div v-if="confirmedBookings.length === 0" class="p-8 text-center text-slate-500">
        <svg class="w-16 h-16 mx-auto mb-3 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
        </svg>
        <p>Chưa có booking nào</p>
      </div>

      <div v-else class="divide-y divide-slate-100">
        <div v-for="booking in confirmedBookings" :key="booking.id" 
             class="p-4 hover:bg-slate-50 transition-colors">
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-3 mb-2">
                <h4 class="font-bold text-slate-800">{{ booking.courtName }}</h4>
                <span class="px-2 py-1 bg-green-100 text-green-800 text-xs font-semibold rounded-full">
                  Đã xác nhận
                </span>
                <span v-if="booking.isCheckedIn" class="px-2 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded-full">
                  ✓ Đã check-in
                </span>
              </div>
              <div class="text-sm text-slate-600 space-y-1">
                <div>📅 {{ formatDate(booking.startTime) }}</div>
                <div>🕐 {{ formatTime(booking.startTime) }} - {{ formatTime(booking.endTime) }}</div>
                <div v-if="booking.notes" class="text-xs">📝 {{ booking.notes }}</div>
                <div v-if="booking.checkInTime" class="text-xs text-blue-600">
                  Check-in: {{ formatDateTime(booking.checkInTime) }}
                </div>
              </div>
            </div>
            
            <div class="ml-4 flex gap-2">
              <button v-if="!booking.isCheckedIn && canCheckIn(booking)" 
                      @click="checkIn(booking.id)"
                      class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 font-medium text-sm transition-colors">
                Check-in
              </button>
              <button @click="viewDetail(booking)" 
                      class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-medium text-sm transition-colors">
                Chi tiết
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Modal -->
    <div v-if="showPaymentModal && selectedBooking" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6 border-b border-slate-200">
          <h3 class="text-xl font-bold text-slate-800">Xác nhận thanh toán</h3>
        </div>
        <div class="p-6 space-y-4">
          <div class="bg-blue-50 p-4 rounded-lg space-y-2">
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Sân:</span>
              <span class="font-bold text-blue-900">{{ selectedBooking.courtName }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Ngày:</span>
              <span class="font-bold text-blue-900">{{ formatDate(selectedBooking.startTime) }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Giờ:</span>
              <span class="font-bold text-blue-900">{{ formatTime(selectedBooking.startTime) }} - {{ formatTime(selectedBooking.endTime) }}</span>
            </div>
            <div class="flex justify-between items-center pt-2 border-t border-blue-200">
              <span class="text-sm text-blue-700">Tổng tiền:</span>
              <span class="text-2xl font-bold text-green-600">{{ formatCurrency(selectedBooking.totalPrice) }}</span>
            </div>
          </div>

          <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-3 text-sm text-yellow-800">
            <strong>Lưu ý:</strong> Số tiền sẽ được trừ từ ví điện tử của bạn.
          </div>
        </div>
        <div class="p-6 bg-slate-50 border-t border-slate-200 flex justify-end space-x-3">
          <button @click="showPaymentModal = false" 
                  class="px-4 py-2 text-slate-700 hover:bg-slate-200 rounded-lg font-medium transition-colors">
            Hủy
          </button>
          <button @click="confirmPayment" 
                  class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 font-medium transition-colors">
            Xác nhận thanh toán
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useToast } from 'vue-toastification';
import { useConfirmDialog } from '@/composables/useConfirmDialog';
import { format, parseISO, differenceInSeconds } from 'date-fns';
import axiosClient from '@/api/axiosClient';

const authStore = useAuthStore();
const toast = useToast();
const { confirm: confirmDialog } = useConfirmDialog();

const bookings = ref([]);
const countdowns = ref({});
const showPaymentModal = ref(false);
const selectedBooking = ref(null);
let intervalId = null;

const PENDING_TIMEOUT = 15 * 60; // 15 minutes in seconds

const pendingBookings = computed(() => 
  bookings.value.filter(b => b.status === 0) // 0 = PendingPayment
);

const confirmedBookings = computed(() => 
  bookings.value.filter(b => b.status === 1) // 1 = Confirmed
);

onMounted(async () => {
  await fetchMyBookings();
  startCountdownTimer();
});

onUnmounted(() => {
  if (intervalId) clearInterval(intervalId);
});

const fetchMyBookings = async () => {
  try {
    const response = await axiosClient.get('/bookings/my-bookings');
    if (response.data.success) {
      bookings.value = response.data.data || [];
      initializeCountdowns();
    }
  } catch (error) {
    console.error('Error fetching bookings:', error);
  }
};

const initializeCountdowns = () => {
  pendingBookings.value.forEach(booking => {
    const createdTime = new Date(booking.createdDate || booking.startTime);
    const now = new Date();
    const elapsed = differenceInSeconds(now, createdTime);
    const remaining = Math.max(0, PENDING_TIMEOUT - elapsed);
    countdowns.value[booking.id] = remaining;
  });
};

const startCountdownTimer = () => {
  intervalId = setInterval(() => {
    let hasExpired = false;
    
    Object.keys(countdowns.value).forEach(bookingId => {
      if (countdowns.value[bookingId] > 0) {
        countdowns.value[bookingId]--;
      } else if (countdowns.value[bookingId] === 0) {
        hasExpired = true;
      }
    });

    if (hasExpired) {
      toast.warning('Một số booking đã hết hạn thanh toán!');
      fetchMyBookings(); // Reload to remove expired bookings
    }
  }, 1000);
};

const getCountdown = (bookingId) => {
  const seconds = countdowns.value[bookingId] || 0;
  const mins = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${mins}:${secs.toString().padStart(2, '0')}`;
};

const getMinutesLeft = (bookingId) => {
  return Math.floor((countdowns.value[bookingId] || 0) / 60);
};

const getCountdownColor = (bookingId) => {
  const mins = getMinutesLeft(bookingId);
  if (mins < 2) return 'text-red-600 animate-pulse';
  if (mins < 5) return 'text-orange-600';
  return 'text-yellow-700';
};

const payNow = (booking) => {
  selectedBooking.value = booking;
  showPaymentModal.value = true;
};

const confirmPayment = async () => {
  try {
    // Call payment API (wallet deduction)
    const response = await axiosClient.post(`/bookings/${selectedBooking.value.id}/pay`);
    if (response.data.success) {
      toast.success('Thanh toán thành công!');
      showPaymentModal.value = false;
      selectedBooking.value = null;
      await fetchMyBookings();
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Thanh toán thất bại');
  }
};

const cancelBooking = async (bookingId) => {
  const confirmed = await confirmDialog('Xác nhận hủy booking này?', { title: 'Hủy booking', type: 'warning' });
  if (confirmed) {
    try {
      const response = await axiosClient.delete(`/bookings/${bookingId}`);
      if (response.data.success) {
        toast.success('Đã hủy booking');
        await fetchMyBookings();
      }
    } catch (error) {
      toast.error('Hủy booking thất bại');
    }
  }
};

const canCheckIn = (booking) => {
  const now = new Date();
  const startTime = new Date(booking.startTime);
  const diffMinutes = (startTime - now) / 1000 / 60;
  
  // Cho phép check-in trước 30 phút
  return diffMinutes <= 30 && diffMinutes >= -15;
};

const checkIn = async (bookingId) => {
  try {
    const response = await axiosClient.post(`/bookings/${bookingId}/check-in`);
    if (response.data.success) {
      toast.success('Check-in thành công! Chúc bạn thi đấu vui vẻ!');
      await fetchMyBookings();
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Check-in thất bại');
  }
};

const viewDetail = (booking) => {
  // Navigate to detail or show modal
  toast.info('Chi tiết booking: ' + booking.id);
};

const formatDate = (date) => format(parseISO(date), 'dd/MM/yyyy');
const formatTime = (date) => format(parseISO(date), 'HH:mm');
const formatDateTime = (date) => format(parseISO(date), 'HH:mm dd/MM/yyyy');
const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
</script>
