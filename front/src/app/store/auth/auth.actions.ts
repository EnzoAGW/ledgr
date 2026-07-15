import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { AuthUser, LoginRequest, LoginResponse } from '../../core/models/auth.model';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    'Login': props<LoginRequest>(),
    'Login Success': props<{ response: LoginResponse }>(),
    'Login Failure': props<{ error: string }>(),
    'Logout': emptyProps(),
    'Restore Session': props<{ user: AuthUser }>(),
  },
});
