<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-4xl md:text-5xl font-bold text-slate-800 mb-2 flex items-center justify-center gap-3">
          <svg class="w-10 h-10 text-red-600 animate-pulse" fill="currentColor" viewBox="0 0 20 20">
            <circle cx="10" cy="10" r="3"/>
          </svg>
          BẢNG ĐIỂM TRỰC TIẾP
        </h1>
        <p class="text-slate-600">Cập nhật theo thời gian thực</p>
      </div>

      <!-- Live Indicator -->
      <div class="flex justify-center mb-6">
        <div class="bg-white rounded-full px-6 py-2 shadow-lg flex items-center gap-2">
          <div class="w-3 h-3 bg-red-600 rounded-full animate-ping absolute"></div>
          <div class="w-3 h-3 bg-red-600 rounded-full"></div>
          <span class="text-sm font-bold text-slate-700 ml-2">ĐANG LIVE</span>
          <span class="text-xs text-slate-500 ml-2">{{ liveMatchesCount }} trận đang diễn ra</span>
        </div>
      </div>

      <!-- Matches Grid -->
      <div v-if="loading" class="text-center py-20">
        <div class="inline-block animate-spin rounded-full h-16 w-16 border-4 border-blue-500 border-t-transparent"></div>
        <p class="mt-4 text-slate-600">Đang tải...</p>
      </div>

      <div v-else-if="liveMatches.length === 0" class="text-center py-20">
        <svg class="w-24 h-24 mx-auto text-slate-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
        </svg>
        <p class="text-xl text-slate-500">Hiện không có trận đấu nào đang diễn ra</p>
        <p class="text-sm text-slate-400 mt-2">Vui lòng quay lại sau</p>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div v-for="match in liveMatches" :key="match.id" 
             class="bg-white rounded-2xl shadow-xl overflow-hidden transform transition-all hover:scale-105 border-4"
             :class="getBorderColor(match)">
          <!-- Match Header -->
          <div class="bg-gradient-to-r from-blue-600 to-indigo-600 p-4 text-white">
            <div class="flex justify-between items-center">
              <div class="flex items-center gap-2">
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"/>
                </svg>
                <span class="font-bold text-sm">{{ match.courtName || 'Sân ' + match.courtId }}</span>
              </div>
              <span class="text-xs bg-white/20 px-3 py-1 rounded-full">{{ match.format }}</span>
            </div>
            <div class="text-xs text-blue-100 mt-1">{{ formatMatchTime(match.startTime) }}</div>
          </div>

          <!-- Scoreboard -->
          <div class="p-6">
            <!-- Team/Player 1 -->
            <div class="flex items-center justify-between mb-4 p-4 rounded-xl transition-all"
                 :class="match.winningSide === 1 ? 'bg-green-50 border-2 border-green-400' : 'bg-slate-50'">
              <div class="flex-1">
                <div class="font-bold text-lg text-slate-800 flex items-center gap-2">
                  <span v-if="match.winningSide === 1" class="text-green-600">👑</span>
                  {{ match.player1Name || 'Đội A' }}
                </div>
                <div v-if="match.player2Name" class="text-sm text-slate-500">{{ match.player2Name }}</div>
              </div>
              <div class="text-4xl font-bold" 
                   :class="match.winningSide === 1 ? 'text-green-600' : 'text-slate-700'">
                {{ match.score1 || 0 }}
              </div>
            </div>

            <!-- VS Divider -->
            <div class="text-center text-xs font-bold text-slate-400 my-2">VS</div>

            <!-- Team/Player 2 -->
            <div class="flex items-center justify-between mb-4 p-4 rounded-xl transition-all"
                 :class="match.winningSide === 2 ? 'bg-green-50 border-2 border-green-400' : 'bg-slate-50'">
              <div class="flex-1">
                <div class="font-bold text-lg text-slate-800 flex items-center gap-2">
                  <span v-if="match.winningSide === 2" class="text-green-600">👑</span>
                  {{ match.player3Name || 'Đội B' }}
                </div>
                <div v-if="match.player4Name" class="text-sm text-slate-500">{{ match.player4Name }}</div>
              </div>
              <div class="text-4xl font-bold" 
                   :class="match.winningSide === 2 ? 'text-green-600' : 'text-slate-700'">
                {{ match.score2 || 0 }}
              </div>
            </div>

            <!-- Match Status -->
            <div class="mt-4 pt-4 border-t border-slate-200 flex justify-between items-center">
              <div class="flex items-center gap-2">
                <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                <span class="text-sm font-semibold text-green-600">Đang thi đấu</span>
              </div>
              <button @click="viewMatchDetail(match)" 
                      class="text-sm text-blue-600 hover:text-blue-700 font-medium flex items-center gap-1">
                Chi tiết
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Tournament Matches Section -->
      <div v-if="tournamentMatches.length > 0" class="mt-12">
        <h2 class="text-2xl font-bold text-slate-800 mb-6 flex items-center gap-2">
          <svg class="w-7 h-7 text-yellow-500" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
          </svg>
          Giải đấu đang diễn ra
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div v-for="match in tournamentMatches" :key="match.id"
               class="bg-gradient-to-br from-yellow-50 to-orange-50 rounded-xl shadow-lg p-6 border-2 border-yellow-300">
            <div class="flex items-center justify-between mb-4">
              <h3 class="font-bold text-lg text-yellow-900">{{ match.tournamentName }}</h3>
              <span class="px-3 py-1 bg-yellow-200 text-yellow-800 text-xs font-bold rounded-full">
                {{ match.roundName || 'Vòng ' + match.round }}
              </span>
            </div>
            
            <div class="bg-white rounded-lg p-4 space-y-3">
              <div class="flex justify-between items-center">
                <span class="font-semibold">{{ match.team1Name }}</span>
                <span class="text-2xl font-bold text-blue-600">{{ match.currentScore_TeamA || 0 }}</span>
              </div>
              <div class="text-center text-slate-400 text-sm">VS</div>
              <div class="flex justify-between items-center">
                <span class="font-semibold">{{ match.team2Name }}</span>
                <span class="text-2xl font-bold text-red-600">{{ match.currentScore_TeamB || 0 }}</span>
              </div>
            </div>

            <div class="mt-4 text-sm text-slate-600">
              🏆 First to {{ match.targetWins || 3 }} wins
            </div>
          </div>
        </div>
      </div>

      <!-- Auto Refresh Info -->
      <div class="mt-8 text-center text-sm text-slate-500">
        <p>Tự động cập nhật mỗi {{ refreshInterval / 1000 }} giây</p>
        <p class="text-xs mt-1">Cập nhật lần cuối: {{ lastUpdate }}</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { format } from 'date-fns';
import axiosClient from '@/api/axiosClient';
import * as signalR from '@microsoft/signalr';

const liveMatches = ref([]);
const tournamentMatches = ref([]);
const loading = ref(true);
const lastUpdate = ref('');
const refreshInterval = 5000; // 5 seconds
let intervalId = null;
let connection = null;

const liveMatchesCount = computed(() => liveMatches.value.length);

onMounted(async () => {
  await fetchLiveMatches();
  startAutoRefresh();
  connectSignalR();
});

onUnmounted(() => {
  if (intervalId) clearInterval(intervalId);
  if (connection) connection.stop();
});

const fetchLiveMatches = async () => {
  try {
    loading.value = true;
    
    // Fetch regular matches
    const matchesResponse = await axiosClient.get('/matches', {
      params: { status: 'InProgress' }
    });
    
    if (matchesResponse.data.success) {
      liveMatches.value = matchesResponse.data.data || [];
    }

    // Fetch tournament matches
    const tournamentResponse = await axiosClient.get('/tournaments/live-matches');
    if (tournamentResponse.data.success) {
      tournamentMatches.value = tournamentResponse.data.data || [];
    }

    lastUpdate.value = format(new Date(), 'HH:mm:ss');
  } catch (error) {
    console.error('Error fetching live matches:', error);
  } finally {
    loading.value = false;
  }
};

const startAutoRefresh = () => {
  intervalId = setInterval(() => {
    fetchLiveMatches();
  }, refreshInterval);
};

const connectSignalR = async () => {
  try {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_BASE_URL.replace('/api', '') + '/scorehub')
      .withAutomaticReconnect()
      .build();

    connection.on('ScoreUpdated', (matchId, score1, score2, winningSide) => {
      const match = liveMatches.value.find(m => m.id === matchId);
      if (match) {
        match.score1 = score1;
        match.score2 = score2;
        match.winningSide = winningSide;
      }
    });

    connection.on('TournamentScoreUpdated', (matchId, scoreA, scoreB) => {
      const match = tournamentMatches.value.find(m => m.id === matchId);
      if (match) {
        match.currentScore_TeamA = scoreA;
        match.currentScore_TeamB = scoreB;
      }
    });

    await connection.start();
    console.log('SignalR connected');
  } catch (error) {
    console.error('SignalR connection error:', error);
  }
};

const getBorderColor = (match) => {
  if (match.winningSide === 1) return 'border-green-400';
  if (match.winningSide === 2) return 'border-blue-400';
  return 'border-slate-200';
};

const formatMatchTime = (time) => {
  if (!time) return '';
  return format(new Date(time), 'HH:mm - dd/MM/yyyy');
};

const viewMatchDetail = (match) => {
  // Could navigate to detail page or show modal
  console.log('View match:', match.id);
};
</script>
