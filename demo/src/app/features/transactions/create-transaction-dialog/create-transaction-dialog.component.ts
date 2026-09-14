import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TransactionService } from '../../../core/services/transaction.service';
import { Account } from '../../../core/models/account.model';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-create-transaction-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule,
    MatDatepickerModule, MatNativeDateModule, MatSnackBarModule,
  ],
  template: `
    <h2 mat-dialog-title>New Transaction</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline">
          <mat-label>Account</mat-label>
          <mat-select formControlName="accountId">
            @for (a of data.accounts; track a.id) {
              <mat-option [value]="a.id">{{ a.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Type</mat-label>
          <mat-select formControlName="type">
            <mat-option value="Income">Income</mat-option>
            <mat-option value="Expense">Expense</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Amount</mat-label>
          <input matInput type="number" step="0.01" formControlName="amount">
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Date</mat-label>
          <input matInput [matDatepicker]="dp" formControlName="date">
          <mat-datepicker-toggle matIconSuffix [for]="dp" />
          <mat-datepicker #dp />
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Category (optional)</mat-label>
          <mat-select formControlName="categoryId">
            <mat-option [value]="null">None</mat-option>
            @for (c of data.categories; track c.id) {
              <mat-option [value]="c.id">{{ c.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Description (optional)</mat-label>
          <input matInput formControlName="description">
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
  styles: ['.dialog-form { display:flex; flex-direction:column; gap:4px; min-width:360px; } mat-form-field { width:100%; }'],
})
export class CreateTransactionDialogComponent {
  private ref = inject(MatDialogRef<CreateTransactionDialogComponent>);
  data: { accounts: Account[]; categories: Category[] } = inject(MAT_DIALOG_DATA);
  private svc = inject(TransactionService);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  saving = false;

  form = this.fb.group({
    accountId:   [null as number | null, Validators.required],
    type:        ['Expense', Validators.required],
    amount:      [null as number | null, [Validators.required, Validators.min(0.01)]],
    date:        [new Date(), Validators.required],
    categoryId:  [null as number | null],
    description: [''],
  });

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    const v = this.form.value;
    this.svc.create({
      accountId:   v.accountId!,
      type:        v.type as 'Income' | 'Expense',
      amount:      v.amount!,
      date:        (v.date as Date).toISOString(),
      categoryId:  v.categoryId ?? undefined,
      description: v.description || undefined,
    }).subscribe({
      next: () => { this.snack.open('Transaction created', 'OK', { duration: 2500 }); this.ref.close(true); },
      error: () => { this.snack.open('Failed to create transaction', 'OK', { duration: 3000 }); this.saving = false; },
    });
  }
}
