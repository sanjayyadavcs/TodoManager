// src/constants/apiRoutes.ts
const API_BASE_URL = "https://localhost:44303/api";

export const apiRoutes = {
  auth: {
    login: `${API_BASE_URL}/auth/login`,
    register: `${API_BASE_URL}/auth/register`,
    me: `${API_BASE_URL}/auth/me`,
    logout: `${API_BASE_URL}/auth/logout`,
  },
  todos: {
    base: `${API_BASE_URL}/todo`,
    stats: `${API_BASE_URL}/todo/stats`,
    byId: (id: number | string) => `${API_BASE_URL}/todo/${id}`,
    toggle: (id: number | string) => `${API_BASE_URL}/todo/${id}/toggle`,
    delete: (id: number | string) => `${API_BASE_URL}/todo/${id}`,
    update: (id: number | string) => `${API_BASE_URL}/todo/${id}`,
    create: `${API_BASE_URL}/todo`,
  },
};