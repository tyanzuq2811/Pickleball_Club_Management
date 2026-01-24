import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';

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
        }
    }
});