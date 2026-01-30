<template>
  <div class="bg-gradient-to-br from-purple-50 to-pink-50 rounded-xl shadow-lg p-6 border-2 border-purple-300">
    <div class="text-center mb-6">
      <h3 class="text-2xl font-bold text-purple-900 mb-1">{{ tournament.name }}</h3>
      <p class="text-sm text-purple-600">{{ tournament.type }} - First to {{ tournament.targetWins }} wins</p>
    </div>

    <!-- Team Battle Score -->
    <div class="grid grid-cols-3 gap-4 items-center mb-6">
      <!-- Team A -->
      <div class="text-center">
        <div class="bg-white rounded-lg p-6 shadow-md border-2"
             :class="currentScore.teamA >= tournament.targetWins ? 'border-green-400' : 'border-blue-400'">
          <div class="text-sm font-medium text-slate-600 mb-2">{{ tournament.team1Name || 'Team A' }}</div>
          <div class="text-5xl font-bold mb-2"
               :class="currentScore.teamA >= tournament.targetWins ? 'text-green-600' : 'text-blue-600'">
            {{ currentScore.teamA }}
          </div>
          <div v-if="currentScore.teamA >= tournament.targetWins" class="text-green-600 font-bold flex items-center justify-center gap-1">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
            </svg>
            CHAMPION
          </div>
        </div>
      </div>

      <!-- VS -->
      <div class="text-center">
        <div class="text-3xl font-bold text-slate-400 mb-4">VS</div>
        <div class="text-sm text-slate-500">{{ matchesPlayed }} trận đã đấu</div>
      </div>

      <!-- Team B -->
      <div class="text-center">
        <div class="bg-white rounded-lg p-6 shadow-md border-2"
             :class="currentScore.teamB >= tournament.targetWins ? 'border-green-400' : 'border-red-400'">
          <div class="text-sm font-medium text-slate-600 mb-2">{{ tournament.team2Name || 'Team B' }}</div>
          <div class="text-5xl font-bold mb-2"
               :class="currentScore.teamB >= tournament.targetWins ? 'text-green-600' : 'text-red-600'">
            {{ currentScore.teamB }}
          </div>
          <div v-if="currentScore.teamB >= tournament.targetWins" class="text-green-600 font-bold flex items-center justify-center gap-1">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
            </svg>
            CHAMPION
          </div>
        </div>
      </div>
    </div>

    <!-- Progress Bar -->
    <div class="mb-6">
      <div class="flex justify-between text-xs text-slate-600 mb-2">
        <span>Team A Progress</span>
        <span>{{ progressPercentage.teamA }}%</span>
        <span>Team B Progress</span>
      </div>
      <div class="relative h-8 bg-slate-200 rounded-full overflow-hidden">
        <div class="absolute left-0 h-full bg-gradient-to-r from-blue-500 to-blue-600 transition-all duration-500"
             :style="{ width: progressPercentage.teamA + '%' }"></div>
        <div class="absolute right-0 h-full bg-gradient-to-l from-red-500 to-red-600 transition-all duration-500"
             :style="{ width: progressPercentage.teamB + '%' }"></div>
        <div class="absolute inset-0 flex items-center justify-center text-white font-bold text-sm">
          {{ currentScore.teamA }} - {{ currentScore.teamB }}
        </div>
      </div>
    </div>

    <!-- Match History -->
    <div v-if="recentMatches.length > 0" class="bg-white rounded-lg p-4">
      <h4 class="font-bold text-slate-800 mb-3 flex items-center gap-2">
        <svg class="w-5 h-5 text-purple-600" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"/>
        </svg>
        Lịch sử đối đầu
      </h4>
      <div class="space-y-2">
        <div v-for="match in recentMatches" :key="match.id"
             class="flex items-center justify-between p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
          <div class="flex items-center gap-3">
            <span class="text-xs font-medium text-slate-500">{{ formatMatchDate(match.date) }}</span>
            <span class="font-medium text-slate-800">{{ match.player1Name }} vs {{ match.player3Name }}</span>
          </div>
          <div class="flex items-center gap-2">
            <span class="text-sm font-bold"
                  :class="match.winningSide === 1 ? 'text-blue-600' : 'text-red-600'">
              {{ match.score1 }} - {{ match.score2 }}
            </span>
            <span class="px-2 py-1 text-xs font-bold rounded-full"
                  :class="match.winningSide === 1 ? 'bg-blue-100 text-blue-800' : 'bg-red-100 text-red-800'">
              {{ match.winningSide === 1 ? 'Team A' : 'Team B' }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Next Match Info -->
    <div v-if="nextMatch" class="mt-4 bg-gradient-to-r from-yellow-50 to-orange-50 rounded-lg p-4 border-2 border-yellow-300">
      <div class="flex items-center gap-2 mb-2">
        <svg class="w-5 h-5 text-yellow-600" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"/>
        </svg>
        <span class="font-bold text-yellow-900">Trận tiếp theo</span>
      </div>
      <div class="text-sm text-yellow-800">
        <div><strong>Đấu thủ:</strong> {{ nextMatch.player1Name }} vs {{ nextMatch.player3Name }}</div>
        <div><strong>Thời gian:</strong> {{ formatMatchDate(nextMatch.date) }}</div>
      </div>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-3 gap-3 mt-6">
      <div class="text-center p-3 bg-white rounded-lg shadow-sm">
        <div class="text-2xl font-bold text-purple-600">{{ stats.totalMatches }}</div>
        <div class="text-xs text-slate-600">Tổng trận</div>
      </div>
      <div class="text-center p-3 bg-white rounded-lg shadow-sm">
        <div class="text-2xl font-bold text-blue-600">{{ stats.teamAWins }}</div>
        <div class="text-xs text-slate-600">Team A thắng</div>
      </div>
      <div class="text-center p-3 bg-white rounded-lg shadow-sm">
        <div class="text-2xl font-bold text-red-600">{{ stats.teamBWins }}</div>
        <div class="text-xs text-slate-600">Team B thắng</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { format } from 'date-fns';
import * as signalR from '@microsoft/signalr';

const props = defineProps({
  tournament: {
    type: Object,
    required: true
  },
  matches: {
    type: Array,
    default: () => []
  }
});

const currentScore = ref({
  teamA: props.tournament.currentScore_TeamA || 0,
  teamB: props.tournament.currentScore_TeamB || 0
});

let connection = null;

onMounted(() => {
  connectSignalR();
});

onUnmounted(() => {
  if (connection) connection.stop();
});

const connectSignalR = async () => {
  try {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_BASE_URL.replace('/api', '') + '/scorehub')
      .withAutomaticReconnect()
      .build();

    connection.on('TournamentScoreUpdated', (tournamentId, scoreA, scoreB) => {
      if (tournamentId === props.tournament.id) {
        currentScore.value = { teamA: scoreA, teamB: scoreB };
      }
    });

    await connection.start();
  } catch (error) {
    console.error('SignalR connection error:', error);
  }
};

const matchesPlayed = computed(() => {
  return props.matches.filter(m => m.winningSide > 0).length;
});

const recentMatches = computed(() => {
  return props.matches
    .filter(m => m.winningSide > 0)
    .sort((a, b) => new Date(b.date) - new Date(a.date))
    .slice(0, 5);
});

const nextMatch = computed(() => {
  return props.matches.find(m => m.winningSide === 0);
});

const progressPercentage = computed(() => {
  const target = props.tournament.targetWins || 3;
  return {
    teamA: Math.min((currentScore.value.teamA / target) * 100, 100),
    teamB: Math.min((currentScore.value.teamB / target) * 100, 100)
  };
});

const stats = computed(() => {
  return {
    totalMatches: matchesPlayed.value,
    teamAWins: currentScore.value.teamA,
    teamBWins: currentScore.value.teamB
  };
});

const formatMatchDate = (date) => {
  return format(new Date(date), 'dd/MM/yyyy HH:mm');
};
</script>
