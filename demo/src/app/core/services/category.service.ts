import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Category, CreateCategoryRequest } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/categories`;

  getAll() {
    return this.http.get<Category[]>(this.base);
  }

  create(req: CreateCategoryRequest) {
    return this.http.post<{ id: number }>(this.base, req);
  }
}
