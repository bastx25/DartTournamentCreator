import axios from "axios";
import type { LoginDto } from "../dtos/auth/LoginDto";
import type { RegisterDto } from "../dtos/auth/RegisterDto";
import type { AuthResponseDto } from "../dtos/auth/AuthResponseDto";
import type { UserDto } from "../dtos/auth/UserDto";

const authClient = axios.create({
  baseURL: "/api/auth",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

export async function login(data: LoginDto): Promise<AuthResponseDto> {
  const response = await authClient.post<AuthResponseDto>("/login", data);
  return response.data;
}

export async function register(data: RegisterDto): Promise<AuthResponseDto> {
  const response = await authClient.post<AuthResponseDto>("/register", data);
  return response.data;
}

export async function refresh(token?: string): Promise<AuthResponseDto> {
  const response = await authClient.post<AuthResponseDto>("/refresh", {
    refreshToken: token,
  });
  return response.data;
}

export async function logout(token?: string): Promise<void> {
  try {
    await authClient.post("/logout", {
      refreshToken: token,
    });
  } catch {
    // Ignore error on logout
  }
}

export async function getMe(accessToken?: string): Promise<UserDto> {
  const response = await authClient.get<UserDto>("/me", {
    headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : undefined,
  });
  return response.data;
}
