<template>
  <div class="space-y-6">
    <h2 class="text-2xl font-bold text-slate-800">Ví Điện Tử</h2>

    <!-- Balance Card -->
    <div class="bg-gradient-to-r from-primary-600 to-primary-800 rounded-2xl p-8 text-white shadow-lg">
      <div class="flex justify-between items-start">
        <div>
          <p class="text-primary-100 font-medium mb-1">Số dư hiện tại</p>
          <h3 class="text-4xl font-bold">{{ formatCurrency(authStore.user?.walletBalance || 0) }}</h3>
        </div>
        <div class="bg-white/20 p-3 rounded-lg backdrop-blur-sm">
          <CreditCardIcon class="w-8 h-8 text-white" />
        </div>
      </div>
      <div class="mt-8 flex space-x-4">
        <button @click="showDepositModal = true" class="bg-white text-primary-700 px-6 py-2 rounded-lg font-bold hover:bg-primary-50 transition-colors shadow-sm">
          + Nạp tiền
        </button>
      </div>
    </div>

    <!-- Transactions History -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div class="p-6 border-b border-slate-100">
        <h3 class="text-lg font-bold text-slate-800">Lịch sử giao dịch</h3>
      </div>
      
      <div v-if="walletStore.loading" class="p-8 text-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600 mx-auto"></div>
      </div>

      <table v-else class="w-full text-sm text-left">
        <thead class="text-xs text-slate-500 uppercase bg-slate-50">
          <tr>
            <th class="px-6 py-3">Thời gian</th>
            <th class="px-6 py-3">Loại</th>
            <th class="px-6 py-3">Nội dung</th>
            <th class="px-6 py-3 text-right">Số tiền</th>
            <th class="px-6 py-3 text-center">Trạng thái</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in walletStore.transactions" :key="tx.id" class="border-b border-slate-100 hover:bg-slate-50">
            <td class="px-6 py-4">{{ formatDate(tx.date) }}</td>
            <td class="px-6 py-4 font-medium">{{ getTypeName(tx.type) }}</td>
            <td class="px-6 py-4 text-slate-600">{{ tx.description }}</td>
            <td class="px-6 py-4 text-right font-bold" :class="tx.amount >= 0 ? 'text-green-600' : 'text-red-600'">
              {{ tx.amount > 0 ? '+' : '' }}{{ formatCurrency(tx.amount) }}
            </td>
            <td class="px-6 py-4 text-center">
              <span class="px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(tx.status)">
                {{ getStatusName(tx.status) }}
              </span>
            </td>
          </tr>
          <tr v-if="walletStore.transactions.length === 0">
            <td colspan="5" class="px-6 py-8 text-center text-slate-500">Chưa có giao dịch nào</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Simple Deposit Modal (Placeholder) -->
    <div v-if="showDepositModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white p-6 rounded-xl w-96 shadow-xl">
        <h3 class="text-lg font-bold mb-4">Nạp tiền vào ví</h3>
        <input type="number" v-model="depositAmount" placeholder="Nhập số tiền" class="w-full border p-2 rounded mb-4">
        <div class="flex justify-end space-x-2">
          <button @click="showDepositModal = false" class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded">Hủy</button>
          <button @click="handleDeposit" class="px-4 py-2 bg-primary-600 text-white rounded hover:bg-primary-700">Xác nhận</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useWalletStore } from '@/stores/wallet';
import { useAuthStore } from '@/stores/auth';
import { CreditCardIcon } from '@heroicons/vue/24/outline';
import { format } from 'date-fns';

const walletStore = useWalletStore();
const authStore = useAuthStore();
const showDepositModal = ref(false);
const depositAmount = ref(50000);

onMounted(() => {
  walletStore.fetchTransactions();
});

const handleDeposit = async () => {
  const success = await walletStore.deposit(depositAmount.value);
  if (success) showDepositModal.value = false;
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
const formatDate = (date) => format(new Date(date), 'dd/MM/yyyy HH:mm');

const getTypeName = (type) => ['Nạp tiền', 'Thanh toán', 'Nhận thưởng', 'Hoàn tiền', 'Phí giải'][type] || 'Khác';
const getStatusName = (status) => ['Chờ xử lý', 'Thành công', 'Thất bại', 'Đã hủy'][status] || 'Unknown';
const getStatusClass = (status) => [
  'bg-yellow-100 text-yellow-800', 
  'bg-green-100 text-green-800', 
  'bg-red-100 text-red-800', 
  'bg-slate-100 text-slate-800'][status];
</script>