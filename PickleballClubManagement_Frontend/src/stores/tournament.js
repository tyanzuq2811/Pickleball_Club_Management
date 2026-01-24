import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';

export const useTournamentStore = defineStore('tournament', {
    state: () => ({
        tournaments: [],
        currentBracket: null,
        loading: false
    }),
    actions: {
        async fetchTournaments() {
            this.loading = true;
            try {
                const response = await axiosClient.get('/tournaments');
                if (response.data.success) {
                    this.tournaments = response.data.data.items;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        async fetchBracket(tournamentId) {
            this.loading = true;
            try {
                const response = await axiosClient.get(`/tournaments/${tournamentId}/bracket`);
                if (response.data.success) {
                    this.currentBracket = response.data.data;
                } else {
                    this.currentBracket = null;
                }
            } catch (error) {
                console.error(error);
                this.currentBracket = null;
            } finally {
                this.loading = false;
            }
        }
    }
});