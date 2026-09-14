import { createSelector } from '@ngrx/store';
import { authFeature } from './auth.reducer';

export const {
  selectAuthState,
  selectUser,
  selectLoading,
  selectError,
} = authFeature;

export const selectIsAuthenticated = createSelector(selectUser, user => !!user);
export const selectUserRole = createSelector(selectUser, user => user?.role ?? null);
export const selectIsAdmin = createSelector(selectUserRole, role => role === 'Admin');
