import { Component, OnInit, ViewChild, OnDestroy, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatProgressBar } from '@angular/material/progress-bar';
import { Validators, FormGroup, FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AppLoaderService } from '../../../shared/services/app-loader/app-loader.service';
import { JwtAuthService } from '../../../shared/services/auth/jwt-auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";

@Component({
    selector: 'app-signin',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.css']
})
export class SigninComponent implements OnInit, AfterViewInit, OnDestroy {
    @ViewChild(MatProgressBar) progressBar: MatProgressBar;
    @ViewChild(MatButton) submitButton: MatButton;

    signinForm: FormGroup;
    errorMsg = '';
    public  environment: any;
    // return: string;

    private _unsubscribeAll: Subject<any>;

    constructor(
        private jwtAuth: JwtAuthService,
        private egretLoader: AppLoaderService,
        private router: Router,
        private route: ActivatedRoute,
        private snackbar: MatSnackBar,
        private translate: TranslateService
    ) {
        this._unsubscribeAll = new Subject();
        this.snackbar = snackbar;
        this.environment = environment;
    }

    ngOnInit() {
        localStorage.clear();
        this.signinForm = new FormGroup({
            username: new FormControl('', Validators.required),
            password: new FormControl('', Validators.required),
            rememberMe: new FormControl(true)
        });

        // this.route.queryParams
        //   .pipe(takeUntil(this._unsubscribeAll))
        //   .subscribe(params => this.return = params['return'] || '/');
    }

    ngAfterViewInit() {
        // this.autoSignIn();
    }

    ngOnDestroy() {
        this._unsubscribeAll.next();
        this._unsubscribeAll.complete();
    }

    signin() {
        const signinData = this.signinForm.value;
        //if (signinData.username == '' && signinData.password == '') {
        //    alert("Kullanýcý adý ve þifre girilmeli");
        //    return;
        //}
        this.submitButton.disabled = true;
        this.progressBar.mode = 'indeterminate';

        this.jwtAuth.signin(signinData.username, signinData.password)
            .subscribe(response => {

                if (response.isSuccess) {
                    this.jwtAuth.setUserAndToken(response.data.sessionId, response.data, true);
                    this.snackbar.open(this.translate.instant("GENERAL.SUCCES"), this.translate.instant( response.message), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });
                    this.jwtAuth.signingIn = false;
                    this.router.navigateByUrl(this.jwtAuth.return);
                }
                else {
                    this.jwtAuth.signingIn = false;
                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(response.errorMessage), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });

                    this.submitButton.disabled = false;
                    this.progressBar.mode = 'determinate';
                }




            }, err => {
                this.submitButton.disabled = false;
                this.progressBar.mode = 'determinate';
                this.errorMsg = err.message;
                // console.log(err);
            })
    }

    autoSignIn() {
        if (this.jwtAuth.return === '/') {
            return
        }
        this.egretLoader.open(`Automatically Signing you in! \n Return url: ${this.jwtAuth.return.substring(0, 20)}...`, { width: '320px' });
        setTimeout(() => {
            this.signin();
            console.log('autoSignIn');
            this.egretLoader.close()
        }, 2000);
    }

}
