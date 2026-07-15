import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { TeamMember, InviteUserRequest } from '../models/team.model';

@Injectable({ providedIn: 'root' })
export class TeamService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/team`;

  getAll() {
    return this.http.get<TeamMember[]>(this.base);
  }

  invite(req: InviteUserRequest) {
    return this.http.post<{ id: number }>(this.base, req);
  }
}
