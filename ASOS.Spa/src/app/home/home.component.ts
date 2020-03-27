import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';
import { UserService } from '../services/user.service';
import { User, LoggedInTokenModel } from '../models/user';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  currentUser: LoggedInTokenModel;
  users = [];

  constructor(
    private authenticationService: AuthenticationService,
    private userService: UserService
    ) {
      this.currentUser = this.authenticationService.currentUserValue;
     }

  ngOnInit() {
    this.loadAllUsers();
  }

  deleteUser(id: string) {
    this.userService.delete(id).pipe(first()).subscribe(() => this.loadAllUsers());
  }

  private loadAllUsers() {
    this.userService.getAll()
    .pipe(first())
    .subscribe(users => this.users = users);
  }

}
