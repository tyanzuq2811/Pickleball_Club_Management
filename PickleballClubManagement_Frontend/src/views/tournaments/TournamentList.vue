<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Danh sách Giải đấu</h2>
      <button @click="showCreateModal = true" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors font-medium">
        + Tạo giải đấu
      </button>
    </div>

    <!-- Filter Section -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
          <select v-model.number="filters.status" 
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
            <option :value="null">Tất cả</option>
            <option :value="0">Đăng ký</option>
            <option :value="1">Đang diễn ra</option>
            <option :value="2">Kết thúc</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Loại giải</label>
          <select v-model.number="filters.type" 
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
            <option :value="null">Tất cả</option>
            <option :value="0">Loại trực tiếp</option>
            <option :value="1">Vòng tròn</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Chế độ chơi</label>
          <select v-model.number="filters.gameMode" 
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
            <option :value="null">Tất cả</option>
            <option :value="0">Đơn</option>
            <option :value="1">Đôi</option>
          </select>
        </div>
      </div>
    </div>

    <div v-if="tournamentStore.loading" class="text-center py-10">
      <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-sky-600 mx-auto"></div>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="tour in filteredTournaments" :key="tour.id" 
           class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden hover:shadow-md transition-shadow">
        <div class="h-32 bg-gradient-to-r from-sky-500 to-sky-600 flex items-center justify-center">
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
            <button v-if="tour.status === 0 && !authStore.isAdmin && !authStore.isReferee" @click="joinTournament(tour.id)" 
                    class="w-full py-2 px-4 bg-green-600 text-white font-medium rounded-lg hover:bg-green-700 transition-colors">
              Đăng ký tham gia
            </button>

            <!-- Admin/Referee Actions -->
            <template v-if="authStore.isAdmin || authStore.isReferee">
              <div class="flex gap-2">
                <button @click="editTournament(tour)" 
                        class="flex-1 inline-flex items-center justify-center gap-1 py-2 px-3 bg-amber-50 text-amber-600 hover:bg-amber-100 font-medium rounded-lg transition-all duration-200 hover:shadow-md text-sm"
                        title="Chỉnh sửa giải đấu">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                  </svg>
                  Sửa
                </button>
                <button @click="confirmDeleteTournament(tour.id)" 
                        class="flex-1 inline-flex items-center justify-center gap-1 py-2 px-3 bg-red-50 text-red-600 hover:bg-red-100 font-medium rounded-lg transition-all duration-200 hover:shadow-md text-sm"
                        title="Xóa giải đấu">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                  </svg>
                  Xóa
                </button>
              </div>
              <button @click="viewParticipants(tour)"
                      class="w-full inline-flex items-center justify-center gap-2 py-2 px-4 bg-sky-50 text-sky-600 font-medium rounded-lg hover:bg-sky-100 transition-all duration-200 hover:shadow-md text-sm border border-sky-200">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
                </svg>
                Xem người tham gia ({{ tour.participantCount || 0 }})
              </button>
              <button v-if="tour.status === 0" @click="autoDivideTeams(tour.id)"
                      class="w-full inline-flex items-center justify-center gap-2 py-2 px-4 bg-purple-600 text-white font-medium rounded-lg hover:bg-purple-700 transition-all duration-200 hover:shadow-md text-sm">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"/>
                </svg>
                Chia đội tự động
              </button>
              <button v-if="tour.status === 0" @click="generateBracket(tour.id)"
                      class="w-full inline-flex items-center justify-center gap-2 py-2 px-4 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 transition-all duration-200 hover:shadow-md text-sm">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
                </svg>
                Tạo cây thi đấu
              </button>
              <button v-if="tour.status === 0" @click="startTournament(tour.id)"
                      class="w-full inline-flex items-center justify-center gap-2 py-2 px-4 bg-orange-600 text-white font-medium rounded-lg hover:bg-orange-700 transition-all duration-200 hover:shadow-md text-sm">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z"/>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                Bắt đầu giải
              </button>
            </template>

            <router-link :to="`/tournaments/${tour.id}/bracket`" 
               class="block w-full text-center py-2 px-4 bg-slate-50 text-sky-600 font-medium rounded-lg hover:bg-sky-50 transition-colors border border-slate-200">
              Xem Cây Thi Đấu
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Tournament Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">
            {{ showCreateModal ? 'Tạo Giải Đấu Mới' : 'Chỉnh Sửa Giải Đấu' }}
          </h3>
          
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Tên giải đấu</label>
              <input v-model="newTournament.title" type="text" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Mô tả</label>
              <textarea v-model="newTournament.description" rows="3"
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500"></textarea>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày bắt đầu</label>
                <input v-model="newTournament.startDate" type="date" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày kết thúc</label>
                <input v-model="newTournament.endDate" type="date" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Phí tham gia</label>
                <input v-model.number="newTournament.entryFee" type="number" min="0" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Giải thưởng</label>
                <input v-model.number="newTournament.prizePool" type="number" min="0" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Số người tối đa</label>
                <input v-model.number="newTournament.maxParticipants" type="number" min="4" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Loại giải</label>
                <select v-model.number="newTournament.type" required
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
                  <option :value="0">Loại trực tiếp</option>
                  <option :value="1">Vòng tròn</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Chế độ chơi</label>
              <select v-model.number="newTournament.gameMode" required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
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
                      class="flex-1 px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 font-medium disabled:opacity-50">
                {{ tournamentStore.loading ? (showCreateModal ? 'Đang tạo...' : 'Đang cập nhật...') : (showCreateModal ? 'Tạo giải đấu' : 'Cập nhật') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Participants Modal -->
    <div v-if="showParticipantsModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-hidden flex flex-col">
        <div class="p-6 border-b border-slate-200">
          <div class="flex justify-between items-center">
            <h3 class="text-xl font-bold text-slate-800">
              Danh sách người tham gia
            </h3>
            <button @click="showParticipantsModal = false" class="text-slate-400 hover:text-slate-600">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
              </svg>
            </button>
          </div>
          <p class="text-sm text-slate-500 mt-1">{{ selectedTournamentForParticipants?.title }}</p>
        </div>
        
        <div class="p-6 overflow-y-auto flex-1">
          <div v-if="loadingParticipants" class="text-center py-10">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-sky-600 mx-auto"></div>
            <p class="text-slate-500 mt-2">Đang tải...</p>
          </div>
          
          <div v-else-if="participants.length === 0" class="text-center py-10">
            <svg class="w-16 h-16 text-slate-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
            </svg>
            <p class="text-slate-500">Chưa có ai đăng ký tham gia giải đấu này</p>
          </div>
          
          <div v-else class="space-y-3">
            <div v-for="(p, index) in participants" :key="p.id" 
                 class="flex items-center justify-between p-4 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
              <div class="flex items-center gap-4">
                <div class="w-10 h-10 rounded-full bg-sky-100 flex items-center justify-center text-sky-600 font-bold">
                  {{ index + 1 }}
                </div>
                <div>
                  <p class="font-semibold text-slate-800">{{ p.memberName }}</p>
                  <p class="text-xs text-slate-500">Đăng ký: {{ formatDate(p.joinedDate) }}</p>
                </div>
              </div>
              <div class="flex items-center gap-3">
                <span v-if="p.team !== null && p.team !== undefined" 
                      class="px-3 py-1 text-xs font-semibold rounded-full"
                      :class="p.team === 0 ? 'bg-blue-100 text-blue-700' : 'bg-orange-100 text-orange-700'">
                  {{ p.team === 0 ? 'Đội A' : 'Đội B' }}
                </span>
                <span class="px-3 py-1 text-xs font-semibold rounded-full"
                      :class="getParticipantStatusClass(p.status)">
                  {{ getParticipantStatusText(p.status) }}
                </span>
                <span v-if="p.entryFeePaid" class="text-green-500" title="Đã thanh toán">
                  <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
                  </svg>
                </span>
              </div>
            </div>
          </div>
        </div>
        
        <div class="p-4 border-t border-slate-200 bg-slate-50">
          <div class="flex justify-between items-center text-sm text-slate-600">
            <span>Tổng số: <strong>{{ participants.length }}</strong> người</span>
            <button @click="showParticipantsModal = false"
                    class="px-4 py-2 bg-slate-200 text-slate-700 rounded-lg hover:bg-slate-300 font-medium">
              Đóng
            </button>
          </div>
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
import { useConfirmDialog } from '@/composables/useConfirmDialog';

const tournamentStore = useTournamentStore();
const authStore = useAuthStore();
const toast = useToast();
const { confirm, confirmDelete, confirmWarning } = useConfirmDialog();
const showCreateModal = ref(false);
const showEditModal = ref(false);
const showParticipantsModal = ref(false);
const selectedTournament = ref(null);
const selectedTournamentForParticipants = ref(null);
const participants = ref([]);
const loadingParticipants = ref(false);
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

const handleSubmit = async () => {
  if (showCreateModal.value) {
    const success = await tournamentStore.createTournament(newTournament.value);
    if (success) {
      closeModal();
      tournamentStore.fetchTournaments();
    }
  } else if (showEditModal.value) {
    const success = await tournamentStore.updateTournament(selectedTournament.value.id, newTournament.value);
    if (success) {
      toast.success('Cập nhật giải đấu thành công!');
      closeModal();
      tournamentStore.fetchTournaments();
    }
  }
};

const editTournament = (tournament) => {
  selectedTournament.value = tournament;
  newTournament.value = {
    title: tournament.title,
    description: tournament.description || '',
    startDate: format(new Date(tournament.startDate), 'yyyy-MM-dd'),
    endDate: format(new Date(tournament.endDate), 'yyyy-MM-dd'),
    entryFee: tournament.entryFee,
    prizePool: tournament.prizePool,
    maxParticipants: tournament.maxParticipants || 16,
    type: tournament.type,
    gameMode: tournament.gameMode
  };
  showEditModal.value = true;
};

const confirmDeleteTournament = async (tournamentId) => {
  const confirmed = await confirmDelete('Bạn có chắc chắn muốn xóa giải đấu này? Hành động này không thể hoàn tác!');
  if (confirmed) {
    const success = await tournamentStore.deleteTournament(tournamentId);
    if (success) {
      toast.success('Xóa giải đấu thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  selectedTournament.value = null;
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

const getParticipantStatusText = (status) => ['Chờ duyệt', 'Đã xác nhận', 'Bị loại', 'Rút lui'][status] || 'Unknown';
const getParticipantStatusClass = (status) => [
  'bg-yellow-100 text-yellow-800',
  'bg-green-100 text-green-800', 
  'bg-red-100 text-red-800',
  'bg-slate-100 text-slate-800'][status];

// View Participants
const viewParticipants = async (tournament) => {
  selectedTournamentForParticipants.value = tournament;
  showParticipantsModal.value = true;
  loadingParticipants.value = true;
  
  try {
    const data = await tournamentStore.fetchParticipants(tournament.id);
    participants.value = data || [];
  } catch (error) {
    console.error('Error loading participants:', error);
    participants.value = [];
  } finally {
    loadingParticipants.value = false;
  }
};

// Tournament Actions
const joinTournament = async (tournamentId) => {
  const confirmed = await confirm('Xác nhận đăng ký tham gia giải đấu này?', { title: 'Đăng ký giải đấu' });
  if (confirmed) {
    const success = await tournamentStore.joinTournament(tournamentId);
    if (success) {
      toast.success('Đăng ký tham gia thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const autoDivideTeams = async (tournamentId) => {
  const confirmed = await confirm('Tự động chia đội dựa trên ELO ranking?', { title: 'Chia đội tự động', type: 'info' });
  if (confirmed) {
    const success = await tournamentStore.autoDivideTeams(tournamentId);
    if (success) {
      toast.success('Chia đội thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const generateBracket = async (tournamentId) => {
  const confirmed = await confirm('Tạo cây thi đấu cho giải này?', { title: 'Tạo cây thi đấu', type: 'info' });
  if (confirmed) {
    const success = await tournamentStore.generateBracket(tournamentId);
    if (success) {
      toast.success('Tạo cây thi đấu thành công!');
      tournamentStore.fetchTournaments();
    }
  }
};

const startTournament = async (tournamentId) => {
  const confirmed = await confirmWarning('Bắt đầu giải đấu? Hành động này không thể hoàn tác!', { title: 'Bắt đầu giải đấu' });
  if (confirmed) {
    const success = await tournamentStore.startTournament(tournamentId);
    if (success) {
      toast.success('Giải đấu đã bắt đầu!');
      tournamentStore.fetchTournaments();
    }
  }
};
</script>
