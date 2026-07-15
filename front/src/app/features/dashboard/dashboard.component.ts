import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { NgxChartsModule, ScaleType, LegendPosition } from '@swimlane/ngx-charts';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardResult } from '../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  imports: [NgxChartsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private svc = inject(DashboardService);

  data = signal<DashboardResult | null>(null);
  loading = signal(true);

  volumeChartData = computed(() => {
    const d = this.data();
    if (!d) return [];
    return [
      { name: 'Income',  series: d.last7Days.map(v => ({ name: v.date, value: v.income })) },
      { name: 'Expense', series: d.last7Days.map(v => ({ name: v.date, value: v.expense })) },
    ];
  });

  categoryChartData = computed(() =>
    this.data()?.topCategories.map(c => ({ name: c.name, value: c.amount })) ?? []
  );

  colorScheme = {
    name: 'ledgr',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#4ade80', '#f87171', '#818cf8', '#fbbf24', '#34d399', '#60a5fa'],
  };

  legendPos = LegendPosition.Below;

  ngOnInit() {
    this.svc.get().subscribe({
      next: d => { this.data.set(d); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  fmt(n: number) {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(n);
  }
}
