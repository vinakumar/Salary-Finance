import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ProblemDetails } from '../../api/generated/models';
import { ErrorService } from '../services/error.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const problemDetails = error.error as Partial<ProblemDetails> | null;
      errorService.addError({
        message: problemDetails?.detail ?? problemDetails?.title ?? error.message ?? 'An unexpected error occurred.',
        status: problemDetails?.status ?? error.status,
        traceId: problemDetails?.traceId,
        timestamp: new Date(),
      });

      return throwError(() => error);
    }),
  );
};
