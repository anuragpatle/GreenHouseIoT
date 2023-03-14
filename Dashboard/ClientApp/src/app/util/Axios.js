import axios from 'axios';
import { toast } from "react-toastify";

const BASE_URL = process.env.REACT_APP_API_URL;

const axiosInstance = axios.create({
    baseURL: BASE_URL
});

axiosInstance.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        const msg = (error.response && error.response.data && error.response.data.message) || 'Something went wrong!';
        if (msg.toLowerCase() === 'invalid user credentials.') {
            localStorage.clear();
            window.location.href = "/";
        } else {
            toast.error(msg);
        }
        return Promise.reject(msg);
    }
);

axiosInstance.interceptors.request.use(
    (request) => {
        if (request.method.toLowerCase() === 'post' || request.method.toLowerCase() === 'put') {
            request.headers['Content-Type'] = 'application/json';
        }
        request.headers.Authorization = 'Bearer RVd01aghEyEuLjNUaIO8kmdQ23Yg0euB';        
        request.headers["Access-Control-Allow-Origin"]='*';              
        return request;
    }
);

export default axiosInstance;
