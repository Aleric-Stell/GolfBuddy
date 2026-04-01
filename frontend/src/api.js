import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7181', // change to your API port
  headers: { 'Content-Type': 'application/json' }
});

export default api;