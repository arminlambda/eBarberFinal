import axios from 'axios';
import * as SecureStore from 'expo-secure-store';

const API_URL = 'http://10.0.2.2:5126/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  async (config) => {
    const token = await SecureStore.getItemAsync('userToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export const login = async (email, password) => {
  const response = await api.post('/auth/login', { email, password });
  if (response.data.token) {
    await SecureStore.setItemAsync('userToken', response.data.token);
    await SecureStore.setItemAsync('userEmail', response.data.email);
  }
  return response.data;
};

export const register = async (email, password, firstName, lastName) => {
  const response = await api.post('/auth/register', {
    email,
    password,
    firstName,
    lastName,
  });
  return response.data;
};

export const logout = async () => {
  await SecureStore.deleteItemAsync('userToken');
  await SecureStore.deleteItemAsync('userEmail');
};

export const getTermini = async () => {
  const response = await api.get('/terminiapi');
  return response.data;
};

export const createTermin = async (datumInUra, lokacijaId, opombe) => {
  const response = await api.post('/terminiapi', {
    datumInUra,
    lokacijaId,
    opombe,
  });
  return response.data;
};

export const getLokacije = async () => {
  const response = await api.get('/lokacijeapi');
  return response.data;
};

export const getOkvirniTermini = async () => {
  const response = await api.get('/OkvirniTerminiApi');
  return response.data;
};

export const getMojePrijave = async () => {
  const response = await api.get('/OkvirniTerminiApi/moje-prijave');
  return response.data;
};

export const prijaviSeNaTermin = async (terminId, opombe) => {
  const response = await api.post(`/OkvirniTerminiApi/${terminId}/prijava`, {
    opombe,
  });
  return response.data;
};

export const odjaviSeOdTermina = async (prijavaId) => {
  const response = await api.delete(`/OkvirniTerminiApi/prijave/${prijavaId}`);
  return response.data;
};

export const getAdminTermini = async () => {
  const response = await api.get('/OkvirniTerminiApi/admin/all');
  return response.data;
};

export const adminCheckIn = async (prijavaId) => {
  const response = await api.post(`/OkvirniTerminiApi/admin/checkin/${prijavaId}`);
  return response.data;
};

export const adminRateUser = async (prijavaId, rating) => {
  const response = await api.post(`/OkvirniTerminiApi/admin/rate/${prijavaId}`, { rating });
  return response.data;
};

export const getAdminUsers = async () => {
  const response = await api.get('/admin/users');
  return response.data;
};

export const getAdminUserDetail = async (userId) => {
  const response = await api.get(`/admin/users/${userId}`);
  return response.data;
};

export const adminSetUserRating = async (userId, rating) => {
  const response = await api.post(`/admin/users/${userId}/rating`, { rating });
  return response.data;
};

export const adminCreateTermin = async (zacetek, konec, lokacijaId, maxUporabnikov, opis) => {
  const response = await api.post('/OkvirniTerminiApi/admin/create', {
    zacetekCasa: zacetek,
    konecCasa: konec,
    lokacijaId,
    maksimalnoUporabnikov: maxUporabnikov,
    opis,
  });
  return response.data;
};

export const sendMessage = async (content) => {
  const response = await api.post('/messages', { content });
  return response.data;
};

export default api;
