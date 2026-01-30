<template>
  <router-view />
  <ConfirmDialog />
</template>

<script setup>
import { onMounted, onUnmounted, watch } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useMatchStore } from '@/stores/match';
import { useBookingStore } from '@/stores/booking';
import { useNotificationStore } from '@/stores/notification';
import signalRService from '@/services/signalrService';
import { useToast } from 'vue-toastification';
import ConfirmDialog from '@/components/ConfirmDialog.vue';

const authStore = useAuthStore();
const matchStore = useMatchStore();
const bookingStore = useBookingStore();
const notificationStore = useNotificationStore();
const toast = useToast();

// Setup SignalR connection when authenticated
const setupSignalR = async () => {
  if (!authStore.isAuthenticated) return;

  try {
    await signalRService.startConnection(authStore.token);
    
    // Subscribe to score updates
    signalRService.onScoreUpdated((data) => {
      console.log('[SignalR] Score updated:', data);
      matchStore.fetchMatches(); // Auto-refresh matches
      toast.info(`Tỉ số cập nhật: ${data.team1Name} ${data.team1Score} - ${data.team2Score} ${data.team2Name}`);
    });

    // Subscribe to match finished
    signalRService.onMatchFinished((data) => {
      console.log('[SignalR] Match finished:', data);
      matchStore.fetchMatches();
      toast.success(`Trận đấu kết thúc: ${data.team1Name} vs ${data.team2Name}`);
    });

    // Subscribe to booking events
    signalRService.onBookingCreated((data) => {
      console.log('[SignalR] Booking created:', data);
      bookingStore.fetchBookings();
    });

    signalRService.onBookingCancelled((data) => {
      console.log('[SignalR] Booking cancelled:', data);
      bookingStore.fetchBookings();
    });

    // Subscribe to notifications
    signalRService.onNewNotification((notification) => {
      console.log('[SignalR] New notification:', notification);
      notificationStore.addNotification(notification);
      toast.info(notification.message);
    });

  } catch (error) {
    console.error('[SignalR] Setup failed:', error);
  }
};

// Cleanup SignalR on logout
const cleanupSignalR = async () => {
  await signalRService.stopConnection();
};

// Watch auth state changes
watch(() => authStore.isAuthenticated, (newVal) => {
  if (newVal) {
    setupSignalR();
  } else {
    cleanupSignalR();
  }
});

onMounted(() => {
  if (authStore.isAuthenticated) {
    setupSignalR();
  }
});

onUnmounted(() => {
  cleanupSignalR();
});
</script>
