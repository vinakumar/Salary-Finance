import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductStore } from '../store/product.store';
import { ProductsService, CategoryResponse } from '../../../api/generated/products.service';
import { HasUnsavedChanges } from '../../../core/guards/can-deactivate.guard';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <div class="page-container">
      <button mat-button (click)="goBack()"><mat-icon>arrow_back</mat-icon> Back</button>
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{ isEdit ? 'Edit Product' : 'Create Product' }}</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Name</mat-label>
              <input matInput formControlName="name" placeholder="Product name" />
              @if (form.get('name')?.hasError('required')) {
                <mat-error>Name is required</mat-error>
              }
              @if (form.get('name')?.hasError('maxlength')) {
                <mat-error>Name must not exceed 200 characters</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Price</mat-label>
              <input matInput type="number" formControlName="price" placeholder="0.00" />
              @if (form.get('price')?.hasError('required')) {
                <mat-error>Price is required</mat-error>
              }
              @if (form.get('price')?.hasError('min')) {
                <mat-error>Price must be greater than 0</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Category</mat-label>
              <mat-select formControlName="categoryId">
                @for (cat of categories; track cat.id) {
                  <mat-option [value]="cat.id">{{ cat.name }}</mat-option>
                }
              </mat-select>
              @if (form.get('categoryId')?.hasError('required')) {
                <mat-error>Category is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="4" placeholder="Product description"></textarea>
              @if (form.get('description')?.hasError('maxlength')) {
                <mat-error>Description must not exceed 2000 characters</mat-error>
              }
            </mat-form-field>

            <div class="actions">
              <button mat-button type="button" (click)="goBack()">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || store.loading()">
                {{ isEdit ? 'Update' : 'Create' }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [
    `
      mat-card {
        margin-top: 16px;
      }
      .full-width {
        width: 100%;
        margin-bottom: 8px;
      }
      .actions {
        display: flex;
        gap: 8px;
        justify-content: flex-end;
        margin-top: 16px;
      }
    `,
  ],
})
export class ProductFormComponent implements OnInit, HasUnsavedChanges {
  readonly store = inject(ProductStore);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly snackBar = inject(MatSnackBar);
  private readonly productsService = inject(ProductsService);

  form!: FormGroup;
  isEdit = false;
  private productId?: number;
  categories: CategoryResponse[] = [];

  ngOnInit() {
    this.productsService.getCategories().subscribe((cats) => {
      this.categories = cats;
    });

    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      price: [null, [Validators.required, Validators.min(0.01)]],
      categoryId: [null, [Validators.required]],
      description: ['', [Validators.maxLength(2000)]],
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.isEdit = true;
      this.productId = Number(id);
      this.loadProduct();
    }
  }

  hasUnsavedChanges(): boolean {
    return this.form.dirty;
  }

  private loadProduct() {
    if (!this.productId) return;
    this.productsService.getProduct(this.productId).subscribe((product) => {
      this.form.patchValue({
        name: product.name,
        price: product.price,
        categoryId: product.categoryId,
        description: product.description,
      });
      this.form.markAsPristine();
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    const value = this.form.value;

    if (this.isEdit && this.productId) {
      this.productsService
        .updateProduct(this.productId, {
          name: value.name,
          price: value.price,
          description: value.description,
        })
        .subscribe({
          next: () => {
            this.snackBar.open('Product updated successfully', 'Dismiss', { duration: 3000 });
            this.form.markAsPristine();
            this.router.navigate(['/products', this.productId]);
          },
          error: () => this.snackBar.open('Failed to update product', 'Dismiss', { duration: 3000 }),
        });
    } else {
      this.productsService
        .createProduct({
          name: value.name,
          price: value.price,
          categoryId: value.categoryId,
          description: value.description,
        })
        .subscribe({
          next: (created) => {
            this.snackBar.open('Product created successfully', 'Dismiss', { duration: 3000 });
            this.form.markAsPristine();
            this.router.navigate(['/products', created.id]);
          },
          error: () => this.snackBar.open('Failed to create product', 'Dismiss', { duration: 3000 }),
        });
    }
  }

  goBack() {
    this.router.navigate(['/products']);
  }
}
