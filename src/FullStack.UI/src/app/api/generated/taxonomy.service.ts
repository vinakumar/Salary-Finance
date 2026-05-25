import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GenericPayload, ProductTaxonomyNode } from './models';

@Injectable({ providedIn: 'root' })
export class TaxonomyService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/taxonomy';

  getTaxonomyNodes(): Observable<ProductTaxonomyNode[]> {
    return this.http.get<ProductTaxonomyNode[]>(this.baseUrl);
  }

  getTaxonomyNode(id: number): Observable<GenericPayload<ProductTaxonomyNode>> {
    return this.http.get<GenericPayload<ProductTaxonomyNode>>(`${this.baseUrl}/${id}`);
  }
}
