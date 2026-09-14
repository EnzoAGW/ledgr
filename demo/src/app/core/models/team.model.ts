import { UserRole } from './auth.model';

export interface TeamMember {
  id: number;
  name: string;
  email: string;
  role: UserRole;
}

export interface InviteUserRequest {
  name: string;
  email: string;
  password: string;
  role: UserRole;
}
