import { createFeature, createReducer, on } from '@ngrx/store';
import { AuthUser } from '../../core/models/auth.model';
import { AuthActions } from './auth.actions';

export interface AuthState {
  user: AuthUser | null;
  loading: boolean;
  error: string | null;
}

function loadUser(): AuthUser | null {
  try { return JSON.parse(localStorage.getItem('auth_user') ?? 'null'); }
  catch { return null; }
}

const initialState: AuthState = {
  user: loadUser(),
  loading: false,
  error: null,
};

export const authFeature = createFeature({
  name: 'auth',
  reducer: createReducer(
    initialState,
    on(AuthActions.login, state => ({ ...state, loading: true, error: null })),
    on(AuthActions.loginSuccess, (state, { response }) => ({
      ...state,
      loading: false,
      user: { userId: response.userId, name: response.name, role: response.role, orgId: response.orgId },
    })),
    on(AuthActions.loginFailure, (state, { error }) => ({ ...state, loading: false, error })),
    on(AuthActions.logout, () => ({ ...initialState, user: null })),
    on(AuthActions.restoreSession, (state, { user }) => ({ ...state, user })),
  ),
});
