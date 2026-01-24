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

          <div class="flex justify-center space-x-4">
            <button @click="updateScore(match, 1)" class="px-4 py-2 bg-slate-100 hover:bg-slate-200 rounded-lg text-sm font-medium text-slate-700">
              + Điểm Team 1
            </button>
            <button @click="updateScore(match, 2)" class="px-4 py-2 bg-slate-100 hover:bg-slate-200 rounded-lg text-sm font-medium text-slate-700">
              + Điểm Team 2
            </button>
          </div>
          
          <button @click="finishMatch(match)" class="w-full mt-6 py-2 bg-sky-600 hover:bg-sky-700 text-white rounded-lg font-bold transition-colors">
            Kết thúc trận đấu
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useMatchStore } from '@/stores/match';

const matchStore = useMatchStore();

onMounted(() => {
  matchStore.fetchMatches();
});

const updateScore = (match, team) => {
  if (team === 1) match.team1Score++;
  else match.team2Score++;
  // Gọi API update real-time nếu cần
};

const finishMatch = async (match) => {
  if (confirm(`Xác nhận kết thúc trận đấu với tỉ số ${match.team1Score} - ${match.team2Score}?`)) {
    await matchStore.updateScore(match.id, match.team1Score, match.team2Score, true);
  }
};
</script>