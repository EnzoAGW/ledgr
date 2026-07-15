import { Component, inject, signal, OnInit } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TeamService } from '../../core/services/team.service';
import { TeamMember } from '../../core/models/team.model';
import { InviteUserDialogComponent } from './invite-user-dialog/invite-user-dialog.component';

@Component({
  selector: 'app-team',
  imports: [MatTableModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './team.component.html',
  styleUrl: './team.component.scss',
})
export class TeamComponent implements OnInit {
  private svc = inject(TeamService);
  private dialog = inject(MatDialog);

  members = signal<TeamMember[]>([]);
  loading = signal(true);
  columns = ['name', 'email', 'role'];

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
