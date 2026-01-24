import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

export const useWalletStore = defineStore('wallet', {
    state: () => ({
        transactions: [],
        loading: false
    }),
    actions: {
        async fetchTransactions() {
            this.loading = true;
            try {
                const response = await axiosClient.get('/wallet/transactions');
                if (response.data.success) {
                    this.transactions = response.data.data;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        async deposit(amount) {
            const toast = useToast();
            try {
                // Gọi API nạp tiền (giả lập hoặc thật)
                const response = await axiosClient.post('/wallet/deposit', { amount });
                if (response.data.success) {
                    toast.success("Yêu cầu nạp tiền thành công! Vui lòng chờ duyệt.");
                    await this.fetchTransactions();
                    return true;
                }
            } catch (error) {
                const msg = error.response?.data?.message || "Nạp tiền thất bại";
                toast.error(msg);
                return false;
            }
            return false;
        }
    }
});