import { Page, Locator } from '@playwright/test';

export class ProductDetailPage {
  readonly page: Page;
  readonly heading: Locator;
  readonly editButton: Locator;
  readonly backButton: Locator;
  readonly deleteButton: Locator;
  readonly productName: Locator;
  readonly productPrice: Locator;
  readonly productCategory: Locator;
  readonly productDescription: Locator;
  readonly emptyState: Locator;

  constructor(page: Page) {
    this.page = page;
    this.heading = page.locator('mat-card-title');
    this.editButton = page.locator('button', { hasText: 'Edit' });
    this.backButton = page.locator('button', { hasText: 'Back' });
    this.deleteButton = page.locator('button', { hasText: 'Delete' });
    this.productName = page.locator('[data-testid="product-name"]');
    this.productPrice = page.locator('[data-testid="product-price"]');
    this.productCategory = page.locator('[data-testid="product-category"]');
    this.productDescription = page.locator('[data-testid="product-description"]');
    this.emptyState = page.locator('app-empty-state');
  }

  async goto(id: number) {
    await this.page.goto(`/products/${id}`);
  }

  async clickEdit() {
    await this.editButton.click();
  }

  async clickBack() {
    await this.backButton.click();
  }

  async clickDelete() {
    await this.deleteButton.click();
  }
}
