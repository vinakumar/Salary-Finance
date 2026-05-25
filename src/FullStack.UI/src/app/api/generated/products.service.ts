import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateProductRequest, PagedResponse, ProductResponse, UpdateProductRequest } from './models';

export interface GetProductsParams {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  ascending?: boolean;
}

export interface CategoryResponse {
  id: number;
  name: string;
  slug: string;
}

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/products';

  getProducts(params: GetProductsParams = {}): Observable<PagedResponse<ProductResponse>> {
    const queryParams = new HttpParams({
      fromObject: {
        PageNumber: String(params.pageNumber ?? 1),
        PageSize: String(params.pageSize ?? 20),
        SortBy: params.sortBy ?? 'name',
        Ascending: String(params.ascending ?? true),
      },
    });

    return this.http.get<PagedResponse<ProductResponse>>(this.baseUrl, { params: queryParams });
  }

  getProduct(id: number): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(`${this.baseUrl}/${id}`);
  }

  createProduct(req: CreateProductRequest): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.baseUrl, req);
  }

  updateProduct(id: number, req: UpdateProductRequest): Observable<ProductResponse> {
    return this.http.put<ProductResponse>(`${this.baseUrl}/${id}`, req);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getCategories(): Observable<CategoryResponse[]> {
    return this.http.get<CategoryResponse[]>('/api/v1/categories');
  }
}
