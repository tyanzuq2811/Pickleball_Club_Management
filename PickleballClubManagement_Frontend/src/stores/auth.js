import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: JSON.parse(sessionStorage.getItem('user')) || null,
        token: sessionStorage.getItem('token') || null,
        loading: false,
    }),
    getters: {
        isAuthenticated: (state) => !!state.token,
        role: (state) => state.user?.roles?.[0] || null,
        isAdmin: (state) => state.user?.roles?.includes('Admin'),
        isTreasurer: (state) => state.user?.roles?.includes('Treasurer'),
        isReferee: (state) => state.user?.roles?.includes('Referee'),
        isMember: (state) => state.user?.roles?.includes('Member'),
        memberName: (state) => state.user?.fullName || 'User',
        avatarUrl: (state) => `https://ui-avatars.com/api/?name=${state.user?.fullName || 'User'}&background=0ea5e9&color=fff`
    },
    actions: {
        async login(email, password) {
            this.loading = true;
            const toast = useToast();
            try {
                // Gọi API Login
                const response = await axiosClient.post('/auth/login', { email, password });
                
                if (response.data.success) {
                    const { token, user } = response.data.data;
                    this.token = token;
                    this.user = user;
                    
                    // Lưu vào SessionStorage (sẽ bị xóa khi đóng tab/browser)
                    sessionStorage.setItem('token', token);
                    sessionStorage.setItem('user', JSON.stringify(user));
                    
                    toast.success(`Chào mừng ${user.fullName} quay trở lại!`);
                    return true;
                } else {
                    toast.error(response.data.message || "Đăng nhập thất bại");
                    return false;
                }
            } catch (error) {
                console.error(error);
                toast.error(error.response?.data?.message || "Lỗi kết nối đến Server");
                return false;
            } finally {
                this.loading = false;
            }
        },
        logout() {
            this.token = null;
            this.user = null;
            sessionStorage.removeItem('token');
            sessionStorage.removeItem('user');
            // Xóa cả localStorage cũ nếu còn
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
    }
});