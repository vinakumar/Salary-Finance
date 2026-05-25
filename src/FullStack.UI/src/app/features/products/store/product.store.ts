import { computed, inject } from '@angular/core';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, EMPTY, pipe, switchMap, tap } from 'rxjs';
import { ProductsService } from '../../../api/generated/products.service';
import { CreateProductRequest, ProductResponse, UpdateProductRequest } from '../../../api/generated/models';

export interface ProductState {
  products: ProductResponse[];
  selectedProduct: ProductResponse | null;
  loading: boolean;
  error: string | null;
  totalCount: number;
  currentPage: number;
  pageSize: number;
}

const initialState: ProductState = {
  products: [],
  selectedProduct: null,
  loading: false,
  error: null,
  totalCount: 0,
  currentPage: 1,
  pageSize: 20,
};

const toErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'An unexpected error occurred.';

export const ProductStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((state) => ({
    hasProducts: computed(() => state.products().length > 0),
    totalPages: computed(() => Math.ceil(state.totalCount() / state.pageSize())),
  })),
  withMethods((store, productsService = inject(ProductsService)) => ({
    loadProducts: rxMethod<{ page?: number; pageSize?: number }>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(({ page, pageSize }) =>
          productsService
            .getProducts({
              pageNumber: page ?? store.currentPage(),
              pageSize: pageSize ?? store.pageSize(),
            })
            .pipe(
              tap((response) =>
                patchState(store, {
                  products: response.items,
                  totalCount: response.totalCount,
                  currentPage: response.pageNumber,
                  pageSize: response.pageSize,
                  loading: false,
                }),
              ),
              catchError((err) => {
                patchState(store, { error: toErrorMessage(err), loading: false });
                return EMPTY;
              }),
            ),
        ),
      ),
    ),
    loadById: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) =>
          productsService.getProduct(id).pipe(
            tap((product) => patchState(store, { selectedProduct: product, loading: false })),
            catchError((err) => {
              patchState(store, { error: toErrorMessage(err), loading: false });
              return EMPTY;
            }),
          ),
        ),
      ),
    ),
    createProduct: rxMethod<CreateProductRequest>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((request) =>
          productsService.createProduct(request).pipe(
            tap((product) => patchState(store, { selectedProduct: product, loading: false })),
            catchError((err) => {
              patchState(store, { error: toErrorMessage(err), loading: false });
              return EMPTY;
            }),
          ),
        ),
      ),
    ),
    updateProduct: rxMethod<{ id: number; request: UpdateProductRequest }>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(({ id, request }) =>
          productsService.updateProduct(id, request).pipe(
            tap((product) => patchState(store, { selectedProduct: product, loading: false })),
            catchError((err) => {
              patchState(store, { error: toErrorMessage(err), loading: false });
              return EMPTY;
            }),
          ),
        ),
      ),
    ),
    deleteProduct: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) =>
          productsService.deleteProduct(id).pipe(
            tap(() =>
              patchState(store, {
                products: store.products().filter((product) => product.id !== id),
                selectedProduct: store.selectedProduct()?.id === id ? null : store.selectedProduct(),
                totalCount: Math.max(0, store.totalCount() - 1),
                loading: false,
              }),
            ),
            catchError((err) => {
              patchState(store, { error: toErrorMessage(err), loading: false });
              return EMPTY;
            }),
          ),
        ),
      ),
    ),
    clearSelectedProduct() {
      patchState(store, { selectedProduct: null });
    },
  })),
);
