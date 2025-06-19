import { MatDialogRef } from "@angular/material/dialog";
import { UserData } from "./user.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { UserService } from './user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { TranslateService } from '@ngx-translate/core';
import { ParameterFilter } from "../Parameter/parameter.component";
import { ParameterService } from "../Parameter/parameter.service";

@Component({
    selector: "dialog-b",
    templateUrl: "./user-dialog.component.html"
})
export class UserDialogComponent {

    row: UserData;
    isNew: boolean;
    userRoleInfos: UserRoleInfo[];
    filterRole: string;
    languageList: any;
    constructor(public dialogRef: MatDialogRef<UserDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private userService: UserService,
        private snackbar: MatSnackBar,
        private translate: TranslateService,
        private parameterService: ParameterService,
        private navigationService: NavigationService) {
        this.row = data.row;
        this.isNew = data.isNew;
        this.filterRole = "";


        this.getUserRoles();

    }

    ngOnInit() {

        let languageParamFilter = new ParameterFilter();
        languageParamFilter.paramType = 13;

        this.parameterService.
            getData(languageParamFilter, null, null, false)
            .subscribe((response: any) => {
                this.languageList = response.data;
            });
    }

    rebuildForm() {

    }

    close() {

        this.dialogRef.close({ data: this.row, operation: "cancel" });
    }

    delete() {

        this.dialogRef.close({ data: this.row, operation: "delete" });
    }

    save() {

        this.userService.saveWithRoles(this.row, this.userRoleInfos).subscribe((response: any) => {

            this.navigationService.sessionControl(response);

            if (response.isSuccess) {

                this.snackbar.open('OK', response.message, {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 1000
                });

                this.dialogRef.close({ data: this.row, operation: "save" });
            }
            else {

                this.snackbar.open('ERROR', response.message, {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 1000
                });
            }

        }, (error) => {

            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                this.translate.instant(error), {
                horizontalPosition: 'start',
                verticalPosition: 'bottom',
                duration: 2000
            });

        });

    }

    getUserRoles() {

        this.userService.getRoleInfo(this.row.userId).subscribe((response: any) => {

            this.navigationService.sessionControl(response);

            if (response.isSuccess) {

                this.userRoleInfos = response.data;

            } else {

                this.snackbar.open('ERROR', response.message, {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 1000
                });

            }

        }, (error) => {

            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                this.translate.instant(error), {
                horizontalPosition: 'start',
                verticalPosition: 'bottom',
                duration: 2000
            });

        });
    }

    getFilteredRoles() {
        if (this.filterRole && this.filterRole.trim()) {

            return this.userRoleInfos.filter(i => i.roleName.includes(this.filterRole));
        }
        else {
            return this.userRoleInfos;
        }
    }

    onUserRoleClick(event: any, row) {
        row.isUserRole = !row.isUserRole;
    }

    onChange($event, row, field) {
        row[field] = $event;
    }

}



export class UserRoleInfo {

    userId: number;
    roleId: string;
    roleDesc: string;
    roleName: string;
    isUserRole: boolean;

    constructor() {

    }
}