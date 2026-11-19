import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  useRef,
  type ReactNode,
} from "react";
import axios, { type InternalAxiosRequestConfig } from "axios";
import type { UserDto } from "../dtos/auth/UserDto";
import * as authService from "../services/authService";

interface AuthContextType {
  user: UserDto | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (
    email: string,
    password: string,
    firstName?: string,
    lastName?: string,
  ) => Promise<void>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<string | null>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Keep a ref of token for axios interceptors
  const tokenRef = useRef<string | null>(null);
  tokenRef.current = token;

  const isRefreshingRef = useRef(false);
  const refreshSubscribersRef = useRef<((newToken: string | null) => void)[]>([]);

  const onRefreshed = (newToken: string | null) => {
    refreshSubscribersRef.current.forEach((cb) => cb(newToken));
    refreshSubscribersRef.current = [];
  };

  const refreshToken = useCallback(async (): Promise<string | null> => {
    try {
      const res = await authService.refresh();
      setToken(res.accessToken);
      setUser(res.user);
      return res.accessToken;
    } catch {
      setToken(null);
      setUser(null);
      return null;
    }
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const res = await authService.login({ email, password });
    setToken(res.accessToken);
    setUser(res.user);
  }, []);

  const register = useCallback(
    async (
      email: string,
      password: string,
      firstName?: string,
      lastName?: string,
    ) => {
      const res = await authService.register({
        email,
        password,
        firstName,
        lastName,
      });
      setToken(res.accessToken);
      setUser(res.user);
    },
    [],
  );

  const logout = useCallback(async () => {
    try {
      await authService.logout();
    } finally {
      setToken(null);
      setUser(null);
    }
  }, []);

  // Global Axios Interceptor for Authorization and Silent Refresh
  useEffect(() => {
    // Enable cookies on all axios requests
    axios.defaults.withCredentials = true;

    const requestInterceptor = axios.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const currentToken = tokenRef.current;
        // Don't override if already set or for public/board-access
        if (
          currentToken &&
          config.url &&
          !config.url.startsWith("/api/public/") &&
          !config.url.startsWith("/api/board-access/") &&
          !config.headers.Authorization
        ) {
          config.headers.Authorization = `Bearer ${currentToken}`;
        }
        return config;
      },
      (error) => Promise.reject(error),
    );

    const responseInterceptor = axios.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        // Skip refresh for auth endpoints or if already retried
        if (
          !originalRequest ||
          originalRequest.url?.includes("/api/auth/") ||
          originalRequest.url?.includes("/api/public/") ||
          originalRequest.url?.includes("/api/board-access/") ||
          originalRequest._retry
        ) {
          return Promise.reject(error);
        }

        if (error.response?.status === 401) {
          if (isRefreshingRef.current) {
            return new Promise((resolve, reject) => {
              refreshSubscribersRef.current.push((newToken) => {
                if (newToken) {
                  originalRequest.headers.Authorization = `Bearer ${newToken}`;
                  resolve(axios(originalRequest));
                } else {
                  reject(error);
                }
              });
            });
          }

          originalRequest._retry = true;
          isRefreshingRef.current = true;

          try {
            const newToken = await refreshToken();
            isRefreshingRef.current = false;
            onRefreshed(newToken);

            if (newToken) {
              originalRequest.headers.Authorization = `Bearer ${newToken}`;
              return axios(originalRequest);
            }
          } catch (refreshErr) {
            isRefreshingRef.current = false;
            onRefreshed(null);
            return Promise.reject(refreshErr);
          }
        }

        return Promise.reject(error);
      },
    );

    return () => {
      axios.interceptors.request.eject(requestInterceptor);
      axios.interceptors.response.eject(responseInterceptor);
    };
  }, [refreshToken]);

  // Initial silent refresh on app mount to restore session via HttpOnly cookie
  useEffect(() => {
    let isMounted = true;

    const initializeAuth = async () => {
      try {
        const res = await authService.refresh();
        if (isMounted) {
          setToken(res.accessToken);
          setUser(res.user);
        }
      } catch {
        if (isMounted) {
          setToken(null);
          setUser(null);
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };

    void initializeAuth();

    return () => {
      isMounted = false;
    };
  }, []);

  const value = {
    user,
    token,
    isAuthenticated: !!user,
    isLoading,
    login,
    register,
    logout,
    refreshToken,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
