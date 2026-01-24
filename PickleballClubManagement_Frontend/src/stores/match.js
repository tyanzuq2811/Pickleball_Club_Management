import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

export const useMatchStore = defineStore('match', {
    state: () => ({
        matches: [],
        loading: false
    }),
    actions: {
        async fetchMatches() {
            this.loading = true;
            try {
                // Gọi API thật
                const response = await axiosClient.get('/matches');
                if (response.data.success) {
                    this.matches = response.data.data;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        async updateScore(matchId, score1, score2, isFinal = false) {
            const toast = useToast();
            try {
                const response = await axiosClient.put(`/matches/${matchId}/score`, { 
                    team1Score: score1, 
                    team2Score: score2,
                    isFinal: isFinal
                });
                if (response.data.success) {
                    toast.success("Cập nhật tỉ số thành công!");
                    await this.fetchMatches();
                    return true;
                }
            } catch (error) {
                const msg = error.response?.data?.message || "Cập nhật thất bại";
                toast.error(msg);
                return false;
            }
        }
    }
});
