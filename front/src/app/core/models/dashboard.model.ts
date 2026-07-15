export interface DashboardResult {
  totalIncome: number;
  totalExpense: number;
  balance: number;
  pendingCount: number;
  last7Days: DailyVolume[];
  topCategories: CategoryBreakdown[];
}

export interface DailyVolume {
  date: string;
  income: number;
  expense: number;
}

export interface CategoryBreakdown {
  name: string;
  color: string;
  amount: number;
}
