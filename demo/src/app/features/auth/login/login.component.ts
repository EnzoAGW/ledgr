import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AsyncPipe } from '@angular/common';
import { AuthActions } from '../../../store/auth/auth.actions';
import { selectError, selectLoading } from '../../../store/auth/auth.selectors';

export interface DemoAccount {
  role: string;
  email: string;
  password: string;
}

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule, MatInputModule,
    MatButtonModule, MatProgressSpinnerModule,
  ],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private store = inject(Store);

  loading = this.store.selectSignal(selectLoading);
  error = this.store.selectSignal(selectError);

  readonly demoAccounts: DemoAccount[] = [
    { role: 'Admin', email: 'admin@ledgr.dev', password: 'Admin@123' },
    { role: 'Manager', email: 'manager@ledgr.dev', password: 'Manager@123' },
    { role: 'Analyst', email: 'analyst@ledgr.dev', password: 'Analyst@123' },
  ];

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  useDemoAccount(account: DemoAccount) {
    this.form.setValue({ email: account.email, password: account.password });
    this.submit();
  }

  submit() {
    if (this.form.invalid) return;
    const { email, password } = this.form.value;
    this.store.dispatch(AuthActions.login({ email: email!, password: password! }));
  }
}
