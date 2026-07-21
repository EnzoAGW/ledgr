import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PaginationComponent, SimplePageEvent } from '../../shared/pagination/pagination.component';
import { TeamService } from '../../core/services/team.service';
import { TeamMember } from '../../core/models/team.model';
import { InviteUserDialogComponent } from './invite-user-dialog/invite-user-dialog.component';

@Component({
  selector: 'app-team',
  imports: [MatTableModule, MatButtonModule, MatIconModule, MatDialogModule, PaginationComponent],
  templateUrl: './team.component.html',
  styleUrl: './team.component.scss',
})
export class TeamComponent implements OnInit {
  private svc = inject(TeamService);
  private dialog = inject(MatDialog);

  members = signal<TeamMember[]>([]);
  loading = signal(true);
  columns = ['name', 'email', 'role'];

  pageIndex = signal(0);
  pageSize = signal(10);
  paged = computed(() => {
    const start = this.pageIndex() * this.pageSize();
    return this.members().slice(start, start + this.pageSize());
  });

  onPage(e: SimplePageEvent) {
    this.pageIndex.set(e.pageIndex);
    this.pageSize.set(e.pageSize);
  }

  ngOnInit() { this.load(); }

  load() {
    this.svc.getAll().subscribe({
      next: m => { this.members.set(m); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openInvite() {
    this.dialog.open(InviteUserDialogComponent, { width: '420px' })
      .afterClosed().subscribe(ok => { if (ok) this.load(); });
  }
}
