export type AccountType = 'Checking' | 'Credit' | 'Savings';

export interface Account {
  id: number;
  name: string;
  type: AccountType;
  balance: number;
}

export interface CreateAccountRequest {
  name: string;
  type: AccountType;
}
