<template>
  <div class="space-y-6">
    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
            <div class="p-3 bg-blue-100 rounded-lg text-blue-600">
                <UsersIcon class="w-8 h-8" />
            </div>
            <div>
                <p class="text-sm text-slate-500 font-medium">Tổng hội viên</p>
                <p class="text-2xl font-bold text-slate-800">128</p>
            </div>
        </div>

        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
            <div class="p-3 bg-green-100 rounded-lg text-green-600">
                <CurrencyDollarIcon class="w-8 h-8" />
            </div>
            <div>
                <p class="text-sm text-slate-500 font-medium">Quỹ CLB</p>
                <p class="text-2xl font-bold text-slate-800">5,200,000 đ</p>
            </div>
        </div>

        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100 flex items-center space-x-4">
            <div class="p-3 bg-purple-100 rounded-lg text-purple-600">
                <TrophyIcon class="w-8 h-8" />
            </div>
            <div>
                <p class="text-sm text-slate-500 font-medium">Giải đấu đang chạy</p>
                <p class="text-2xl font-bold text-slate-800">2</p>
            </div>
        </div>
    </div>

    <!-- Main Content Area -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
            <h3 class="text-lg font-semibold text-slate-800 mb-4">Tin tức mới nhất</h3>
            <div class="space-y-4">
                <div class="p-4 bg-slate-50 rounded-lg border border-slate-100">
                    <span class="text-xs font-bold text-sky-600 bg-sky-50 px-2 py-1 rounded">Ghim</span>
                    <h4 class="font-medium text-slate-800 mt-1">Thông báo lịch nghỉ Tết Nguyên Đán</h4>
                    <p class="text-sm text-slate-500 mt-1">CLB sẽ nghỉ từ ngày 08/02 đến hết 14/02...</p>
                </div>
                <div class="p-4 bg-slate-50 rounded-lg border border-slate-100">
                    <h4 class="font-medium text-slate-800">Kết quả giải đấu tháng 1</h4>
                    <p class="text-sm text-slate-500 mt-1">Chúc mừng cặp đôi Nguyễn Văn A và Trần Thị B...</p>
                </div>
            </div>
        </div>

        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-100">
            <h3 class="text-lg font-semibold text-slate-800 mb-4">Top Ranking (ELO)</h3>
            <div class="overflow-x-auto">
                <table class="w-full text-sm text-left">
                    <thead class="text-xs text-slate-500 uppercase bg-slate-50">
                        <tr>
                            <th class="px-4 py-3">#</th>
                            <th class="px-4 py-3">Tên</th>
                            <th class="px-4 py-3 text-right">ELO</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr class="border-b border-slate-100 hover:bg-slate-50">
                            <td class="px-4 py-3 font-medium text-sky-600">1</td>
                            <td class="px-4 py-3">Nguyễn Văn A</td>
                            <td class="px-4 py-3 text-right font-bold">1520</td>
                        </tr>
                        <tr class="border-b border-slate-100 hover:bg-slate-50">
                            <td class="px-4 py-3 font-medium text-sky-600">2</td>
                            <td class="px-4 py-3">Lê Văn C</td>
                            <td class="px-4 py-3 text-right font-bold">1480</td>
                        </tr>
                        <tr class="border-b border-slate-100 hover:bg-slate-50">
                            <td class="px-4 py-3 font-medium text-sky-600">3</td>
                            <td class="px-4 py-3">Phạm Thị D</td>
                            <td class="px-4 py-3 text-right font-bold">1450</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
  </div>
</template>

<script setup>
import { UsersIcon, CurrencyDollarIcon, TrophyIcon } from '@heroicons/vue/24/outline';
</script>