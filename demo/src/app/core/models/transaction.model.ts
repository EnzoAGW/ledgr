export type TransactionType = 'Income' | 'Expense';
export type TransactionStatus = 'Pending' | 'Confirmed' | 'Cancelled';

export interface Transaction {
  id: number;
  amount: number;
  type: TransactionType;
  status: TransactionStatus;
  date: string;
  description?: string;
  accountId: number;
  accountName: string;
  categoryId?: number;
  categoryName?: string;
  categoryColor?: string;
}

export interface CreateTransactionRequest {
  accountId: number;
  categoryId?: number;
  amount: number;
  type: TransactionType;
  date: string;
  description?: string;
}

export interface UpdateTransactionStatusRequest {
  status: 'Confirmed' | 'Cancelled';
}

export interface TransactionFilters {
  search?: string;
  accountId?: number;
  categoryId?: number;
  type?: TransactionType;
  status?: TransactionStatus;
  from?: string;
  to?: string;
  page: number;
  pageSize: number;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
