<template>
  <div class="space-y-6">
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <p class="text-gray-500">Đang tải dữ liệu...</p>
    </div>

    <template v-else>
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
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
import { ref, onMounted } from 'vue'
import { UsersIcon, CurrencyDollarIcon, TrophyIcon } from '@heroicons/vue/24/outline'
import axiosClient from '@/api/axiosClient'

const loading = ref(true)
const stats = ref({
  totalMembers: 0,
  clubFund: 0,
  ongoingTournaments: 0
})
const pinnedNews = ref([])
const latestNews = ref([])
const topRanking = ref([])

onMounted(async () => {
  await Promise.all([
    fetchStats(),
    fetchPinnedNews(),
    fetchLatestNews(),
    fetchTopRanking()
  ])
  loading.value = false
})

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

const truncate = (text, length) => {
  if (!text) return ''
  return text.length > length ? text.substring(0, length) + '...' : text
}
</script>