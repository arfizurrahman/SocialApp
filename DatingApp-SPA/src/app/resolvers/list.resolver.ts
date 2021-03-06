import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router'
import { UserService } from '../services/user.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../services/alertify.service';

@Injectable()
export class ListResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 5;
    likesParam = 'Likers';

    constructor(private userService: UserService, private router: Router,
                private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParam).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }


}
