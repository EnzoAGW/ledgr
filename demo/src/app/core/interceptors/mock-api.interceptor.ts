import { HttpErrorResponse, HttpInterceptorFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { throwError, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ApiError, mockDb } from '../mock/mock-data';

const randomLatency = () => 200 + Math.random() * 350;

interface Route {
  method: string;
  pattern: RegExp;
  handle: (match: RegExpMatchArray, req: HttpRequest<any>) => unknown;
}

const routes: Route[] = [
  { method: 'POST', pattern: /^\/api\/auth\/login$/, handle: (_m, req) => mockDb.login(req.body) },

  { method: 'GET', pattern: /^\/api\/dashboard$/, handle: () => mockDb.getDashboard() },

  { method: 'GET', pattern: /^\/api\/accounts$/, handle: () => mockDb.getAccounts() },
  { method: 'POST', pattern: /^\/api\/accounts$/, handle: (_m, req) => mockDb.createAccount(req.body) },

  { method: 'GET', pattern: /^\/api\/categories$/, handle: () => mockDb.getCategories() },
  { method: 'POST', pattern: /^\/api\/categories$/, handle: (_m, req) => mockDb.createCategory(req.body) },

  { method: 'GET', pattern: /^\/api\/team$/, handle: () => mockDb.getTeam() },
  { method: 'POST', pattern: /^\/api\/team$/, handle: (_m, req) => mockDb.inviteTeamMember(req.body) },

  { method: 'GET', pattern: /^\/api\/transactions$/, handle: (_m, req) => mockDb.getTransactions(req.params) },
  { method: 'POST', pattern: /^\/api\/transactions$/, handle: (_m, req) => mockDb.createTransaction(req.body) },
  { method: 'PATCH', pattern: /^\/api\/transactions\/(\d+)\/status$/, handle: (m, req) => mockDb.updateTransactionStatus(Number(m[1]), req.body) },
];

export const mockApiInterceptor: HttpInterceptorFn = (req) => {
  const url = req.url.split('?')[0];
  const route = routes.find(r => r.method === req.method && url.match(r.pattern));

  if (!route) {
    return throwError(() => new HttpErrorResponse({ status: 404, url: req.url, error: { message: `No mock route for ${req.method} ${url}` } }))
      .pipe(delay(randomLatency()));
  }

  try {
    const match = url.match(route.pattern)!;
    const body = route.handle(match, req);
    return of(new HttpResponse({ status: 200, body: body ?? null })).pipe(delay(randomLatency()));
  } catch (e) {
    const status = e instanceof ApiError ? e.status : 400;
    const message = e instanceof Error ? e.message : 'Unexpected error.';
    return throwError(() => new HttpErrorResponse({ status, url: req.url, error: { message } }))
      .pipe(delay(randomLatency()));
  }
};
