<template>
  <div class="flex h-screen bg-slate-50">
    <!-- Sidebar -->
    <aside class="w-64 bg-white border-r border-slate-200 hidden md:flex flex-col">
      <div class="h-16 flex items-center px-6 border-b border-slate-100">
        <span class="text-xl font-bold text-sky-600 tracking-tight">PCM System</span>
      </div>
      
      <nav class="flex-1 px-4 py-6 space-y-1 overflow-y-auto">
        <router-link to="/" class="flex items-center px-4 py-3 text-slate-600 rounded-lg hover:bg-sky-50 hover:text-sky-600 transition-colors group" active-class="bg-sky-50 text-sky-600 font-medium">
            <HomeIcon class="w-5 h-5 mr-3 group-hover:text-sky-600" />
            Dashboard
        </router-link>
        
        <div class="pt-4 pb-2">
            <p class="px-4 text-xs font-semibold text-slate-400 uppercase tracking-wider">Quản lý</p>
        </div>

        <router-link to="/bookings" class="flex items-center px-4 py-3 text-slate-600 rounded-lg hover:bg-sky-50 hover:text-sky-600 transition-colors group" active-class="bg-sky-50 text-sky-600 font-medium">
            <CalendarIcon class="w-5 h-5 mr-3 group-hover:text-sky-600" />
            Đặt sân
        </router-link>

        <router-link to="/tournaments" class="flex items-center px-4 py-3 text-slate-600 rounded-lg hover:bg-sky-50 hover:text-sky-600 transition-colors group" active-class="bg-sky-50 text-sky-600 font-medium">
            <TrophyIcon class="w-5 h-5 mr-3 group-hover:text-sky-600" />
            Giải đấu
        </router-link>

        <router-link to="/wallet" class="flex items-center px-4 py-3 text-slate-600 rounded-lg hover:bg-sky-50 hover:text-sky-600 transition-colors group" active-class="bg-sky-50 text-sky-600 font-medium">
            <CreditCardIcon class="w-5 h-5 mr-3 group-hover:text-sky-600" />
            Ví của tôi
        </router-link>
      </nav>

      <div class="p-4 border-t border-slate-100">
        <button @click="authStore.logout" class="flex items-center w-full px-4 py-2 text-sm text-red-600 rounded-lg hover:bg-red-50 transition-colors">
            <ArrowRightOnRectangleIcon class="w-5 h-5 mr-3" />
            Đăng xuất
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <!-- Header -->
      <header class="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-6 shadow-sm z-10">
        <h1 class="text-lg font-semibold text-slate-800 capitalize">{{ $route.name }}</h1>
        
        <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-2">
                <span class="text-sm font-medium text-slate-700">{{ authStore.memberName }}</span>
                <img :src="authStore.avatarUrl" alt="Avatar" class="w-8 h-8 rounded-full border border-slate-200">
            </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="flex-1 overflow-y-auto p-6 bg-slate-50">
        <router-view v-slot="{ Component }">
            <transition name="fade" mode="out-in">
                <component :is="Component" />
            </transition>
        </router-view>
      </main>
    </div>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/stores/auth';
import { 
    HomeIcon, 
    CalendarIcon, 
    TrophyIcon, 
    UsersIcon, 
    ArrowRightOnRectangleIcon,
    CreditCardIcon
} from '@heroicons/vue/24/outline';

const authStore = useAuthStore();
</script>