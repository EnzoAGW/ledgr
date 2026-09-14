import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { PaginationComponent, SimplePageEvent } from '../../shared/pagination/pagination.component';
import { AccountService } from '../../core/services/account.service';
import { Account } from '../../core/models/account.model';
import { CreateAccountDialogComponent } from './create-account-dialog/create-account-dialog.component';

@Component({
  selector: 'app-accounts',
  imports: [DecimalPipe, MatCardModule, MatButtonModule, MatIconModule, MatDialogModule, MatChipsModule, PaginationComponent],
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
})
export class AccountsComponent implements OnInit {
  private svc = inject(AccountService);
  private dialog = inject(MatDialog);

  accounts = signal<Account[]>([]);
  loading = signal(true);

  pageIndex = signal(0);
  pageSize = signal(10);
  paged = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    return this.accounts().slice(start, start + this.pageSize());
  });

  onPage(e: SimplePageEvent) {
    this.pageIndex.set(e.pageIndex);
    this.pageSize.set(e.pageSize);
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.svc.getAll().subscribe({
      next: a => { this.accounts.set(a); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() {
    this.dialog.open(CreateAccountDialogComponent, { width: '400px' })
      .afterClosed().subscribe(ok => { if (ok) this.load(); });
  }

  typeIcon(type: string) {
    return { Checking: 'account_balance_wallet', Credit: 'credit_card', Savings: 'savings' }[type] ?? 'account_balance';
  }
}
