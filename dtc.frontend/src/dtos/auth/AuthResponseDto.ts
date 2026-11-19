import type { UserDto } from "./UserDto";

export interface AuthResponseDto {
  accessToken: string;
  refreshToken?: string;
  expiresAt: string;
  user: UserDto;
}
