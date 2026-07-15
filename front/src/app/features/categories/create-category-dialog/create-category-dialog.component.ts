import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-create-category-dialog',
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatSnackBarModule,
  ],
  template: `
    <h2 mat-dialog-title>New Category</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name">
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Type</mat-label>
          <mat-select formControlName="type">
            <mat-option value="Income">Income</mat-option>
            <mat-option value="Expense">Expense</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Color</mat-label>
          <input matInput type="color" formControlName="color" style="height:36px;cursor:pointer">
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
export class CreateCategoryDialogComponent {
  private ref = inject(MatDialogRef<CreateCategoryDialogComponent>);
  private svc = inject(CategoryService);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  saving = false;
  form = this.fb.group({
    name:  ['', Validators.required],
    type:  ['Expense', Validators.required],
    color: ['#6366F1', Validators.required],
  });

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    const v = this.form.value;
    this.svc.create({ name: v.name!, type: v.type as any, color: v.color! }).subscribe({
      next: () => { this.snack.open('Category created', 'OK', { duration: 2500 }); this.ref.close(true); },
      error: () => { this.snack.open('Failed to create category', 'OK', { duration: 3000 }); this.saving = false; },
    });
  }
}
