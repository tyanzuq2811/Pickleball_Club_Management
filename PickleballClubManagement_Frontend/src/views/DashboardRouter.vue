<template>
  <component :is="dashboardComponent" />
</template>

<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import AdminDashboard from './dashboard/AdminDashboard.vue'
import TreasurerDashboard from './dashboard/TreasurerDashboard.vue'
import RefereeDashboard from './dashboard/RefereeDashboard.vue'
import MemberDashboard from './dashboard/MemberDashboard.vue'

const authStore = useAuthStore()

const dashboardComponent = computed(() => {
  if (authStore.isAdmin) return AdminDashboard
  if (authStore.isTreasurer) return TreasurerDashboard
  if (authStore.isReferee) return RefereeDashboard
  if (authStore.isMember) return MemberDashboard
  return MemberDashboard // Default
})
</script>
