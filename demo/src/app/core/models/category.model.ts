export type CategoryType = 'Income' | 'Expense';

export interface Category {
  id: number;
  name: string;
  type: CategoryType;
  color: string;
}

export interface CreateCategoryRequest {
  name: string;
  type: CategoryType;
  color: string;
}
