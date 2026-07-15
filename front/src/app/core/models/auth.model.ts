export type UserRole = 'Admin' | 'Manager' | 'Analyst';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  name: string;
  role: UserRole;
  orgId: number;
}

export interface AuthUser {
  userId: number;
  name: string;
  role: UserRole;
  orgId: number;
}
