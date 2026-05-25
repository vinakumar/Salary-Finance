import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { ProductFormComponent } from './product-form.component';

describe('ProductFormComponent', () => {
  let component: ProductFormComponent;
  let fixture: ComponentFixture<ProductFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductFormComponent, NoopAnimationsModule, ReactiveFormsModule],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with empty values', () => {
    expect(component.form.get('name')?.value).toBe('');
    expect(component.form.get('price')?.value).toBeNull();
    expect(component.form.get('categoryId')?.value).toBeNull();
  });

  it('should validate required fields', () => {
    component.form.get('name')?.setValue('');
    component.form.get('price')?.setValue(null);
    component.form.get('categoryId')?.setValue(null);

    expect(component.form.valid).toBeFalse();
    expect(component.form.get('name')?.hasError('required')).toBeTrue();
    expect(component.form.get('price')?.hasError('required')).toBeTrue();
    expect(component.form.get('categoryId')?.hasError('required')).toBeTrue();
  });

  it('should validate price must be greater than 0', () => {
    component.form.get('price')?.setValue(0);
    expect(component.form.get('price')?.hasError('min')).toBeTrue();

    component.form.get('price')?.setValue(0.01);
    expect(component.form.get('price')?.hasError('min')).toBeFalse();
  });

  it('should report unsaved changes when form is dirty', () => {
    expect(component.hasUnsavedChanges()).toBeFalse();
    component.form.get('name')?.setValue('Changed');
    component.form.markAsDirty();
    expect(component.hasUnsavedChanges()).toBeTrue();
  });

  it('should be valid with all required fields', () => {
    component.form.patchValue({
      name: 'Test Product',
      price: 29.99,
      categoryId: 1,
      description: 'A test product',
    });
    expect(component.form.valid).toBeTrue();
  });
});
