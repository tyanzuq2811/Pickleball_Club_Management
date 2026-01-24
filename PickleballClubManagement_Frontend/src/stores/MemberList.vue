<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Danh sách Hội viên</h2>
      <div class="flex space-x-2">
        <input type="text" placeholder="Tìm kiếm..." class="border border-slate-300 rounded-lg px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-sky-500">
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
          </tr>
        </thead>
        <tbody>
          <tr v-for="member in memberStore.members" :key="member.id" class="border-b border-slate-100 hover:bg-slate-50">
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
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useMemberStore } from '@/stores/member';

const memberStore = useMemberStore();

onMounted(() => {
  memberStore.fetchMembers();
});

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
</script>