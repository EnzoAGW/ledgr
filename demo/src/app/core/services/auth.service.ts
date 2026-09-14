import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, AuthUser } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/auth`;

  login(req: LoginRequest) {
    return this.http.post<LoginResponse>(`${this.base}/login`, req).pipe(
      tap(res => {
        localStorage.setItem('auth_token', res.token);
        const user: AuthUser = { userId: res.userId, name: res.name, role: res.role, orgId: res.orgId };
        localStorage.setItem('auth_user', JSON.stringify(user));
      })
    );
  }

  logout() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
  }

  getUser(): AuthUser | null {
    try { return JSON.parse(localStorage.getItem('auth_user') ?? 'null'); }
    catch { return null; }
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
