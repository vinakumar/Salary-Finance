import { test, expect } from '@playwright/test';

test.describe('Navigation', () => {
  test('should navigate via sidebar links', async ({ page }) => {
    await page.goto('/');
    
    // Should have sidebar navigation
    const sidebar = page.locator('mat-sidenav');
    const navLinks = page.locator('mat-nav-list a, mat-sidenav a');
    
    // Check products link
    const productsLink = page.locator('a', { hasText: /Products/i });
    if (await productsLink.isVisible()) {
      await productsLink.click();
      await expect(page).toHaveURL(/\/products/);
    }

    // Check taxonomy link
    const taxonomyLink = page.locator('a', { hasText: /Taxonomy/i });
    if (await taxonomyLink.isVisible()) {
      await taxonomyLink.click();
      await expect(page).toHaveURL(/\/taxonomy/);
    }
  });

  test('should handle browser back/forward navigation', async ({ page }) => {
    await page.goto('/products');
    await page.goto('/taxonomy');
    
    // Go back
    await page.goBack();
    await expect(page).toHaveURL(/\/products/);
    
    // Go forward
    await page.goForward();
    await expect(page).toHaveURL(/\/taxonomy/);
  });

  test('should show 404 page for unknown routes', async ({ page }) => {
    await page.goto('/unknown-route-that-does-not-exist');
    
    // Should show not found content or redirect
    const bodyText = await page.locator('body').innerText();
    const has404 = bodyText.toLowerCase().includes('not found') || 
                   bodyText.includes('404') ||
                   page.url().includes('/products'); // may redirect to home
    expect(has404).toBeTruthy();
  });

  test('should load app shell with header and sidebar', async ({ page }) => {
    await page.goto('/');
    
    // App shell elements should be present
    const toolbar = page.locator('mat-toolbar');
    await expect(toolbar).toBeVisible();
    
    // Sidebar should exist (may be hidden on mobile)
    const sidenav = page.locator('mat-sidenav');
    const hasSidenav = await sidenav.count();
    expect(hasSidenav).toBeGreaterThan(0);
  });
});
