import axios from 'axios';

const axiosClient = axios.create({
    baseURL: 'http://localhost:5000/api', // URL Backend của bạn
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor: Gắn Token vào Header
axiosClient.interceptors.request.use(
    (config) => {
        const token = sessionStorage.getItem('token');
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
            sessionStorage.removeItem('token');
            sessionStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default axiosClient;