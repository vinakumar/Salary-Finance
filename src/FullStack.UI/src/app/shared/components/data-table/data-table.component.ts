import { Component, Input, Output, EventEmitter, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatSortModule],
  template: `
    <div class="table-container">
      <table mat-table [dataSource]="dataSource" matSort (matSortChange)="onSortChange($event)">
        <ng-content></ng-content>
        <tr mat-header-row *matHeaderRowDef="columns"></tr>
        <tr mat-row *matRowDef="let row; columns: columns" (click)="rowClick.emit(row)"></tr>
      </table>
      <mat-paginator
        [length]="totalCount"
        [pageSize]="pageSize"
        [pageIndex]="pageIndex"
        [pageSizeOptions]="[5, 10, 20, 50]"
        (page)="onPageChange($event)"
        showFirstLastButtons
      >
      </mat-paginator>
    </div>
  `,
  styles: [
    `
      .table-container {
        width: 100%;
        overflow-x: auto;
      }
      table {
        width: 100%;
      }
      tr.mat-mdc-row:hover {
        background-color: rgba(0, 0, 0, 0.04);
        cursor: pointer;
      }
    `,
  ],
})
export class DataTableComponent<T> implements AfterViewInit {
  @Input() set data(value: T[]) {
    this.dataSource.data = value;
  }
  @Input() columns: string[] = [];
  @Input() totalCount = 0;
  @Input() pageSize = 20;
  @Input() pageIndex = 0;
  @Output() pageChange = new EventEmitter<PageEvent>();
  @Output() sortChange = new EventEmitter<Sort>();
  @Output() rowClick = new EventEmitter<T>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  dataSource = new MatTableDataSource<T>([]);

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  onPageChange(event: PageEvent) {
    this.pageChange.emit(event);
  }

  onSortChange(event: Sort) {
    this.sortChange.emit(event);
  }
}
