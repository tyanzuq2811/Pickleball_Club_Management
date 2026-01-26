import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

export const useTransactionStore = defineStore('transaction', {
    state: () => ({
        transactions: [],
        categories: [],
        pendingDeposits: [],
        loading: false,
        error: null
    }),
    actions: {
        async fetchTransactions() {
            this.loading = true;
            this.error = null;
            try {
                const response = await axiosClient.get('/transactions');
                if (response.data.success) {
                    this.transactions = response.data.data.items || response.data.data;
                }
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi tải giao dịch';
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        
        async fetchCategories() {
            this.loading = true;
            this.error = null;
            try {
                const response = await axiosClient.get('/transactioncategories');
                if (response.data.success) {
                    this.categories = response.data.data;
                }
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi tải danh mục';
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        
        async createCategory(categoryData) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axiosClient.post('/transactioncategories', categoryData);
                if (response.data.success) {
                    await this.fetchCategories();
                    return { success: true, data: response.data.data };
                }
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi tạo danh mục';
                console.error(error);
                return { success: false, message: this.error };
            } finally {
                this.loading = false;
            }
        },
        
        async updateCategory(id, categoryData) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axiosClient.put(`/transactioncategories/${id}`, categoryData);
                if (response.data.success) {
                    await this.fetchCategories();
                    return { success: true, data: response.data.data };
                }
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi cập nhật danh mục';
                console.error(error);
                return { success: false, message: this.error };
            } finally {
                this.loading = false;
            }
        },
        
        async deleteCategory(id) {
            this.loading = true;
            this.error = null;
            try {
                const response = await axiosClient.delete(`/transactioncategories/${id}`);
                if (response.data.success) {
                    await this.fetchCategories();
                    return { success: true };
                }
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi xóa danh mục';
                console.error(error);
                return { success: false, message: this.error };
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