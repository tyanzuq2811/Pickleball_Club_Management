<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row justify-between items-center gap-4">
      <h2 class="text-2xl font-bold text-slate-800">Lịch Đặt Sân</h2>
      
      <div class="flex items-center space-x-4">
        <select v-model="selectedCourtId" class="form-select rounded-lg border-slate-300 focus:ring-primary-500 focus:border-primary-500">
          <option v-for="court in bookingStore.courts" :key="court.id" :value="court.id">
            {{ court.name }}
          </option>
        </select>
        
        <div class="flex items-center bg-white rounded-lg shadow-sm border border-slate-200 p-1">
          <button @click="prevWeek" class="p-2 hover:bg-slate-100 rounded-md text-slate-600">
            <ChevronLeftIcon class="w-5 h-5" />
          </button>
          <span class="px-4 font-medium text-slate-700 min-w-[140px] text-center">
            {{ weekRange }}
          </span>
          <button @click="nextWeek" class="p-2 hover:bg-slate-100 rounded-md text-slate-600">
            <ChevronRightIcon class="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>

    <!-- Calendar Grid -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <!-- Header Row (Days) -->
      <div class="grid grid-cols-8 border-b border-slate-200 bg-slate-50">
        <div class="p-4 text-center text-xs font-semibold text-slate-500 uppercase tracking-wider border-r border-slate-200">
          Giờ
        </div>
        <div v-for="day in weekDays" :key="day.date" 
             class="p-4 text-center border-r border-slate-200 last:border-r-0"
             :class="{'bg-primary-50': isToday(day.date)}">
          <div class="text-xs font-semibold text-slate-500 uppercase">{{ day.dayName }}</div>
          <div class="text-lg font-bold text-slate-800" :class="{'text-primary-600': isToday(day.date)}">
            {{ day.dayNumber }}
          </div>
        </div>
      </div>

      <!-- Time Slots -->
      <div class="max-h-[600px] overflow-y-auto custom-scrollbar">
        <div v-for="hour in hours" :key="hour" class="grid grid-cols-8 border-b border-slate-100 last:border-b-0">
          <!-- Time Column -->
          <div class="p-3 text-center text-xs font-medium text-slate-500 border-r border-slate-200 bg-slate-50 sticky left-0">
            {{ formatHour(hour) }}
          </div>

          <!-- Day Columns -->
          <div v-for="day in weekDays" :key="day.date + hour" 
               class="relative border-r border-slate-100 last:border-r-0 h-16 group transition-colors hover:bg-slate-50"
               @click="handleSlotClick(day.date, hour)">
            
            <!-- Booking Block -->
            <div v-if="getBooking(day.date, hour)" 
                 class="absolute inset-1 rounded-md p-1 text-xs font-medium shadow-sm cursor-pointer transition-all hover:shadow-md z-10"
                 :class="getStatusColor(getBooking(day.date, hour).status)">
              <div class="truncate font-bold">{{ getBooking(day.date, hour).memberName }}</div>
              <div class="truncate opacity-75">{{ formatStatus(getBooking(day.date, hour).status) }}</div>
            </div>
            
            <!-- Empty Slot Hover Effect -->
            <div v-else class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 cursor-pointer">
              <PlusIcon class="w-6 h-6 text-primary-400" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useBookingStore } from '@/stores/booking';
import { ChevronLeftIcon, ChevronRightIcon, PlusIcon } from '@heroicons/vue/24/outline';
import { startOfWeek, addDays, format, isSameDay, addHours, startOfDay } from 'date-fns';
import { vi } from 'date-fns/locale';

const bookingStore = useBookingStore();
const currentDate = ref(new Date());
const selectedCourtId = ref(null);

// Generate hours 6:00 - 22:00
const hours = Array.from({ length: 17 }, (_, i) => i + 6);

const weekStart = computed(() => startOfWeek(currentDate.value, { weekStartsOn: 1 }));

const weekDays = computed(() => {
  return Array.from({ length: 7 }, (_, i) => {
    const date = addDays(weekStart.value, i);
    return {
      date: date,
      dayName: format(date, 'EEEE', { locale: vi }),
      dayNumber: format(date, 'dd')
    };
  });
});

const weekRange = computed(() => {
  const start = format(weekStart.value, 'dd/MM');
  const end = format(addDays(weekStart.value, 6), 'dd/MM/yyyy');
  return `${start} - ${end}`;
});

onMounted(async () => {
  await bookingStore.fetchCourts();
  if (bookingStore.courts.length > 0) {
    selectedCourtId.value = bookingStore.courts[0].id;
  }
  await bookingStore.fetchBookings();
});

const prevWeek = () => currentDate.value = addDays(currentDate.value, -7);
const nextWeek = () => currentDate.value = addDays(currentDate.value, 7);
const isToday = (date) => isSameDay(date, new Date());
const formatHour = (h) => `${h}:00`;

const getBooking = (date, hour) => {
  if (!selectedCourtId.value) return null;
  
  // Tạo thời gian bắt đầu của slot hiện tại
  const slotStart = addHours(startOfDay(date), hour);
  const slotEnd = addHours(slotStart, 1);
  
  return bookingStore.bookings.find(b => {
    if (b.courtId !== selectedCourtId.value) return false;
    const bookingStart = new Date(b.startTime);
    const bookingEnd = new Date(b.endTime);
    
    // Check trùng lặp thời gian: Booking bắt đầu trước khi Slot kết thúc VÀ Booking kết thúc sau khi Slot bắt đầu
    return bookingStart < slotEnd && bookingEnd > slotStart;
  });
};

const getStatusColor = (status) => {
  switch (status) {
    case 0: return 'bg-yellow-100 text-yellow-800 border border-yellow-200'; // Pending
    case 1: return 'bg-primary-100 text-primary-800 border border-primary-200'; // Confirmed
    case 2: return 'bg-red-100 text-red-800 border border-red-200'; // Cancelled
    case 3: return 'bg-green-100 text-green-800 border border-green-200'; // CheckedIn
    default: return 'bg-slate-100 text-slate-800';
  }
};

const formatStatus = (status) => {
  const map = { 0: 'Chờ thanh toán', 1: 'Đã đặt', 2: 'Đã hủy', 3: 'Đã check-in' };
  return map[status] || 'Unknown';
};

const handleSlotClick = async (date, hour) => {
  if (getBooking(date, hour)) return; // Đã có booking

  const confirm = window.confirm(`Bạn muốn đặt sân lúc ${hour}:00 ngày ${format(date, 'dd/MM')}?`);
  if (confirm) {
    const startTime = addHours(startOfDay(date), hour);
    const endTime = addHours(startTime, 1); // Mặc định 1 tiếng

    const success = await bookingStore.createBooking({
      courtId: selectedCourtId.value,
      startTime: startTime.toISOString(),
      endTime: endTime.toISOString(),
      notes: "Đặt nhanh từ lịch"
    });
  }
};
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: #f1f5f9;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}
</style>