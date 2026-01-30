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
                    return true;
                }
                return false;
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi tạo danh mục';
                console.error(error);
                return false;
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
                    return true;
                }
                return false;
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi cập nhật danh mục';
                console.error(error);
                return false;
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
                    return true;
                }
                return false;
            } catch (error) {
                this.error = error.response?.data?.message || 'Lỗi khi xóa danh mục';
                console.error(error);
                return false;
            } finally {
                this.loading = false;
            }
        },
        
        async createTransaction(transactionData) {
            const toast = useToast();
            this.loading = true;
            try {
                const response = await axiosClient.post('/transactions', transactionData);
                if (response.data.success) {
                    await this.fetchTransactions();
                    return true;
                }
                toast.error(response.data.message || 'Tạo giao dịch thất bại');
                return false;
            } catch (error) {
                toast.error(error.response?.data?.message || 'Lỗi khi tạo giao dịch');
                return false;
            } finally {
                this.loading = false;
            }
        },
        
        async updateTransaction(id, transactionData) {
            const toast = useToast();
            this.loading = true;
            try {
                const response = await axiosClient.put(`/transactions/${id}`, transactionData);
                if (response.data.success) {
                    await this.fetchTransactions();
                    return true;
                }
                toast.error(response.data.message || 'Cập nhật giao dịch thất bại');
                return false;
            } catch (error) {
                toast.error(error.response?.data?.message || 'Lỗi khi cập nhật giao dịch');
                return false;
            } finally {
                this.loading = false;
            }
        },
        
        async deleteTransaction(id) {
            this.loading = true;
            try {
                const response = await axiosClient.delete(`/transactions/${id}`);
                if (response.data.success) {
                    await this.fetchTransactions();
                    return true;
                }
                return false;
            } catch (error) {
                console.error(error);
                return false;
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
        },
        async rejectDeposit(id) {
            const toast = useToast();
            try {
                const response = await axiosClient.post(`/wallet/reject/${id}`);
                if (response.data.success) {
                    return true;
                }
                toast.error(response.data.message || "Từ chối thất bại");
                return false;
            } catch (error) {
                toast.error(error.response?.data?.message || "Lỗi khi từ chối");
                return false;
            }
        }
    }
});