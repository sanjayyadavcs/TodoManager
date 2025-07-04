import { apiRoutes } from "@/config/apiRoutes";
import type { LoginCredentials, RegisterRequest, User, AuthResponse } from "@/models/schema";


export const TOKEN_KEY = "todo_auth_token";

export const getToken = (): string | null => {
    return localStorage.getItem(TOKEN_KEY);
};

export const setToken = (token: string): void => {
    localStorage.setItem(TOKEN_KEY, token);
};

export const removeToken = (): void => {
    localStorage.removeItem(TOKEN_KEY);
};

// Auth API calls
export const authApi = {
    login: async (credentials: LoginCredentials): Promise<AuthResponse> => {
        const response = await fetch(apiRoutes.auth.login, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(credentials),
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || "Login failed");
        }

        const res = await response.json();
        let data = res.data as AuthResponse;
        setToken(data.token);
        return data;
    },

    register: async (userData: RegisterRequest): Promise<any> => {
        const response = await fetch(apiRoutes.auth.register, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(userData),
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || "Registration failed");
        }

        const res = await response.json();
        let data = res.data as AuthResponse;
        // setToken(data.token);
        return data;
    },

    getMe: async (): Promise<User> => {
        const token = getToken();
        if (!token) {
            throw new Error("No authentication token found");
        }

        const response = await fetch(apiRoutes.auth.me, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });

        if (!response.ok) {
            if (response.status === 401) {
                removeToken();
                throw new Error("Token expired");
            }
            throw new Error("Failed to get user information");
        }

        const res = await response.json();
        if (!res.success) {
            throw new Error(res.message || "Failed to get user information");
        }


        let data = res.data as User;

        return data;
    },

    logout: async (): Promise<void> => {
        const token = getToken();
        if (token) {
            try {
                await fetch(apiRoutes.auth.logout, {
                    method: "POST",
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
            } catch (error) {
                // Ignore logout errors, still remove token
            }
        }
        removeToken();
    },
};

// Authenticated fetch wrapper
export const authenticatedFetch = async (
    url: string,
    options: RequestInit = {}
): Promise<Response> => {
    const token = getToken();

    const headers: Record<string, string> = {
        "Content-Type": "application/json",
    };

    // Add existing headers if they exist
    if (options.headers) {
        Object.entries(options.headers).forEach(([key, value]) => {
            if (typeof value === 'string') {
                headers[key] = value;
            }
        });
    }

    if (token) {
        headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch(url, {
        ...options,
        headers,
    });

    if (response.status === 401) {
        removeToken();
        window.location.href = "/auth";
    }

    return response;
};