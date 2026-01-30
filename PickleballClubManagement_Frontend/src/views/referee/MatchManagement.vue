<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-slate-800">Quản Lý Trận Đấu</h2>
      <button v-if="authStore.isReferee || authStore.isAdmin" @click="showCreateModal = true" 
              class="px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 transition-colors font-medium">
        + Tạo trận đấu
      </button>
    </div>

    <!-- Search & Filter -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Tìm kiếm</label>
          <input v-model="searchQuery" type="text" placeholder="Tên người chơi, giải đấu..."
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Trạng thái</label>
          <select v-model="filters.status" class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            <option :value="null">Tất cả</option>
            <option value="Scheduled">Chờ thi đấu</option>
            <option value="Completed">Hoàn thành</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Loại trận</label>
          <select v-model="filters.matchType" class="w-full px-4 py-2 border border-slate-300 rounded-lg">
            <option :value="null">Tất cả</option>
            <option value="ranked">Ranked (Tính ELO)</option>
            <option value="friendly">Giao hữu</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">Ngày</label>
          <input v-model="filters.date" type="date" 
                 class="w-full px-4 py-2 border border-slate-300 rounded-lg">
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-500">Tổng trận</p>
            <p class="text-2xl font-bold text-slate-800">{{ matches.length }}</p>
          </div>
          <div class="w-10 h-10 bg-sky-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-sky-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
            </svg>
          </div>
        </div>
      </div>
      <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-500">Đã hoàn thành</p>
            <p class="text-2xl font-bold text-green-600">{{ completedMatches }}</p>
          </div>
          <div class="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>
      <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-500">Chờ thi đấu</p>
            <p class="text-2xl font-bold text-yellow-600">{{ scheduledMatches }}</p>
          </div>
          <div class="w-10 h-10 bg-yellow-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>
      <div class="bg-white rounded-lg shadow-sm border border-slate-200 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-500">Trận Ranked</p>
            <p class="text-2xl font-bold text-purple-600">{{ rankedMatches }}</p>
          </div>
          <div class="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"/>
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Matches Table -->
    <div class="bg-white rounded-lg shadow-sm border border-slate-200 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th class="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">ID</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">Đội 1</th>
              <th class="px-4 py-3 text-center text-xs font-medium text-slate-500 uppercase">VS</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">Đội 2</th>
              <th class="px-4 py-3 text-center text-xs font-medium text-slate-500 uppercase">Tỷ số</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">Ngày</th>
              <th class="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">Giải đấu</th>
              <th class="px-4 py-3 text-center text-xs font-medium text-slate-500 uppercase">Loại</th>
              <th class="px-4 py-3 text-center text-xs font-medium text-slate-500 uppercase">Trạng thái</th>
              <th class="px-4 py-3 text-center text-xs font-medium text-slate-500 uppercase">Hành động</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-slate-200">
            <tr v-for="match in filteredMatches" :key="match.id" class="hover:bg-slate-50">
              <td class="px-4 py-4 text-sm text-slate-500">#{{ match.id }}</td>
              <td class="px-4 py-4">
                <div class="flex items-center">
                  <div :class="{'bg-green-100 border-green-400': match.winningSide === 1}" 
                       class="w-8 h-8 rounded-full bg-sky-100 border-2 border-sky-300 flex items-center justify-center mr-3">
                    <span class="text-xs font-bold text-sky-700">{{ getInitials(match.team1Player1Name) }}</span>
                  </div>
                  <div>
                    <p class="font-medium text-slate-800 text-sm">{{ match.team1Player1Name || 'TBD' }}</p>
                    <p v-if="match.team1Player2Name" class="text-xs text-slate-500">& {{ match.team1Player2Name }}</p>
                  </div>
                </div>
              </td>
              <td class="px-4 py-4 text-center">
                <span class="text-xs font-bold text-slate-400">VS</span>
              </td>
              <td class="px-4 py-4">
                <div class="flex items-center">
                  <div :class="{'bg-green-100 border-green-400': match.winningSide === 2}"
                       class="w-8 h-8 rounded-full bg-orange-100 border-2 border-orange-300 flex items-center justify-center mr-3">
                    <span class="text-xs font-bold text-orange-700">{{ getInitials(match.team2Player1Name) }}</span>
                  </div>
                  <div>
                    <p class="font-medium text-slate-800 text-sm">{{ match.team2Player1Name || 'TBD' }}</p>
                    <p v-if="match.team2Player2Name" class="text-xs text-slate-500">& {{ match.team2Player2Name }}</p>
                  </div>
                </div>
              </td>
              <td class="px-4 py-4 text-center">
                <div class="inline-flex items-center gap-1">
                  <span class="text-lg font-bold" :class="match.winningSide === 1 ? 'text-green-600' : 'text-slate-700'">
                    {{ match.team1Score || 0 }}
                  </span>
                  <span class="text-slate-400">-</span>
                  <span class="text-lg font-bold" :class="match.winningSide === 2 ? 'text-green-600' : 'text-slate-700'">
                    {{ match.team2Score || 0 }}
                  </span>
                </div>
              </td>
              <td class="px-4 py-4 text-sm text-slate-600">{{ formatDate(match.date) }}</td>
              <td class="px-4 py-4">
                <span v-if="match.tournamentTitle" class="text-xs px-2 py-1 bg-purple-100 text-purple-700 rounded-full">
                  {{ match.tournamentTitle }}
                </span>
                <span v-else class="text-xs text-slate-400">Giao hữu</span>
              </td>
              <td class="px-4 py-4 text-center">
                <span v-if="match.isRanked" class="inline-flex items-center gap-1 text-xs px-2 py-1 bg-amber-100 text-amber-700 rounded-full">
                  <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
                  </svg>
                  Ranked
                </span>
                <span v-else class="text-xs px-2 py-1 bg-slate-100 text-slate-500 rounded-full">Normal</span>
              </td>
              <td class="px-4 py-4">
                <span class="px-2 py-1 text-xs font-semibold rounded-full"
                      :class="match.status === 'Completed' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'">
                  {{ match.status === 'Completed' ? 'Hoàn thành' : 'Chờ đấu' }}
                </span>
              </td>
              <td class="px-4 py-4 text-center">
                <div class="flex items-center justify-center gap-2">
                  <button v-if="authStore.isReferee || authStore.isAdmin" @click="editMatch(match)" 
                          class="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-600 hover:bg-amber-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md" 
                          title="Chỉnh sửa">
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                    Sửa
                  </button>
                  <button v-if="(authStore.isReferee || authStore.isAdmin) && match.status !== 'Completed'" @click="openScoreModal(match)"
                          class="inline-flex items-center gap-1 px-3 py-1.5 bg-green-50 text-green-600 hover:bg-green-100 rounded-lg text-xs font-medium transition-all duration-200 hover:shadow-md"
                          title="Cập nhật tỷ số">
                    <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                    </svg>
                    Điểm
                  </button>
                  <button v-if="authStore.isAdmin" @click="confirmDelete(match.id)" 
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
            <tr v-if="filteredMatches.length === 0">
              <td colspan="10" class="px-4 py-8 text-center text-slate-500">
                <svg class="w-12 h-12 mx-auto text-slate-300 mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/>
                </svg>
                Không có trận đấu nào
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Create/Edit Match Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-lg w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-6">
            {{ showCreateModal ? 'Tạo Trận Đấu Mới' : 'Chỉnh Sửa Trận Đấu' }}
          </h3>
          
          <form @submit.prevent="handleSubmit" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Giải đấu</label>
              <select v-model="matchForm.tournamentId" 
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                <option :value="null">Trận giao hữu</option>
                <option v-for="t in tournaments" :key="t.id" :value="t.id">{{ t.title }}</option>
              </select>
            </div>

            <div class="bg-sky-50 p-4 rounded-lg space-y-3">
              <h4 class="font-semibold text-sky-800 flex items-center gap-2">
                <span class="w-6 h-6 bg-sky-200 rounded-full flex items-center justify-center text-xs font-bold">1</span>
                Đội 1
              </h4>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-slate-600 mb-1">Người chơi 1 *</label>
                  <select v-model.number="matchForm.team1Player1Id" required
                          class="w-full px-3 py-2 border border-slate-300 rounded-lg text-sm focus:ring-2 focus:ring-sky-500">
                    <option :value="null">Chọn người chơi</option>
                    <option v-for="m in members" :key="m.id" :value="m.id">{{ m.fullName }} ({{ m.rankELO }})</option>
                  </select>
                </div>
                <div>
                  <label class="block text-xs font-medium text-slate-600 mb-1">Người chơi 2 (đôi)</label>
                  <select v-model.number="matchForm.team1Player2Id"
                          class="w-full px-3 py-2 border border-slate-300 rounded-lg text-sm focus:ring-2 focus:ring-sky-500">
                    <option :value="null">Không có</option>
                    <option v-for="m in members" :key="m.id" :value="m.id">{{ m.fullName }} ({{ m.rankELO }})</option>
                  </select>
                </div>
              </div>
            </div>

            <div class="bg-orange-50 p-4 rounded-lg space-y-3">
              <h4 class="font-semibold text-orange-800 flex items-center gap-2">
                <span class="w-6 h-6 bg-orange-200 rounded-full flex items-center justify-center text-xs font-bold">2</span>
                Đội 2
              </h4>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-slate-600 mb-1">Người chơi 1 *</label>
                  <select v-model.number="matchForm.team2Player1Id" required
                          class="w-full px-3 py-2 border border-slate-300 rounded-lg text-sm focus:ring-2 focus:ring-sky-500">
                    <option :value="null">Chọn người chơi</option>
                    <option v-for="m in members" :key="m.id" :value="m.id">{{ m.fullName }} ({{ m.rankELO }})</option>
                  </select>
                </div>
                <div>
                  <label class="block text-xs font-medium text-slate-600 mb-1">Người chơi 2 (đôi)</label>
                  <select v-model.number="matchForm.team2Player2Id"
                          class="w-full px-3 py-2 border border-slate-300 rounded-lg text-sm focus:ring-2 focus:ring-sky-500">
                    <option :value="null">Không có</option>
                    <option v-for="m in members" :key="m.id" :value="m.id">{{ m.fullName }} ({{ m.rankELO }})</option>
                  </select>
                </div>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Ngày thi đấu</label>
                <input v-model="matchForm.matchDate" type="datetime-local" required
                       class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-2">Định dạng</label>
                <select v-model.number="matchForm.matchFormat" required
                        class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-sky-500">
                  <option :value="0">Đơn (Singles)</option>
                  <option :value="1">Đôi (Doubles)</option>
                </select>
              </div>
            </div>

            <div class="flex items-center p-3 bg-amber-50 rounded-lg">
              <input v-model="matchForm.isRanked" type="checkbox" id="isRanked"
                     class="w-5 h-5 text-amber-600 border-slate-300 rounded focus:ring-amber-500">
              <label for="isRanked" class="ml-3 text-sm font-medium text-amber-800">
                Trận Ranked (tính vào bảng xếp hạng ELO)
              </label>
            </div>

            <div class="flex space-x-3 pt-4">
              <button type="button" @click="closeModal"
                      class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
                Hủy
              </button>
              <button type="submit" 
                      class="flex-1 px-4 py-2 bg-sky-600 text-white rounded-lg hover:bg-sky-700 font-medium">
                {{ showCreateModal ? 'Tạo trận đấu' : 'Cập nhật' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Score Update Modal -->
    <div v-if="showScoreModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
        <div class="p-6">
          <h3 class="text-xl font-bold text-slate-800 mb-2">Cập Nhật Tỷ Số</h3>
          <p class="text-sm text-slate-500 mb-6">{{ selectedMatch?.team1Name }} vs {{ selectedMatch?.team2Name }}</p>
          
          <div class="flex items-center justify-center gap-4 mb-6">
            <div class="text-center">
              <label class="block text-sm font-medium text-sky-700 mb-2">Đội 1</label>
              <input v-model.number="scoreForm.team1Score" type="number" min="0" 
                     class="w-20 h-16 text-center text-3xl font-bold border-2 border-sky-300 rounded-lg focus:ring-2 focus:ring-sky-500">
            </div>
            <span class="text-3xl font-bold text-slate-300 mt-6">-</span>
            <div class="text-center">
              <label class="block text-sm font-medium text-orange-700 mb-2">Đội 2</label>
              <input v-model.number="scoreForm.team2Score" type="number" min="0"
                     class="w-20 h-16 text-center text-3xl font-bold border-2 border-orange-300 rounded-lg focus:ring-2 focus:ring-orange-500">
            </div>
          </div>

          <div class="mb-6">
            <label class="block text-sm font-medium text-slate-700 mb-2">Đội thắng</label>
            <div class="grid grid-cols-3 gap-2">
              <button type="button" @click="scoreForm.winningSide = 0"
                      :class="scoreForm.winningSide === 0 ? 'bg-slate-600 text-white' : 'bg-slate-100 text-slate-700'"
                      class="py-2 px-3 rounded-lg text-sm font-medium transition-colors">
                Chưa xác định
              </button>
              <button type="button" @click="scoreForm.winningSide = 1"
                      :class="scoreForm.winningSide === 1 ? 'bg-sky-600 text-white' : 'bg-sky-100 text-sky-700'"
                      class="py-2 px-3 rounded-lg text-sm font-medium transition-colors">
                Đội 1 thắng
              </button>
              <button type="button" @click="scoreForm.winningSide = 2"
                      :class="scoreForm.winningSide === 2 ? 'bg-orange-600 text-white' : 'bg-orange-100 text-orange-700'"
                      class="py-2 px-3 rounded-lg text-sm font-medium transition-colors">
                Đội 2 thắng
              </button>
            </div>
          </div>

          <div class="flex space-x-3">
            <button type="button" @click="showScoreModal = false"
                    class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 font-medium">
              Hủy
            </button>
            <button @click="submitScore"
                    class="flex-1 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 font-medium">
              Lưu tỷ số
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { format, parseISO } from 'date-fns';
import { useToast } from 'vue-toastification';
import { useConfirmDialog } from '@/composables/useConfirmDialog';
import axiosClient from '@/api/axiosClient';

const authStore = useAuthStore();
const toast = useToast();
const { confirmDelete: confirmDeleteDialog } = useConfirmDialog();

const searchQuery = ref('');
const filters = ref({
  status: null,
  matchType: null,
  date: ''
});

const matches = ref([]);
const members = ref([]);
const tournaments = ref([]);
const showCreateModal = ref(false);
const showEditModal = ref(false);
const showScoreModal = ref(false);
const selectedMatch = ref(null);

const matchForm = ref({
  tournamentId: null,
  team1Player1Id: null,
  team1Player2Id: null,
  team2Player1Id: null,
  team2Player2Id: null,
  matchDate: '',
  matchFormat: 1,
  isRanked: true
});

const scoreForm = ref({
  team1Score: 0,
  team2Score: 0,
  winningSide: 0
});

// Computed stats
const completedMatches = computed(() => matches.value.filter(m => m.status === 'Completed').length);
const scheduledMatches = computed(() => matches.value.filter(m => m.status === 'Scheduled').length);
const rankedMatches = computed(() => matches.value.filter(m => m.isRanked).length);

const filteredMatches = computed(() => {
  let result = matches.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(m => 
      m.team1Name?.toLowerCase().includes(query) ||
      m.team2Name?.toLowerCase().includes(query) ||
      m.team1Player1Name?.toLowerCase().includes(query) ||
      m.team2Player1Name?.toLowerCase().includes(query) ||
      m.tournamentTitle?.toLowerCase().includes(query)
    );
  }

  if (filters.value.status) {
    result = result.filter(m => m.status === filters.value.status);
  }

  if (filters.value.matchType === 'ranked') {
    result = result.filter(m => m.isRanked);
  } else if (filters.value.matchType === 'friendly') {
    result = result.filter(m => !m.isRanked);
  }

  if (filters.value.date) {
    result = result.filter(m => {
      const matchDate = format(parseISO(m.date), 'yyyy-MM-dd');
      return matchDate === filters.value.date;
    });
  }

  return result;
});

const fetchMatches = async () => {
  try {
    const response = await axiosClient.get('/matches?pageSize=100');
    if (response.data.success) {
      matches.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching matches:', error);
  }
};

const fetchMembers = async () => {
  try {
    const response = await axiosClient.get('/members?pageSize=100');
    if (response.data.success) {
      members.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching members:', error);
  }
};

const fetchTournaments = async () => {
  try {
    const response = await axiosClient.get('/tournaments?pageSize=100');
    if (response.data.success) {
      tournaments.value = response.data.data.items || [];
    }
  } catch (error) {
    console.error('Error fetching tournaments:', error);
  }
};

onMounted(() => {
  fetchMatches();
  fetchMembers();
  fetchTournaments();
});

const getInitials = (name) => {
  if (!name) return '?';
  return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
};

const editMatch = (match) => {
  selectedMatch.value = match;
  matchForm.value = {
    tournamentId: match.tournamentId,
    team1Player1Id: match.team1Player1Id,
    team1Player2Id: match.team1Player2Id,
    team2Player1Id: match.team2Player1Id,
    team2Player2Id: match.team2Player2Id,
    matchDate: format(parseISO(match.date), "yyyy-MM-dd'T'HH:mm"),
    matchFormat: match.matchFormat === 'Singles' ? 0 : 1,
    isRanked: match.isRanked || false
  };
  showEditModal.value = true;
};

const openScoreModal = (match) => {
  selectedMatch.value = match;
  scoreForm.value = {
    team1Score: match.team1Score || 0,
    team2Score: match.team2Score || 0,
    winningSide: match.winningSide || 0
  };
  showScoreModal.value = true;
};

const submitScore = async () => {
  try {
    const response = await axiosClient.put(`/matches/${selectedMatch.value.id}/score`, scoreForm.value);
    if (response.data.success) {
      toast.success('Cập nhật tỷ số thành công!');
      showScoreModal.value = false;
      fetchMatches();
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Lỗi khi cập nhật tỷ số');
  }
};

const handleSubmit = async () => {
  try {
    const data = {
      ...matchForm.value,
      matchDate: new Date(matchForm.value.matchDate).toISOString()
    };

    if (showCreateModal.value) {
      const response = await axiosClient.post('/matches', data);
      if (response.data.success) {
        toast.success('Tạo trận đấu thành công!');
        closeModal();
        fetchMatches();
      }
    } else {
      const response = await axiosClient.put(`/matches/${selectedMatch.value.id}`, data);
      if (response.data.success) {
        toast.success('Cập nhật trận đấu thành công!');
        closeModal();
        fetchMatches();
      }
    }
  } catch (error) {
    toast.error(error.response?.data?.message || 'Lỗi khi xử lý trận đấu');
  }
};

const confirmDelete = async (matchId) => {
  const confirmed = await confirmDeleteDialog('Bạn có chắc chắn muốn xóa trận đấu này?');
  if (confirmed) {
    try {
      const response = await axiosClient.delete(`/matches/${matchId}`);
      if (response.data.success) {
        toast.success('Xóa trận đấu thành công!');
        fetchMatches();
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Lỗi khi xóa trận đấu');
    }
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  showScoreModal.value = false;
  selectedMatch.value = null;
  matchForm.value = {
    tournamentId: null,
    team1Player1Id: null,
    team1Player2Id: null,
    team2Player1Id: null,
    team2Player2Id: null,
    matchDate: '',
    matchFormat: 1,
    isRanked: true
  };
};

const formatDate = (dateStr) => {
  try {
    return format(parseISO(dateStr), 'dd/MM/yyyy HH:mm');
  } catch {
    return dateStr;
  }
};
</script>
