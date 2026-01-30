<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Quản Lý Trận Đấu</h2>
      <button v-if="authStore.isReferee || authStore.isAdmin" @click="showCreateModal = true" 
              class="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors font-medium">
        + Tạo trận đấu
      </button>
    </div>

    <!-- Search & Filter -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Tìm kiếm</label>
          <input v-model="searchQuery" type="text" placeholder="Tên đội, giải đấu..."
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
          <select v-model="filters.status" class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            <option :value="null">Tất cả</option>
            <option :value="0">Chờ</option>
            <option :value="1">Đang diễn ra</option>
            <option :value="2">Hoàn thành</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Ngày</label>
          <input v-model="filters.date" type="date" 
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg">
        </div>
      </div>
    </div>

    <!-- Matches Table -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 overflow-hidden">
      <table class="min-w-full divide-y divide-slate-200">
        <thead class="bg-slate-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">ID</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Đội 1</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Đội 2</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Tỷ số</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Ngày</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Trạng thái</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Hành động</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-slate-200">
          <tr v-for="match in filteredMatches" :key="match.id" class="hover:bg-slate-50">
            <td class="px-6 py-4 text-sm text-slate-900">#{{ match.id }}</td>
            <td class="px-6 py-4 text-sm text-slate-900">{{ match.team1Names || 'Team 1' }}</td>
            <td class="px-6 py-4 text-sm text-slate-900">{{ match.team2Names || 'Team 2' }}</td>
            <td class="px-6 py-4 text-sm font-semibold">
              {{ match.team1Score || 0 }} - {{ match.team2Score || 0 }}
            </td>
            <td class="px-6 py-4 text-sm text-slate-900">{{ formatDate(match.matchDate) }}</td>
            <td class="px-6 py-4">
              <span class="px-2 py-1 text-xs font-semibold rounded-full"
                    :class="getStatusClass(match.status)">
                {{ getStatusText(match.status) }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm">
              <div class="flex items-center justify-center gap-2">
                <button @click="editMatch(match)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Chỉnh sửa">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                  </svg>
                  Sửa
                </button>
                <button v-if="authStore.isAdmin" @click="confirmDelete(match.id)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-red-50 text-red-600 hover:bg-red-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Xóa">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                  </svg>
                  Xóa
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Match Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">
            {{ showCreateModal ? 'Tạo Trận Đấu' : 'Chỉnh Sửa Trận Đấu' }}
          </h3>
          
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Giải đấu</label>
              <select v-model="matchForm.tournamentId" 
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg">
                <option :value="null">Trận giao hữu</option>
                <option v-for="t in tournaments" :key="t.id" :value="t.id">{{ t.title }}</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Người chơi Đội 1 (ID, phân cách bằng dấu phẩy)</label>
              <input v-model="matchForm.team1Ids" type="text" required placeholder="1,2"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Người chơi Đội 2 (ID, phân cách bằng dấu phẩy)</label>
              <input v-model="matchForm.team2Ids" type="text" required placeholder="3,4"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Ngày thi đấu</label>
              <input v-model="matchForm.matchDate" type="datetime-local" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Chế độ</label>
              <select v-model.number="matchForm.gameMode" required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg">
                <option :value="0">Đơn</option>
                <option :value="1">Đôi</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Định dạng</label>
              <select v-model.number="matchForm.matchFormat" required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg">
                <option :value="0">Best of 1</option>
                <option :value="1">Best of 3</option>
                <option :value="2">Best of 5</option>
              </select>
            </div>

            <div class="flex items-center">
              <input v-model="matchForm.isRanked" type="checkbox" 
                     class="w-4 h-4 text-primary-600 border-slate-300 rounded focus:ring-primary-500">
              <label class="ml-2 text-sm text-slate-700">Trận ranked (tính ELO)</label>
            </div>

            <div class="flex space-x-3 pt-4">
              <button type="button" @click="closeModal"
                      class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
                Hủy
              </button>
              <button type="submit" 
                      class="flex-1 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 font-medium">
                {{ showCreateModal ? 'Tạo' : 'Cập nhật' }}
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
import { useAuthStore } from '@/stores/auth';
import { format, parseISO } from 'date-fns';
import { useToast } from 'vue-toastification';
import axiosClient from '@/api/axiosClient';

const authStore = useAuthStore();
const toast = useToast();

const searchQuery = ref('');
const filters = ref({
  status: null,
  date: ''
});

const matches = ref([]);
const tournaments = ref([]);
const showCreateModal = ref(false);
const showEditModal = ref(false);
const selectedMatch = ref(null);

const matchForm = ref({
  tournamentId: null,
  team1Ids: '',
  team2Ids: '',
  matchDate: '',
  gameMode: 1,
  matchFormat: 1,
  isRanked: false
});

const filteredMatches = computed(() => {
  let result = matches.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(m => 
      m.team1Names?.toLowerCase().includes(query) ||
      m.team2Names?.toLowerCase().includes(query) ||
      m.tournamentTitle?.toLowerCase().includes(query)
    );
  }

  if (filters.value.status !== null) {
    result = result.filter(m => m.status === filters.value.status);
  }

  if (filters.value.date) {
    result = result.filter(m => {
      const matchDate = format(parseISO(m.matchDate), 'yyyy-MM-dd');
      return matchDate === filters.value.date;
    });
  }

  return result;
});

const fetchMatches = async () => {
  try {
    const response = await axiosClient.get('/matches?pageSize=100');
    if (response.data.success) {
      matches.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching matches:', error);
  }
};

const fetchTournaments = async () => {
  try {
    const response = await axiosClient.get('/tournaments?pageSize=100');
    if (response.data.success) {
      tournaments.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching tournaments:', error);
  }
};

onMounted(() => {
  fetchMatches();
  fetchTournaments();
});

const editMatch = (match) => {
  selectedMatch.value = match;
  matchForm.value = {
    tournamentId: match.tournamentId,
    team1Ids: match.team1Ids?.join(',') || '',
    team2Ids: match.team2Ids?.join(',') || '',
    matchDate: format(parseISO(match.matchDate), "yyyy-MM-dd'T'HH:mm"),
    gameMode: match.gameMode || 1,
    matchFormat: match.matchFormat || 1,
    isRanked: match.isRanked || false
  };
  showEditModal.value = true;
};

const handleSubmit = async () => {
  try {
    const data = {
      ...matchForm.value,
      team1Ids: matchForm.value.team1Ids.split(',').map(id => parseInt(id.trim())),
      team2Ids: matchForm.value.team2Ids.split(',').map(id => parseInt(id.trim())),
      matchDate: new Date(matchForm.value.matchDate).toISOString()
    };

    if (showCreateModal.value) {
      const response = await axiosClient.post('/matches', data);
      if (response.data.success) {
        toast.success('Tạo trận đấu thành công!');
        closeModal();
        fetchMatches();
      }
    } else {
      const response = await axiosClient.put(`/matches/${selectedMatch.value.id}`, data);
      if (response.data.success) {
        toast.success('Cập nhật trận đấu thành công!');
        closeModal();
        fetchMatches();
      }
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Lỗi khi xử lý trận đấu');
  }
};

const confirmDelete = async (matchId) => {
  if (confirm('Bạn có chắc chắn muốn xóa trận đấu này?')) {
    try {
      const response = await axiosClient.delete(`/matches/${matchId}`);
      if (response.data.success) {
        toast.success('Xóa trận đấu thành công!');
        fetchMatches();
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Lỗi khi xóa trận đấu');
    }
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  selectedMatch.value = null;
  matchForm.value = {
    tournamentId: null,
    team1Ids: '',
    team2Ids: '',
    matchDate: '',
    gameMode: 1,
    matchFormat: 1,
    isRanked: false
  };
};

const formatDate = (dateStr) => {
  try {
    return format(parseISO(dateStr), 'dd/MM/yyyy HH:mm');
  } catch {
    return dateStr;
  }
};

const getStatusText = (status) => ['Chờ', 'Đang diễn ra', 'Hoàn thành'][status] || 'Unknown';
const getStatusClass = (status) => [
  'bg-yellow-100 text-yellow-800',
  'bg-blue-100 text-blue-800',
  'bg-green-100 text-green-800'
][status];
</script>
