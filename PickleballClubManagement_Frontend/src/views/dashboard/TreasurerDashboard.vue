<template>
  <div class="space-y-6">
    <div>
      <h2 class="text-2xl font-bold text-slate-800">Dashboard - Thủ quỹ</h2>
      <p class="text-slate-600 mt-1">Quản lý tài chính câu lạc bộ</p>
    </div>

    <!-- Financial Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
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
            <p class="text-sm font-medium text-slate-600">Thu tháng này</p>
            <p class="text-2xl font-bold text-green-600 mt-2">{{ formatCurrency(stats.monthlyIncome) }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <ArrowTrendingUpIcon class="w-6 h-6 text-green-600" />
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600">Chi tháng này</p>
            <p class="text-2xl font-bold text-red-600 mt-2">{{ formatCurrency(Math.abs(stats.monthlyExpense)) }}</p>
          </div>
          <div class="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
            <ArrowTrendingDownIcon class="w-6 h-6 text-red-600" />
          </div>
        </div>
      </div>

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
    </div>

    <!-- Recent Transactions -->
    <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-lg font-semibold text-slate-800">Giao dịch gần đây</h3>
        <router-link to="/treasury" class="text-sm text-sky-600 hover:underline">Xem tất cả</router-link>
      </div>
      
      <div v-if="recentTransactions.length === 0" class="text-center py-8 text-gray-500">
        Chưa có giao dịch nào
      </div>
      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-slate-50 border-b border-slate-200">
            <tr>
              <th class="px-4 py-3 text-left text-xs font-semibold text-slate-600 uppercase">Ngày</th>
              <th class="px-4 py-3 text-left text-xs font-semibold text-slate-600 uppercase">Mô tả</th>
              <th class="px-4 py-3 text-left text-xs font-semibold text-slate-600 uppercase">Danh mục</th>
              <th class="px-4 py-3 text-right text-xs font-semibold text-slate-600 uppercase">Số tiền</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="tx in recentTransactions.slice(0, 10)" :key="tx.id" class="border-b border-slate-100 hover:bg-slate-50">
              <td class="px-4 py-3 text-sm text-slate-600">{{ formatDate(tx.date) }}</td>
              <td class="px-4 py-3 text-sm text-slate-800">{{ tx.description }}</td>
              <td class="px-4 py-3 text-sm text-slate-600">{{ tx.categoryName || 'N/A' }}</td>
              <td class="px-4 py-3 text-sm text-right font-semibold" :class="tx.amount > 0 ? 'text-green-600' : 'text-red-600'">
                {{ tx.amount > 0 ? '+' : '' }}{{ formatCurrency(tx.amount) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Financial Overview Charts (Placeholder) -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Cảnh báo tài chính</h3>
        <div v-if="stats.clubFund < 5000000" class="p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
          <p class="text-sm text-yellow-800">⚠️ Quỹ CLB thấp (dưới 5 triệu VNĐ)</p>
        </div>
        <div v-else class="p-4 bg-green-50 border border-green-200 rounded-lg">
          <p class="text-sm text-green-800">✓ Tình hình tài chính ổn định</p>
        </div>
      </div>

      <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
        <h3 class="text-lg font-semibold text-slate-800 mb-4">Báo cáo nhanh</h3>
        <div class="space-y-3">
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Số giao dịch tháng này</span>
            <span class="font-semibold text-slate-800">{{ stats.monthlyTransactionCount }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-sm text-slate-600">Thu/Chi trung bình</span>
            <span class="font-semibold text-slate-800">{{ formatCurrency(stats.avgTransaction) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axiosClient from '@/api/axiosClient'
import { format, parseISO, startOfMonth, endOfMonth } from 'date-fns'
import { BanknotesIcon, UsersIcon, ArrowTrendingUpIcon, ArrowTrendingDownIcon } from '@heroicons/vue/24/outline'

const stats = ref({
  clubFund: 0,
  monthlyIncome: 0,
  monthlyExpense: 0,
  totalMembers: 0,
  monthlyTransactionCount: 0,
  avgTransaction: 0
})

const recentTransactions = ref([])

onMounted(async () => {
  await Promise.all([
    fetchStats(),
    fetchTransactions()
  ])
})

const fetchStats = async () => {
  try {
    const now = new Date()
    const startDate = format(startOfMonth(now), 'yyyy-MM-dd')
    const endDate = format(endOfMonth(now), 'yyyy-MM-dd')

    const [summaryRes, membersRes] = await Promise.all([
      axiosClient.get(`/transactions/summary?startDate=${startDate}&endDate=${endDate}`),
      axiosClient.get('/members/count')
    ])

    if (summaryRes.data.success) {
      stats.value.clubFund = summaryRes.data.data.balance || 0
      stats.value.monthlyIncome = summaryRes.data.data.totalIncome || 0
      stats.value.monthlyExpense = summaryRes.data.data.totalExpense || 0
    }

    if (membersRes.data.success) {
      stats.value.totalMembers = membersRes.data.data || 0
    }
  } catch (error) {
    console.error('Error fetching stats:', error)
  }
}

const fetchTransactions = async () => {
  try {
    const response = await axiosClient.get('/transactions?pageSize=20')
    if (response.data.success) {
      recentTransactions.value = response.data.data.items || []
      stats.value.monthlyTransactionCount = recentTransactions.value.length
      
      if (recentTransactions.value.length > 0) {
        const total = recentTransactions.value.reduce((sum, tx) => sum + Math.abs(tx.amount), 0)
        stats.value.avgTransaction = total / recentTransactions.value.length
      }
    }
  } catch (error) {
    console.error('Error fetching transactions:', error)
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
