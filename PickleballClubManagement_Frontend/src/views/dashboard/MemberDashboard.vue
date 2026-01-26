<template>
  <div class="space-y-6">
    <div>
      <h2 class="text-2xl font-bold text-slate-800">Xin chào, {{ memberName }}!</h2>
      <p class="text-slate-600 mt-1">Chào mừng bạn đến với Câu lạc bộ Pickleball</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Số dư ví</p>
            <p class="text-2xl font-bold text-green-600 mt-2">{{ formatCurrency(stats.walletBalance) }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <CreditCardIcon class="w-6 h-6 text-green-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">ELO Rating</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.rankELO }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <TrophyIcon class="w-6 h-6 text-purple-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Tổng trận</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.totalMatches }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <ChartBarIcon class="w-6 h-6 text-blue-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Tỷ lệ thắng</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ winRate }}%</p>
          </div>
          <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
            <FireIcon class="w-6 h-6 text-orange-600" />
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Upcoming Bookings -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold text-slate-800">Booking sắp tới</h3>
          <router-link to="/bookings" class="text-sm text-sky-600 hover:underline">Đặt sân mới</router-link>
        </div>

        <div v-if="upcomingBookings.length === 0" class="text-center py-8 text-gray-500">
          Bạn chưa có booking nào
        </div>
        <div v-else class="space-y-3">
          <div v-for="booking in upcomingBookings.slice(0, 5)" :key="booking.id" class="p-3 bg-slate-50 rounded-lg border border-slate-100">
            <div class="flex justify-between items-center">
              <div class="flex-1">
                <p class="font-medium text-slate-800 text-sm">Sân {{ booking.courtName || booking.CourtName }}</p>
                <p class="text-xs text-slate-500 mt-1">{{ formatDate(booking.startTime || booking.StartTime) }} • {{ formatTimeRange(booking.startTime || booking.StartTime, booking.endTime || booking.EndTime) }}</p>
              </div>
              <div class="text-right">
                <span class="px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">
                  Đã xác nhận
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- My Tournaments -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold text-slate-800">Giải đấu của tôi</h3>
          <router-link to="/tournaments" class="text-sm text-sky-600 hover:underline">Xem tất cả</router-link>
        </div>

        <div v-if="myTournaments.length === 0" class="text-center py-8 text-gray-500">
          Bạn chưa tham gia giải đấu nào
        </div>
        <div v-else class="space-y-3">
          <div v-for="tournament in myTournaments.slice(0, 5)" :key="tournament.id" class="p-3 bg-slate-50 rounded-lg border border-slate-100 hover:border-sky-200 transition-colors cursor-pointer">
            <div class="flex justify-between items-center">
              <div class="flex-1">
                <p class="font-medium text-slate-800 text-sm">{{ tournament.title }}</p>
                <p class="text-xs text-slate-500 mt-1">{{ formatDate(tournament.startDate) }}</p>
              </div>
              <span class="px-2 py-1 text-xs font-semibold rounded-full"
                    :class="getTournamentStatusClass(tournament.status)">
                {{ getTournamentStatusText(tournament.status) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Latest News -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Tin tức câu lạc bộ</h3>
        <div v-if="latestNews.length === 0" class="text-center py-8 text-gray-500">
          Chưa có tin tức nào
        </div>
        <div v-else class="space-y-3">
          <div v-for="news in latestNews.slice(0, 5)" :key="news.id" class="p-3 bg-slate-50 rounded-lg border border-slate-100 hover:border-sky-200 transition-colors cursor-pointer">
            <h4 class="font-medium text-slate-800 text-sm">{{ news.title }}</h4>
            <p class="text-xs text-slate-500 mt-1 line-clamp-2">{{ news.summary }}</p>
            <p class="text-xs text-slate-400 mt-2">{{ formatDate(news.createdDate) }}</p>
          </div>
        </div>
      </div>

      <!-- Top Ranking -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Bảng xếp hạng Top 5</h3>
        <div v-if="topRanking.length === 0" class="text-center py-8 text-gray-500">
          Chưa có dữ liệu xếp hạng
        </div>
        <div v-else class="space-y-3">
          <div v-for="(member, index) in topRanking" :key="member.id" class="flex items-center justify-between p-3 bg-slate-50 rounded-lg border border-slate-100">
            <div class="flex items-center space-x-3">
              <div class="w-8 h-8 rounded-full flex items-center justify-center font-bold text-sm"
                   :class="index === 0 ? 'bg-yellow-100 text-yellow-700' : index === 1 ? 'bg-slate-200 text-slate-700' : index === 2 ? 'bg-orange-100 text-orange-700' : 'bg-slate-100 text-slate-600'">
                {{ index + 1 }}
              </div>
              <div>
                <p class="font-medium text-slate-800 text-sm">{{ member.fullName }}</p>
                <p class="text-xs text-slate-500">{{ member.totalMatches }} trận</p>
              </div>
            </div>
            <div class="text-right">
              <p class="font-bold text-slate-800">{{ member.rankELO }}</p>
              <p class="text-xs text-slate-500">ELO</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import axiosClient from '@/api/axiosClient'
import { format, parseISO, isFuture } from 'date-fns'
import { CreditCardIcon, TrophyIcon, ChartBarIcon, FireIcon } from '@heroicons/vue/24/outline'

const authStore = useAuthStore()
const memberName = computed(() => authStore.memberName || 'Hội viên')

const stats = ref({
  walletBalance: 0,
  rankELO: 1000,
  totalMatches: 0,
  winMatches: 0
})

const upcomingBookings = ref([])
const myTournaments = ref([])
const latestNews = ref([])
const topRanking = ref([])

const winRate = computed(() => {
  if (stats.value.totalMatches === 0) return 0
  return Math.round((stats.value.winMatches / stats.value.totalMatches) * 100)
})

onMounted(async () => {
  await Promise.all([
    fetchMemberStats(),
    fetchBookings(),
    fetchTournaments(),
    fetchNews(),
    fetchTopRanking()
  ])
})

const fetchMemberStats = async () => {
  try {
    // Get member info (includes wallet balance)
    const memberRes = await axiosClient.get('/members/me')
    if (memberRes.data.success) {
      const member = memberRes.data.data
      stats.value.walletBalance = member.walletBalance || 0
      stats.value.rankELO = member.rankELO || 1000
      stats.value.totalMatches = member.totalMatches || 0
      stats.value.winMatches = member.winMatches || 0
    }
  } catch (error) {
    console.error('Error fetching member stats:', error)
  }
}

const fetchBookings = async () => {
  try {
    const response = await axiosClient.get('/bookings/my-bookings')
    if (response.data.success) {
      const allBookings = response.data.data || []
      upcomingBookings.value = allBookings.filter(booking => {
        const startTime = booking.startTime || booking.StartTime
        return startTime && isFuture(parseISO(startTime)) && booking.status === 0
      }).sort((a, b) => {
        const timeA = a.startTime || a.StartTime
        const timeB = b.startTime || b.StartTime
        return new Date(timeA) - new Date(timeB)
      })
    }
  } catch (error) {
    console.error('Error fetching bookings:', error)
  }
}

const fetchTournaments = async () => {
  try {
    const response = await axiosClient.get('/tournaments?pageSize=10')
    if (response.data.success) {
      myTournaments.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('Error fetching tournaments:', error)
  }
}

const fetchNews = async () => {
  try {
    const response = await axiosClient.get('/news?pageSize=5')
    if (response.data.success) {
      latestNews.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('Error fetching news:', error)
  }
}

const fetchTopRanking = async () => {
  try {
    const response = await axiosClient.get('/members/top-ranking?limit=5')
    if (response.data.success) {
      topRanking.value = response.data.data || []
    }
  } catch (error) {
    console.error('Error fetching ranking:', error)
  }
}

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount)
}

const formatDate = (dateStr) => {
  if (!dateStr) return 'N/A'
  return format(parseISO(dateStr), 'dd/MM/yyyy')
}

const formatTimeRange = (start, end) => {
  if (!start || !end) return 'N/A'
  return `${format(parseISO(start), 'HH:mm')} - ${format(parseISO(end), 'HH:mm')}`
}

const getTournamentStatusText = (status) => {
  const texts = { 0: 'Chưa bắt đầu', 1: 'Đang diễn ra', 2: 'Đã kết thúc' }
  return texts[status] || 'Unknown'
}

const getTournamentStatusClass = (status) => {
  if (status === 0) return 'bg-blue-100 text-blue-700'
  if (status === 1) return 'bg-green-100 text-green-700'
  if (status === 2) return 'bg-slate-100 text-slate-700'
  return 'bg-gray-100 text-gray-700'
}
</script>
