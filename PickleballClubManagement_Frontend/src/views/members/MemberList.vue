<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Danh sách Hội viên</h2>
      <div class="flex space-x-2">
        <input v-model="searchQuery" @input="handleSearch" type="text" placeholder="Tìm kiếm theo tên, email..." 
               class="border border-slate-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-sky-500 w-64">
        <button @click="showCreateModal = true" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors font-medium text-sm">
          + Thêm hội viên
        </button>
      </div>
    </div>

    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
      <div v-if="memberStore.loading" class="p-8 text-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-sky-600 mx-auto"></div>
      </div>

      <table v-else class="w-full text-sm text-left">
        <thead class="text-xs text-slate-500 uppercase bg-slate-50">
          <tr>
            <th class="px-6 py-3">Họ tên</th>
            <th class="px-6 py-3">Email</th>
            <th class="px-6 py-3">SĐT</th>
            <th class="px-6 py-3 text-center">Rank ELO</th>
            <th class="px-6 py-3 text-right">Số dư ví</th>
            <th class="px-6 py-3 text-center">Trạng thái</th>
            <th class="px-6 py-3 text-center">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="member in filteredMembers" :key="member.id" class="border-b border-slate-100 hover:bg-slate-50">
            <td class="px-6 py-4 font-medium text-slate-900">{{ member.fullName }}</td>
            <td class="px-6 py-4 text-slate-600">{{ member.email }}</td>
            <td class="px-6 py-4 text-slate-600">{{ member.phoneNumber || '-' }}</td>
            <td class="px-6 py-4 text-center">
              <span class="bg-sky-100 text-sky-800 text-xs font-bold px-2.5 py-0.5 rounded">{{ member.rankELO }}</span>
            </td>
            <td class="px-6 py-4 text-right font-medium text-slate-700">
              {{ formatCurrency(member.walletBalance) }}
            </td>
            <td class="px-6 py-4 text-center">
              <span v-if="member.isActive" class="text-green-600 bg-green-100 px-2 py-1 rounded-full text-xs">Hoạt động</span>
              <span v-else class="text-red-600 bg-red-100 px-2 py-1 rounded-full text-xs">Khóa</span>
            </td>
            <td class="px-6 py-4 text-center">
              <div class="flex items-center justify-center gap-2">
                <button @click="viewMember(member)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-blue-50 text-blue-600 hover:bg-blue-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Xem chi tiết">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                  </svg>
                  Xem
                </button>
                <button @click="editMember(member)" 
                        class="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                        title="Chỉnh sửa">
                  <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                  </svg>
                  Sửa
                </button>
                <button @click="deleteMember(member.id)" 
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
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Member Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">{{ showCreateModal ? 'Thêm Hội Viên Mới' : 'Chỉnh Sửa Hội Viên' }}</h3>
          
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Họ và tên</label>
              <input v-model="formData.fullName" type="text" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Email</label>
              <input v-model="formData.email" type="email" required
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số điện thoại</label>
              <input v-model="formData.phoneNumber" type="tel"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Rank ELO</label>
              <input v-model.number="formData.rankELO" type="number" min="0" step="0.1"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Số dư ví</label>
              <input v-model.number="formData.walletBalance" type="number" min="0" step="any"
                     class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>

            <div class="flex items-center">
              <input v-model="formData.isActive" type="checkbox" class="rounded mr-2">
              <label class="text-sm font-medium text-slate-700">Kích hoạt tài khoản</label>
            </div>

            <div class="flex justify-end space-x-2 pt-4">
              <button type="button" @click="closeModal" 
                      class="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-lg transition-colors">
                Hủy
              </button>
              <button type="submit" class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors font-medium">
                {{ showCreateModal ? 'Thêm' : 'Cập nhật' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- View Member Detail Modal -->
    <div v-if="showDetailModal && selectedMember" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">Chi tiết Hội viên</h3>
          
          <div class="grid grid-cols-2 gap-4 mb-6">
            <div>
              <p class="text-sm text-slate-500">Họ tên</p>
              <p class="font-semibold text-slate-800">{{ selectedMember.fullName }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Email</p>
              <p class="font-semibold text-slate-800">{{ selectedMember.email }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Số điện thoại</p>
              <p class="font-semibold text-slate-800">{{ selectedMember.phoneNumber || '-' }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Rank ELO</p>
              <p class="font-semibold text-sky-600 text-lg">{{ selectedMember.rankELO }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Số dư ví</p>
              <p class="font-semibold text-green-600 text-lg">{{ formatCurrency(selectedMember.walletBalance) }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Tổng trận đấu</p>
              <p class="font-semibold text-slate-800">{{ selectedMember.totalMatches || 0 }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Trận thắng</p>
              <p class="font-semibold text-slate-800">{{ selectedMember.winMatches || 0 }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-500">Tỷ lệ thắng</p>
              <p class="font-semibold text-slate-800">{{ calculateWinRate(selectedMember) }}%</p>
            </div>
          </div>

          <button @click="showDetailModal = false" class="w-full px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
            Đóng
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useMemberStore } from '@/stores/member';
import { useToast } from 'vue-toastification';

const memberStore = useMemberStore();
const toast = useToast();
const searchQuery = ref('');
const showCreateModal = ref(false);
const showEditModal = ref(false);
const showDetailModal = ref(false);
const selectedMember = ref(null);

const formData = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  rankELO: 1200,
  walletBalance: 0,
  isActive: true
});

const filteredMembers = computed(() => {
  if (!searchQuery.value) return memberStore.members;
  const query = searchQuery.value.toLowerCase();
  return memberStore.members.filter(m => 
    m.fullName?.toLowerCase().includes(query) || 
    m.email?.toLowerCase().includes(query)
  );
});

onMounted(() => {
  memberStore.fetchMembers();
});

const handleSearch = () => {
  // Search is reactive via computed property
};

const viewMember = (member) => {
  selectedMember.value = member;
  showDetailModal.value = true;
};

const editMember = (member) => {
  formData.value = { ...member };
  showEditModal.value = true;
};

const deleteMember = async (id) => {
  if (confirm('Xác nhận xóa hội viên này?')) {
    const success = await memberStore.deleteMember(id);
    if (success) {
      toast.success('Xóa hội viên thành công!');
    }
  }
};

const handleSubmit = async () => {
  if (showCreateModal.value) {
    const success = await memberStore.createMember(formData.value);
    if (success) {
      toast.success('Thêm hội viên thành công!');
      await memberStore.fetchMembers(); // Refresh danh sách
      closeModal();
    }
  } else {
    const success = await memberStore.updateMember(formData.value.id, formData.value);
    if (success) {
      toast.success('Cập nhật hội viên thành công!');
      await memberStore.fetchMembers(); // Refresh danh sách
      closeModal();
    }
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  formData.value = {
    fullName: '',
    email: '',
    phoneNumber: '',
    rankELO: 1200,
    walletBalance: 0,
    isActive: true
  };
};

const calculateWinRate = (member) => {
  if (!member.totalMatches || member.totalMatches === 0) return 0;
  return ((member.winMatches || 0) / member.totalMatches * 100).toFixed(1);
};

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
</script>
