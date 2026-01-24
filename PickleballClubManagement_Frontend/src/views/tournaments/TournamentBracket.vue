<template>
  <div class="h-full flex flex-col">
    <div class="mb-6 flex items-center space-x-4">
      <router-link to="/tournaments" class="p-2 hover:bg-slate-100 rounded-full text-slate-500">
        <ArrowLeftIcon class="w-5 h-5" />
      </router-link>
      <h2 class="text-2xl font-bold text-slate-800">Cây Thi Đấu</h2>
    </div>

    <div v-if="tournamentStore.loading" class="flex-1 flex items-center justify-center">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>

    <div v-else-if="!tournamentStore.currentBracket" class="flex-1 flex flex-col items-center justify-center text-slate-500">
      <TrophyIcon class="w-16 h-16 mb-4 opacity-20" />
      <p>Chưa có sơ đồ thi đấu cho giải này.</p>
    </div>

    <!-- Bracket Container -->
    <div v-else class="flex-1 overflow-x-auto custom-scrollbar bg-white rounded-xl shadow-sm border border-slate-200 p-8">
      <div class="flex space-x-16 min-w-max h-full">
        <!-- Rounds -->
        <div v-for="round in tournamentStore.currentBracket.rounds" :key="round.roundNumber" 
             class="flex flex-col justify-around w-64 relative">
          
          <h3 class="text-center font-bold text-slate-400 uppercase text-sm mb-6 absolute -top-8 w-full">
            Vòng {{ round.roundNumber }}
          </h3>

          <!-- Matches -->
          <div v-for="match in round.matches" :key="match.matchId" 
               class="relative bg-white border border-slate-200 rounded-lg shadow-sm mb-8 last:mb-0">
            
            <!-- Connector Lines (CSS Magic) -->
            <div v-if="match.nextMatchId" class="connector-line"></div>

            <!-- Team 1 -->
            <div class="flex justify-between items-center p-3 border-b border-slate-100"
                 :class="{'bg-green-50': match.winningSide === 1}">
              <span class="font-medium text-slate-700 truncate mr-2">{{ match.team1Player1 }}</span>
              <span class="font-bold text-slate-800 bg-slate-100 px-2 py-0.5 rounded text-sm">
                {{ match.team1Score ?? '-' }}
              </span>
            </div>

            <!-- Team 2 -->
            <div class="flex justify-between items-center p-3"
                 :class="{'bg-green-50': match.winningSide === 2}">
              <span class="font-medium text-slate-700 truncate mr-2">{{ match.team2Player1 }}</span>
              <span class="font-bold text-slate-800 bg-slate-100 px-2 py-0.5 rounded text-sm">
                {{ match.team2Score ?? '-' }}
              </span>
            </div>

            <!-- Match Info -->
            <div class="absolute -right-3 top-1/2 transform -translate-y-1/2 translate-x-full pl-2">
               <span v-if="match.winningSide !== 0" class="text-xs font-bold text-green-600">Kết thúc</span>
               <span v-else class="text-xs font-medium text-slate-400">Chưa đấu</span>
            </div>
          </div>
        </div>
        
        <!-- Winner Box (Optional) -->
        <div class="flex flex-col justify-center w-48">
             <div class="border-2 border-dashed border-primary-200 rounded-lg p-4 text-center">
                <TrophyIcon class="w-8 h-8 text-yellow-500 mx-auto mb-2" />
                <span class="text-sm font-bold text-primary-700">Nhà Vô Địch</span>
             </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useTournamentStore } from '@/stores/tournament';
import { ArrowLeftIcon, TrophyIcon } from '@heroicons/vue/24/outline';

const route = useRoute();
const tournamentStore = useTournamentStore();

onMounted(() => {
  tournamentStore.fetchBracket(route.params.id);
});
</script>

<style scoped>
/* Connector Lines Logic */
.connector-line {
  position: absolute;
  right: -2rem; /* Half of space-x-16 (4rem) */
  top: 50%;
  width: 2rem;
  height: 1px;
  background-color: #cbd5e1; /* slate-300 */
  z-index: 0;
}

/* Custom Scrollbar */
.custom-scrollbar::-webkit-scrollbar {
  height: 8px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: #f1f5f9;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 4px;
}
</style>