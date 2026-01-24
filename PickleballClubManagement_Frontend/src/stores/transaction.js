import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

export const useTransactionStore = defineStore('transaction', {
    state: () => ({
        transactions: [],
        pendingDeposits: [],
        loading: false
    }),
    actions: {
        async fetchTransactions() {
            this.loading = true;
            try {
                const response = await axiosClient.get('/transactions');
                if (response.data.success) {
                    this.transactions = response.data.data.items || response.data.data;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        // Giả lập lấy danh sách nạp tiền chờ duyệt (Cần API tương ứng ở Backend)
        async fetchPendingDeposits() {
            this.loading = true;
            try {
                // Gọi API thật từ Backend
                const response = await axiosClient.get('/wallet/pending');
                if (response.data.success) {
                    this.pendingDeposits = response.data.data;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        async approveDeposit(id) {
            const toast = useToast();
            try {
                // Gọi API thật từ Backend
                const response = await axiosClient.post(`/wallet/approve/${id}`);
                if (response.data.success) {
                    toast.success("Đã duyệt nạp tiền!");
                    await this.fetchPendingDeposits();
                    return true;
                }
            } catch (error) {
                const msg = error.response?.data?.message || "Duyệt thất bại";
                toast.error(msg);
                return false;
            }
        }
    }
});