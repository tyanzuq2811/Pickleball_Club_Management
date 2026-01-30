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
                console.log('Fetch members response:', response.data);
                if (response.data.success) {
                    const newMembers = response.data.data.items || response.data.data;
                    console.log('New members count:', newMembers?.length);
                    this.members = newMembers;
                }
            } catch (error) {
                console.error('Fetch members error:', error);
            } finally {
                this.loading = false;
            }
        },
        async createMember(memberData) {
            const toast = useToast();
            try {
                const response = await axiosClient.post('/members', memberData);
                if (response.data.success) {
                    // Fetch members và đợi hoàn tất trước khi return
                    await this.fetchMembers();
                    console.log('Members after create:', this.members.length);
                    return true;
                }
                toast.error(response.data.message || 'Tạo hội viên thất bại');
                return false;
            } catch (error) {
                console.error('Create member error:', error);
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