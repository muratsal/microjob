import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { EmployeeService } from './employee.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { EmployeeDialogComponent } from './employee-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { MY_DATE_FORMATS } from '../../../shared/dateformat'
import { MAT_DATE_FORMATS } from '@angular/material/core';



@Component({
    selector: 'employee-table',
    templateUrl: './employee.component.html',
    styleUrls: ['./employee.component.scss'],
    providers: [
        { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMATS }
    ]
})
export class EmployeeComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: EmployeeFilter;
    pageEvent: PageEvent;


    constructor(private employeeService: EmployeeService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) { }

    ngOnInit() {

        this.filter = new EmployeeFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop)
        this.getData(false);
    }

    ngAfterViewInit() {

    }

    clearFilter() {
        this.filter = new EmployeeFilter();
    }

    filterData() {
        this.queryInfo.pager.currentPage = 0;
        this.getData(false);
    }

    downloadData() {
        this.getData(true);
    }

    sortData($event) {


        this.queryInfo.orderby = $event.active;
        if ($event.direction == 'desc') this.queryInfo.orderby = '-' + this.queryInfo.orderby;
        this.queryInfo.pager.currentPage = 0;
        this.getData(false);
    }

    onPaginateChange(event: PageEvent) {
        this.queryInfo.pager.currentPage = event.pageIndex;
        this.queryInfo.pager.pageSize = event.pageSize;
        this.getData(false);
    }

    getData(isExport: boolean) {
        this.showSpinner = true;
        let columnInfo = null;
        if (isExport) {

            columnInfo = this.getDataConf().filter(x => x.isExport == true)
                .map((x) => ({
                    prop: x.prop,
                    localText: this.translate.instant('EMPLOYEE.' + x.translate)
                }));

        }


        this.employeeService.getData(this.filter, this.queryInfo, columnInfo, isExport)
            .subscribe((response: any) => {
                this.navigationService.sessionControl(response);
                this.showSpinner = false;
                if (response.isSuccess) {
                    if (isExport) {
                        window.open(environment.apiURL + "/Download/FromCacheByKey?key=" + response.key, '_blank');
                    }
                    else {

                        this.queryInfo.pager.totalCount = response.totalCount;
                        this.dataSource = new MatTableDataSource(response.data);
                    }
                }
                else {

                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(response.error), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });
                }
            },
                (error) => {

                    this.showSpinner = false;

                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(error), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });

                });
    }

    newItemDialog() {

        let emptyData = new EmployeeData();

        emptyData.employeeId = 0;
        emptyData.firstName = '';
        emptyData.lastName = '';
        emptyData.fullName = '';
        emptyData.workStartDate = new Date();
        emptyData.birthDate = new Date();
        emptyData.titleId = 0;
        emptyData.departmentId = 0;
        emptyData.userId = 0;
        emptyData.emailAddress = '';
        emptyData.phoneNumber = '';
        emptyData.gsmNumber = '';
        emptyData.image = '';
        emptyData.isWorking = true;
        emptyData.endWorkDate = null;
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: EmployeeData) {
        this.openDialog(row, false);
    }

    openDialog(row: EmployeeData, isNew: boolean) {

        this.dialog.open(EmployeeDialogComponent, {
            width: '100%',
            hasBackdrop: true,
            disableClose: true,
            data: {
                row: row,
                isNew: isNew
            }
        }).afterClosed()
            .subscribe(response => {

                if (response.operation == "cancel") {
                    this.getData(false);
                }

                else if (response.operation == "delete") {
                    this.delete(response.data);
                }

                else if (response.operation == "save") {
                    this.getData(false);
                }

            });
    }


    delete(data: EmployeeData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.employeeService.delete(deleteRow).subscribe((response: any) => {
                        this.navigationService.sessionControl(response);
                        if (response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                                this.translate.instant(response.message), {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                            this.getData(false);
                        }
                        else if (!response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                                response.errorMessage, {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                        }
                    },
                        (error) => {

                            this.showSpinner = false;

                            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                                error, {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });

                        });
                }
                else {
                    this.snackbar.open(this.translate.instant("GENERAL.CANCELED"), "", {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });
                }
            });
    }




    getDataConf() {
        return [

            {
                prop: 'Actions',
                translate: 'ACTIONS',
                showColumn: true,
                isExport: false,
                localText: ''
            },
            {
                prop: 'employeeId',
                translate: 'EMPLOYEEID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'firstName',
                translate: 'FIRSTNAME',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'lastName',
                translate: 'LASTNAME',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'fullName',
                translate: 'FULLNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'workStartDate',
                translate: 'WORKSTARTDATE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'birthDate',
                translate: 'BIRTHDATE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'titleId',
                translate: 'TITLEID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'departmentId',
                translate: 'DEPARTMENTID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'companyId',
                translate: 'COMPANYID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'userId',
                translate: 'USERID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'emailAddress',
                translate: 'EMAILADDRESS',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'phoneNumber',
                translate: 'PHONENUMBER',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'gsmNumber',
                translate: 'GSMNUMBER',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'image',
                translate: 'IMAGE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'isWorking',
                translate: 'ISWORKING',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'integrationCode',
                translate: 'INTEGRATIONCODE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'endWorkDate',
                translate: 'ENDWORKDATE',
                showColumn: false,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'createdDate',
                translate: 'CREATEDDATE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'createdUser',
                translate: 'CREATEDUSER',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'createdUserText',
                translate: 'CREATEDUSERTEXT',
                showColumn: false,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'updatedDate',
                translate: 'UPDATEDDATE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'updatedUser',
                translate: 'UPDATEDUSER',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'updatedUserText',
                translate: 'UPDATEDUSERTEXT',
                showColumn: true,
                isExport: true,
                localText: ''
            }


        ];
    }
}



export class EmployeeData {

    employeeId: number;
    firstName: string;
    lastName: string;
    fullName: string;
    workStartDate: Date;
    birthDate: Date;
    titleId: number;
    departmentId: number;
    companyId: number;
    companyName: string;
    userId: number;
    emailAddress: string;
    phoneNumber: string;
    gsmNumber: string;
    image: string;
    isWorking: boolean;
    endWorkDate: Date;
    integrationCode: string;
    responsibleFor: number;
    responsibleForName: string;

    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class EmployeeFilter {

    employeeId: number;
    firstName: string;
    lastName: string;
    fullName: string;
    workStartDate: Date;
    workStartDate2: Date
    birthDate: Date;
    birthDate2: Date
    titleId: number;
    departmentId: number;
    companyId: number;
    userId: number;
    emailAddress: string;
    phoneNumber: string;
    gsmNumber: string;
    image: string;
    isWorking: boolean;
    endWorkDate: Date;
    endWorkDate2: Date
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;

    constructor() {

    }
}


