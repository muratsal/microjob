import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { UserData } from '../../System/User/user.component';
import { JwtAuthService } from '../../../shared/services/auth/jwt-auth.service';
import { UserService } from '../../System/User/user.service';



@Component({
    selector: 'profile-overview-table',
    templateUrl: './overview.component.html',
    styleUrls: ['./overview.component.scss']
})
export class OverviewComponent implements OnInit {

    auth: UserData;
    passwordModel: PasswordModel;
    environment: any;

    constructor(
        public dialog: MatDialog,
        public translate: TranslateService,
        public userService: UserService,
        private jwtAuth: JwtAuthService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) {
        this.environment = environment;

    }

    ngOnInit() {
        this.auth = new UserData();
        this.passwordModel = new PasswordModel();


        this.jwtAuth.getUserInfo().subscribe((response: any) => {
            this.navigationService.sessionControl(response);
            this.auth.userName = response.data["userName"];
            this.auth.firstName = response.data["firstName"];
            this.auth.lastName = response.data["lastName"];
            this.auth.email = response.data["email"];
            this.auth.profileImage = response.data["profileImage"];
        });
    }

    ngAfterViewInit() {

    }

    updatePassword() {

        let requiredMessage = this.translate.instant("GENERAL.REQUIREDFIELDSMUSTFILLED");
        let isValid: boolean = true;
        let fieldName: string;
        if (!this.passwordModel.oldPassword) {
            fieldName = this.translate.instant("USER.OLDPASSWORD");
            isValid = false;
        }

        if (!this.passwordModel.newPassword) {
            fieldName = this.translate.instant("USER.NEWPASSWORD");
            isValid = false;
        }

        if (!this.passwordModel.newPasswordAgain) {
            fieldName = this.translate.instant("USER.NEWPASSWORDAGAIN");
            isValid = false;
        }

        if (this.passwordModel.newPasswordAgain != this.passwordModel.newPassword) {
            fieldName = this.translate.instant("USER.PASSWORDSNOTEQUAL");
            isValid = false;
        }

        if (!isValid) {

            requiredMessage += "[ " + fieldName + " ]";
            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                requiredMessage,
                {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 2500
                });

            return;
        }

        this.jwtAuth.updatePassword(this.passwordModel).subscribe((response: any) => {

            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                this.translate.instant(response.isSuccess ? response.message : response.errorMessage),
                {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 2500
                });

            if (response.isSuccess) {
                this.passwordModel = new PasswordModel();
            }
        });

    }

   

    onFileSelected(event) {

        const file: File = event.target.files[0];

        if (file) {

            this.userService.uploadProfileImage(file).subscribe((response: any) => {
                if (response.isSuccess) {
                    this.auth.profileImage = response.data;
                }
            });
        }
    }

    onPaste(event: any) {
        var items = (event.clipboardData || event.originalEvent.clipboardData).items;

        if (items.length > 0 && items[0].type.indexOf("image") != -1) {
            var file = items[0].getAsFile();
            this.userService.uploadProfileImage(file).subscribe((response: any) => {
                if (response.isSuccess) {
                    this.auth.profileImage = response.data;
                }
            });
        }

    }


}

export class PasswordModel {


    oldPassword: string;
    newPassword: string;
    newPasswordAgain: string;

    constructor() {

    }
}






