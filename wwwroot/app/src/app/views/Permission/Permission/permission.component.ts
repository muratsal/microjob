import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { PermissionService } from './permission.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { PermissionDialogComponent } from './permission-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { UtilitiesService } from '../../../utilities.service';
import { Router } from '@angular/router';



@Component({
    selector: 'permission-table',
    templateUrl: './permission.component.html',
    styleUrls: ['./permission.component.scss']
})
export class PermissionComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: PermissionFilter;
    pageEvent: PageEvent;
    allUserAuth: boolean;
    editPermission: boolean;
    pageType: number;

    constructor(private permissionService: PermissionService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private utilities: UtilitiesService,
        private router: Router,
        private navigationService: NavigationService) { }

    ngOnInit() {

        this.filter = new PermissionFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        this.allUserAuth = this.utilities.userHasAuth("AUTH_ALLPERMISSION");
        this.editPermission = this.utilities.userHasAuth("AUTH_EDITPERMISSION");
        this.pageType = parseInt(this.router.url.charAt(this.router.url.length - 1));
        this.pageType = isNaN(this.pageType) ? 1 : this.pageType;
      

        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop)
        this.getData(false);


    }

    ngAfterViewInit() {

    }

    clearFilter() {
        this.filter = new PermissionFilter();
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
        this.filter.pageType = this.pageType;
        this.showSpinner = true;
        let columnInfo = null;
        if (isExport) {

            columnInfo = this.getDataConf().filter(x => x.isExport == true)
                .map((x) => ({
                    prop: x.prop,
                    localText: this.translate.instant('PERMISSION.' + x.translate)
                }));

        }


        this.permissionService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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

        let emptyData = new PermissionData();

        emptyData.permissionId = 0;
        emptyData.permissionType = 0;
        emptyData.startDate = new Date();
        emptyData.dayCount = 0;
        emptyData.endDate = new Date();
        emptyData.status = 1;
        emptyData.description = '';
        //emptyData.proxyEmployee = 0;
        emptyData.duties = '';
        emptyData.note = '';
        //emptyData.approvedWaitingEmployee = 0;
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: PermissionData) {
        this.openDialog(row, false);
    }

    openDialog(row: PermissionData, isNew: boolean) {

        this.dialog.open(PermissionDialogComponent, {
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

                else if (response.operation == "comfirm") {
                    this.comfirm(response.data,response.comfirm);
                }

            });
    }


    delete(data: PermissionData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.permissionService.delete(deleteRow).subscribe((response: any) => {
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

    comfirm(data: PermissionData,comfirm:boolean) {

        let comfirmData = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true,
            data: {
                action: comfirm == true ? "GENERAL.COMFIRM" : "GENERAL.REJECT",
                message: comfirm == true ? "GENERAL.AREYOUSURECOMFIRM" : "GENERAL.AREYOUSUREREJECT"
                }
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.permissionService.comfirmPermission(comfirmData, comfirm).subscribe((response: any) => {
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
                prop: 'permissionId',
                translate: 'PERMISSIONID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'employeeId',
                translate: 'EMPLOYEEID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'employeeName',
                translate: 'EMPLOYEENAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'permissionType',
                translate: 'PERMISSIONTYPE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'startDate',
                translate: 'STARTDATE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'dayCount',
                translate: 'DAYCOUNT',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'endDate',
                translate: 'ENDDATE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'status',
                translate: 'STATUS',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'description',
                translate: 'DESCRIPTION',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'proxyEmployee',
                translate: 'PROXYEMPLOYEE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'duties',
                translate: 'DUTIES',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'note',
                translate: 'NOTE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'approvedWaitingEmployee',
                translate: 'APPROVEDWAITINGEMPLOYEE',
                showColumn: true,
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
    onChange($event, row, field) {
        row[field] = $event;
    }
}



export class PermissionData {

    permissionId: number;
    employeeId: number;
    employeeName: string;
    permissionType: number;
    startDate: Date;
    dayCount: number;
    endDate: Date;
    status: number;
    description: string;
    proxyEmployee: number;
    duties: string;
    note: string;
    approvedWaitingEmployee: number;
    ApprovedWaitingEmployeeName: string;

    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class PermissionFilter {

    permissionId: number;
    employeeId: number;
    permissionType: number;
    startDate: Date;
    startDate2: Date
    dayCount: number;
    endDate: Date;
    endDate2: Date
    status: number;
    description: number;
    proxyEmployee: number;
    duties: string;
    note: string;
    approvedWaitingEmployee: number;
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;
    pageType: number;

    constructor() {

    }
}


