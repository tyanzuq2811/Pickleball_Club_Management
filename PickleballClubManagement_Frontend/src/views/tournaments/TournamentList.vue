<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Danh sách Giải đấu</h2>
      <button class="btn-primary">
        Tạo giải đấu
      </button>
    </div>

    <div v-if="tournamentStore.loading" class="text-center py-10">
      <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-primary-600 mx-auto"></div>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tour in tournamentStore.tournaments" :key="tour.id" 
           class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden hover:shadow-md transition-shadow">
        <div class="h-32 bg-gradient-to-r from-primary-500 to-primary-600 flex items-center justify-center">
          <TrophyIcon class="w-16 h-16 text-white opacity-80" />
        </div>
        <div class="p-6">
          <div class="flex justify-between items-start mb-4">
            <h3 class="text-lg font-bold text-slate-800 line-clamp-1">{{ tour.title }}</h3>
            <span class="px-2 py-1 text-xs font-semibold rounded-full" 
                  :class="getStatusClass(tour.status)">
              {{ getStatusText(tour.status) }}
            </span>
          </div>
          
          <div class="space-y-2 text-sm text-slate-600 mb-6">
            <div class="flex items-center">
              <CalendarIcon class="w-4 h-4 mr-2 text-slate-400" />
              {{ formatDate(tour.startDate) }}
            </div>
            <div class="flex items-center">
              <CurrencyDollarIcon class="w-4 h-4 mr-2 text-slate-400" />
              Phí: {{ formatCurrency(tour.entryFee) }}
            </div>
          </div>

          <router-link :to="`/tournaments/${tour.id}/bracket`" 
             class="block w-full text-center py-2 px-4 bg-slate-50 text-primary-600 font-medium rounded-lg hover:bg-primary-50 transition-colors border border-slate-200">
            Xem Cây Thi Đấu
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useTournamentStore } from '@/stores/tournament';
import { TrophyIcon, CalendarIcon, CurrencyDollarIcon } from '@heroicons/vue/24/outline';
import { format } from 'date-fns';

const tournamentStore = useTournamentStore();

onMounted(() => {
  tournamentStore.fetchTournaments();
});

const formatDate = (date) => date ? format(new Date(date), 'dd/MM/yyyy') : 'Chưa xác định';
const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
const getStatusText = (status) => ['Đăng ký', 'Đang diễn ra', 'Kết thúc'][status] || 'Unknown';
const getStatusClass = (status) => [
  'bg-green-100 text-green-800', 
  'bg-blue-100 text-blue-800', 
  'bg-slate-100 text-slate-800'][status];
</script>