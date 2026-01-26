<template>
  <div class="space-y-6">
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <p class="text-gray-500">Đang tải dữ liệu...</p>
    </div>

    <template v-else>
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6">
          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
              <div class="p-3 bg-blue-100 rounded-lg text-blue-600">
                  <UsersIcon class="w-8 h-8" />
              </div>
              <div>
                  <p class="text-sm text-slate-500 font-medium">Tổng hội viên</p>
                  <p class="text-2xl font-bold text-slate-800">{{ stats.totalMembers }}</p>
              </div>
          </div>

          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
              <div class="p-3 bg-green-100 rounded-lg text-green-600">
                  <CurrencyDollarIcon class="w-8 h-8" />
              </div>
              <div>
                  <p class="text-sm text-slate-500 font-medium">Quỹ CLB</p>
                  <p class="text-2xl font-bold text-slate-800">{{ formatCurrency(stats.clubFund) }}</p>
              </div>
          </div>

          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
              <div class="p-3 bg-purple-100 rounded-lg text-purple-600">
                  <TrophyIcon class="w-8 h-8" />
              </div>
              <div>
                  <p class="text-sm text-slate-500 font-medium">Giải đấu đang chạy</p>
                  <p class="text-2xl font-bold text-slate-800">{{ stats.ongoingTournaments }}</p>
              </div>
          </div>

          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
              <div class="p-3 bg-orange-100 rounded-lg text-orange-600">
                  <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
              </div>
              <div>
                  <p class="text-sm text-slate-500 font-medium">Trận hôm nay</p>
                  <p class="text-2xl font-bold text-slate-800">{{ todayMatches.length }}</p>
              </div>
          </div>

          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
              <div class="p-3 bg-sky-100 rounded-lg text-sky-600">
                  <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                  </svg>
              </div>
              <div>
                  <p class="text-sm text-slate-500 font-medium">Booking sắp tới</p>
                  <p class="text-2xl font-bold text-slate-800">{{ upcomingBookings.length }}</p>
              </div>
          </div>
      </div>

      <!-- New Widgets Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
              <h3 class="text-lg font-semibold text-slate-800 mb-4 flex items-center">
                <svg class="w-5 h-5 mr-2 text-orange-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                Trận đấu hôm nay
              </h3>
              
              <div v-if="todayMatches.length === 0" class="text-center py-8 text-gray-500">
                Không có trận đấu nào hôm nay
              </div>
              
              <div v-else class="space-y-3">
                  <div v-for="match in todayMatches" :key="match.id" class="p-3 bg-slate-50 rounded-lg border border-slate-100">
                      <div class="flex justify-between items-center">
                        <div class="flex-1">
                          <p class="font-medium text-slate-800 text-sm">{{ match.team1Name }} vs {{ match.team2Name }}</p>
                          <p class="text-xs text-slate-500 mt-1">Sân: {{ match.courtName }} • {{ formatTime(match.matchDate) }}</p>
                        </div>
                        <div class="text-right">
                          <span class="px-2 py-1 text-xs font-semibold rounded-full" :class="getMatchStatusClass(match.status)">
                            {{ getMatchStatusText(match.status) }}
                          </span>
                        </div>
                      </div>
                  </div>
              </div>
          </div>

          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
              <h3 class="text-lg font-semibold text-slate-800 mb-4 flex items-center">
                <svg class="w-5 h-5 mr-2 text-sky-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                </svg>
                Booking sắp tới của bạn
              </h3>
              
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
      </div>

      <!-- Main Content Area -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
              <h3 class="text-lg font-semibold text-slate-800 mb-4">Tin tức mới nhất</h3>
              
              <div v-if="pinnedNews.length === 0 && latestNews.length === 0" class="text-center py-8 text-gray-500">
                Chưa có tin tức
              </div>
              
              <div v-else class="space-y-4">
                  <div v-for="news in pinnedNews" :key="'pinned-' + news.id" class="p-4 bg-yellow-50 rounded-lg border border-yellow-200">
                      <span class="text-xs font-bold text-yellow-600 bg-yellow-100 px-2 py-1 rounded">Ghim</span>
                      <h4 class="font-medium text-slate-800 mt-1">{{ news.title }}</h4>
                      <p class="text-sm text-slate-500 mt-1">{{ news.summary || truncate(news.content, 80) }}</p>
                  </div>
                  <div v-for="news in latestNews.slice(0, 2)" :key="news.id" class="p-4 bg-slate-50 rounded-lg border border-slate-100">
                      <h4 class="font-medium text-slate-800">{{ news.title }}</h4>
                      <p class="text-sm text-slate-500 mt-1">{{ news.summary || truncate(news.content, 80) }}</p>
                  </div>
              </div>
          </div>

          <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
              <h3 class="text-lg font-semibold text-slate-800 mb-4">Top Ranking (ELO)</h3>
              
              <div v-if="topRanking.length === 0" class="text-center py-8 text-gray-500">
                Chưa có dữ liệu xếp hạng
              </div>
              
              <div v-else class="overflow-x-auto">
                  <table class="w-full text-sm text-left">
                      <thead class="text-xs text-slate-500 uppercase bg-slate-50">
                          <tr>
                              <th class="px-4 py-3">#</th>
                              <th class="px-4 py-3">Tên</th>
                              <th class="px-4 py-3 text-right">ELO</th>
                          </tr>
                      </thead>
                      <tbody>
                          <tr v-for="(member, index) in topRanking" :key="member.id" class="border-b border-slate-100 hover:bg-slate-50">
                              <td class="px-4 py-3 font-medium text-sky-600">{{ index + 1 }}</td>
                              <td class="px-4 py-3">{{ member.fullName }}</td>
                              <td class="px-4 py-3 text-right font-bold">{{ member.rankELO?.toFixed(0) }}</td>
                          </tr>
                      </tbody>
                  </table>
              </div>
          </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { UsersIcon, CurrencyDollarIcon, TrophyIcon } from '@heroicons/vue/24/outline'
import axiosClient from '@/api/axiosClient'
import { format, isToday, isFuture, parseISO } from 'date-fns'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const loading = ref(true)
const stats = ref({
  totalMembers: 0,
  clubFund: 0,
  ongoingTournaments: 0
})
const pinnedNews = ref([])
const latestNews = ref([])
const topRanking = ref([])
const allMatches = ref([])
const allBookings = ref([])

const todayMatches = computed(() => {
  return allMatches.value.filter(match => {
    if (!match.matchDate && !match.date) return false
    const matchDate = parseISO(match.matchDate || match.date)
    return isToday(matchDate)
  })
})

const upcomingBookings = computed(() => {
  return allBookings.value.filter(booking => {
    const startTime = booking.startTime || booking.StartTime
    if (!startTime) return false
    const parsedTime = parseISO(startTime)
    return isFuture(parsedTime) && booking.status === 0 // Confirmed bookings
  }).sort((a, b) => {
    const timeA = a.startTime || a.StartTime
    const timeB = b.startTime || b.StartTime
    return new Date(timeA) - new Date(timeB)
  })
})

onMounted(async () => {
  await Promise.all([
    fetchStats(),
    fetchPinnedNews(),
    fetchLatestNews(),
    fetchTopRanking(),
    fetchMatches(),
    fetchBookings()
  ])
  loading.value = false
})

const fetchMatches = async () => {
  try {
    const response = await axiosClient.get('/matches?pageSize=100')
    if (response.data.success) {
      allMatches.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('Error fetching matches:', error)
  }
}

const fetchBookings = async () => {
  try {
    const response = await axiosClient.get('/bookings/my-bookings')
    if (response.data.success) {
      allBookings.value = response.data.data || []
    }
  } catch (error) {
    console.error('Error fetching bookings:', error)
  }
}

const fetchStats = async () => {
  try {
    // Fetch members count
    const membersRes = await axiosClient.get('/members/count')
    if (membersRes.data.success) {
      stats.value.totalMembers = membersRes.data.data || 0
    }

    // Fetch club fund
    const transactionsRes = await axiosClient.get('/transactions/summary')
    if (transactionsRes.data.success) {
      stats.value.clubFund = transactionsRes.data.data.balance || 0
    }

    // Fetch tournaments
    const tournamentsRes = await axiosClient.get('/tournaments?pageSize=100')
    if (tournamentsRes.data.success) {
      const ongoing = tournamentsRes.data.data.items?.filter(t => t.status === 'Ongoing' || t.status === 1) || []
      stats.value.ongoingTournaments = ongoing.length
    }
  } catch (error) {
    console.error('Error fetching stats:', error)
  }
}

const fetchPinnedNews = async () => {
  try {
    const response = await axiosClient.get('/news/pinned')
    if (response.data.success) {
      pinnedNews.value = response.data.data || []
    }
  } catch (error) {
    console.error('Error fetching pinned news:', error)
  }
}

const fetchLatestNews = async () => {
  try {
    const response = await axiosClient.get('/news?pageSize=3')
    if (response.data.success) {
      latestNews.value = response.data.data.items || []
    }
  } catch (error) {
    console.error('Error fetching latest news:', error)
  }
}

const fetchTopRanking = async () => {
  try {
    const response = await axiosClient.get('/members/top-ranking?limit=5')
    if (response.data.success) {
      topRanking.value = response.data.data || []
    }
  } catch (error) {
    console.error('Error fetching top ranking:', error)
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

const formatTime = (dateStr) => {
  if (!dateStr) return 'N/A'
  return format(parseISO(dateStr), 'HH:mm')
}

const formatTimeRange = (start, end) => {
  if (!start || !end) return 'N/A'
  return `${formatTime(start)} - ${formatTime(end)}`
}

const getMatchStatusText = (status) => {
  const texts = { 0: 'Chờ', 1: 'Đang chơi', 2: 'Hoàn thành' }
  return texts[status] || 'Unknown'
}

const getMatchStatusClass = (status) => {
  const classes = {
    0: 'bg-yellow-100 text-yellow-800',
    1: 'bg-blue-100 text-blue-800',
    2: 'bg-green-100 text-green-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const truncate = (text, length) => {
  if (!text) return ''
  return text.length > length ? text.substring(0, length) + '...' : text
}
</script>