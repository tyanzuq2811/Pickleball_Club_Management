<template>
  <div class="space-y-6">
    <div>
      <h2 class="text-2xl font-bold text-slate-800">Tổng quan - Trọng tài</h2>
      <p class="text-slate-600 mt-1">Quản lý trận đấu và điểm số</p>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Trận hôm nay</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.todayMatches }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <CalendarIcon class="w-6 h-6 text-blue-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Đang diễn ra</p>
            <p class="text-2xl font-bold text-green-600 mt-2">{{ stats.ongoingMatches }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <PlayIcon class="w-6 h-6 text-green-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Chờ xử lý</p>
            <p class="text-2xl font-bold text-orange-600 mt-2">{{ stats.pendingMatches }}</p>
          </div>
          <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
            <ClockIcon class="w-6 h-6 text-orange-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Đã hoàn thành</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.completedMatches }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <CheckCircleIcon class="w-6 h-6 text-purple-600" />
          </div>
        </div>
      </div>
    </div>

    <!-- Today's Matches -->
    <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-lg font-semibold text-slate-800">Trận đấu hôm nay</h3>
        <router-link to="/referee" class="text-sm text-sky-600 hover:underline">Xem tất cả</router-link>
      </div>

      <div v-if="todayMatches.length === 0" class="text-center py-8 text-gray-500">
        Không có trận đấu nào hôm nay
      </div>
      <div v-else class="space-y-3">
        <div v-for="match in todayMatches" :key="match.id" class="p-4 bg-slate-50 rounded-lg border border-slate-100 hover:border-sky-200 transition-colors">
          <div class="flex justify-between items-center">
            <div class="flex-1">
              <div class="flex items-center space-x-2 mb-2">
                <span class="px-2 py-1 text-xs font-semibold rounded-full"
                      :class="getStatusClass(match.status)">
                  {{ getStatusText(match.status) }}
                </span>
                <span class="text-xs text-slate-500">{{ formatDateTime(match.date) }}</span>
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <p class="text-xs text-slate-500 mb-1">Đội 1</p>
                  <p class="font-medium text-slate-800 text-sm">{{ match.team1Player1Name || 'TBD' }}</p>
                  <p v-if="match.team1Player2Name" class="text-slate-600 text-sm">{{ match.team1Player2Name }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500 mb-1">Đội 2</p>
                  <p class="font-medium text-slate-800 text-sm">{{ match.team2Player1Name || 'TBD' }}</p>
                  <p v-if="match.team2Player2Name" class="text-slate-600 text-sm">{{ match.team2Player2Name }}</p>
                </div>
              </div>
            </div>
            <div v-if="match.status === 0" class="ml-4">
              <button @click="() => $router.push('/referee')" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 text-sm font-medium">
                Bắt đầu
              </button>
            </div>
            <div v-else-if="match.status === 1" class="ml-4">
              <button @click="() => $router.push('/referee')" class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 text-sm font-medium">
                Cập nhật
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Thống kê tuần này</h3>
        <div class="space-y-3">
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Tổng trận đấu</span>
            <span class="font-semibold text-slate-800">{{ stats.weeklyTotal }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Trận ranked</span>
            <span class="font-semibold text-slate-800">{{ stats.weeklyRanked }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Trận giải đấu</span>
            <span class="font-semibold text-slate-800">{{ stats.weeklyTournament }}</span>
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Lưu ý</h3>
        <div class="space-y-2">
          <div v-if="stats.pendingMatches > 0" class="p-3 bg-orange-50 border border-orange-200 rounded-lg">
            <p class="text-sm text-orange-800">⚠️ {{ stats.pendingMatches }} trận đang chờ xử lý</p>
          </div>
          <div v-if="stats.ongoingMatches > 0" class="p-3 bg-green-50 border border-green-200 rounded-lg">
            <p class="text-sm text-green-800">▶️ {{ stats.ongoingMatches }} trận đang diễn ra</p>
          </div>
          <div v-if="stats.todayMatches === 0" class="p-3 bg-blue-50 border border-blue-200 rounded-lg">
            <p class="text-sm text-blue-800">ℹ️ Không có trận đấu nào hôm nay</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axiosClient from '@/api/axiosClient'
import { format, parseISO, isToday } from 'date-fns'
import { CalendarIcon, PlayIcon, ClockIcon, CheckCircleIcon } from '@heroicons/vue/24/outline'

const stats = ref({
  todayMatches: 0,
  ongoingMatches: 0,
  pendingMatches: 0,
  completedMatches: 0,
  weeklyTotal: 0,
  weeklyRanked: 0,
  weeklyTournament: 0
})

const todayMatches = ref([])

onMounted(async () => {
  await fetchMatches()
})

const fetchMatches = async () => {
  try {
    const response = await axiosClient.get('/matches?pageSize=100')
    if (response.data.success) {
      const allMatches = response.data.data.items || []
      
      todayMatches.value = allMatches.filter(m => {
        const matchDate = m.date || m.matchDate
        return matchDate && isToday(parseISO(matchDate))
      })

      stats.value.todayMatches = todayMatches.value.length
      stats.value.ongoingMatches = allMatches.filter(m => m.status === 1).length
      stats.value.pendingMatches = allMatches.filter(m => m.status === 0).length
      stats.value.completedMatches = allMatches.filter(m => m.status === 2).length
      stats.value.weeklyTotal = allMatches.length
      stats.value.weeklyRanked = allMatches.filter(m => m.isRanked).length
      stats.value.weeklyTournament = allMatches.filter(m => m.tournamentId).length
    }
  } catch (error) {
    console.error('Error fetching matches:', error)
  }
}

const getStatusText = (status) => {
  const texts = { 0: 'Chờ', 1: 'Đang chơi', 2: 'Hoàn thành' }
  return texts[status] || 'Unknown'
}

const getStatusClass = (status) => {
  if (status === 0) return 'bg-orange-100 text-orange-700'
  if (status === 1) return 'bg-green-100 text-green-700'
  if (status === 2) return 'bg-slate-100 text-slate-700'
  return 'bg-gray-100 text-gray-700'
}

const formatDateTime = (dateStr) => {
  if (!dateStr) return 'N/A'
  return format(parseISO(dateStr), 'dd/MM/yyyy HH:mm')
}
</script>
