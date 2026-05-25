import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { ProductStore } from '../store/product.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format.pipe';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    CurrencyFormatPipe,
  ],
  template: `
    <div class="page-container">
      @if (store.loading()) {
        <app-loading-spinner />
      } @else if (!store.selectedProduct()) {
        <app-empty-state
          icon="search_off"
          title="Product not found"
          message="The product you're looking for doesn't exist."
        >
          <button mat-flat-button color="primary" (click)="goBack()">Back to List</button>
        </app-empty-state>
      } @else {
        <button mat-button (click)="goBack()"><mat-icon>arrow_back</mat-icon> Back</button>
        <mat-card>
          <mat-card-header>
            <mat-card-title>{{ store.selectedProduct()!.name }}</mat-card-title>
            <mat-card-subtitle>
              <mat-chip-set>
                <mat-chip>{{ store.selectedProduct()!.categoryName }}</mat-chip>
                @if (store.selectedProduct()!.isActive) {
                  <mat-chip color="primary" highlighted>Active</mat-chip>
                }
              </mat-chip-set>
            </mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <div class="detail-grid">
              <div class="detail-item">
                <label>Price</label>
                <span class="price">{{ store.selectedProduct()!.price | currencyFormat }}</span>
              </div>
              <div class="detail-item">
                <label>Description</label>
                <p>{{ store.selectedProduct()!.description || 'No description' }}</p>
              </div>
              <div class="detail-item">
                <label>Created</label>
                <span>{{ store.selectedProduct()!.createdAt | date: 'medium' }}</span>
              </div>
              <div class="detail-item">
                <label>Product ID</label>
                <span>{{ store.selectedProduct()!.id }}</span>
              </div>
            </div>
          </mat-card-content>
          <mat-card-actions>
            <button mat-flat-button color="primary" (click)="onEdit()"><mat-icon>edit</mat-icon> Edit</button>
          </mat-card-actions>
        </mat-card>
      }
    </div>
  `,
  styles: [
    `
      mat-card {
        margin-top: 16px;
      }
      .detail-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 24px;
        padding: 16px 0;
      }
      .detail-item label {
        display: block;
        font-size: 12px;
        color: rgba(0, 0, 0, 0.54);
        margin-bottom: 4px;
      }
      .price {
        font-size: 24px;
        font-weight: 500;
      }
    `,
  ],
})
export class ProductDetailComponent implements OnInit {
  readonly store = inject(ProductStore);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.store.loadById(id);
    }
  }

  goBack() {
    this.router.navigate(['/products']);
  }

  onEdit() {
    const id = this.store.selectedProduct()?.id;
    if (id) {
      this.router.navigate(['/products', id, 'edit']);
    }
  }
}
