import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from 'vue-toastification';

export const useMemberStore = defineStore('member', {
    state: () => ({
        members: [],
        loading: false
    }),
    actions: {
        async fetchMembers() {
            this.loading = true;
            try {
                const response = await axiosClient.get('/members');
                if (response.data.success) {
                    this.members = response.data.data.items || response.data.data;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        async createMember(memberData) {
            const toast = useToast();
            try {
                const response = await axiosClient.post('/members', memberData);
                if (response.data.success) {
                    await this.fetchMembers();
                    return true;
                }
                toast.error(response.data.message || 'Tạo hội viên thất bại');
                return false;
            } catch (error) {
                toast.error(error.response?.data?.message || 'Lỗi khi tạo hội viên');
                return false;
            }
        },
        async updateMember(id, memberData) {
            const toast = useToast();
            try {
                const response = await axiosClient.put(`/members/${id}`, memberData);
                if (response.data.success) {
                    await this.fetchMembers();
                    return true;
                }
                toast.error(response.data.message || 'Cập nhật hội viên thất bại');
                return false;
            } catch (error) {
                toast.error(error.response?.data?.message || 'Lỗi khi cập nhật hội viên');
                return false;
            }
        },
        async deleteMember(id) {
            const toast = useToast();
            try {
                const response = await axiosClient.delete(`/members/${id}`);
                if (response.data.success) {
                    await this.fetchMembers();
                    return true;
                }
                toast.error(response.data.message || 'Xóa hội viên thất bại');
                return false;
            } catch (error) {
                toast.error(error.response?.data?.message || 'Lỗi khi xóa hội viên');
                return false;
            }
        }
    }
});