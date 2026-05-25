import { Injectable, computed, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly _loadingCount = signal(0);

  readonly loadingCount = this._loadingCount.asReadonly();
  readonly loading = computed(() => this._loadingCount() > 0);

  show(): void {
    this._loadingCount.update((count) => count + 1);
  }

  hide(): void {
    this._loadingCount.update((count) => Math.max(0, count - 1));
  }
}
