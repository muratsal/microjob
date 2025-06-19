import { MatDialogRef } from "@angular/material/dialog";
import { RoleData } from "./role.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { RoleService } from './role.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'

@Component({
    selector: "role-dialog",
    templateUrl: "./role-dialog.component.html"
})
export class RoleDialogComponent {

    row: RoleData;
    isNew: boolean;
    filterAuth: string;
    roleAuthInfos: RoleAuthInfo[];
    isDataLoaded: boolean;

    constructor(public dialogRef: MatDialogRef<RoleDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private roleService: RoleService,
        private translate: TranslateService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) {
        this.row = data.row;
        this.isNew = data.isNew;
        this.isDataLoaded = false;

        this.getRoleAuths();

    }

    ngOnInit() {


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

        this.roleService.saveWithAuths(this.row, this.roleAuthInfos).subscribe((response: any) => {

            if (response.isSuccess) {

                this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                    this.translate.instant(response.message),
                    {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });

                this.dialogRef.close({ data: this.row, operation: "save" });
            }
            else {

                this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                    this.translate.instant(response.errorMessage),
                    {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });
            }

        });


    }

    getRoleAuths() {

        this.roleService.getAuthInfo(this.row.roleId).subscribe((response: any) => {

            this.navigationService.sessionControl(response);
            this.isDataLoaded = true;

            if (response.isSuccess) {

                this.roleAuthInfos = response.data;

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

    getFilteredAuths() {

        if (this.filterAuth && this.filterAuth.trim()) {

            return this.roleAuthInfos.filter(i => i.authCode.includes(this.filterAuth));
        }
        else {
            return this.roleAuthInfos;
        }
    }

    onRoleAuthClick(event: any, row) {
        row.isRoleAuth = !row.isRoleAuth;
    }

}


export class RoleAuthInfo {

    roleId: number;
    authId: string;
    authCode: string;
    authDesc: string;
    isRoleAuth: boolean;

    constructor() {

    }
}