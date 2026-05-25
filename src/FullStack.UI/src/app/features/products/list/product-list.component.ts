import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductStore } from '../store/product.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format.pipe';
import { RelativeDatePipe } from '../../../shared/pipes/relative-date.pipe';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    CurrencyFormatPipe,
    RelativeDatePipe,
  ],
  template: `
    <div class="page-container">
      <div class="header">
        <h1>Products</h1>
        <button mat-fab extended color="primary" (click)="onCreate()"><mat-icon>add</mat-icon> New Product</button>
      </div>

      @if (store.loading()) {
        <app-loading-spinner />
      } @else if (!store.hasProducts()) {
        <app-empty-state icon="inventory_2" title="No products yet" message="Create your first product to get started.">
          <button mat-flat-button color="primary" (click)="onCreate()">Create Product</button>
        </app-empty-state>
      } @else {
        <table mat-table [dataSource]="store.products()" matSort (matSortChange)="onSort()">
          <ng-container matColumnDef="id">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>ID</th>
            <td mat-cell *matCellDef="let row">{{ row.id }}</td>
          </ng-container>
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Name</th>
            <td mat-cell *matCellDef="let row">{{ row.name }}</td>
          </ng-container>
          <ng-container matColumnDef="price">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Price</th>
            <td mat-cell *matCellDef="let row">{{ row.price | currencyFormat }}</td>
          </ng-container>
          <ng-container matColumnDef="categoryName">
            <th mat-header-cell *matHeaderCellDef>Category</th>
            <td mat-cell *matCellDef="let row">{{ row.categoryName }}</td>
          </ng-container>
          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Created</th>
            <td mat-cell *matCellDef="let row">{{ row.createdAt | relativeDate }}</td>
          </ng-container>
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let row">
              <button mat-icon-button color="warn" (click)="onDelete(row, $event)">
                <mat-icon>delete</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr
            mat-row
            *matRowDef="let row; columns: displayedColumns"
            (click)="onRowClick(row)"
            class="clickable-row"
          ></tr>
        </table>

        <mat-paginator
          [length]="store.totalCount()"
          [pageSize]="store.pageSize()"
          [pageIndex]="store.currentPage() - 1"
          [pageSizeOptions]="[5, 10, 20, 50]"
          (page)="onPage($event)"
          showFirstLastButtons
        >
        </mat-paginator>
      }
    </div>
  `,
  styles: [
    `
      .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;
      }
      table {
        width: 100%;
      }
      .clickable-row:hover {
        background-color: rgba(0, 0, 0, 0.04);
        cursor: pointer;
      }
    `,
  ],
})
export class ProductListComponent implements OnInit {
  readonly store = inject(ProductStore);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  displayedColumns = ['id', 'name', 'price', 'categoryName', 'createdAt', 'actions'];

  ngOnInit() {
    this.store.loadProducts({ page: 1, pageSize: 20 });
  }

  onRowClick(row: { id: number }) {
    this.router.navigate(['/products', row.id]);
  }

  onCreate() {
    this.router.navigate(['/products', 'new']);
  }

  onDelete(row: { id: number; name: string }, event: Event) {
    event.stopPropagation();
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Product',
        message: `Are you sure you want to delete "${row.name}"?`,
        confirmText: 'Delete',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.store.deleteProduct(row.id);
        this.snackBar.open('Product deleted', 'Dismiss', { duration: 3000 });
        setTimeout(() => this.store.loadProducts({ page: 1 }), 500);
      }
    });
  }

  onPage(event: PageEvent) {
    this.store.loadProducts({ page: event.pageIndex + 1, pageSize: event.pageSize });
  }

  onSort() {
    this.store.loadProducts({ page: 1 });
  }
}
