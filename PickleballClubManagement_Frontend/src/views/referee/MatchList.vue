<template>
  <div class="space-y-6">
    <h2 class="text-2xl font-bold text-slate-800">Khu Vực Trọng Tài</h2>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="match in matchStore.matches" :key="match.id" class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="bg-slate-50 p-4 border-b border-slate-100 flex justify-between items-center">
          <span class="font-bold text-slate-700">Trận #{{ match.id }}</span>
          <span class="text-xs px-2 py-1 rounded-full bg-blue-100 text-blue-800">Đang diễn ra</span>
        </div>
        
        <div class="p-6">
          <div class="flex justify-between items-center mb-6">
            <div class="text-center w-1/3">
              <div class="font-bold text-lg text-slate-800 truncate">{{ match.team1Name }}</div>
              <div class="text-4xl font-black text-sky-600 mt-2">{{ match.team1Score }}</div>
            </div>
            <div class="text-slate-400 font-bold text-xl">VS</div>
            <div class="text-center w-1/3">
              <div class="font-bold text-lg text-slate-800 truncate">{{ match.team2Name }}</div>
              <div class="text-4xl font-black text-sky-600 mt-2">{{ match.team2Score }}</div>
            </div>
          </div>

          <!-- Set-by-Set Scores -->
          <div v-if="match.scores && match.scores.length > 0" class="mb-4 p-3 bg-slate-50 rounded-lg">
            <p class="text-xs font-semibold text-slate-500 mb-2">Chi tiết từng set</p>
            <div class="space-y-1">
              <div v-for="(set, idx) in match.scores" :key="idx" class="flex justify-between text-sm">
                <span class="text-slate-600">Set {{ set.setNumber }}:</span>
                <span class="font-bold text-slate-800">{{ set.team1Score }} - {{ set.team2Score }}</span>
              </div>
            </div>
          </div>

          <div class="flex justify-center space-x-4 mb-4">
            <button @click="openScoreModal(match, 1)" class="px-4 py-2 bg-slate-100 hover:bg-slate-200 rounded-lg text-sm font-medium text-slate-700">
              + Điểm Team 1
            </button>
            <button @click="openScoreModal(match, 2)" class="px-4 py-2 bg-slate-100 hover:bg-slate-200 rounded-lg text-sm font-medium text-slate-700">
              + Điểm Team 2
            </button>
          </div>

          <button @click="openScoreModal(match)" class="w-full mb-2 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg font-bold transition-colors text-sm">
            Nhập tỉ số chi tiết
          </button>
          
          <button @click="finishMatch(match)" class="w-full py-2 bg-sky-600 hover:bg-sky-700 text-white rounded-lg font-bold transition-colors">
            Kết thúc trận đấu
          </button>
        </div>
      </div>
    </div>

    <!-- Score Input Modal -->
    <div v-if="showScoreModal && selectedMatch" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Nhập Tỉ Số Chi Tiết</h3>
          
          <div class="mb-4">
            <p class="text-sm text-slate-600 mb-2">{{ selectedMatch.team1Name }} vs {{ selectedMatch.team2Name }}</p>
          </div>

          <div class="space-y-3 mb-6">
            <div v-for="(set, idx) in scoreForm.sets" :key="idx" class="p-3 bg-slate-50 rounded-lg">
              <div class="flex justify-between items-center mb-2">
                <span class="font-semibold text-slate-700">Set {{ idx + 1 }}</span>
                <button v-if="idx > 0" @click="removeSet(idx)" class="text-red-600 text-xs hover:text-red-800">
                  Xóa set
                </button>
              </div>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs text-slate-500 mb-1">Team 1</label>
                  <input v-model.number="set.team1Score" type="number" min="0" max="30" 
                         class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
                </div>
                <div>
                  <label class="block text-xs text-slate-500 mb-1">Team 2</label>
                  <input v-model.number="set.team2Score" type="number" min="0" max="30"
                         class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
                </div>
              </div>
            </div>
          </div>

          <button @click="addSet" class="w-full mb-4 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium text-sm">
            + Thêm set
          </button>

          <div class="flex space-x-2">
            <button @click="showScoreModal = false" 
                    class="flex-1 px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
              Hủy
            </button>
            <button @click="saveScores" 
                    class="flex-1 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors">
              Lưu tỉ số
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useMatchStore } from '@/stores/match';
import { useToast } from 'vue-toastification';

const matchStore = useMatchStore();
const toast = useToast();
const showScoreModal = ref(false);
const selectedMatch = ref(null);
const scoreForm = ref({
  sets: [{ team1Score: 0, team2Score: 0 }]
});

onMounted(() => {
  matchStore.fetchMatches();
});

const openScoreModal = (match, quickTeam = null) => {
  if (quickTeam) {
    // Quick update: just add 1 point
    updateQuickScore(match, quickTeam);
  } else {
    // Open detailed modal
    selectedMatch.value = match;
    scoreForm.value = {
      sets: match.scores && match.scores.length > 0 
        ? JSON.parse(JSON.stringify(match.scores))
        : [{ team1Score: 0, team2Score: 0 }]
    };
    showScoreModal.value = true;
  }
};

const updateQuickScore = async (match, team) => {
  const updated = {
    ...match,
    team1Score: team === 1 ? match.team1Score + 1 : match.team1Score,
    team2Score: team === 2 ? match.team2Score + 1 : match.team2Score
  };
  
  const success = await matchStore.updateMatchScore(match.id, updated);
  if (success) {
    toast.success(`Đã cộng điểm cho Team ${team}`);
  }
};

const addSet = () => {
  scoreForm.value.sets.push({ team1Score: 0, team2Score: 0 });
};

const removeSet = (idx) => {
  scoreForm.value.sets.splice(idx, 1);
};

const saveScores = async () => {
  const totalTeam1 = scoreForm.value.sets.reduce((sum, set) => sum + (set.team1Score > set.team2Score ? 1 : 0), 0);
  const totalTeam2 = scoreForm.value.sets.reduce((sum, set) => sum + (set.team2Score > set.team1Score ? 1 : 0), 0);
  
  const updated = {
    ...selectedMatch.value,
    team1Score: totalTeam1,
    team2Score: totalTeam2,
    scores: scoreForm.value.sets.map((set, idx) => ({
      setNumber: idx + 1,
      team1Score: set.team1Score,
      team2Score: set.team2Score,
      isFinalSet: idx === scoreForm.value.sets.length - 1
    }))
  };

  const success = await matchStore.updateMatchScore(selectedMatch.value.id, updated);
  if (success) {
    toast.success('Cập nhật tỉ số thành công!');
    showScoreModal.value = false;
  }
};

const finishMatch = async (match) => {
  if (confirm(`Xác nhận kết thúc trận đấu với tỉ số ${match.team1Score} - ${match.team2Score}?`)) {
    const success = await matchStore.finishMatch(match.id);
    if (success) {
      toast.success('Trận đấu đã kết thúc! ELO đã được cập nhật.');
    }
  }
};
</script>