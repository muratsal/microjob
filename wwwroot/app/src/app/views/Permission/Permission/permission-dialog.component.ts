import { MatDialogRef } from "@angular/material/dialog";
import { PermissionData } from "./permission.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { PermissionService } from './permission.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'
import { ParameterFilter } from "../../System/Parameter/parameter.component";
import { ParameterService } from "../../System/Parameter/parameter.service";
import { UtilitiesService } from "../../../utilities.service";
import { UserInfo } from "../../../shared/models/user.model";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from "@angular/material/core";
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from "@angular/material-moment-adapter";
import { Router } from "@angular/router";
import { Pager, QueryInfo } from "../../../shared/query-info";
import { PermissionLogFilter } from "../PermissionLog/permissionlog.component";
import { PageEvent } from "@angular/material/paginator";
import { environment } from "../../../../environments/environment";
import { MatTableDataSource } from "@angular/material/table";
import { PermissionLogService } from "../PermissionLog/permissionlog.service";

@Component({
    selector: "permission-dialog",
    templateUrl: "./permission-dialog.component.html",
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
        { provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
        { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS] },
    ]
})
export class PermissionDialogComponent {

    row: PermissionData;
    isNew: boolean;
    permissionTypeList: any;
    allUserAuth: boolean;
    userInfo: UserInfo;
    pageType: number;
    editPermission: boolean;


    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: PermissionLogFilter;
    pageEvent: PageEvent;
    environment: any;


    constructor(public dialogRef: MatDialogRef<PermissionDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private permissionService: PermissionService,
        private translate : TranslateService,
        private parameterService: ParameterService,
        private permissionLogService: PermissionLogService,
        private navigationService: NavigationService,
        private utilities: UtilitiesService,
        private router: Router,
        private snackbar: MatSnackBar) {

        this.environment = environment;
        this.row = data.row;
        this.isNew = data.isNew;
        this.allUserAuth = this.utilities.userHasAuth("AUTH_ALLPERMISSION");
        this.editPermission = this.utilities.userHasAuth("AUTH_EDITPERMISSION");

        this.userInfo = JSON.parse(localStorage.getItem("USER_INFO"));
        if (this.isNew) {
            this.row.employeeId = this.userInfo.employeeId;
        }
       
    }

    ngOnInit() {

        this.pageType = parseInt(this.router.url.charAt(this.router.url.length - 1));
        this.pageType = isNaN(this.pageType) ? 1 : this.pageType;

        let permissionTypeParamFilter = new ParameterFilter();
        permissionTypeParamFilter.paramType = 4;

        this.parameterService.
            getData(permissionTypeParamFilter, null, null, false)
            .subscribe((response: any) => {
                this.permissionTypeList = response.data;
            });

        this.filter = new PermissionLogFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop)
        this.getData(false);
    }


    rebuildForm() {

    }

    close() {

        this.dialogRef.close({ data: this.row, operation: "cancel" });
    }

    delete() {

        this.dialogRef.close({ data: this.row, operation: "delete" });
    }

    comfirmPermission(comfirm: boolean) {
        this.dialogRef.close({ data: this.row,comfirm:comfirm, operation: "comfirm" });
    }

    save() {

        this.permissionService.save(this.row).subscribe((response: any) => {

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

    onChange($event, row, field) {
        row[field] = $event;
    }

    clearFilter() {
        this.filter = new PermissionLogFilter();
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
        this.filter.permissionId = this.row.permissionId;
        this.showSpinner = true;
        let columnInfo = null;
        if (isExport) {

            columnInfo = this.getDataConf().filter(x => x.isExport == true)
                .map((x) => ({
                    prop: x.prop,
                    localText: this.translate.instant('PERMISSIONLOG.' + x.translate)
                }));

        }


        this.permissionLogService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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
                prop: 'permissionLogId',
                translate: 'PERMISSIONLOGID',
                showColumn: true,
                isExport: true,
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
                prop: 'status',
                translate: 'STATUS',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'permissionId',
                translate: 'PERMISSIONID',
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

}