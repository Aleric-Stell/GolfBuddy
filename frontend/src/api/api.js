import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:5254'
});

export default api;