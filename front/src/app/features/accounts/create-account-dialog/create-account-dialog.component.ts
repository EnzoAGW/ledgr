import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AccountService } from '../../../core/services/account.service';

@Component({
  selector: 'app-create-account-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatSnackBarModule,
  ],
  template: `
    <h2 mat-dialog-title>New Account</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name">
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Type</mat-label>
          <mat-select formControlName="type">
            <mat-option value="Checking">Checking</mat-option>
            <mat-option value="Savings">Savings</mat-option>
            <mat-option value="Credit">Credit</mat-option>
          </mat-select>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button (click)="save()" [disabled]="form.invalid || saving">
        {{ saving ? 'Saving…' : 'Create' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: ['.dialog-form { display:flex; flex-direction:column; min-width:320px; } mat-form-field { width:100%; }'],
})
export class CreateAccountDialogComponent {
  private ref = inject(MatDialogRef<CreateAccountDialogComponent>);
  private svc = inject(AccountService);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  saving = false;
  form = this.fb.group({
    name: ['', Validators.required],
    type: ['Checking', Validators.required],
  });

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    this.svc.create({ name: this.form.value.name!, type: this.form.value.type as any }).subscribe({
      next: () => { this.snack.open('Account created', 'OK', { duration: 2500 }); this.ref.close(true); },
      error: () => { this.snack.open('Failed to create account', 'OK', { duration: 3000 }); this.saving = false; },
    });
  }
}
