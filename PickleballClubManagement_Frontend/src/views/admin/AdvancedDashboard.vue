<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-3xl font-bold text-slate-800">Tổng Quan Nâng Cao</h2>
      <div class="flex gap-3">
        <select v-model="timeRange" @change="fetchDashboardData" 
                class="px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500">
          <option value="7">7 ngày qua</option>
          <option value="30">30 ngày qua</option>
          <option value="90">90 ngày qua</option>
          <option value="365">1 năm</option>
        </select>
        <button @click="fetchDashboardData" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
          </svg>
        </button>
      </div>
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg p-6 text-white">
        <div class="flex items-center justify-between mb-2">
          <div class="text-blue-100 text-sm font-medium">Tổng Doanh Thu</div>
          <svg class="w-8 h-8 text-blue-200" fill="currentColor" viewBox="0 0 20 20">
            <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z"/>
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd"/>
          </svg>
        </div>
        <div class="text-3xl font-bold mb-1">{{ formatCurrency(stats.totalRevenue) }}</div>
        <div class="text-blue-100 text-sm">
          <span :class="stats.revenueGrowth >= 0 ? 'text-green-300' : 'text-red-300'">
            {{ stats.revenueGrowth >= 0 ? '↑' : '↓' }} {{ Math.abs(stats.revenueGrowth) }}%
          </span>
          so với kỳ trước
        </div>
      </div>

      <div class="bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg p-6 text-white">
        <div class="flex items-center justify-between mb-2">
          <div class="text-green-100 text-sm font-medium">Booking Thành Công</div>
          <svg class="w-8 h-8 text-green-200" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"/>
            <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm9.707 5.707a1 1 0 00-1.414-1.414L9 12.586l-1.293-1.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
          </svg>
        </div>
        <div class="text-3xl font-bold mb-1">{{ stats.totalBookings }}</div>
        <div class="text-green-100 text-sm">
          {{ stats.bookingCompletionRate }}% tỷ lệ hoàn thành
        </div>
      </div>

      <div class="bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl shadow-lg p-6 text-white">
        <div class="flex items-center justify-between mb-2">
          <div class="text-purple-100 text-sm font-medium">Hội Viên Hoạt Động</div>
          <svg class="w-8 h-8 text-purple-200" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z"/>
          </svg>
        </div>
        <div class="text-3xl font-bold mb-1">{{ stats.activeMembers }}</div>
        <div class="text-purple-100 text-sm">
          {{ stats.newMembersThisMonth }} thành viên mới tháng này
        </div>
      </div>

      <div class="bg-gradient-to-br from-orange-500 to-orange-600 rounded-xl shadow-lg p-6 text-white">
        <div class="flex items-center justify-between mb-2">
          <div class="text-orange-100 text-sm font-medium">Giải Đấu Đang Diễn Ra</div>
          <svg class="w-8 h-8 text-orange-200" fill="currentColor" viewBox="0 0 20 20">
            <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
          </svg>
        </div>
        <div class="text-3xl font-bold mb-1">{{ stats.activeTournaments }}</div>
        <div class="text-orange-100 text-sm">
          {{ stats.totalMatches }} trận đấu đã hoàn thành
        </div>
      </div>
    </div>

    <!-- Charts Row -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Revenue Chart -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Doanh Thu Theo Ngày</h3>
        <canvas ref="revenueChartRef" height="300"></canvas>
      </div>

      <!-- Booking Chart -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Booking Theo Trạng Thái</h3>
        <canvas ref="bookingChartRef" height="300"></canvas>
      </div>
    </div>

    <!-- More Charts -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Court Usage -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Tỷ Lệ Sử Dụng Sân</h3>
        <canvas ref="courtUsageChartRef" height="200"></canvas>
      </div>

      <!-- Member Growth -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Tăng Trưởng Hội Viên</h3>
        <canvas ref="memberGrowthChartRef" height="200"></canvas>
      </div>

      <!-- Payment Methods -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Phương Thức Thanh Toán</h3>
        <canvas ref="paymentMethodsChartRef" height="200"></canvas>
      </div>
    </div>

    <!-- Recent Activities -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Top Courts -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Top Sân Được Đặt Nhiều Nhất</h3>
        <div class="space-y-3">
          <div v-for="court in stats.topCourts" :key="court.id" 
               class="flex items-center justify-between p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
                <span class="text-blue-600 font-bold">{{ court.rank }}</span>
              </div>
              <div>
                <div class="font-medium text-slate-800">{{ court.name }}</div>
                <div class="text-sm text-slate-500">{{ court.bookings }} lượt đặt</div>
              </div>
            </div>
            <div class="text-right">
              <div class="font-bold text-green-600">{{ formatCurrency(court.revenue) }}</div>
              <div class="text-xs text-slate-500">doanh thu</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Top Members -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
        <h3 class="text-lg font-bold text-slate-800 mb-4">Top Hội Viên Tích Cực</h3>
        <div class="space-y-3">
          <div v-for="member in stats.topMembers" :key="member.id"
               class="flex items-center justify-between p-3 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-purple-100 rounded-full flex items-center justify-center">
                <span class="text-purple-600 font-bold">{{ member.initials }}</span>
              </div>
              <div>
                <div class="font-medium text-slate-800">{{ member.name }}</div>
                <div class="text-sm text-slate-500">{{ member.bookings }} booking</div>
              </div>
            </div>
            <div class="text-right">
              <div class="font-bold text-purple-600">{{ formatCurrency(member.totalSpent) }}</div>
              <div class="text-xs text-slate-500">tổng chi</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue';
import { Chart, registerables } from 'chart.js';
import axiosClient from '@/api/axiosClient';
import { useToast } from 'vue-toastification';

Chart.register(...registerables);

const toast = useToast();
const timeRange = ref('30');
const revenueChartRef = ref(null);
const bookingChartRef = ref(null);
const courtUsageChartRef = ref(null);
const memberGrowthChartRef = ref(null);
const paymentMethodsChartRef = ref(null);

const stats = ref({
  totalRevenue: 0,
  revenueGrowth: 0,
  totalBookings: 0,
  bookingCompletionRate: 0,
  activeMembers: 0,
  newMembersThisMonth: 0,
  activeTournaments: 0,
  totalMatches: 0,
  topCourts: [],
  topMembers: []
});

let revenueChart = null;
let bookingChart = null;
let courtUsageChart = null;
let memberGrowthChart = null;
let paymentMethodsChart = null;

onMounted(async () => {
  await fetchDashboardData();
});

const fetchDashboardData = async () => {
  try {
    const response = await axiosClient.get(`/dashboard/stats?days=${timeRange.value}`);
    if (response.data.success) {
      stats.value = response.data.data;
      await nextTick();
      renderCharts();
    } else {
      toast.error('Không thể tải dữ liệu dashboard');
    }
  } catch (error) {
    console.error('Error fetching dashboard data:', error);
    toast.error('Lỗi khi tải dữ liệu dashboard: ' + (error.response?.data?.message || error.message));
  }
};

const renderCharts = () => {
  if (!stats.value) return;
  
  // Revenue Chart
  if (revenueChart) revenueChart.destroy();
  revenueChart = new Chart(revenueChartRef.value, {
    type: 'line',
    data: {
      labels: stats.value.revenueChartLabels || [],
      datasets: [{
        label: 'Doanh thu (VNĐ)',
        data: stats.value.revenueChartData || [],
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        tension: 0.4,
        fill: true
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { 
        legend: { display: false },
        tooltip: {
          callbacks: {
            label: (context) => formatCurrency(context.parsed.y)
          }
        }
      }
    }
  });

  // Booking Chart
  if (bookingChart) bookingChart.destroy();
  bookingChart = new Chart(bookingChartRef.value, {
    type: 'doughnut',
    data: {
      labels: stats.value.bookingStatusLabels || [],
      datasets: [{
        data: stats.value.bookingStatusData || [],
        backgroundColor: ['#10b981', '#f59e0b', '#ef4444']
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false
    }
  });

  // Court Usage Chart
  if (courtUsageChart) courtUsageChart.destroy();
  courtUsageChart = new Chart(courtUsageChartRef.value, {
    type: 'bar',
    data: {
      labels: stats.value.courtUsageLabels || [],
      datasets: [{
        label: 'Tỷ lệ sử dụng (%)',
        data: stats.value.courtUsageData || [],
        backgroundColor: '#8b5cf6'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        y: {
          beginAtZero: true,
          max: 100,
          ticks: {
            callback: (value) => value + '%'
          }
        }
      }
    }
  });

  // Member Growth Chart
  if (memberGrowthChart) memberGrowthChart.destroy();
  memberGrowthChart = new Chart(memberGrowthChartRef.value, {
    type: 'line',
    data: {
      labels: stats.value.memberGrowthLabels || [],
      datasets: [{
        label: 'Hội viên mới',
        data: stats.value.memberGrowthData || [],
        borderColor: '#8b5cf6',
        backgroundColor: 'rgba(139, 92, 246, 0.1)',
        tension: 0.4,
        fill: true
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            stepSize: 1
          }
        }
      }
    }
  });

  // Payment Methods Chart - Static for now (can be dynamic later)
  if (paymentMethodsChart) paymentMethodsChart.destroy();
  paymentMethodsChart = new Chart(paymentMethodsChartRef.value, {
    type: 'pie',
    data: {
      labels: ['Ví CLB', 'Chuyển khoản', 'Tiền mặt'],
      datasets: [{
        data: [60, 30, 10],
        backgroundColor: ['#3b82f6', '#10b981', '#f59e0b']
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false
    }
  });
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
</script>
