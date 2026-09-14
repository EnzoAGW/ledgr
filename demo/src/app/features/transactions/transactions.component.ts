import { Component, inject, signal, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { PaginationComponent, SimplePageEvent } from '../../shared/pagination/pagination.component';
import { TransactionService } from '../../core/services/transaction.service';
import { AccountService } from '../../core/services/account.service';
import { CategoryService } from '../../core/services/category.service';
import { Transaction, TransactionStatus } from '../../core/models/transaction.model';
import { Account } from '../../core/models/account.model';
import { Category } from '../../core/models/category.model';
import { CreateTransactionDialogComponent } from './create-transaction-dialog/create-transaction-dialog.component';

@Component({
  selector: 'app-transactions',
  imports: [
    ReactiveFormsModule, DatePipe, DecimalPipe,
    MatTableModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule,
    MatIconModule, MatDialogModule, MatMenuModule, MatSnackBarModule,
    PaginationComponent,
  ],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.scss',
})
export class TransactionsComponent implements OnInit {
  private svc = inject(TransactionService);
  private accountSvc = inject(AccountService);
  private categorySvc = inject(CategoryService);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  transactions = signal<Transaction[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  accounts = signal<Account[]>([]);
  categories = signal<Category[]>([]);

  page = signal(0);
  pageSize = signal(10);

  columns = ['date', 'description', 'account', 'category', 'type', 'amount', 'status', 'actions'];

  filters = this.fb.group({
    search:     [''],
    accountId:  [null as number | null],
    categoryId: [null as number | null],
    type:       ['' as '' | 'Income' | 'Expense'],
    status:     ['' as '' | TransactionStatus],
  });

  ngOnInit() {
    this.accountSvc.getAll().subscribe(a => this.accounts.set(a));
    this.categorySvc.getAll().subscribe(c => this.categories.set(c));
    this.load();
  }

  load() {
    this.loading.set(true);
    const f = this.filters.value;
    this.svc.getAll({
      page: this.page() + 1,
      pageSize: this.pageSize(),
      search:     f.search     || undefined,
      accountId:  f.accountId  || undefined,
      categoryId: f.categoryId || undefined,
      type:       f.type       || undefined,
      status:     f.status     || undefined,
    }).subscribe({
      next: r => { this.transactions.set(r.items); this.totalCount.set(r.totalCount); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  onPage(e: SimplePageEvent) {
    this.page.set(e.pageIndex);
    this.pageSize.set(e.pageSize);
    this.load();
  }

  applyFilters() { this.page.set(0); this.load(); }

  clearFilters() { this.filters.reset(); this.page.set(0); this.load(); }

  openCreate() {
    const ref = this.dialog.open(CreateTransactionDialogComponent, {
      width: '480px',
      data: { accounts: this.accounts(), categories: this.categories() },
    });
    ref.afterClosed().subscribe(ok => { if (ok) { this.load(); } });
  }

  updateStatus(tx: Transaction, status: TransactionStatus) {
    this.svc.updateStatus(tx.id, { status: status as 'Confirmed' | 'Cancelled' }).subscribe({
      next: () => { this.snack.open('Status updated', 'OK', { duration: 2500 }); this.load(); },
      error: () => this.snack.open('Failed to update status', 'OK', { duration: 3000 }),
    });
  }
}
