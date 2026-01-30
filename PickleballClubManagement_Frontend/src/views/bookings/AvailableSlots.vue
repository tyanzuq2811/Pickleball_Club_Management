<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Đặt sân trực tuyến</h2>
      <div class="flex gap-3">
        <button @click="showAvailableSlotsModal = true" 
                class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium text-sm flex items-center gap-2">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
          </svg>
          Xem slot trống
        </button>
        <router-link to="/bookings/manage" v-if="authStore.isAdmin || authStore.isTreasurer"
                     class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors font-medium text-sm">
          Quản lý đặt sân
        </router-link>
      </div>
    </div>

    <!-- Available Slots Modal -->
    <div v-if="showAvailableSlotsModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-xl max-w-4xl w-full shadow-2xl max-h-[90vh] overflow-y-auto">
        <div class="p-6 border-b border-slate-200 flex justify-between items-center sticky top-0 bg-white">
          <h3 class="text-2xl font-bold text-slate-800">Khung giờ còn trống</h3>
          <button @click="showAvailableSlotsModal = false" class="text-slate-400 hover:text-slate-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>

        <div class="p-6 space-y-4">
          <!-- Filters -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Chọn sân</label>
              <select v-model="slotsFilter.courtId" @change="loadAvailableSlots"
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
                <option :value="null">-- Chọn sân --</option>
                <option v-for="court in courtStore.courts" :key="court.id" :value="court.id">
                  {{ court.name }}
                </option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Chọn ngày</label>
              <input v-model="slotsFilter.date" @change="loadAvailableSlots" type="date" :min="minDate"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
            </div>
          </div>

          <!-- Loading -->
          <div v-if="loadingSlots" class="py-12 text-center">
            <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600 mx-auto"></div>
            <p class="mt-4 text-slate-500">Đang tải khung giờ...</p>
          </div>

          <!-- No selection -->
          <div v-else-if="!slotsFilter.courtId || !slotsFilter.date" class="py-12 text-center text-slate-500">
            <svg class="w-16 h-16 mx-auto mb-3 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
            </svg>
            <p>Vui lòng chọn sân và ngày để xem khung giờ trống</p>
          </div>

          <!-- Slots Grid -->
          <div v-else class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
            <button v-for="slot in availableSlots" :key="slot.startTime"
                    @click="selectSlotForBooking(slot)"
                    :disabled="!slot.isAvailable"
                    class="p-4 rounded-lg border-2 transition-all text-center"
                    :class="slot.isAvailable 
                      ? 'border-green-300 bg-green-50 hover:bg-green-100 hover:border-green-500 cursor-pointer' 
                      : 'border-slate-200 bg-slate-50 cursor-not-allowed opacity-60'">
              <div class="text-lg font-bold" :class="slot.isAvailable ? 'text-green-700' : 'text-slate-400'">
                {{ slot.startTime }} - {{ slot.endTime }}
              </div>
              <div v-if="slot.isAvailable" class="text-xs text-green-600 mt-1 font-medium">
                ✓ Trống
              </div>
              <div v-else class="text-xs text-slate-500 mt-1">
                <div>✗ Đã đặt</div>
                <div v-if="slot.bookedBy" class="truncate">{{ slot.bookedBy }}</div>
              </div>
            </button>
          </div>

          <!-- No slots available -->
          <div v-if="!loadingSlots && slotsFilter.courtId && slotsFilter.date && availableSlots.length === 0" 
               class="py-12 text-center text-slate-500">
            <svg class="w-16 h-16 mx-auto mb-3 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <p>Không có khung giờ nào trong ngày này</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Booking Modal (từ Available Slots) -->
    <div v-if="showQuickBookingModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4">
      <div class="bg-white rounded-xl max-w-md w-full shadow-2xl">
        <div class="p-6 border-b border-slate-200">
          <h3 class="text-xl font-bold text-slate-800">Xác nhận đặt sân</h3>
        </div>
        <div class="p-6 space-y-4">
          <div class="bg-blue-50 p-4 rounded-lg space-y-2">
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Sân:</span>
              <span class="font-bold text-blue-900">{{ getCourtName(slotsFilter.courtId) }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Ngày:</span>
              <span class="font-bold text-blue-900">{{ formatDateVN(slotsFilter.date) }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Giờ:</span>
              <span class="font-bold text-blue-900">{{ selectedSlot?.startTime }} - {{ selectedSlot?.endTime }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-blue-700">Thời lượng:</span>
              <span class="font-bold text-blue-900">{{ calculateDuration(selectedSlot) }} giờ</span>
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Ghi chú (tùy chọn)</label>
            <textarea v-model="quickBookingNotes" rows="3" placeholder="VD: Đánh đôi, cần 4 người..."
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500"></textarea>
          </div>
        </div>
        <div class="p-6 bg-slate-50 border-t border-slate-200 flex justify-end space-x-3">
          <button @click="showQuickBookingModal = false" 
                  class="px-4 py-2 text-slate-700 hover:bg-slate-200 rounded-lg font-medium transition-colors">
            Hủy
          </button>
          <button @click="confirmQuickBooking" 
                  class="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 font-medium transition-colors">
            Xác nhận đặt sân
          </button>
        </div>
      </div>
    </div>

    <!-- Existing calendar UI... -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
      <p class="text-slate-600">Calendar view here (existing code)...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useCourtStore } from '@/stores/court';
import { useAuthStore } from '@/stores/auth';
import { useToast } from 'vue-toastification';
import axiosClient from '@/api/axiosClient';
import { format, parseISO } from 'date-fns';

const courtStore = useCourtStore();
const authStore = useAuthStore();
const toast = useToast();

const showAvailableSlotsModal = ref(false);
const showQuickBookingModal = ref(false);
const loadingSlots = ref(false);
const availableSlots = ref([]);
const selectedSlot = ref(null);
const quickBookingNotes = ref('');

const slotsFilter = ref({
  courtId: null,
  date: format(new Date(), 'yyyy-MM-dd')
});

const minDate = computed(() => format(new Date(), 'yyyy-MM-dd'));

onMounted(async () => {
  await courtStore.fetchCourts();
  if (courtStore.courts.length > 0) {
    slotsFilter.value.courtId = courtStore.courts[0].id;
    await loadAvailableSlots();
  }
});

const loadAvailableSlots = async () => {
  if (!slotsFilter.value.courtId || !slotsFilter.value.date) return;
  
  loadingSlots.value = true;
  try {
    const response = await axiosClient.get('/bookings/available-slots', {
      params: {
        courtId: slotsFilter.value.courtId,
        date: slotsFilter.value.date
      }
    });
    
    if (response.data.success) {
      availableSlots.value = response.data.data || [];
    }
  } catch (error) {
    console.error('Error loading slots:', error);
    toast.error('Lỗi khi tải khung giờ');
  } finally {
    loadingSlots.value = false;
  }
};

const selectSlotForBooking = (slot) => {
  if (!slot.isAvailable) return;
  selectedSlot.value = slot;
  showQuickBookingModal.value = true;
};

const confirmQuickBooking = async () => {
  try {
    const [startHour, startMin] = selectedSlot.value.startTime.split(':');
    const [endHour, endMin] = selectedSlot.value.endTime.split(':');
    
    const startDateTime = new Date(slotsFilter.value.date);
    startDateTime.setHours(parseInt(startHour), parseInt(startMin), 0);
    
    const endDateTime = new Date(slotsFilter.value.date);
    endDateTime.setHours(parseInt(endHour), parseInt(endMin), 0);

    const bookingData = {
      courtId: slotsFilter.value.courtId,
      startTime: startDateTime.toISOString(),
      endTime: endDateTime.toISOString(),
      notes: quickBookingNotes.value || ''
    };

    const response = await axiosClient.post('/bookings', bookingData);
    
    if (response.data.success) {
      toast.success('Đặt sân thành công!');
      showQuickBookingModal.value = false;
      showAvailableSlotsModal.value = false;
      quickBookingNotes.value = '';
      selectedSlot.value = null;
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Đặt sân thất bại');
  }
};

const getCourtName = (courtId) => {
  const court = courtStore.courts.find(c => c.id === courtId);
  return court?.name || 'N/A';
};

const formatDateVN = (dateStr) => {
  try {
    return format(parseISO(dateStr), 'dd/MM/yyyy');
  } catch {
    return dateStr;
  }
};

const calculateDuration = (slot) => {
  if (!slot) return 0;
  const [startH] = slot.startTime.split(':').map(Number);
  const [endH] = slot.endTime.split(':').map(Number);
  return endH - startH;
};
</script>
