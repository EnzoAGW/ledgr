import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TeamService } from '../../../core/services/team.service';

@Component({
  selector: 'app-invite-user-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatSnackBarModule,
  ],
  template: `
    <h2 mat-dialog-title>Invite Team Member</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline">
          <mat-label>Full Name</mat-label>
          <input matInput formControlName="name">
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email">
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Temporary Password</mat-label>
          <input matInput type="password" formControlName="password">
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Role</mat-label>
          <mat-select formControlName="role">
            <mat-option value="Manager">Manager</mat-option>
            <mat-option value="Analyst">Analyst</mat-option>
          </mat-select>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button (click)="save()" [disabled]="form.invalid || saving">
        {{ saving ? 'Sending…' : 'Invite' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: ['.dialog-form { display:flex; flex-direction:column; min-width:340px; } mat-form-field { width:100%; }'],
})
export class InviteUserDialogComponent {
  private ref = inject(MatDialogRef<InviteUserDialogComponent>);
  private svc = inject(TeamService);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  saving = false;
  form = this.fb.group({
    name:     ['', Validators.required],
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role:     ['Analyst', Validators.required],
  });

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    const v = this.form.value;
    this.svc.invite({ name: v.name!, email: v.email!, password: v.password!, role: v.role as any }).subscribe({
      next: () => { this.snack.open('Invitation sent', 'OK', { duration: 2500 }); this.ref.close(true); },
      error: (e) => {
        const msg = e?.error?.message ?? 'Failed to invite user';
        this.snack.open(msg, 'OK', { duration: 3000 });
        this.saving = false;
      },
    });
  }
}
