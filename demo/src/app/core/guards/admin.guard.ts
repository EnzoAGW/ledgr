import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthUser } from '../models/auth.model';

export const adminGuard: CanActivateFn = () => {
  const router = inject(Router);
  try {
    const user: AuthUser | null = JSON.parse(localStorage.getItem('auth_user') ?? 'null');
    return user?.role === 'Admin' ? true : router.createUrlTree(['/dashboard']);
  } catch {
    return router.createUrlTree(['/dashboard']);
  }
};
