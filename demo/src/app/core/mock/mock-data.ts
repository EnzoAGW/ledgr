import { Account, CreateAccountRequest } from '../models/account.model';
import { LoginRequest, LoginResponse, UserRole } from '../models/auth.model';
import { Category, CreateCategoryRequest } from '../models/category.model';
import { CategoryBreakdown, DailyVolume, DashboardResult } from '../models/dashboard.model';
import { InviteUserRequest, TeamMember } from '../models/team.model';
import {
  CreateTransactionRequest, PaginatedResult, Transaction,
  TransactionStatus, TransactionType, UpdateTransactionStatusRequest
} from '../models/transaction.model';

export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message);
  }
}

interface MockUser {
  id: number;
  name: string;
  email: string;
  password: string;
  role: UserRole;
}

const ORG_ID = 1;

const USERS: MockUser[] = [
  { id: 1, name: 'Marina Costa', email: 'admin@ledgr.dev', password: 'Admin@123', role: 'Admin' },
  { id: 2, name: 'Diego Ramos', email: 'manager@ledgr.dev', password: 'Manager@123', role: 'Manager' },
  { id: 3, name: 'Julia Nakamura', email: 'analyst@ledgr.dev', password: 'Analyst@123', role: 'Analyst' },
];

let team: TeamMember[] = [
  { id: 1, name: 'Marina Costa', email: 'admin@ledgr.dev', role: 'Admin' },
  { id: 2, name: 'Diego Ramos', email: 'manager@ledgr.dev', role: 'Manager' },
  { id: 3, name: 'Julia Nakamura', email: 'analyst@ledgr.dev', role: 'Analyst' },
  { id: 4, name: 'Pedro Alves', email: 'pedro@ledgr.dev', role: 'Analyst' },
  { id: 5, name: 'Sofia Lindqvist', email: 'sofia@ledgr.dev', role: 'Manager' },
];
let nextTeamId = team.length + 1;

let accounts: Account[] = [
  { id: 1, name: 'Main Checking', type: 'Checking', balance: 48250.30 },
  { id: 2, name: 'Business Savings', type: 'Savings', balance: 120000.00 },
  { id: 3, name: 'Corporate Credit Card', type: 'Credit', balance: -8340.15 },
  { id: 4, name: 'Payroll Account', type: 'Checking', balance: 15600.00 },
];
let nextAccountId = accounts.length + 1;

let categories: Category[] = [
  { id: 1, name: 'Client Payments', type: 'Income', color: '#22c55e' },
  { id: 2, name: 'Consulting Revenue', type: 'Income', color: '#06b6d4' },
  { id: 3, name: 'Interest & Investments', type: 'Income', color: '#84cc16' },
  { id: 4, name: 'Payroll', type: 'Expense', color: '#ef4444' },
  { id: 5, name: 'Software & Tools', type: 'Expense', color: '#6366f1' },
  { id: 6, name: 'Office & Rent', type: 'Expense', color: '#f59e0b' },
  { id: 7, name: 'Marketing', type: 'Expense', color: '#ec4899' },
  { id: 8, name: 'Taxes', type: 'Expense', color: '#64748b' },
];
let nextCategoryId = categories.length + 1;

function daysAgo(n: number): Date {
  const d = new Date();
  d.setHours(9, 0, 0, 0);
  d.setDate(d.getDate() - n);
  return d;
}

function categoryOf(id: number): Category {
  return categories.find(c => c.id === id)!;
}

function accountOf(id: number): Account {
  return accounts.find(a => a.id === id)!;
}

const INCOME_DESCRIPTIONS: Record<string, string[]> = {
  'Client Payments': ['Invoice #{n} — Acme Retail', 'Invoice #{n} — Bluepeak Logistics', 'Invoice #{n} — Nortown Clinic'],
  'Consulting Revenue': ['Consulting retainer — Q{q}', 'Advisory session block', 'Strategy workshop fee'],
  'Interest & Investments': ['Savings account interest', 'CDB investment yield'],
};

const EXPENSE_DESCRIPTIONS: Record<string, string[]> = {
  'Payroll': ['Payroll run — {month}', 'Contractor payout — {month}'],
  'Software & Tools': ['AWS subscription', 'GitHub Enterprise', 'Figma subscription', 'Notion subscription', 'Vercel Pro plan'],
  'Office & Rent': ['Coworking rent', 'Utilities', 'Office supplies'],
  'Marketing': ['Google Ads campaign', 'Instagram Ads campaign', 'Content agency retainer'],
  'Taxes': ['Quarterly tax payment', 'Municipal service tax'],
};

function pick<T>(arr: T[]): T {
  return arr[Math.floor(Math.random() * arr.length)];
}

function buildDescription(category: Category, dayOffset: number): string {
  const pool = category.type === 'Income' ? INCOME_DESCRIPTIONS[category.name] : EXPENSE_DESCRIPTIONS[category.name];
  const template = pick(pool ?? ['Transaction']);
  const d = daysAgo(dayOffset);
  const month = d.toLocaleString('en-US', { month: 'long' });
  const quarter = Math.floor(d.getMonth() / 3) + 1;
  return template
    .replace('{n}', String(1000 + Math.floor(Math.random() * 9000)))
    .replace('{month}', month)
    .replace('{q}', String(quarter));
}

function amountFor(category: Category): number {
  switch (category.name) {
    case 'Payroll': return round2(3200 + Math.random() * 5800);
    case 'Client Payments': return round2(1500 + Math.random() * 13500);
    case 'Consulting Revenue': return round2(2000 + Math.random() * 6000);
    case 'Interest & Investments': return round2(80 + Math.random() * 420);
    case 'Office & Rent': return round2(600 + Math.random() * 1400);
    case 'Marketing': return round2(300 + Math.random() * 2700);
    case 'Taxes': return round2(500 + Math.random() * 4500);
    default: return round2(50 + Math.random() * 950); // Software & Tools
  }
}

function round2(n: number): number {
  return Math.round(n * 100) / 100;
}

let transactions: Transaction[] = [];
let nextTransactionId = 1;

function seedTransactions() {
  const incomeCategories = categories.filter(c => c.type === 'Income');
  const expenseCategories = categories.filter(c => c.type === 'Expense');
  const cashAccounts = accounts.filter(a => a.type !== 'Credit');
  const creditAccount = accounts.find(a => a.type === 'Credit')!;

  for (let dayOffset = 60; dayOffset >= 0; dayOffset--) {
    const entriesToday = Math.random() < 0.55 ? (Math.random() < 0.85 ? 1 : 2) : 0;

    for (let i = 0; i < entriesToday; i++) {
      const isIncome = Math.random() < 0.35;
      const category = isIncome ? pick(incomeCategories) : pick(expenseCategories);
      const account = isIncome || Math.random() < 0.6 ? pick(cashAccounts) : creditAccount;

      let status: TransactionStatus = 'Confirmed';
      if (dayOffset <= 3) status = Math.random() < 0.5 ? 'Pending' : 'Confirmed';
      else if (Math.random() < 0.04) status = 'Cancelled';

      const createdAt = daysAgo(dayOffset);

      transactions.push({
        id: nextTransactionId++,
        amount: amountFor(category),
        type: category.type as TransactionType,
        status,
        date: createdAt.toISOString(),
        description: buildDescription(category, dayOffset),
        accountId: account.id,
        accountName: account.name,
        categoryId: category.id,
        categoryName: category.name,
        categoryColor: category.color,
      });
    }
  }

  transactions.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
}
seedTransactions();

function sameDay(a: Date, b: Date): boolean {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}

function toDateOnly(d: Date): Date {
  const copy = new Date(d);
  copy.setHours(0, 0, 0, 0);
  return copy;
}

export const mockDb = {
  login(body: LoginRequest): LoginResponse {
    const user = USERS.find(u => u.email.toLowerCase() === body.email?.toLowerCase() && u.password === body.password);
    if (!user) throw new ApiError(401, 'Invalid email or password.');

    const fakeToken = `demo.${btoa(JSON.stringify({ sub: user.id, exp: Date.now() + 12 * 3600_000 }))}.mock`;
    return { token: fakeToken, userId: user.id, name: user.name, role: user.role, orgId: ORG_ID };
  },

  getAccounts(): Account[] {
    return [...accounts];
  },

  createAccount(body: CreateAccountRequest): { id: number } {
    const account: Account = { id: nextAccountId++, name: body.name, type: body.type, balance: 0 };
    accounts.push(account);
    return { id: account.id };
  },

  getCategories(): Category[] {
    return [...categories];
  },

  createCategory(body: CreateCategoryRequest): { id: number } {
    const category: Category = { id: nextCategoryId++, name: body.name, type: body.type, color: body.color };
    categories.push(category);
    return { id: category.id };
  },

  getTeam(): TeamMember[] {
    return [...team];
  },

  inviteTeamMember(body: InviteUserRequest): { id: number } {
    if (team.some(t => t.email.toLowerCase() === body.email.toLowerCase())) {
      throw new ApiError(409, 'A team member with this email already exists.');
    }
    const member: TeamMember = { id: nextTeamId++, name: body.name, email: body.email, role: body.role };
    team.push(member);
    return { id: member.id };
  },

  getTransactions(params: { get(name: string): string | null }): PaginatedResult<Transaction> {
    const page = Number(params.get('page') ?? 1);
    const pageSize = Number(params.get('pageSize') ?? 20);
    const search = params.get('search')?.toLowerCase();
    const accountId = params.get('accountId');
    const categoryId = params.get('categoryId');
    const type = params.get('type');
    const status = params.get('status');
    const from = params.get('from');
    const to = params.get('to');

    let items = [...transactions];
    if (search) items = items.filter(t =>
      t.description?.toLowerCase().includes(search) ||
      t.accountName.toLowerCase().includes(search) ||
      (t.categoryName?.toLowerCase().includes(search) ?? false)
    );
    if (accountId) items = items.filter(t => t.accountId === Number(accountId));
    if (categoryId) items = items.filter(t => t.categoryId === Number(categoryId));
    if (type) items = items.filter(t => t.type === type);
    if (status) items = items.filter(t => t.status === status);
    if (from) items = items.filter(t => new Date(t.date) >= new Date(from));
    if (to) items = items.filter(t => new Date(t.date) <= new Date(to));

    items.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

    const totalCount = items.length;
    const start = (page - 1) * pageSize;
    return { items: items.slice(start, start + pageSize), totalCount, page, pageSize };
  },

  createTransaction(body: CreateTransactionRequest): { id: number } {
    const account = accountOf(body.accountId);
    if (!account) throw new ApiError(400, 'Invalid account.');
    const category = body.categoryId ? categoryOf(body.categoryId) : undefined;
    if (body.categoryId && !category) throw new ApiError(400, 'Invalid category.');

    const tx: Transaction = {
      id: nextTransactionId++,
      amount: body.amount,
      type: body.type,
      status: 'Pending',
      date: body.date,
      description: body.description,
      accountId: account.id,
      accountName: account.name,
      categoryId: category?.id,
      categoryName: category?.name,
      categoryColor: category?.color,
    };
    transactions.unshift(tx);
    return { id: tx.id };
  },

  updateTransactionStatus(id: number, body: UpdateTransactionStatusRequest): void {
    const tx = transactions.find(t => t.id === id);
    if (!tx) throw new ApiError(404, 'Transaction not found.');

    if (tx.status === 'Pending' && body.status === 'Confirmed') {
      const account = accountOf(tx.accountId);
      account.balance = round2(account.balance + (tx.type === 'Income' ? tx.amount : -tx.amount));
    }
    tx.status = body.status;
  },

  getDashboard(): DashboardResult {
    const now = new Date();
    const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);

    const confirmed = transactions.filter(t => t.status === 'Confirmed');
    const confirmedThisMonth = confirmed.filter(t => new Date(t.date) >= monthStart);

    const totalIncome = confirmedThisMonth.filter(t => t.type === 'Income').reduce((s, t) => s + t.amount, 0);
    const totalExpense = confirmedThisMonth.filter(t => t.type === 'Expense').reduce((s, t) => s + t.amount, 0);
    const balance = accounts.reduce((s, a) => s + a.balance, 0);
    const pendingCount = transactions.filter(t => t.status === 'Pending').length;

    const last7Days: DailyVolume[] = [];
    for (let i = 6; i >= 0; i--) {
      const day = toDateOnly(daysAgo(i));
      const dayTx = confirmed.filter(t => sameDay(new Date(t.date), day));
      last7Days.push({
        date: day.toISOString(),
        income: round2(dayTx.filter(t => t.type === 'Income').reduce((s, t) => s + t.amount, 0)),
        expense: round2(dayTx.filter(t => t.type === 'Expense').reduce((s, t) => s + t.amount, 0)),
      });
    }

    const byCategory = new Map<number, CategoryBreakdown>();
    for (const t of confirmed.filter(t => t.type === 'Expense' && t.categoryId)) {
      const existing = byCategory.get(t.categoryId!);
      if (existing) existing.amount += t.amount;
      else byCategory.set(t.categoryId!, { name: t.categoryName!, color: t.categoryColor!, amount: t.amount });
    }
    const topCategories = [...byCategory.values()]
      .map(c => ({ ...c, amount: round2(c.amount) }))
      .sort((a, b) => b.amount - a.amount)
      .slice(0, 5);

    return {
      totalIncome: round2(totalIncome),
      totalExpense: round2(totalExpense),
      balance: round2(balance),
      pendingCount,
      last7Days,
      topCategories,
    };
  },
};
