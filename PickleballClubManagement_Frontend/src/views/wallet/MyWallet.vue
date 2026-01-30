<template>
  <div class="space-y-6">
    <h2 class="text-2xl font-bold text-slate-800">Ví Điện Tử</h2>

    <!-- Balance Card -->
    <div class="bg-gradient-to-r from-sky-600 to-sky-800 rounded-2xl p-8 text-white shadow-lg">
      <div class="flex justify-between items-start">
        <div>
          <p class="text-sky-100 font-medium mb-1">Số dư hiện tại</p>
          <h3 class="text-4xl font-bold">{{ formatCurrency(authStore.user?.walletBalance || 0) }}</h3>
        </div>
        <div class="bg-white/20 p-3 rounded-lg backdrop-blur-sm">
          <CreditCardIcon class="w-8 h-8 text-white" />
        </div>
      </div>
      <div class="mt-8 flex space-x-4">
        <button @click="showDepositModal = true" class="bg-white text-sky-700 px-6 py-2 rounded-lg font-bold hover:bg-sky-50 transition-colors shadow-sm">
          + Nạp tiền
        </button>
      </div>
    </div>

    <!-- Transactions History -->
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div class="p-6 border-b border-slate-100 bg-white">
        <h3 class="text-lg font-bold text-slate-800">Lịch sử giao dịch</h3>
      </div>
      
      <div v-if="walletStore.loading" class="p-8 text-center bg-white">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-sky-600 mx-auto"></div>
      </div>

      <div v-else class="overflow-x-auto bg-white">
        <table class="w-full text-sm text-left">
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
    </div>

    <!-- Enhanced Deposit Modal -->
    <div v-if="showDepositModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-xl max-w-2xl w-full shadow-2xl max-h-[90vh] overflow-y-auto">
        <div class="p-6 border-b border-slate-200">
          <h3 class="text-2xl font-bold text-slate-800">Nạp tiền vào ví</h3>
          <p class="text-sm text-slate-500 mt-1">Chọn phương thức thanh toán và nhập số tiền cần nạp</p>
        </div>

        <div class="p-6 space-y-6">
          <!-- Amount Input -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Số tiền cần nạp</label>
            <input v-model.number="depositAmount" type="number" step="10000" min="10000" required
                   class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500 text-lg font-semibold"
                   placeholder="Nhập số tiền (VNĐ)">
            <div class="flex gap-2 mt-3">
              <button v-for="amount in quickAmounts" :key="amount" @click="depositAmount = amount"
                      class="px-4 py-2 bg-slate-100 hover:bg-sky-50 hover:text-sky-700 rounded-lg text-sm font-medium transition-colors">
                {{ formatCurrency(amount) }}
              </button>
            </div>
          </div>

          <!-- Payment Method Selection -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-3">Phương thức thanh toán</label>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
              <button v-for="method in paymentMethods" :key="method.id" @click="selectedPayment = method.id"
                      class="p-4 border-2 rounded-lg transition-all hover:border-sky-500"
                      :class="selectedPayment === method.id ? 'border-sky-600 bg-sky-50' : 'border-slate-200'">
                <div class="text-2xl mb-2">{{ method.icon }}</div>
                <div class="font-semibold text-slate-800">{{ method.name }}</div>
                <div class="text-xs text-slate-500 mt-1">{{ method.desc }}</div>
              </button>
            </div>
          </div>

          <!-- Bank Transfer Info -->
          <div v-if="selectedPayment === 'bank'" class="bg-blue-50 rounded-lg p-5 border border-blue-200">
            <h4 class="font-bold text-blue-900 mb-3 flex items-center">
              <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path d="M4 4a2 2 0 00-2 2v1h16V6a2 2 0 00-2-2H4z"/>
                <path fill-rule="evenodd" d="M18 9H2v5a2 2 0 002 2h12a2 2 0 002-2V9zM4 13a1 1 0 011-1h1a1 1 0 110 2H5a1 1 0 01-1-1zm5-1a1 1 0 100 2h1a1 1 0 100-2H9z" clip-rule="evenodd"/>
              </svg>
              Thông tin chuyển khoản
            </h4>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between">
                <span class="text-blue-700">Ngân hàng:</span>
                <span class="font-bold text-blue-900">Vietcombank</span>
              </div>
              <div class="flex justify-between">
                <span class="text-blue-700">Số tài khoản:</span>
                <span class="font-mono font-bold text-blue-900">1234567890</span>
              </div>
              <div class="flex justify-between">
                <span class="text-blue-700">Chủ tài khoản:</span>
                <span class="font-bold text-blue-900">CLB PICKLEBALL HCM</span>
              </div>
              <div class="flex justify-between items-start">
                <span class="text-blue-700">Nội dung:</span>
                <span class="font-mono font-bold text-blue-900 text-right">NAP {{ authStore.user?.memberId }} {{ depositAmount }}</span>
              </div>
            </div>
            <div class="mt-4 p-3 bg-yellow-50 border border-yellow-200 rounded">
              <p class="text-xs text-yellow-800">
                ⚠️ <strong>Quan trọng:</strong> Vui lòng ghi ĐÚNG nội dung chuyển khoản để được duyệt nhanh chóng!
              </p>
            </div>
            
            <!-- QR Code cho chuyển khoản ngân hàng -->
            <div class="mt-4 text-center">
              <p class="text-sm font-medium text-blue-800 mb-2">Quét mã QR để chuyển khoản nhanh</p>
              <div class="bg-white p-3 rounded-lg inline-block shadow-sm border">
                <img :src="bankQRCodeUrl" 
                     alt="QR Code Chuyển khoản" 
                     class="w-48 h-48 mx-auto"
                     @error="handleQRError">
              </div>
              <p class="text-xs text-blue-600 mt-2">VietQR - Chuyển khoản liên ngân hàng 24/7</p>
            </div>
          </div>

          <!-- Momo/ZaloPay Info -->
          <div v-if="selectedPayment === 'momo' || selectedPayment === 'zalopay'" class="bg-purple-50 rounded-lg p-5 border border-purple-200">
            <h4 class="font-bold text-purple-900 mb-3">
              {{ selectedPayment === 'momo' ? '📱 Thanh toán qua Momo' : '💳 Thanh toán qua ZaloPay' }}
            </h4>
            <div class="text-center">
              <div class="bg-white p-3 rounded-lg inline-block mb-3 shadow-sm border">
                <img :src="eWalletQRCodeUrl" 
                     alt="QR Code Ví điện tử" 
                     class="w-48 h-48 mx-auto"
                     @error="handleQRError">
              </div>
              <p class="text-sm text-purple-800">Số điện thoại: <strong>0901234567</strong></p>
              <p class="text-sm text-purple-800">Tên: <strong>CLB Pickleball HCM</strong></p>
              <p class="text-xs text-purple-600 mt-2">Nội dung: <strong>NAP {{ authStore.user?.memberId }} {{ depositAmount }}</strong></p>
            </div>
          </div>

          <!-- Notes Input -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Ghi chú (tùy chọn)</label>
            <textarea v-model="depositNotes" rows="3" placeholder="Nhập ghi chú nếu có..."
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500"></textarea>
          </div>
        </div>

        <!-- Footer Actions -->
        <div class="p-6 bg-slate-50 border-t border-slate-200 flex justify-between items-center">
          <div class="text-left">
            <p class="text-xs text-slate-500">Tổng tiền nạp</p>
            <p class="text-2xl font-bold text-sky-700">{{ formatCurrency(depositAmount) }}</p>
          </div>
          <div class="flex space-x-3">
            <button @click="closeDepositModal" class="px-6 py-2.5 text-slate-700 hover:bg-slate-200 rounded-lg font-medium transition-colors">
              Hủy
            </button>
            <button @click="handleDeposit" :disabled="!selectedPayment || depositAmount < 10000"
                    class="px-6 py-2.5 bg-sky-600 text-white rounded-lg hover:bg-sky-700 font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed">
              Xác nhận nạp tiền
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useWalletStore } from '@/stores/wallet';
import { useAuthStore } from '@/stores/auth';
import { CreditCardIcon } from '@heroicons/vue/24/outline';
import { format } from 'date-fns';
import { useToast } from 'vue-toastification';

const walletStore = useWalletStore();
const authStore = useAuthStore();
const toast = useToast();
const showDepositModal = ref(false);
const depositAmount = ref(100000);
const selectedPayment = ref('bank');
const depositNotes = ref('');

const quickAmounts = [50000, 100000, 200000, 500000, 1000000];

const paymentMethods = [
  { id: 'bank', name: 'Chuyển khoản', icon: '🏦', desc: 'Ngân hàng nội địa' },
  { id: 'momo', name: 'Momo', icon: '📱', desc: 'Ví điện tử' },
  { id: 'zalopay', name: 'ZaloPay', icon: '💳', desc: 'Ví điện tử' }
];

// Thông tin ngân hàng (có thể cấu hình)
const bankInfo = {
  bankId: 'VCB', // Mã ngân hàng Vietcombank
  accountNo: '1234567890',
  accountName: 'CLB PICKLEBALL HCM',
  template: 'compact2' // compact, compact2, qr_only, print
};

// QR Code cho chuyển khoản ngân hàng (VietQR API)
const bankQRCodeUrl = computed(() => {
  const content = `NAP ${authStore.user?.memberId || 'MEMBER'} ${depositAmount.value}`;
  // Sử dụng VietQR API miễn phí
  return `https://img.vietqr.io/image/${bankInfo.bankId}-${bankInfo.accountNo}-${bankInfo.template}.png?amount=${depositAmount.value}&addInfo=${encodeURIComponent(content)}&accountName=${encodeURIComponent(bankInfo.accountName)}`;
});

// QR Code cho ví điện tử (Momo/ZaloPay)
const eWalletQRCodeUrl = computed(() => {
  const content = `NAP ${authStore.user?.memberId || 'MEMBER'} ${depositAmount.value}`;
  const phone = '0901234567';
  // Sử dụng QR code API cơ bản
  if (selectedPayment.value === 'momo') {
    // Momo deeplink QR
    const momoData = `2|99|${phone}|||0|0|${depositAmount.value}|${content}|transfer_myqr`;
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(momoData)}`;
  } else {
    // ZaloPay QR
    const zalopayData = `zalopay://qr/p2p?phone=${phone}&amount=${depositAmount.value}&note=${encodeURIComponent(content)}`;
    return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(zalopayData)}`;
  }
});

const handleQRError = (e) => {
  // Fallback nếu QR không load được
  e.target.src = `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent('CLB Pickleball HCM - Nap tien')}`;
};

onMounted(() => {
  walletStore.fetchTransactions();
});

const handleDeposit = async () => {
  if (depositAmount.value < 10000) {
    toast.warning('Số tiền nạp tối thiểu là 10,000 VNĐ');
    return;
  }
  
  const depositData = {
    amount: depositAmount.value,
    paymentMethod: selectedPayment.value,
    notes: depositNotes.value
  };
  
  const success = await walletStore.deposit(depositData.amount);
  if (success) {
    closeDepositModal();
  }
};

const closeDepositModal = () => {
  showDepositModal.value = false;
  depositAmount.value = 100000;
  selectedPayment.value = 'bank';
  depositNotes.value = '';
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