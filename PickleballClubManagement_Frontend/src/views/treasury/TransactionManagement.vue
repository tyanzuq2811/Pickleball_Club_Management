<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Quản Lý Tài Chính (Thủ Quỹ)</h2>
      <button @click="showCategoryModal = true" class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors font-medium text-sm">
        Quản lý Danh mục
      </button>
    </div>

    <!-- Fund Balance Warning -->
    <div v-if="clubFundBalance < 0" class="bg-red-50 border-l-4 border-red-500 p-4 rounded-lg">
      <div class="flex items-center">
        <svg class="w-5 h-5 text-red-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
        </svg>
        <p class="text-red-800 font-bold">⚠️ Cảnh báo: Quỹ CLB đang âm {{ formatCurrency(clubFundBalance) }}!</p>
      </div>
    </div>

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
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-bold text-slate-800">Lịch sử thu chi CLB</h3>
          <button @click="showTransactionModal = true" class="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors text-sm font-medium">
            + Thêm giao dịch
          </button>
        </div>
        
        <!-- Date Range Filter -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Từ ngày</label>
            <input v-model="dateFilters.startDate" type="date"
                   class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Đến ngày</label>
            <input v-model="dateFilters.endDate" type="date"
                   class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500">
          </div>
          <div class="flex items-end">
            <button @click="clearDateFilter" class="w-full px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 transition-colors text-sm font-medium">
              Xóa lọc
            </button>
          </div>
        </div>
      </div>
      
      <div v-if="transactionStore.loading" class="p-8 text-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600 mx-auto"></div>
      </div>

      <table v-else class="w-full text-sm text-left">
        <thead class="text-xs text-slate-500 uppercase bg-slate-50">
          <tr>
            <th class="px-6 py-3">Ngày</th>
            <th class="px-6 py-3">Danh mục</th>
            <th class="px-6 py-3">Mô tả</th>
            <th class="px-6 py-3 text-right">Số tiền</th>
            <th class="px-6 py-3 text-center">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="tx in filteredTransactions" :key="tx.id" class="border-b border-slate-100 hover:bg-slate-50">
            <td class="px-6 py-4">{{ formatDate(tx.date) }}</td>
            <td class="px-6 py-4">
              <span class="px-2 py-1 text-xs font-semibold rounded-full" :class="tx.categoryType === 0 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                {{ tx.categoryName }}
              </span>
            </td>
            <td class="px-6 py-4 text-slate-600">{{ tx.description }}</td>
            <td class="px-6 py-4 text-right font-bold" :class="tx.amount >= 0 ? 'text-green-600' : 'text-red-600'">
              {{ tx.amount > 0 ? '+' : '' }}{{ formatCurrency(tx.amount) }}
            </td>
            <td class="px-6 py-4 text-center">
              <button @click="deleteTransaction(tx.id)" class="text-red-600 hover:text-red-800 text-xs font-medium">
                Xóa
              </button>
            </td>
          </tr>
          <tr v-if="transactionStore.transactions.length === 0">
            <td colspan="5" class="px-6 py-8 text-center text-slate-500">Chưa có giao dịch nào</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Category Management Modal -->
    <div v-if="showCategoryModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Quản lý Danh mục Thu/Chi</h3>
          
          <form @submit.prevent="handleCreateCategory" class="mb-6 flex gap-2">
            <input v-model="newCategory.name" type="text" placeholder="Tên danh mục" required
                   class="flex-1 px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
            <select v-model.number="newCategory.type" required class="px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
              <option :value="0">Thu</option>
              <option :value="1">Chi</option>
            </select>
            <button type="submit" class="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 font-medium">
              Thêm
            </button>
          </form>

          <div class="space-y-2 mb-6">
            <div v-for="cat in transactionStore.categories" :key="cat.id" 
                 class="flex justify-between items-center p-3 bg-slate-50 rounded-lg">
              <div class="flex items-center space-x-3">
                <span class="px-2 py-1 text-xs font-semibold rounded-full" 
                      :class="cat.type === 0 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                  {{ cat.type === 0 ? 'Thu' : 'Chi' }}
                </span>
                <span class="font-medium text-slate-800">{{ cat.name }}</span>
              </div>
              <button @click="deleteCategory(cat.id)" class="text-red-600 hover:text-red-800 text-sm">
                Xóa
              </button>
            </div>
          </div>

          <button @click="showCategoryModal = false" class="w-full px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
            Đóng
          </button>
        </div>
      </div>
    </div>

    <!-- Add Transaction Modal -->
    <div v-if="showTransactionModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Thêm Giao Dịch</h3>
          
          <form @submit.prevent="handleCreateTransaction" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Danh mục</label>
              <select v-model.number="newTransaction.categoryId" required class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
                <option value="">-- Chọn danh mục --</option>
                <option v-for="cat in transactionStore.categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }} ({{ cat.type === 0 ? 'Thu' : 'Chi' }})
                </option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số tiền</label>
              <input v-model.number="newTransaction.amount" type="number" min="0" step="1000" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Mô tả</label>
              <textarea v-model="newTransaction.description" rows="3" required
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500"></textarea>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Ngày giao dịch</label>
              <input v-model="newTransaction.date" type="date" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-primary-500">
            </div>

            <div class="flex justify-end space-x-2 pt-4">
              <button type="button" @click="showTransactionModal = false" 
                      class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
                Hủy
              </button>
              <button type="submit" class="px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition-colors">
                Thêm giao dịch
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useTransactionStore } from '@/stores/transaction';
import { format, isWithinInterval, parseISO } from 'date-fns';
import { useToast } from 'vue-toastification';

const transactionStore = useTransactionStore();
const toast = useToast();
const showCategoryModal = ref(false);
const showTransactionModal = ref(false);

const newCategory = ref({ name: '', type: 0 });
const newTransaction = ref({
  categoryId: '',
  amount: 0,
  description: '',
  date: format(new Date(), 'yyyy-MM-dd')
});

const dateFilters = ref({
  startDate: '',
  endDate: ''
});

const clubFundBalance = computed(() => {
  return transactionStore.transactions.reduce((sum, tx) => sum + tx.amount, 0);
});

const filteredTransactions = computed(() => {
  let result = transactionStore.transactions;
  
  if (dateFilters.value.startDate && dateFilters.value.endDate) {
    const start = parseISO(dateFilters.value.startDate);
    const end = parseISO(dateFilters.value.endDate);
    
    result = result.filter(tx => {
      const txDate = parseISO(tx.date);
      return isWithinInterval(txDate, { start, end });
    });
  } else if (dateFilters.value.startDate) {
    const start = parseISO(dateFilters.value.startDate);
    result = result.filter(tx => parseISO(tx.date) >= start);
  } else if (dateFilters.value.endDate) {
    const end = parseISO(dateFilters.value.endDate);
    result = result.filter(tx => parseISO(tx.date) <= end);
  }
  
  return result;
});

const clearDateFilter = () => {
  dateFilters.value.startDate = '';
  dateFilters.value.endDate = '';
};

onMounted(async () => {
  await transactionStore.fetchPendingDeposits();
  await transactionStore.fetchCategories();
  await transactionStore.fetchTransactions();
});

const handleApprove = async (id) => {
  if (confirm('Xác nhận duyệt khoản nạp này?')) {
    await transactionStore.approveDeposit(id);
  }
};

const handleCreateCategory = async () => {
  const success = await transactionStore.createCategory(newCategory.value);
  if (success) {
    toast.success('Thêm danh mục thành công!');
    newCategory.value = { name: '', type: 0 };
    await transactionStore.fetchCategories();
  }
};

const deleteCategory = async (id) => {
  if (confirm('Xác nhận xóa danh mục này?')) {
    const success = await transactionStore.deleteCategory(id);
    if (success) {
      toast.success('Xóa danh mục thành công!');
      await transactionStore.fetchCategories();
    }
  }
};

const handleCreateTransaction = async () => {
  const success = await transactionStore.createTransaction(newTransaction.value);
  if (success) {
    toast.success('Thêm giao dịch thành công!');
    showTransactionModal.value = false;
    newTransaction.value = {
      categoryId: '',
      amount: 0,
      description: '',
      date: format(new Date(), 'yyyy-MM-dd')
    };
    await transactionStore.fetchTransactions();
  }
};

const deleteTransaction = async (id) => {
  if (confirm('Xác nhận xóa giao dịch này?')) {
    const success = await transactionStore.deleteTransaction(id);
    if (success) {
      toast.success('Xóa giao dịch thành công!');
      await transactionStore.fetchTransactions();
    }
  }
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
const formatDate = (date) => format(new Date(date), 'dd/MM/yyyy HH:mm');
</script>
