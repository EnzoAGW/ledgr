import { Component, inject, signal, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CategoryService } from '../../core/services/category.service';
import { Category } from '../../core/models/category.model';
import { CreateCategoryDialogComponent } from './create-category-dialog/create-category-dialog.component';

@Component({
  selector: 'app-categories',
  imports: [MatTableModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent implements OnInit {
  private svc = inject(CategoryService);
  private dialog = inject(MatDialog);

  categories = signal<Category[]>([]);
  loading = signal(true);
  columns = ['color', 'name', 'type'];

  ngOnInit() { this.load(); }

  load() {
    this.svc.getAll().subscribe({
      next: c => { this.categories.set(c); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() {
    this.dialog.open(CreateCategoryDialogComponent, { width: '400px' })
      .afterClosed().subscribe(ok => { if (ok) this.load(); });
  }
}
