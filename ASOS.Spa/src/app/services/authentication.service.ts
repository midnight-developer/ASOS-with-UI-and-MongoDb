import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User, LoggedInTokenModel } from '../models/user';
import { environment } from 'src/environments/environment';

@Injectable({providedIn: 'root'})
export class AuthenticationService {
    private currentUserSubject: BehaviorSubject<LoggedInTokenModel>;
    public currentUser: Observable<LoggedInTokenModel>;

    constructor(private http: HttpClient) {
        this.currentUserSubject = new BehaviorSubject<LoggedInTokenModel>(JSON.parse(localStorage.getItem('currentUser')));
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): LoggedInTokenModel {
        return this.currentUserSubject.value;
    }

    login(username, password) {
        const body = new HttpParams()
        .set('username', username)
        .set('password', password)
        .set('grant_type', 'password');

        return this.http.post<any>(`${environment.api.url}/authenticate`, body)
        .pipe(map(loggedInTokenModel => {
            console.log('user', loggedInTokenModel);
            localStorage.setItem('currentUser', JSON.stringify(loggedInTokenModel));
            this.currentUserSubject.next(loggedInTokenModel);
            return loggedInTokenModel;
        }));
    }

    logout() {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }
}
