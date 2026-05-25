import { Page, Locator, expect } from '@playwright/test';

export class ProductListPage {
  readonly page: Page;
  readonly heading: Locator;
  readonly createButton: Locator;
  readonly table: Locator;
  readonly rows: Locator;
  readonly emptyState: Locator;
  readonly loadingSpinner: Locator;

  constructor(page: Page) {
    this.page = page;
    this.heading = page.locator('h1', { hasText: 'Products' });
    this.createButton = page.locator('button', { hasText: 'New Product' });
    this.table = page.locator('table[mat-table]');
    this.rows = page.locator('tr[mat-row]');
    this.emptyState = page.locator('app-empty-state');
    this.loadingSpinner = page.locator('app-loading-spinner');
  }

  async goto() {
    await this.page.goto('/products');
  }

  async waitForLoad() {
    await this.page.waitForResponse(resp =>
      resp.url().includes('/api/v1/products') && resp.status() === 200
    );
  }

  async getRowCount(): Promise<number> {
    return this.rows.count();
  }

  async clickCreate() {
    await this.createButton.click();
  }

  async clickRow(index: number) {
    await this.rows.nth(index).click();
  }

  async deleteRow(index: number) {
    const row = this.rows.nth(index);
    const deleteButton = row.locator('button[color="warn"]');
    await deleteButton.click();
  }

  async confirmDelete() {
    const dialog = this.page.locator('app-confirm-dialog');
    await dialog.locator('button', { hasText: 'Delete' }).click();
  }

  async getProductName(index: number): Promise<string> {
    const row = this.rows.nth(index);
    const nameCell = row.locator('td').nth(1);
    return nameCell.innerText();
  }
}
