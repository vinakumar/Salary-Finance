import { test, expect } from '@playwright/test';
import { ProductListPage } from '../page-objects/product-list.page';
import { ProductFormPage } from '../page-objects/product-form.page';
import { ProductDetailPage } from '../page-objects/product-detail.page';

test.describe('Products CRUD', () => {
  let listPage: ProductListPage;
  let formPage: ProductFormPage;
  let detailPage: ProductDetailPage;

  test.beforeEach(async ({ page }) => {
    listPage = new ProductListPage(page);
    formPage = new ProductFormPage(page);
    detailPage = new ProductDetailPage(page);
  });

  test('should display product list', async ({ page }) => {
    await listPage.goto();
    await expect(listPage.heading).toBeVisible();
    await expect(listPage.createButton).toBeVisible();
    // Either table or empty state should be visible
    const hasTable = await listPage.table.isVisible().catch(() => false);
    const hasEmpty = await listPage.emptyState.isVisible().catch(() => false);
    expect(hasTable || hasEmpty).toBeTruthy();
  });

  test('should create a new product', async ({ page }) => {
    await formPage.goto();
    await expect(formPage.nameInput).toBeVisible();

    const productName = `E2E Test Product ${Date.now()}`;
    await formPage.fillForm({
      name: productName,
      price: '19.99',
      categoryIndex: 0,
      description: 'Test product for E2E validation'
    });

    await expect(formPage.submitButton).toBeEnabled();
    await formPage.submit();

    // Should navigate back to list or detail
    await page.waitForURL(/\/products/);
    await expect(page.locator('body')).toContainText(productName);
  });

  test('should show validation errors on empty form submit', async ({ page }) => {
    await formPage.goto();
    await formPage.triggerValidation();

    // Submit button should be disabled when form is invalid
    await expect(formPage.submitButton).toBeDisabled();
  });

  test('should navigate to product detail', async ({ page }) => {
    await listPage.goto();
    const hasRows = await listPage.rows.first().isVisible({ timeout: 5000 }).catch(() => false);
    if (hasRows) {
      await listPage.clickRow(0);
      await page.waitForURL(/\/products\/\d+/);
      await expect(detailPage.heading).toBeVisible();
    }
  });

  test('should delete a product with confirmation', async ({ page }) => {
    // First create a product to delete
    await formPage.goto();
    const productName = `Delete Me ${Date.now()}`;
    await formPage.fillForm({
      name: productName,
      price: '5.00',
      categoryIndex: 0,
      description: 'Will be deleted'
    });
    await formPage.submit();
    await page.waitForURL(/\/products/);

    // Now go to list and delete it
    await listPage.goto();
    const rowCount = await listPage.getRowCount();
    if (rowCount > 0) {
      await listPage.deleteRow(0);
      await listPage.confirmDelete();

      // Wait for snackbar or row removal
      await page.waitForTimeout(1000);
    }
  });
});
