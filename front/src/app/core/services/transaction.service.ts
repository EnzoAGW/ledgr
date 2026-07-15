import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  Transaction, CreateTransactionRequest,
  UpdateTransactionStatusRequest, TransactionFilters, PaginatedResult
} from '../models/transaction.model';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/transactions`;

  getAll(filters: TransactionFilters) {
    let params = new HttpParams()
      .set('page', filters.page)
      .set('pageSize', filters.pageSize);
    if (filters.search)    params = params.set('search', filters.search);
    if (filters.accountId) params = params.set('accountId', filters.accountId);
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId);
    if (filters.type)      params = params.set('type', filters.type);
    if (filters.status)    params = params.set('status', filters.status);
    if (filters.from)      params = params.set('from', filters.from);
    if (filters.to)        params = params.set('to', filters.to);
    return this.http.get<PaginatedResult<Transaction>>(this.base, { params });
  }

  create(req: CreateTransactionRequest) {
    return this.http.post<{ id: number }>(this.base, req);
  }

  updateStatus(id: number, req: UpdateTransactionStatusRequest) {
    return this.http.patch<void>(`${this.base}/${id}/status`, req);
  }
}
