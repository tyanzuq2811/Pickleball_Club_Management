<template>
  <div class="space-y-6">
    <h2 class="text-2xl font-bold text-slate-800">Quản Lý Tài Chính (Thủ Quỹ)</h2>

    <!-- Pending Deposits Section -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div class="p-6 border-b border-slate-100 flex justify-between items-center">
        <h3 class="text-lg font-bold text-slate-800 flex items-center">
          <span class="bg-yellow-100 text-yellow-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">Pending</span>
          Yêu cầu nạp tiền chờ duyệt
        </h3>
        <button @click="transactionStore.fetchPendingDeposits" class="text-sm text-sky-600 hover:underline">Làm mới</button>
      </div>
      
      <div v-if="transactionStore.pendingDeposits.length === 0" class="p-8 text-center text-slate-500">
        Không có yêu cầu nào đang chờ.
      </div>

      <table v-else class="w-full text-sm text-left">
        <thead class="text-xs text-slate-500 uppercase bg-slate-50">
          <tr>
            <th class="px-6 py-3">Hội viên</th>
            <th class="px-6 py-3">Số tiền</th>
            <th class="px-6 py-3">Nội dung</th>
            <th class="px-6 py-3">Thời gian</th>
            <th class="px-6 py-3 text-right">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in transactionStore.pendingDeposits" :key="item.id" class="border-b border-slate-100 hover:bg-slate-50">
            <td class="px-6 py-4 font-medium text-slate-900">{{ item.memberName }}</td>
            <td class="px-6 py-4 font-bold text-green-600">+{{ formatCurrency(item.amount) }}</td>
            <td class="px-6 py-4">{{ item.description }}</td>
            <td class="px-6 py-4 text-slate-500">{{ formatDate(item.date) }}</td>
            <td class="px-6 py-4 text-right">
              <button @click="handleApprove(item.id)" class="bg-green-600 text-white px-3 py-1 rounded hover:bg-green-700 text-xs font-medium transition-colors">
                Duyệt
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- All Transactions Section -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div class="p-6 border-b border-slate-100">
        <h3 class="text-lg font-bold text-slate-800">Lịch sử thu chi CLB</h3>
      </div>
      <!-- Table content similar to MyWallet but for club transactions -->
      <div class="p-6 text-center text-slate-500 italic">
        (Danh sách toàn bộ giao dịch của hệ thống sẽ hiển thị ở đây)
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useTransactionStore } from '@/stores/transaction';
import { format } from 'date-fns';

const transactionStore = useTransactionStore();

onMounted(() => {
  transactionStore.fetchPendingDeposits();
});

const handleApprove = async (id) => {
  if (confirm('Xác nhận duyệt khoản nạp này?')) {
    await transactionStore.approveDeposit(id);
  }
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
const formatDate = (date) => format(new Date(date), 'dd/MM/yyyy HH:mm');
</script>
