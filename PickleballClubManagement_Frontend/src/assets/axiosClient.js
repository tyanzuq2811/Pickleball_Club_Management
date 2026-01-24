import axios from 'axios';

const axiosClient = axios.create({
    baseURL: 'https://localhost:5001/api', // URL Backend của bạn
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor: Gắn Token vào Header
axiosClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Interceptor: Xử lý lỗi (Token hết hạn -> Logout)
axiosClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response && error.response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default axiosClient;