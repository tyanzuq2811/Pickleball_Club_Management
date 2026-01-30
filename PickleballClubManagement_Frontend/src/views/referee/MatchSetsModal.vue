<template>
  <div v-if="show" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="$emit('close')">
    <div class="bg-white rounded-xl shadow-xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto">
      <!-- Header -->
      <div class="p-6 border-b border-slate-200 flex justify-between items-center bg-gradient-to-r from-blue-600 to-indigo-600">
        <div class="text-white">
          <h3 class="text-2xl font-bold">Chi tiết điểm số trận đấu</h3>
          <p class="text-blue-100 text-sm mt-1">{{ match.format }} - {{ formatDate(match.startTime) }}</p>
        </div>
        <button @click="$emit('close')" class="text-white hover:text-blue-100">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
          </svg>
        </button>
      </div>

      <!-- Match Overview -->
      <div class="p-6 bg-slate-50">
        <div class="grid grid-cols-3 gap-4 items-center">
          <!-- Team 1 -->
          <div class="text-center">
            <div class="bg-white rounded-lg p-4 shadow-sm">
              <div class="font-bold text-lg text-slate-800 mb-1">{{ match.player1Name }}</div>
              <div v-if="match.player2Name" class="text-sm text-slate-600">{{ match.player2Name }}</div>
              <div class="text-4xl font-bold mt-2" :class="match.winningSide === 1 ? 'text-green-600' : 'text-slate-700'">
                {{ match.score1 }}
              </div>
              <div v-if="match.winningSide === 1" class="mt-2 text-green-600 font-bold">🏆 Thắng</div>
            </div>
          </div>

          <!-- VS -->
          <div class="text-center">
            <div class="text-3xl font-bold text-slate-400">VS</div>
          </div>

          <!-- Team 2 -->
          <div class="text-center">
            <div class="bg-white rounded-lg p-4 shadow-sm">
              <div class="font-bold text-lg text-slate-800 mb-1">{{ match.player3Name }}</div>
              <div v-if="match.player4Name" class="text-sm text-slate-600">{{ match.player4Name }}</div>
              <div class="text-4xl font-bold mt-2" :class="match.winningSide === 2 ? 'text-green-600' : 'text-slate-700'">
                {{ match.score2 }}
              </div>
              <div v-if="match.winningSide === 2" class="mt-2 text-green-600 font-bold">🏆 Thắng</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sets Detail -->
      <div class="p-6">
        <div class="flex justify-between items-center mb-4">
          <h4 class="text-lg font-bold text-slate-800">Chi tiết từng set</h4>
          <button v-if="canEdit" @click="showAddSetModal = true" 
                  class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-medium text-sm flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            Thêm Set
          </button>
        </div>

        <div v-if="sets.length === 0" class="text-center py-12 bg-slate-50 rounded-lg">
          <svg class="w-16 h-16 mx-auto text-slate-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/>
          </svg>
          <p class="text-slate-500">Chưa có dữ liệu điểm theo set</p>
          <p class="text-sm text-slate-400 mt-1">Thêm điểm từng set để theo dõi chi tiết</p>
        </div>

        <div v-else class="space-y-3">
          <div v-for="set in sets" :key="set.id" 
               class="bg-white border-2 rounded-lg p-4 transition-all hover:shadow-md"
               :class="set.isFinalSet ? 'border-yellow-400 bg-yellow-50' : 'border-slate-200'">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-4 flex-1">
                <div class="flex items-center gap-2">
                  <span class="text-lg font-bold text-slate-700">Set {{ set.setNumber }}</span>
                  <span v-if="set.isFinalSet" class="px-2 py-1 bg-yellow-400 text-yellow-900 text-xs font-bold rounded-full">
                    FINAL SET
                  </span>
                </div>
                
                <div class="flex items-center gap-6 flex-1">
                  <!-- Team 1 Score -->
                  <div class="flex items-center gap-2">
                    <span class="text-slate-600 text-sm">{{ match.player1Name }}</span>
                    <div class="w-16 text-center">
                      <span class="text-2xl font-bold" 
                            :class="set.winningSide === 1 ? 'text-green-600' : 'text-slate-700'">
                        {{ set.team1Score }}
                      </span>
                    </div>
                  </div>

                  <span class="text-slate-400">-</span>

                  <!-- Team 2 Score -->
                  <div class="flex items-center gap-2">
                    <div class="w-16 text-center">
                      <span class="text-2xl font-bold"
                            :class="set.winningSide === 2 ? 'text-green-600' : 'text-slate-700'">
                        {{ set.team2Score }}
                      </span>
                    </div>
                    <span class="text-slate-600 text-sm">{{ match.player3Name }}</span>
                  </div>

                  <div v-if="set.winningSide" class="ml-auto">
                    <span class="px-3 py-1 bg-green-100 text-green-800 text-xs font-bold rounded-full">
                      {{ set.winningSide === 1 ? match.player1Name : match.player3Name }} thắng
                    </span>
                  </div>
                </div>
              </div>

              <div v-if="canEdit" class="flex gap-2 ml-4">
                <button @click="editSet(set)" 
                        class="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                  </svg>
                </button>
                <button @click="deleteSet(set.id)" 
                        class="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Summary Stats -->
        <div v-if="sets.length > 0" class="mt-6 p-4 bg-blue-50 rounded-lg">
          <div class="grid grid-cols-3 gap-4 text-center">
            <div>
              <div class="text-sm text-slate-600">Tổng số set</div>
              <div class="text-2xl font-bold text-blue-600">{{ sets.length }}</div>
            </div>
            <div>
              <div class="text-sm text-slate-600">{{ match.player1Name }} thắng</div>
              <div class="text-2xl font-bold text-green-600">{{ setsWonByTeam1 }}</div>
            </div>
            <div>
              <div class="text-sm text-slate-600">{{ match.player3Name }} thắng</div>
              <div class="text-2xl font-bold text-red-600">{{ setsWonByTeam2 }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Add/Edit Set Modal -->
      <div v-if="showAddSetModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-[60]">
        <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
          <div class="p-6 border-b border-slate-200">
            <h4 class="text-xl font-bold text-slate-800">{{ editingSet ? 'Chỉnh sửa Set' : 'Thêm Set Mới' }}</h4>
          </div>
          <form @submit.prevent="submitSet" class="p-6 space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số set</label>
              <input v-model.number="setForm.setNumber" type="number" min="1" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">
                  Điểm {{ match.player1Name }}
                </label>
                <input v-model.number="setForm.team1Score" type="number" min="0" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">
                  Điểm {{ match.player3Name }}
                </label>
                <input v-model.number="setForm.team2Score" type="number" min="0" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
              </div>
            </div>
            <div class="flex items-center">
              <input v-model="setForm.isFinalSet" type="checkbox" 
                     class="w-4 h-4 text-blue-600 border-slate-300 rounded focus:ring-2 focus:ring-blue-500">
              <label class="ml-2 text-sm font-medium text-slate-700">Set quyết định (Final Set)</label>
            </div>
            <div class="flex gap-3">
              <button type="button" @click="closeSetModal" 
                      class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50">
                Hủy
              </button>
              <button type="submit" 
                      class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-medium">
                {{ editingSet ? 'Cập nhật' : 'Thêm' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import { format } from 'date-fns';
import axiosClient from '@/api/axiosClient';
import { useToast } from 'vue-toastification';
import { useAuthStore } from '@/stores/auth';

const props = defineProps({
  show: Boolean,
  matchId: Number
});

const emit = defineEmits(['close', 'updated']);

const authStore = useAuthStore();
const toast = useToast();

const match = ref({});
const sets = ref([]);
const showAddSetModal = ref(false);
const editingSet = ref(null);

const setForm = ref({
  setNumber: 1,
  team1Score: 0,
  team2Score: 0,
  isFinalSet: false
});

const canEdit = computed(() => {
  const role = authStore.role;
  return role === 'Admin' || role === 'Referee';
});

const setsWonByTeam1 = computed(() => sets.value.filter(s => s.winningSide === 1).length);
const setsWonByTeam2 = computed(() => sets.value.filter(s => s.winningSide === 2).length);

watch(() => props.show, async (newVal) => {
  if (newVal && props.matchId) {
    await fetchMatchData();
  }
});

const fetchMatchData = async () => {
  try {
    const response = await axiosClient.get(`/matches/${props.matchId}/sets`);
    if (response.data.success) {
      match.value = response.data.data;
      sets.value = response.data.data.sets || [];
    }
  } catch (error) {
    console.error('Error fetching match data:', error);
    toast.error('Không thể tải dữ liệu trận đấu');
  }
};

const editSet = (set) => {
  editingSet.value = set;
  setForm.value = {
    setNumber: set.setNumber,
    team1Score: set.team1Score,
    team2Score: set.team2Score,
    isFinalSet: set.isFinalSet
  };
  showAddSetModal.value = true;
};

const submitSet = async () => {
  try {
    if (editingSet.value) {
      await axiosClient.put(`/matches/sets/${editingSet.value.id}`, setForm.value);
      toast.success('Cập nhật set thành công');
    } else {
      await axiosClient.post(`/matches/${props.matchId}/sets`, {
        ...setForm.value,
        matchId: props.matchId
      });
      toast.success('Thêm set thành công');
    }
    
    closeSetModal();
    await fetchMatchData();
    emit('updated');
  } catch (error) {
    toast.error('Có lỗi xảy ra');
  }
};

const deleteSet = async (setId) => {
  if (confirm('Xác nhận xóa set này?')) {
    try {
      await axiosClient.delete(`/matches/sets/${setId}`);
      toast.success('Xóa set thành công');
      await fetchMatchData();
      emit('updated');
    } catch (error) {
      toast.error('Xóa set thất bại');
    }
  }
};

const closeSetModal = () => {
  showAddSetModal.value = false;
  editingSet.value = null;
  setForm.value = {
    setNumber: sets.value.length + 1,
    team1Score: 0,
    team2Score: 0,
    isFinalSet: false
  };
};

const formatDate = (date) => format(new Date(date), 'dd/MM/yyyy HH:mm');
</script>
