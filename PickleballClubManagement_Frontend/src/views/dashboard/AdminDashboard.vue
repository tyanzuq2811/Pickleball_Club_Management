<template>
  <div class="space-y-6">
    <div>
      <h2 class="text-2xl font-bold text-slate-800">Tổng quan - Quản trị viên</h2>
      <p class="text-slate-600 mt-1">Tổng quan hệ thống quản lý CLB Pickleball</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Tổng hội viên</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.totalMembers }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <UsersIcon class="w-6 h-6 text-blue-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Quỹ CLB</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ formatCurrency(stats.clubFund) }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <BanknotesIcon class="w-6 h-6 text-green-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Giải đấu đang diễn ra</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.ongoingTournaments }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <TrophyIcon class="w-6 h-6 text-purple-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Số sân</p>
            <p class="text-2xl font-bold text-slate-800 mt-2">{{ stats.totalCourts }}</p>
          </div>
          <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
            <BuildingOfficeIcon class="w-6 h-6 text-orange-600" />
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Latest News -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Tin tức mới nhất</h3>
        <div v-if="latestNews.length === 0" class="text-center py-8 text-gray-500">
          Chưa có tin tức nào
        </div>
        <div v-else class="space-y-3">
          <div v-for="news in latestNews.slice(0, 5)" :key="news.id" class="p-3 bg-slate-50 rounded-lg border border-slate-100 hover:border-sky-200 transition-colors cursor-pointer">
            <h4 class="font-medium text-slate-800 text-sm">{{ news.title }}</h4>
            <p class="text-xs text-slate-500 mt-1">{{ formatDate(news.createdDate) }}</p>
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

      <!-- Recent Members -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Hội viên mới</h3>
        <div v-if="recentMembers.length === 0" class="text-center py-8 text-gray-500">
          Chưa có hội viên mới
        </div>
        <div v-else class="space-y-3">
          <div v-for="member in recentMembers.slice(0, 5)" :key="member.id" class="flex items-center justify-between p-3 bg-slate-50 rounded-lg border border-slate-100">
            <div class="flex items-center space-x-3">
              <div class="w-10 h-10 bg-sky-100 rounded-full flex items-center justify-center border-2 border-white">
                <span class="text-sm font-semibold text-sky-700">{{ member.fullName?.charAt(0)?.toUpperCase() || 'M' }}</span>
              </div>
              <div>
                <p class="font-medium text-slate-800 text-sm">{{ member.fullName }}</p>
                <p class="text-xs text-slate-500">{{ member.email }}</p>
              </div>
            </div>
            <span class="text-xs text-slate-500">{{ formatDate(member.joinDate || member.createdDate) }}</span>
          </div>
        </div>
      </div>

      <!-- System Overview -->
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Tổng quan hoạt động</h3>
        <div class="space-y-4">
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Booking hôm nay</span>
            <span class="font-semibold text-slate-800">{{ stats.todayBookings }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Trận đấu hôm nay</span>
            <span class="font-semibold text-slate-800">{{ stats.todayMatches }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Tin tức đã đăng</span>
            <span class="font-semibold text-slate-800">{{ stats.totalNews }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Giải đấu (tổng)</span>
            <span class="font-semibold text-slate-800">{{ stats.totalTournaments }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axiosClient from '@/api/axiosClient'
import { format, parseISO } from 'date-fns'
import { UsersIcon, BanknotesIcon, TrophyIcon, BuildingOfficeIcon } from '@heroicons/vue/24/outline'

const stats = ref({
  totalMembers: 0,
  clubFund: 0,
  ongoingTournaments: 0,
  totalCourts: 0,
  todayBookings: 0,
  todayMatches: 0,
  totalNews: 0,
  totalTournaments: 0
})

const latestNews = ref([])
const topRanking = ref([])
const recentMembers = ref([])

onMounted(async () => {
  await Promise.all([
    fetchStats(),
    fetchLatestNews(),
    fetchTopRanking(),
    fetchRecentMembers()
  ])
})

const fetchStats = async () => {
  try {
    const [membersRes, courtsRes, newsRes] = await Promise.all([
      axiosClient.get('/members/count'),
      axiosClient.get('/courts'),
      axiosClient.get('/news?pageSize=100')
    ])

    if (membersRes.data.success) stats.value.totalMembers = membersRes.data.data
    if (courtsRes.data.success) stats.value.totalCourts = courtsRes.data.data.length
    if (newsRes.data.success) stats.value.totalNews = newsRes.data.data.totalCount || newsRes.data.data.items?.length || 0

    // Try to fetch financial data (may fail if not authorized)
    try {
      const transactionsRes = await axiosClient.get('/transactions/summary')
      if (transactionsRes.data.success) stats.value.clubFund = transactionsRes.data.data.balance || 0
    } catch (error) {
      console.warn('Could not fetch financial data:', error.response?.status)
      stats.value.clubFund = 0
    }

    // Fetch tournament data
    const tournamentsRes = await axiosClient.get('/tournaments?pageSize=100')
    if (tournamentsRes.data.success) {
      const tournaments = tournamentsRes.data.data.items || []
      stats.value.totalTournaments = tournaments.length
      stats.value.ongoingTournaments = tournaments.filter(t => t.status === 1).length
    }
  } catch (error) {
    console.error('Error fetching stats:', error)
  }
}

const fetchLatestNews = async () => {
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

const fetchRecentMembers = async () => {
  try {
    const response = await axiosClient.get('/members?pageSize=5')
    if (response.data.success) {
      const members = response.data.data.items || []
      recentMembers.value = members.sort((a, b) => new Date(b.createdDate) - new Date(a.createdDate))
    }
  } catch (error) {
    console.error('Error fetching members:', error)
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
</script>
