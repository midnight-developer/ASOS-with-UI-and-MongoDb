import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user';
import { environment } from 'src/environments/environment';

@Injectable({providedIn: 'root'})
export class UserService {
    constructor(private http: HttpClient) {}

    getAll() {
        return this.http.get<User[]>(`${environment.api.url}/api/user`);
    }

    register(user: User) {
        console.log('User', user);
        return this.http.post(`${environment.api.url}/api/user`, user);
    }

    delete(id: string) {
        return this.http.delete(`${environment.api.url}/api/user/${id}`);
    }
}
