import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { UserRoleService } from './userrole.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { UserRoleDialogComponent } from './userrole-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';



@Component({
    selector: 'userrole-table',
    templateUrl: './userrole.component.html',
    styleUrls: ['./userrole.component.scss']
})
export class UserRoleComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: UserRoleFilter;
    pageEvent: PageEvent;


    constructor(private userRoleService:  UserRoleService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar) { }

    ngOnInit() {

        this.filter = new  UserRoleFilter();
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
        this.filter = new  UserRoleFilter();
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
                    localText: this.translate.instant('USERROLE.' + x.translate)
                }));

        }


        this.userRoleService.getData(this.filter, this.queryInfo, columnInfo, isExport)
            .subscribe((response: any) => {
                console.log(response);
                this.showSpinner = false;
                if (isExport) {
                    window.open(environment.apiURL + "/Download/FromCacheByKey?key=" + response.key, '_blank');
                }
                else {

                    this.queryInfo.pager.totalCount = response.totalCount;
                    this.dataSource = new MatTableDataSource(response.data);
                }
            });
    }

    newItemDialog() {

        let emptyData = new UserRoleData();

        emptyData.userRoleId = 0;
emptyData.userId = 0;
emptyData.roleId = 0;
emptyData.createdDate = new Date();
emptyData.createdUser = 0;
emptyData.updatedDate = new Date();
emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: UserRoleData) {
        this.openDialog(row, false);
    }

    openDialog(row: UserRoleData, isNew: boolean) {

        this.dialog.open(UserRoleDialogComponent, {
            width: '100%',
            hasBackdrop: true,
            disableClose:true,
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


    delete(data: UserRoleData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.userRoleService.delete(deleteRow).subscribe((response: any) => {
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
                                    this.translate.instant(response.error), {
                                    horizontalPosition: 'start',
                                    verticalPosition: 'bottom',
                                    duration: 2000
                                });
                        }

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
   prop: 'userRoleId',
   translate: 'USERROLEID',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'userId',
   translate: 'USERID',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'roleId',
   translate: 'ROLEID',
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



export class UserRoleData {

    userRoleId : number;
userId : number;
roleId : number;
   
    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class UserRoleFilter {

    userRoleId : number;
userId : number;
roleId : number;
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;

    constructor() {

    }
}


