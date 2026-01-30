<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Quản Lý Tài Chính (Thủ Quỹ)</h2>
      <div class="flex gap-3">
        <div class="relative">
          <button @click="showExportMenu = !showExportMenu" 
                  class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-medium text-sm flex items-center gap-2">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
            </svg>
            Xuất báo cáo
          </button>
          <div v-if="showExportMenu" class="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-xl border border-slate-200 z-10">
            <button @click="exportData('excel')" class="w-full text-left px-4 py-2 hover:bg-slate-50 flex items-center gap-2">
              <svg class="w-4 h-4 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9 2a2 2 0 00-2 2v8a2 2 0 002 2h6a2 2 0 002-2V6.414A2 2 0 0016.414 5L14 2.586A2 2 0 0012.586 2H9z"/>
              </svg>
              Excel (.xlsx)
            </button>
            <button @click="exportData('pdf')" class="w-full text-left px-4 py-2 hover:bg-slate-50 flex items-center gap-2 border-t border-slate-100">
              <svg class="w-4 h-4 text-red-600" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M4 4a2 2 0 012-2h4.586A2 2 0 0112 2.586L15.414 6A2 2 0 0116 7.414V16a2 2 0 01-2 2H6a2 2 0 01-2-2V4z" clip-rule="evenodd"/>
              </svg>
              PDF (.pdf)
            </button>
          </div>
        </div>
        <button @click="showCategoryModal = true" class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors font-medium text-sm">
          Quản lý Danh mục
        </button>
      </div>
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
          <span class="bg-yellow-100 text-yellow-800 text-xs font-semibold mr-2 px-2.5 py-0.5 rounded">
            {{ transactionStore.pendingDeposits.length }}
          </span>
          Yêu cầu nạp tiền chờ duyệt
        </h3>
        <button @click="refreshPendingDeposits" class="text-sm text-sky-600 hover:underline flex items-center gap-1">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
          </svg>
          Làm mới
        </button>
      </div>
      
      <div v-if="loadingPending" class="p-8 text-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-sky-600 mx-auto"></div>
      </div>

      <div v-else-if="transactionStore.pendingDeposits.length === 0" class="p-8 text-center text-slate-500">
        <svg class="w-16 h-16 mx-auto mb-3 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
        </svg>
        <p>Không có yêu cầu nạp tiền nào đang chờ duyệt</p>
      </div>

      <table v-else class="w-full text-sm text-left">
        <thead class="text-xs text-slate-500 uppercase bg-slate-50">
          <tr>
            <th class="px-6 py-3">ID</th>
            <th class="px-6 py-3">Hội viên</th>
            <th class="px-6 py-3">Số tiền</th>
            <th class="px-6 py-3">Phương thức</th>
            <th class="px-6 py-3">Ghi chú</th>
            <th class="px-6 py-3">Thời gian</th>
            <th class="px-6 py-3 text-right">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in transactionStore.pendingDeposits" :key="item.id" class="border-b border-slate-100 hover:bg-slate-50">
            <td class="px-6 py-4 font-mono text-xs text-slate-500">#{{ item.id }}</td>
            <td class="px-6 py-4 font-medium text-slate-900">
              <div>{{ item.memberName }}</div>
              <div class="text-xs text-slate-500">{{ item.memberEmail }}</div>
            </td>
            <td class="px-6 py-4 font-bold text-green-600 text-lg">+{{ formatCurrency(item.amount) }}</td>
            <td class="px-6 py-4">
              <span class="px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 text-blue-800">
                {{ item.paymentMethod || 'N/A' }}
              </span>
            </td>
            <td class="px-6 py-4 text-slate-600 max-w-xs truncate">{{ item.description || '-' }}</td>
            <td class="px-6 py-4 text-slate-500">{{ formatDate(item.date) }}</td>
            <td class="px-6 py-4">
              <div class="flex items-center justify-end gap-2">
                <button @click="viewDepositDetail(item)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-blue-50 text-blue-600 hover:bg-blue-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Xem chi tiết">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                  </svg>
                  Xem
                </button>
                <button @click="handleApprove(item.id)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-green-600 text-white hover:bg-green-700 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Phê duyệt">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                  </svg>
                  Duyệt
                </button>
                <button @click="handleReject(item.id)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-red-600 text-white hover:bg-red-700 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Từ chối">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                  </svg>
                  Từ chối
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Deposit Detail Modal -->
    <div v-if="showDepositDetailModal && selectedDeposit" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-lg w-full mx-4">
        <div class="p-6 border-b border-slate-200">
          <h3 class="text-xl font-bold text-slate-800">Chi tiết yêu cầu nạp tiền</h3>
        </div>
        <div class="p-6 space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <p class="text-sm text-slate-500">ID giao dịch</p>
              <p class="font-mono font-semibold">#{{ selectedDeposit.id }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Thời gian</p>
              <p class="font-semibold">{{ formatDate(selectedDeposit.date) }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Hội viên</p>
              <p class="font-semibold">{{ selectedDeposit.memberName }}</p>
              <p class="text-xs text-slate-500">{{ selectedDeposit.memberEmail }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Số tiền</p>
              <p class="font-bold text-green-600 text-2xl">{{ formatCurrency(selectedDeposit.amount) }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Phương thức</p>
              <p class="font-semibold">{{ selectedDeposit.paymentMethod || 'N/A' }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Trạng thái</p>
              <span class="px-2 py-1 text-xs font-semibold rounded-full bg-yellow-100 text-yellow-800">
                Chờ duyệt
              </span>
            </div>
          </div>
          <div>
            <p class="text-sm text-slate-500 mb-2">Ghi chú</p>
            <p class="p-3 bg-slate-50 rounded text-sm">{{ selectedDeposit.description || 'Không có ghi chú' }}</p>
          </div>
        </div>
        <div class="p-6 bg-slate-50 border-t border-slate-200 flex justify-end space-x-3">
          <button @click="showDepositDetailModal = false" 
                  class="px-4 py-2 text-slate-700 hover:bg-slate-200 rounded-lg font-medium transition-colors">
            Đóng
          </button>
          <button @click="handleReject(selectedDeposit.id)" 
                  class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 font-medium transition-colors">
            Từ chối
          </button>
          <button @click="handleApprove(selectedDeposit.id)" 
                  class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 font-medium transition-colors">
            Duyệt ngay
          </button>
        </div>
      </div>
    </div>

    <!-- All Transactions Section -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div class="p-6 border-b border-slate-100">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-bold text-slate-800">Lịch sử thu chi CLB</h3>
          <button @click="showTransactionModal = true" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors text-sm font-medium">
            + Thêm giao dịch
          </button>
        </div>
        
        <!-- Date Range Filter -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Từ ngày</label>
            <input v-model="dateFilters.startDate" type="date"
                   class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Đến ngày</label>
            <input v-model="dateFilters.endDate" type="date"
                   class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-sky-500">
          </div>
          <div class="flex items-end">
            <button @click="clearDateFilter" class="w-full px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 transition-colors text-sm font-medium">
              Xóa lọc
            </button>
          </div>
        </div>
      </div>
      
      <div v-if="transactionStore.loading" class="p-8 text-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-sky-600 mx-auto"></div>
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
              <div class="flex items-center justify-center gap-2">
                <button @click="editTransaction(tx)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Chỉnh sửa">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                  </svg>
                  Sửa
                </button>
                <button @click="deleteTransaction(tx.id)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-red-50 text-red-600 hover:bg-red-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Xóa">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                  </svg>
                  Xóa
                </button>
              </div>
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
                   class="flex-1 px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            <select v-model.number="newCategory.type" required class="px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
              <option :value="0">Thu</option>
              <option :value="1">Chi</option>
            </select>
            <button type="submit" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 font-medium">
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
              <button @click="deleteCategory(cat.id)" 
                      class="inline-flex items-center gap-1 px-3 py-1.5 bg-red-50 text-red-600 hover:bg-red-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                      title="Xóa danh mục">
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                </svg>
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

    <!-- Add/Edit Transaction Modal -->
    <div v-if="showTransactionModal || showEditTransactionModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">
            {{ showTransactionModal ? 'Thêm Giao Dịch' : 'Chỉnh Sửa Giao Dịch' }}
          </h3>
          
          <form @submit.prevent="handleSubmitTransaction" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Danh mục</label>
              <select v-model.number="newTransaction.categoryId" required class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                <option value="">-- Chọn danh mục --</option>
                <option v-for="cat in transactionStore.categories" :key="cat.id" :value="cat.id">
                  {{ cat.name }} ({{ cat.type === 0 ? 'Thu' : 'Chi' }})
                </option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số tiền</label>
              <input v-model.number="newTransaction.amount" type="number" min="0" step="1000" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Mô tả</label>
              <textarea v-model="newTransaction.description" rows="3" required
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500"></textarea>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Ngày giao dịch</label>
              <input v-model="newTransaction.date" type="date" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div class="flex justify-end space-x-2 pt-4">
              <button type="button" @click="closeTransactionModal" 
                      class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
                Hủy
              </button>
              <button type="submit" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors">
                {{ showTransactionModal ? 'Thêm giao dịch' : 'Cập nhật' }}
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
import { useConfirmDialog } from '@/composables/useConfirmDialog';

const transactionStore = useTransactionStore();
const toast = useToast();
const { confirm: confirmDialog, confirmDelete, confirmWarning } = useConfirmDialog();
const showCategoryModal = ref(false);
const showTransactionModal = ref(false);
const showEditTransactionModal = ref(false);
const selectedTransaction = ref(null);
const showDepositDetailModal = ref(false);
const selectedDeposit = ref(null);
const loadingPending = ref(false);
const showExportMenu = ref(false);

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

const refreshPendingDeposits = async () => {
  loadingPending.value = true;
  await transactionStore.fetchPendingDeposits();
  loadingPending.value = false;
};

const viewDepositDetail = (deposit) => {
  selectedDeposit.value = deposit;
  showDepositDetailModal.value = true;
};

const handleApprove = async (id) => {
  const confirmed = await confirmDialog('Xác nhận duyệt khoản nạp này? Tiền sẽ được cộng vào ví hội viên ngay lập tức.', { title: 'Duyệt nạp tiền', type: 'success' });
  if (confirmed) {
    const success = await transactionStore.approveDeposit(id);
    if (success) {
      showDepositDetailModal.value = false;
      await refreshPendingDeposits();
    }
  }
};

const handleReject = async (id) => {
  const confirmed = await confirmWarning('Xác nhận từ chối khoản nạp này? Hành động này không thể hoàn tác.');
  if (confirmed) {
    const success = await transactionStore.rejectDeposit(id);
    if (success) {
      toast.success('Đã từ chối yêu cầu nạp tiền');
      showDepositDetailModal.value = false;
      await refreshPendingDeposits();
    }
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
  const confirmed = await confirmDelete('Xác nhận xóa danh mục này?');
  if (confirmed) {
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
    closeTransactionModal();
    await transactionStore.fetchTransactions();
  }
};

const editTransaction = (tx) => {
  selectedTransaction.value = tx;
  newTransaction.value = {
    categoryId: tx.categoryId,
    amount: Math.abs(tx.amount),
    description: tx.description,
    date: format(new Date(tx.date), 'yyyy-MM-dd')
  };
  showEditTransactionModal.value = true;
};

const handleUpdateTransaction = async () => {
  const success = await transactionStore.updateTransaction(selectedTransaction.value.id, newTransaction.value);
  if (success) {
    toast.success('Cập nhật giao dịch thành công!');
    closeTransactionModal();
    await transactionStore.fetchTransactions();
  }
};

const handleSubmitTransaction = async () => {
  if (showTransactionModal.value) {
    await handleCreateTransaction();
  } else {
    await handleUpdateTransaction();
  }
};

const closeTransactionModal = () => {
  showTransactionModal.value = false;
  showEditTransactionModal.value = false;
  selectedTransaction.value = null;
  newTransaction.value = {
    categoryId: '',
    amount: 0,
    description: '',
    date: format(new Date(), 'yyyy-MM-dd')
  };
};

const deleteTransaction = async (id) => {
  const confirmed = await confirmDelete('Xác nhận xóa giao dịch này?');
  if (confirmed) {
    const success = await transactionStore.deleteTransaction(id);
    if (success) {
      toast.success('Xóa giao dịch thành công!');
      await transactionStore.fetchTransactions();
    }
  }
};

const exportData = async (exportFormat) => {
  try {
    showExportMenu.value = false;
    toast.info(`Đang xuất báo cáo ${exportFormat.toUpperCase()}...`);
    
    const params = new URLSearchParams();
    params.append('format', exportFormat);
    if (dateFilters.value.startDate) params.append('startDate', dateFilters.value.startDate);
    if (dateFilters.value.endDate) params.append('endDate', dateFilters.value.endDate);
    
    const response = await axiosClient.get(`/transactions/export?${params.toString()}`, {
      responseType: 'blob'
    });
    
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `Transactions_${format(new Date(), 'yyyyMMdd')}.${exportFormat === 'excel' ? 'xlsx' : 'pdf'}`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
    
    toast.success('Xuất báo cáo thành công!');
  } catch (error) {
    console.error('Export error:', error);
    toast.error('Xuất báo cáo thất bại');
  }
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
const formatDate = (date) => format(new Date(date), 'dd/MM/yyyy HH:mm');
</script>
