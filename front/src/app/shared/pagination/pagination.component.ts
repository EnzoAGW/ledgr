import { Component, Input, Output, EventEmitter } from '@angular/core';

export interface SimplePageEvent {
  pageIndex: number;
  pageSize: number;
  length: number;
}

@Component({
  selector: 'app-pagination',
  standalone: true,
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss',
})
export class PaginationComponent {
  @Input() length = 0;
  @Input() pageSize = 10;
  @Input() pageIndex = 0;
  @Output() pageChange = new EventEmitter<SimplePageEvent>();

  pageSizeOptions = [10, 20, 50];
  protected readonly Math = Math;

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.length / this.pageSize));
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i);
  }

  goTo(page: number) {
    if (page < 0 || page >= this.totalPages || page === this.pageIndex) return;
    this.pageChange.emit({ pageIndex: page, pageSize: this.pageSize, length: this.length });
  }

  onPageSizeChange(value: string) {
    this.pageChange.emit({ pageIndex: 0, pageSize: +value, length: this.length });
  }
}
