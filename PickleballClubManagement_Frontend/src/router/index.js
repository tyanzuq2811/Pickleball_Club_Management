import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import Login from '@/views/auth/Login.vue'
import MainLayout from '@/components/layout/MainLayout.vue'
import DashboardRouter from '@/views/DashboardRouter.vue'
import BookingCalendar from '@/views/bookings/BookingCalendar.vue'
import BookingManagement from '@/views/bookings/BookingManagement.vue'
import TournamentList from '@/views/tournaments/TournamentList.vue'
import TournamentBracket from '@/views/tournaments/TournamentBracket.vue'
import MyWallet from '@/views/wallet/MyWallet.vue'
import TransactionManagement from '@/views/treasury/TransactionManagement.vue'
import MatchList from '@/views/referee/MatchList.vue'
import MatchManagement from '@/views/referee/MatchManagement.vue'
import MemberList from '@/views/members/MemberList.vue'
import CourtList from '@/views/courts/CourtList.vue'
import NewsList from '@/views/news/NewsList.vue'
import MyBookings from '@/views/bookings/MyBookings.vue'
import LiveScoreboard from '@/views/public/LiveScoreboard.vue'
import ActivityLogs from '@/views/admin/ActivityLogs.vue'
import AdvancedDashboard from '@/views/admin/AdvancedDashboard.vue'

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
      path: '/scoreboard',
      name: 'scoreboard',
      component: LiveScoreboard
    },
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          name: 'dashboard',
          component: DashboardRouter
        },
        {
          path: 'bookings',
          name: 'bookings',
          component: BookingCalendar,
          meta: { roles: ['Member'] }
        },
        {
          path: 'my-bookings',
          name: 'my-bookings',
          component: MyBookings,
          meta: { roles: ['Member'] }
        },
        {
          path: 'bookings/manage',
          name: 'bookings-manage',
          component: BookingManagement,
          meta: { roles: ['Admin', 'Treasurer'] }
        },
        {
          path: 'tournaments',
          name: 'tournaments',
          component: TournamentList,
          meta: { roles: ['Member'] }
        },
        {
          path: 'tournaments/:id/bracket',
          name: 'tournament-bracket',
          component: TournamentBracket,
          meta: { roles: ['Member'] }
        },
        {
          path: 'wallet',
          name: 'wallet',
          component: MyWallet,
          meta: { roles: ['Member'] }
        },
        {
          path: 'treasury',
          name: 'treasury',
          component: TransactionManagement,
          meta: { roles: ['Treasurer'] }
        },
        {
          path: 'referee',
          name: 'referee',
          component: MatchList,
          meta: { roles: ['Referee'] }
        },
        {
          path: 'referee/manage',
          name: 'referee-manage',
          component: MatchManagement,
          meta: { roles: ['Referee', 'Admin'] }
        },
        {
          path: 'members',
          name: 'members',
          component: MemberList,
          meta: { roles: ['Admin', 'Treasurer'] }
        },
        {
          path: 'courts',
          name: 'courts',
          component: CourtList,
          meta: { roles: ['Admin'] }
        },
        {
          path: 'news',
          name: 'news',
          component: NewsList,
          meta: { roles: ['Admin'] }
        },
        {
          path: 'activity-logs',
          name: 'activity-logs',
          component: ActivityLogs,
          meta: { roles: ['Admin'] }
        },
        {
          path: 'admin/dashboard',
          name: 'admin-dashboard',
          component: AdvancedDashboard,
          meta: { roles: ['Admin', 'Treasurer'] }
        }
      ]
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  
  // Chặn người chưa đăng nhập
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next('/login');
  }
  
  // Chặn người đã đăng nhập vào trang login
  if (to.meta.guest && authStore.isAuthenticated) {
    return next('/');
  }
  
  // Kiểm tra quyền truy cập
  if (to.meta.roles && to.meta.roles.length > 0) {
    const userRole = authStore.role;
    if (!to.meta.roles.includes(userRole)) {
      console.warn(`Access denied: User role '${userRole}' not allowed for route '${to.path}'`);
      return next('/'); // Redirect về dashboard
    }
  }
  
  next();
});

export default router
