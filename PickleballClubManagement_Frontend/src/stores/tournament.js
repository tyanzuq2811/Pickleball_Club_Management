import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

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
        },
        async createTournament(tournamentData) {
            const toast = useToast();
            this.loading = true;
            try {
                const response = await axiosClient.post('/tournaments', tournamentData);
                if (response.data.success) {
                    toast.success("Tạo giải đấu thành công!");
                    return true;
                } else {
                    toast.error(response.data.message || "Tạo giải đấu thất bại");
                    return false;
                }
            } catch (error) {
                console.error(error);
                toast.error(error.response?.data?.message || "Lỗi khi tạo giải đấu");
                return false;
            } finally {
                this.loading = false;
            }
        },
        async joinTournament(tournamentId) {
            const toast = useToast();
            try {
                const response = await axiosClient.post(`/tournaments/${tournamentId}/join`);
                if (response.data.success) {
                    return true;
                } else {
                    toast.error(response.data.message || "Đăng ký thất bại");
                    return false;
                }
            } catch (error) {
                const errorMessage = error.response?.data?.message || "Lỗi khi đăng ký giải";
                
                // Customize error messages
                if (errorMessage.toLowerCase().includes('already joined') || errorMessage.toLowerCase().includes('already participating')) {
                    toast.warning('⚠️ Bạn đã đăng ký giải đấu này rồi!');
                } else if (errorMessage.toLowerCase().includes('full') || errorMessage.toLowerCase().includes('maximum')) {
                    toast.error('❌ Giải đấu đã đủ số lượng người tham gia!');
                } else if (errorMessage.toLowerCase().includes('closed') || errorMessage.toLowerCase().includes('ended')) {
                    toast.error('❌ Giải đấu đã đóng đăng ký!');
                } else {
                    toast.error(errorMessage);
                }
                return false;
            }
        },
        async autoDivideTeams(tournamentId) {
            const toast = useToast();
            try {
                const response = await axiosClient.post(`/tournaments/${tournamentId}/auto-divide-teams`);
                if (response.data.success) {
                    return true;
                } else {
                    toast.error(response.data.message || "Chia đội thất bại");
                    return false;
                }
            } catch (error) {
                toast.error(error.response?.data?.message || "Lỗi khi chia đội");
                return false;
            }
        },
        async generateBracket(tournamentId) {
            const toast = useToast();
            try {
                const response = await axiosClient.post(`/tournaments/${tournamentId}/generate-bracket`);
                if (response.data.success) {
                    return true;
                } else {
                    toast.error(response.data.message || "Tạo cây đấu thất bại");
                    return false;
                }
            } catch (error) {
                toast.error(error.response?.data?.message || "Lỗi khi tạo cây đấu");
                return false;
            }
        },
        async startTournament(tournamentId) {
            const toast = useToast();
            try {
                const response = await axiosClient.post(`/tournaments/${tournamentId}/start`);
                if (response.data.success) {
                    return true;
                } else {
                    toast.error(response.data.message || "Bắt đầu giải thất bại");
                    return false;
                }
            } catch (error) {
                toast.error(error.response?.data?.message || "Lỗi khi bắt đầu giải");
                return false;
            }
        },
        async updateTournament(tournamentId, tournamentData) {
            const toast = useToast();
            this.loading = true;
            try {
                const response = await axiosClient.put(`/tournaments/${tournamentId}`, tournamentData);
                if (response.data.success) {
                    return true;
                } else {
                    toast.error(response.data.message || "Cập nhật giải đấu thất bại");
                    return false;
                }
            } catch (error) {
                console.error(error);
                toast.error(error.response?.data?.message || "Lỗi khi cập nhật giải đấu");
                return false;
            } finally {
                this.loading = false;
            }
        },
        async deleteTournament(tournamentId) {
            const toast = useToast();
            try {
                const response = await axiosClient.delete(`/tournaments/${tournamentId}`);
                if (response.data.success) {
                    return true;
                } else {
                    toast.error(response.data.message || "Xóa giải đấu thất bại");
                    return false;
                }
            } catch (error) {
                console.error(error);
                toast.error(error.response?.data?.message || "Lỗi khi xóa giải đấu");
                return false;
            }
        }
    }
});