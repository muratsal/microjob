import { Injectable } from "@angular/core";
import { LocalStoreService } from "../local-store.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Router, ActivatedRoute } from "@angular/router";
import { map, catchError, delay } from "rxjs/operators";
import { UserInfo } from "../../models/user.model";
import { of, BehaviorSubject, throwError } from "rxjs";
import { environment } from "environments/environment";
import { PasswordModel } from "../../../views/Profile/Overview/overview.component";


// ================= only for demo purpose ===========
const DEMO_TOKEN =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfaWQiOiI1YjhkNDc4MDc4NmM3MjE3MjBkYzU1NzMiLCJlbWFpbCI6InJhZmkuYm9ncmFAZ21haWwuY29tIiwicm9sZSI6IlNBIiwiYWN0aXZlIjp0cnVlLCJpYXQiOjE1ODc3MTc2NTgsImV4cCI6MTU4ODMyMjQ1OH0.dXw0ySun5ex98dOzTEk0lkmXJvxg3Qgz4ed";


// ================= you will get those data from server =======

@Injectable({
    providedIn: "root",
})
export class JwtAuthService {
    token;
    isAuthenticated: Boolean;
    user: UserInfo = {};
    user$ = (new BehaviorSubject<UserInfo>(this.user));
    signingIn: Boolean;
    return: string;
    APP_TOKEN = "APP_TOKEN";
    USER_INFO = "USER_INFO";

    constructor(
        private ls: LocalStoreService,
        private http: HttpClient,
        private router: Router,
        private route: ActivatedRoute
    ) {
        this.route.queryParams
            .subscribe(params => this.return = params['return'] || '/');
    }

    public signin(username, password) {
        //return of({token: DEMO_TOKEN, user: DEMO_USER})
        //  .pipe(
        //    delay(1000),
        //    map((res: any) => {
        //      this.setUserAndToken(res.token, res.user, !!res);
        //      this.signingIn = false;
        //      return res;
        //    }),
        //    catchError((error) => {
        //      return throwError(error);
        //    })
        //  );

        // FOLLOWING CODE SENDS SIGNIN REQUEST TO SERVER

        this.signingIn = true;
        const httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json; charset=utf-8'
            })
        };
        return this.http.post<any>(`${environment.apiURL}/Login/Authenticate`, { userName: username , password:password } , httpOptions);
        //.pipe(
        //    map((res: any) => {
        //        if (res.IsSuccess) {
        //            this.setUserAndToken(res.Data.SessionId, res.Data, res.IsSuccess);
        //        }
        //        this.signingIn = false;
        //        return res;
        //    }),
        //    catchError((error) => {
        //        return throwError(error);
        //    })
        //);
    }

    redirectToLogin() {
        this.router.navigateByUrl("sessions/signin");
    }

    /*
      checkTokenIsValid is called inside constructor of
      shared/components/layouts/admin-layout/admin-layout.component.ts
    */
    public getUserInfo() {

        return this.http.get(`${environment.apiURL}/Login/GetAuthenticatedUser?session=${this.getToken()}`)
    }
    public updatePassword(passwordModel: PasswordModel) {

        return this.http.post(`${environment.apiURL}/Login/UpdatePassword?session=${this.getToken()}`,passwordModel)
    }

    public checkTokenIsValid() {
        //return of(DEMO_USER)
        //    .pipe(
        //        map((profile: UserInfo) => {
        //            this.setUserAndToken(this.getToken(), profile, true);
        //            this.signingIn = false;
        //            return profile;
        //        }),
        //        catchError((error) => {
        //            return of(error);
        //        })
        //    );



        /*
          The following code get user data and jwt token is assigned to
          Request header using token.interceptor
          This checks if the existing token is valid when app is reloaded
        */

        return this.http.get(`${environment.apiURL}/Login/GetAuthenticatedUser?session=${this.getToken()}`)
            //.pipe(
            //    map((response: any) => {
            //        if (response.isSuccess) {
            //            this.setUserAndToken(this.getToken(), response.data, true);
            //            return response.data;
            //        }
            //        else {
            //            this.signout();
            //        }
            //    }),
            //    catchError((error) => {
            //        this.signout();
            //        return of(error);
            //    })
            //);
    }

    public signout() {
        this.setUserAndToken(null, null, false);
        this.router.navigateByUrl("sessions/signin");
    }

    isLoggedIn(): Boolean {
        return !!this.getToken();
    }

    getToken() {
        return this.ls.getItem(this.APP_TOKEN);
    }
    getUser() {
        return this.ls.getItem(this.USER_INFO);
    }
    getUserId() {
        let obj = this.ls.getItem(this.USER_INFO);
        return obj.userId;
    }

    setUserAndToken(token: String, user: UserInfo, isAuthenticated: Boolean) {
        this.isAuthenticated = isAuthenticated;
        this.token = token;
        this.user = user;
        this.user$.next(user);
        this.ls.setItem(this.APP_TOKEN, token);
        this.ls.setItem(this.USER_INFO, user);
    }
}
