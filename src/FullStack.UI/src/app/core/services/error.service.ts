import { Injectable, signal } from '@angular/core';

export interface AppError {
  message: string;
  status?: number;
  traceId?: string;
  timestamp: Date;
}

@Injectable({ providedIn: 'root' })
export class ErrorService {
  private readonly _errors = signal<AppError[]>([]);

  readonly errors = this._errors.asReadonly();

  addError(error: AppError | string, status?: number): void {
    const nextError: AppError =
      typeof error === 'string'
        ? { message: error, status, timestamp: new Date() }
        : { ...error, timestamp: error.timestamp ?? new Date() };

    this._errors.update((errors) => [...errors, nextError]);
  }

  clearErrors(): void {
    this._errors.set([]);
  }
}
