import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Account, CreateAccountRequest } from '../models/account.model';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/accounts`;

  getAll() {
    return this.http.get<Account[]>(this.base);
  }

  create(req: CreateAccountRequest) {
    return this.http.post<{ id: number }>(this.base, req);
  }
}
