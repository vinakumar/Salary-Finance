import { computed, inject } from '@angular/core';
import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { GenericPayload, ProductTaxonomyNode } from '../../../api/generated/models';
import { TaxonomyService } from '../../../api/generated/taxonomy.service';

export interface TaxonomyState {
  nodes: ProductTaxonomyNode[];
  selectedNode: ProductTaxonomyNode | null;
  selectedPayload: GenericPayload<ProductTaxonomyNode> | null;
  loading: boolean;
  error: string | null;
}

const initialState: TaxonomyState = {
  nodes: [],
  selectedNode: null,
  selectedPayload: null,
  loading: false,
  error: null,
};

const toErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'An unexpected error occurred.';

export const TaxonomyStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((state) => ({
    hasNodes: computed(() => state.nodes().length > 0),
  })),
  withMethods((store, taxonomyService = inject(TaxonomyService)) => ({
    loadNodes: rxMethod<void | Record<string, never>>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          taxonomyService.getTaxonomyNodes().pipe(
            tap({
              next: (nodes) => patchState(store, { nodes, loading: false }),
              error: (err) => patchState(store, { error: toErrorMessage(err), loading: false }),
            }),
          ),
        ),
      ),
    ),
    loadNode: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) =>
          taxonomyService.getTaxonomyNode(id).pipe(
            tap({
              next: (payload) =>
                patchState(store, {
                  selectedNode: payload.data,
                  selectedPayload: payload,
                  loading: false,
                }),
              error: (err) => patchState(store, { error: toErrorMessage(err), loading: false }),
            }),
          ),
        ),
      ),
    ),
    clearSelectedNode() {
      patchState(store, { selectedNode: null, selectedPayload: null });
    },
  })),
);
