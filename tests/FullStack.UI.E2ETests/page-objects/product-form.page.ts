import { Page, Locator } from '@playwright/test';

export class ProductFormPage {
  readonly page: Page;
  readonly nameInput: Locator;
  readonly priceInput: Locator;
  readonly categorySelect: Locator;
  readonly descriptionInput: Locator;
  readonly submitButton: Locator;
  readonly cancelButton: Locator;
  readonly backButton: Locator;
  readonly nameError: Locator;
  readonly priceError: Locator;
  readonly categoryError: Locator;

  constructor(page: Page) {
    this.page = page;
    this.nameInput = page.locator('input[formControlName="name"]');
    this.priceInput = page.locator('input[formControlName="price"]');
    this.categorySelect = page.locator('mat-select[formControlName="categoryId"]');
    this.descriptionInput = page.locator('textarea[formControlName="description"]');
    this.submitButton = page.locator('button[type="submit"]');
    this.cancelButton = page.locator('button', { hasText: 'Cancel' });
    this.backButton = page.locator('button', { hasText: 'Back' });
    this.nameError = page.locator('mat-error', { hasText: 'Name is required' });
    this.priceError = page.locator('mat-error', { hasText: 'Price is required' });
    this.categoryError = page.locator('mat-error', { hasText: 'Category is required' });
  }

  async goto() {
    await this.page.goto('/products/new');
  }

  async gotoEdit(id: number) {
    await this.page.goto(`/products/${id}/edit`);
  }

  async fillForm(data: { name?: string; price?: string; categoryIndex?: number; description?: string }) {
    if (data.name !== undefined) {
      await this.nameInput.fill(data.name);
    }
    if (data.price !== undefined) {
      await this.priceInput.fill(data.price);
    }
    if (data.categoryIndex !== undefined) {
      await this.categorySelect.click();
      const options = this.page.locator('mat-option');
      await options.nth(data.categoryIndex).click();
    }
    if (data.description !== undefined) {
      await this.descriptionInput.fill(data.description);
    }
  }

  async submit() {
    await this.submitButton.click();
  }

  async cancel() {
    await this.cancelButton.click();
  }

  async isSubmitDisabled(): Promise<boolean> {
    return this.submitButton.isDisabled();
  }

  async triggerValidation() {
    await this.nameInput.focus();
    await this.nameInput.blur();
    await this.priceInput.focus();
    await this.priceInput.blur();
  }
}
