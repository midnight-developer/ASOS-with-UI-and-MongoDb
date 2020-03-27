import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from '@angular/common/http';
import { AuthenticationService } from '../services/authentication.service';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            console.log("err",err);
            if (err.staus === 401) {
                this.authenticationService.logout();
                location.reload();
            }
            // if (err.staus !== 400) {
            //     this.authenticationService.logout();
            //     location.reload();
            // }

            const error = err.error.error_description || err.error.error;
            console.log("interErr",err.error.error_description);
            return throwError(error);
        }));
    }
}
