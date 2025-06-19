import { MatDialogRef } from "@angular/material/dialog";
import { EmployeeData } from "./employee.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { EmployeeService } from './employee.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'
import { MY_DATE_FORMATS } from '../../../shared/dateformat'
import { MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { ParameterFilter } from "../../System/Parameter/parameter.component";
import { ParameterService } from "../../System/Parameter/parameter.service";
import { UserService } from "../../System/User/user.service";
import { UserFilter } from "../../System/User/user.component";
import { environment } from "environments/environment";
import { CompanyService } from "../../System/Company/company.service";
import { CompanyFilter } from "../../System/Company/company.component";


@Component({
    selector: "employee-dialog",
    templateUrl: "./employee-dialog.component.html",
    providers: [
        { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMATS },
    ]
})
export class EmployeeDialogComponent {

    row: EmployeeData;
    isNew: boolean;
    titleList: any;
    departmentList: any;
    officeList: any;
    userList: any;
    environment: any;
    companyList: any;
    constructor(public dialogRef: MatDialogRef<EmployeeDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private employeeService: EmployeeService,
        private translate : TranslateService,
        private navigationService: NavigationService,
        private parameterService: ParameterService,
        private companyService: CompanyService,
        private userService: UserService,
        private snackbar: MatSnackBar) {
        this.row = data.row;
        this.isNew = data.isNew;

        this.environment = environment;
    }

    ngOnInit() {

        let titleParamFilter = new ParameterFilter();
        titleParamFilter.paramType = 8;

        this.parameterService.
            getData(titleParamFilter, null, null, false)
            .subscribe((response: any) => {
                this.titleList = response.data;
            });

        let departmentParamFilter = new ParameterFilter();
        departmentParamFilter.paramType = 2;

        this.parameterService.
            getData(departmentParamFilter, null, null, false)
            .subscribe((response: any) => {
                this.departmentList = response.data;
            });


       
        let userFilter = new UserFilter();

        this.userService.
            getData(userFilter, null, null, false)
            .subscribe((response: any) => {
                this.userList = response.data;
            });

        let companyFilter = new CompanyFilter();

        this.companyService.
            getData(companyFilter, null, null, false)
            .subscribe((response: any) => {
                this.companyList = response.data;
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

        this.employeeService.save(this.row).subscribe((response: any) => {

            this.navigationService.sessionControl(response);

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

        }, (error) => {

            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                this.translate.instant(error), {
                horizontalPosition: 'start',
                verticalPosition: 'bottom',
                duration: 2000
            });

        });


    }

    changeDate() {
        console.log(this.row.birthDate.getTimezoneOffset());
    }

    onFileSelected(event) {
        const file: File = event.target.files[0];

        if (file) {

            this.employeeService.uploadEmployeeImage(file).subscribe((response: any) => {
                if (response.isSuccess) {
                    this.row.image = response.data;
                }

            });
        }
    }

    onChange($event, row, field) {
        row[field] = $event;
    }


}