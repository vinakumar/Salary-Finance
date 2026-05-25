export interface ProductResponse {
  id: number;
  name: string;
  price: number;
  description: string;
  categoryId: number;
  categoryName: string;
  createdAt: string;
  isActive: boolean;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreateProductRequest {
  name: string;
  price: number;
  categoryId: number;
  description?: string;
}

export interface UpdateProductRequest {
  name?: string;
  price?: number;
  description?: string;
}

export interface ProductTaxonomyNode {
  nodeId: number;
  label: string;
  depth: number;
  isLeaf: boolean;
  children?: ProductTaxonomyNode[];
}

export interface GenericPayload<T> {
  version: number;
  isActive: boolean;
  source: string;
  correlationId: string;
  data: T;
  createdAt: string;
}

export interface ProblemDetails {
  title: string;
  detail: string;
  status: number;
  instance: string;
  traceId?: string;
  errors?: any[];
}
