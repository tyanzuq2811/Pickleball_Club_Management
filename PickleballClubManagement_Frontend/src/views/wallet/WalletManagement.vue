<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Quản lý Yêu cầu Nạp tiền</h2>
      <button @click="fetchPending" class="px-4 py-2 bg-sky-100 text-sky-600 rounded-lg hover:bg-sky-200 transition-colors font-medium">
        <ArrowPathIcon class="w-5 h-5 inline mr-1" />
        Làm mới
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div class="bg-white rounded-xl p-6 border border-slate-200 shadow-sm">
        <div class="flex items-center gap-4">
          <div class="p-3 bg-amber-100 rounded-lg">
            <ClockIcon class="w-6 h-6 text-amber-600" />
          </div>
          <div>
            <p class="text-sm text-slate-500">Chờ duyệt</p>
            <p class="text-2xl font-bold text-slate-800">{{ pendingDeposits.length }}</p>
          </div>
        </div>
      </div>
      <div class="bg-white rounded-xl p-6 border border-slate-200 shadow-sm">
        <div class="flex items-center gap-4">
          <div class="p-3 bg-green-100 rounded-lg">
            <CheckCircleIcon class="w-6 h-6 text-green-600" />
          </div>
          <div>
            <p class="text-sm text-slate-500">Đã duyệt hôm nay</p>
            <p class="text-2xl font-bold text-slate-800">{{ approvedToday }}</p>
          </div>
        </div>
      </div>
      <div class="bg-white rounded-xl p-6 border border-slate-200 shadow-sm">
        <div class="flex items-center gap-4">
          <div class="p-3 bg-sky-100 rounded-lg">
            <BanknotesIcon class="w-6 h-6 text-sky-600" />
          </div>
          <div>
            <p class="text-sm text-slate-500">Tổng chờ duyệt</p>
            <p class="text-2xl font-bold text-slate-800">{{ formatCurrency(totalPending) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Pending Deposits Table -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div class="p-6 border-b border-slate-100">
        <h3 class="text-lg font-bold text-slate-800">Danh sách yêu cầu chờ duyệt</h3>
      </div>
      
      <div v-if="loading" class="p-8 text-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-sky-600 mx-auto"></div>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-sm text-left">
          <thead class="text-xs text-slate-500 uppercase bg-slate-50">
            <tr>
              <th class="px-6 py-3">ID</th>
              <th class="px-6 py-3">Hội viên</th>
              <th class="px-6 py-3">Số tiền</th>
              <th class="px-6 py-3">Thời gian yêu cầu</th>
              <th class="px-6 py-3">Mô tả</th>
              <th class="px-6 py-3 text-center">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="deposit in pendingDeposits" :key="deposit.id" class="border-b border-slate-100 hover:bg-slate-50">
              <td class="px-6 py-4 font-medium text-slate-800">#{{ deposit.id }}</td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 bg-sky-100 rounded-full flex items-center justify-center">
                    <span class="text-xs font-bold text-sky-600">{{ getInitials(deposit.memberName) }}</span>
                  </div>
                  <span class="font-medium text-slate-700">{{ deposit.memberName || 'N/A' }}</span>
                </div>
              </td>
              <td class="px-6 py-4 font-bold text-green-600">+{{ formatCurrency(deposit.amount) }}</td>
              <td class="px-6 py-4 text-slate-600">{{ formatDate(deposit.date) }}</td>
              <td class="px-6 py-4 text-slate-500 max-w-xs truncate">{{ deposit.description || 'Yêu cầu nạp tiền' }}</td>
              <td class="px-6 py-4">
                <div class="flex items-center justify-center gap-2">
                  <button @click="approveDeposit(deposit.id)" 
                          :disabled="processingId === deposit.id"
                          class="px-3 py-1.5 bg-green-100 text-green-700 rounded-lg hover:bg-green-200 font-medium text-sm transition-colors disabled:opacity-50">
                    <CheckIcon class="w-4 h-4 inline mr-1" />
                    Duyệt
                  </button>
                  <button @click="rejectDeposit(deposit.id)"
                          :disabled="processingId === deposit.id"
                          class="px-3 py-1.5 bg-red-100 text-red-700 rounded-lg hover:bg-red-200 font-medium text-sm transition-colors disabled:opacity-50">
                    <XMarkIcon class="w-4 h-4 inline mr-1" />
                    Từ chối
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="pendingDeposits.length === 0">
              <td colspan="6" class="px-6 py-12 text-center text-slate-500">
                <CheckCircleIcon class="w-12 h-12 mx-auto text-green-300 mb-3" />
                <p class="font-medium">Không có yêu cầu nào đang chờ duyệt</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useToast } from 'vue-toastification';
import axiosClient from '@/api/axiosClient';
import { ArrowPathIcon, ClockIcon, CheckCircleIcon, BanknotesIcon, CheckIcon, XMarkIcon } from '@heroicons/vue/24/outline';
import { useConfirmDialog } from '@/composables/useConfirmDialog';

const toast = useToast();
const { confirm: confirmDialog, confirmWarning } = useConfirmDialog();
const loading = ref(false);
const processingId = ref(null);
const pendingDeposits = ref([]);
const approvedToday = ref(0);

const totalPending = computed(() => {
  return pendingDeposits.value.reduce((sum, d) => sum + d.amount, 0);
});

const formatCurrency = (value) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value || 0);
};

const formatDate = (dateStr) => {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleString('vi-VN');
};

const getInitials = (name) => {
  if (!name) return '?';
  return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
};

const fetchPending = async () => {
  loading.value = true;
  try {
    const response = await axiosClient.get('/wallet/pending');
    if (response.data.success) {
      pendingDeposits.value = response.data.data || [];
    }
  } catch (error) {
    console.error('Error fetching pending deposits:', error);
    toast.error('Không thể tải danh sách yêu cầu');
  } finally {
    loading.value = false;
  }
};

const approveDeposit = async (id) => {
  const confirmed = await confirmDialog('Xác nhận duyệt yêu cầu nạp tiền này?', {
    title: 'Duyệt yêu cầu nạp tiền',
    type: 'success',
    confirmText: 'Duyệt'
  });
  if (!confirmed) return;
  
  processingId.value = id;
  try {
    const response = await axiosClient.post(`/wallet/approve/${id}`);
    if (response.data.success) {
      toast.success('Đã duyệt yêu cầu nạp tiền!');
      approvedToday.value++;
      await fetchPending();
    } else {
      toast.error(response.data.message || 'Duyệt thất bại');
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Có lỗi xảy ra');
  } finally {
    processingId.value = null;
  }
};

const rejectDeposit = async (id) => {
  const confirmed = await confirmWarning('Xác nhận từ chối yêu cầu nạp tiền này?', {
    title: 'Từ chối yêu cầu nạp tiền',
    confirmText: 'Từ chối'
  });
  if (!confirmed) return;
  
  processingId.value = id;
  try {
    const response = await axiosClient.post(`/wallet/reject/${id}`);
    if (response.data.success) {
      toast.success('Đã từ chối yêu cầu nạp tiền');
      await fetchPending();
    } else {
      toast.error(response.data.message || 'Từ chối thất bại');
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Có lỗi xảy ra');
  } finally {
    processingId.value = null;
  }
};

onMounted(() => {
  fetchPending();
});
</script>
