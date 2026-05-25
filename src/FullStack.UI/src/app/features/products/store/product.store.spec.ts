import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ProductStore } from './product.store';

describe('ProductStore', () => {
  let httpMock: HttpTestingController;
  let store: InstanceType<typeof ProductStore>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), ProductStore],
    });
    store = TestBed.inject(ProductStore);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.match(() => true).forEach((req) => req.flush(null));
    httpMock.verify();
  });

  it('should have initial state', () => {
    expect(store.products()).toEqual([]);
    expect(store.loading()).toBeFalse();
    expect(store.totalCount()).toBe(0);
  });

  it('should load products successfully', () => {
    const mockResponse = {
      items: [
        {
          id: 1,
          name: 'Test',
          price: 10,
          description: '',
          categoryId: 1,
          categoryName: 'Cat',
          createdAt: '2024-01-01',
          isActive: true,
        },
      ],
      totalCount: 1,
      pageNumber: 1,
      pageSize: 20,
      hasNextPage: false,
      hasPreviousPage: false,
    };

    store.loadProducts({ page: 1, pageSize: 20 });

    const req = httpMock.expectOne((r) => r.url === '/api/v1/products');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);

    expect(store.products().length).toBe(1);
    expect(store.totalCount()).toBe(1);
    expect(store.loading()).toBeFalse();
  });

  it('should load product by id', () => {
    const mockProduct = {
      id: 1,
      name: 'Test',
      price: 10,
      description: '',
      categoryId: 1,
      categoryName: 'Cat',
      createdAt: '2024-01-01',
      isActive: true,
    };

    store.loadById(1);

    const req = httpMock.expectOne('/api/v1/products/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockProduct);

    expect(store.selectedProduct()?.id).toBe(1);
    expect(store.loading()).toBeFalse();
  });

  it('should handle error on load', () => {
    store.loadProducts({ page: 1, pageSize: 20 });

    const req = httpMock.expectOne((r) => r.url === '/api/v1/products');
    req.flush('Error', { status: 500, statusText: 'Server Error' });

    expect(store.loading()).toBeFalse();
    expect(store.error()).toBeTruthy();
  });

  it('should delete product', () => {
    store.deleteProduct(1);

    const req = httpMock.expectOne('/api/v1/products/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null, { status: 204, statusText: 'No Content' });

    expect(store.loading()).toBeFalse();
  });
});
