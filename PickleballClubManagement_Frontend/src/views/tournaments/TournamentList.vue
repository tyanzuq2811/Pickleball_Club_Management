<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Danh sách Giải đấu</h2>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium">
        + Tạo giải đấu
      </button>
    </div>

    <!-- Filter Section -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
          <select v-model.number="filters.status" 
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
            <option :value="null">Tất cả</option>
            <option :value="0">Đăng ký</option>
            <option :value="1">Đang diễn ra</option>
            <option :value="2">Kết thúc</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Loại giải</label>
          <select v-model.number="filters.type" 
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
            <option :value="null">Tất cả</option>
            <option :value="0">Loại trực tiếp</option>
            <option :value="1">Vòng tròn</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Chế độ chơi</label>
          <select v-model.number="filters.gameMode" 
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
            <option :value="null">Tất cả</option>
            <option :value="0">Đơn</option>
            <option :value="1">Đôi</option>
          </select>
        </div>
      </div>
    </div>

    <div v-if="tournamentStore.loading" class="text-center py-10">
      <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-primary-600 mx-auto"></div>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tour in filteredTournaments" :key="tour.id" 
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
            <div class="flex items-center text-xs">
              <span class="text-slate-500">{{ tour.participantCount || 0 }}/{{ tour.maxParticipants }} người</span>
            </div>
          </div>

          <div class="space-y-2">
            <!-- Member: Join Tournament -->
            <button v-if="tour.status === 0 && !authStore.isAdmin" @click="joinTournament(tour.id)" 
                    class="w-full py-2 px-4 bg-green-600 text-white font-medium rounded-lg hover:bg-green-700 transition-colors">
              Đăng ký tham gia
            </button>

            <!-- Admin Actions -->
            <template v-if="authStore.isAdmin">
              <button v-if="tour.status === 0" @click="autoDivideTeams(tour.id)"
                      class="w-full py-2 px-4 bg-purple-600 text-white font-medium rounded-lg hover:bg-purple-700 transition-colors text-sm">
                Chia đội tự động
              </button>
              <button v-if="tour.status === 0" @click="generateBracket(tour.id)"
                      class="w-full py-2 px-4 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 transition-colors text-sm">
                Tạo cây thi đấu
              </button>
              <button v-if="tour.status === 0" @click="startTournament(tour.id)"
                      class="w-full py-2 px-4 bg-orange-600 text-white font-medium rounded-lg hover:bg-orange-700 transition-colors text-sm">
                Bắt đầu giải
              </button>
            </template>

            <router-link :to="`/tournaments/${tour.id}/bracket`" 
               class="block w-full text-center py-2 px-4 bg-slate-50 text-primary-600 font-medium rounded-lg hover:bg-primary-50 transition-colors border border-slate-200">
              Xem Cây Thi Đấu
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Tournament Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Tạo Giải Đấu Mới</h3>
          
          <form @submit.prevent="handleCreateTournament" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Tên giải đấu</label>
              <input v-model="newTournament.title" type="text" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Mô tả</label>
              <textarea v-model="newTournament.description" rows="3"
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500"></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày bắt đầu</label>
                <input v-model="newTournament.startDate" type="date" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày kết thúc</label>
                <input v-model="newTournament.endDate" type="date" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Phí tham gia</label>
                <input v-model.number="newTournament.entryFee" type="number" min="0" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Giải thưởng</label>
                <input v-model.number="newTournament.prizePool" type="number" min="0" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Số người tối đa</label>
                <input v-model.number="newTournament.maxParticipants" type="number" min="4" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Loại giải</label>
                <select v-model.number="newTournament.type" required
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
                  <option :value="0">Loại trực tiếp</option>
                  <option :value="1">Vòng tròn</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Chế độ chơi</label>
              <select v-model.number="newTournament.gameMode" required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
                <option :value="0">Đơn</option>
                <option :value="1">Đôi</option>
              </select>
            </div>

            <div class="flex space-x-3 pt-4">
              <button type="button" @click="closeModal"
                      class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
                Hủy
              </button>
              <button type="submit" :disabled="tournamentStore.loading"
                      class="flex-1 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 font-medium disabled:opacity-50">
                {{ tournamentStore.loading ? 'Đang tạo...' : 'Tạo giải đấu' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useTournamentStore } from '@/stores/tournament';
import { useAuthStore } from '@/stores/auth';
import { TrophyIcon, CalendarIcon, CurrencyDollarIcon } from '@heroicons/vue/24/outline';
import { format } from 'date-fns';
import { useToast } from 'vue-toastification';

const tournamentStore = useTournamentStore();
const authStore = useAuthStore();
const toast = useToast();
const showCreateModal = ref(false);
const newTournament = ref({
  title: '',
  description: '',
  startDate: '',
  endDate: '',
  entryFee: 50000,
  prizePool: 500000,
  maxParticipants: 16,
  type: 0,
  gameMode: 0
});

const filters = ref({
  status: null,
  type: null,
  gameMode: null
});

const filteredTournaments = computed(() => {
  let result = tournamentStore.tournaments;
  
  if (filters.value.status !== null) {
    result = result.filter(t => t.status === filters.value.status);
  }
  
  if (filters.value.type !== null) {
    result = result.filter(t => t.type === filters.value.type);
  }
  
  if (filters.value.gameMode !== null) {
    result = result.filter(t => t.gameMode === filters.value.gameMode);
  }
  
  return result;
});

onMounted(() => {
  tournamentStore.fetchTournaments();
});

const handleCreateTournament = async () => {
  const success = await tournamentStore.createTournament(newTournament.value);
  if (success) {
    closeModal();
    tournamentStore.fetchTournaments();
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  newTournament.value = {
    title: '',
    description: '',
    startDate: '',
    endDate: '',
    entryFee: 50000,
    prizePool: 500000,
    maxParticipants: 16,
    type: 0,
    gameMode: 0
  };
};

const formatDate = (date) => date ? format(new Date(date), 'dd/MM/yyyy') : 'Chưa xác định';
const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
const getStatusText = (status) => ['Đăng ký', 'Đang diễn ra', 'Kết thúc'][status] || 'Unknown';
const getStatusClass = (status) => [
  'bg-green-100 text-green-800', 
  'bg-blue-100 text-blue-800', 
  'bg-slate-100 text-slate-800'][status];

// Tournament Actions
const joinTournament = async (tournamentId) => {
  if (confirm('Xác nhận đăng ký tham gia giải đấu này?')) {
    const success = await tournamentStore.joinTournament(tournamentId);
    if (success) {
      toast.success('Đăng ký tham gia thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const autoDivideTeams = async (tournamentId) => {
  if (confirm('Tự động chia đội dựa trên ELO ranking?')) {
    const success = await tournamentStore.autoDivideTeams(tournamentId);
    if (success) {
      toast.success('Chia đội thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const generateBracket = async (tournamentId) => {
  if (confirm('Tạo cây thi đấu cho giải này?')) {
    const success = await tournamentStore.generateBracket(tournamentId);
    if (success) {
      toast.success('Tạo cây thi đấu thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const startTournament = async (tournamentId) => {
  if (confirm('Bắt đầu giải đấu? Không thể hoàn tác!')) {
    const success = await tournamentStore.startTournament(tournamentId);
    if (success) {
      toast.success('Giải đấu đã bắt đầu!');
      tournamentStore.fetchTournaments();
    }
  }
};
</script>