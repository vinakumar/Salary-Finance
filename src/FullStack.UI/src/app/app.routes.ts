import { Routes } from '@angular/router';
import { canDeactivateGuard } from './core/guards/can-deactivate.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full',
  },
  {
    path: 'products',
    loadComponent: () => import('./features/products/list/product-list.component').then((m) => m.ProductListComponent),
  },
  {
    path: 'products/new',
    loadComponent: () => import('./features/products/form/product-form.component').then((m) => m.ProductFormComponent),
    canDeactivate: [canDeactivateGuard],
  },
  {
    path: 'products/:id',
    loadComponent: () =>
      import('./features/products/detail/product-detail.component').then((m) => m.ProductDetailComponent),
  },
  {
    path: 'products/:id/edit',
    loadComponent: () => import('./features/products/form/product-form.component').then((m) => m.ProductFormComponent),
    canDeactivate: [canDeactivateGuard],
  },
  {
    path: 'taxonomy',
    loadComponent: () =>
      import('./features/taxonomy/tree/taxonomy-tree.component').then((m) => m.TaxonomyTreeComponent),
  },
  {
    path: '**',
    loadComponent: () =>
      import('./shared/components/empty-state/empty-state.component').then((m) => m.EmptyStateComponent),
  },
];
