import { defineStore } from 'pinia';
import axiosClient from '@/api/axiosClient';
import { useToast } from "vue-toastification";

export const useBookingStore = defineStore('booking', {
    state: () => ({
        bookings: [],
        courts: [],
        loading: false
    }),
    actions: {
        async fetchCourts() {
            try {
                const response = await axiosClient.get('/courts');
                if (response.data.success) {
                    this.courts = response.data.data;
                }
            } catch (error) {
                console.error(error);
            }
        },
        async fetchBookings(page = 1, pageSize = 100) {
            this.loading = true;
            try {
                const response = await axiosClient.get(`/bookings?pageNumber=${page}&pageSize=${pageSize}`);
                if (response.data.success) {
                    this.bookings = response.data.data.items;
                }
            } catch (error) {
                console.error(error);
            } finally {
                this.loading = false;
            }
        },
        async createBooking(bookingData) {
            const toast = useToast();
            try {
                const response = await axiosClient.post('/bookings', bookingData);
                if (response.data.success) {
                    toast.success("Đặt sân thành công!");
                    await this.fetchBookings();
                    return true;
                }
            } catch (error) {
                const msg = error.response?.data?.message || "Đặt sân thất bại";
                toast.error(msg);
                return false;
            }
            return false;
        }
    }
});