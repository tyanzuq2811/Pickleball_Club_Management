import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import Login from '@/views/auth/Login.vue'
import MainLayout from '@/components/layout/MainLayout.vue'
import Dashboard from '@/views/Dashboard.vue'
import BookingCalendar from '@/views/bookings/BookingCalendar.vue'
import TournamentList from '@/views/tournaments/TournamentList.vue'
import TournamentBracket from '@/views/tournaments/TournamentBracket.vue'
import MyWallet from '@/views/wallet/MyWallet.vue'
import TransactionManagement from '@/views/treasury/TransactionManagement.vue'
import MatchList from '@/views/referee/MatchList.vue'
import MemberList from '@/views/members/MemberList.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: Login,
      meta: { guest: true }
    },
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          name: 'dashboard',
          component: Dashboard
        },
        {
          path: 'bookings',
          name: 'bookings',
          component: BookingCalendar
        },
        {
          path: 'tournaments',
          name: 'tournaments',
          component: TournamentList
        },
        {
          path: 'tournaments/:id/bracket',
          name: 'tournament-bracket',
          component: TournamentBracket
        },
        {
          path: 'wallet',
          name: 'wallet',
          component: MyWallet
        },
        {
          path: 'treasury',
          name: 'treasury',
          component: TransactionManagement
        },
        {
          path: 'referee',
          name: 'referee',
          component: MatchList
        },
        {
          path: 'members',
          name: 'members',
          component: MemberList
        }
      ]
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  if (to.meta.requiresAuth && !authStore.isAuthenticated) next('/login');
  else if (to.meta.guest && authStore.isAuthenticated) next('/');
  else next();
});

export default router