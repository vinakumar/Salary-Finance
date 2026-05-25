import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { Router } from '@angular/router';
import { ProductListComponent } from './product-list.component';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let httpMock: HttpTestingController;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductListComponent, NoopAnimationsModule],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    httpMock.match(() => true).forEach((req) => req.flush([]));
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have correct displayed columns', () => {
    expect(component.displayedColumns).toEqual(['id', 'name', 'price', 'categoryName', 'createdAt', 'actions']);
  });

  it('should load products on init', () => {
    fixture.detectChanges();
    const req = httpMock.expectOne((r) => r.url.includes('/api/v1/products'));
    expect(req.request.method).toBe('GET');
    req.flush({ items: [], totalCount: 0 });
  });

  it('should navigate to detail on row click', () => {
    const routerSpy = spyOn(router, 'navigate');
    component.onRowClick({ id: 5 });
    expect(routerSpy).toHaveBeenCalledWith(['/products', 5]);
  });

  it('should navigate to create form on create click', () => {
    const routerSpy = spyOn(router, 'navigate');
    component.onCreate();
    expect(routerSpy).toHaveBeenCalledWith(['/products', 'new']);
  });
});
