import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { EmailConfigService } from './emailconfig.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { EmailConfigDialogComponent } from './emailconfig-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'



@Component({
    selector: 'emailconfig-table',
    templateUrl: './emailconfig.component.html',
    styleUrls: ['./emailconfig.component.scss']
})
export class EmailConfigComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: EmailConfigFilter;
    pageEvent: PageEvent;


    constructor(private emailConfigService:  EmailConfigService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) { }

    ngOnInit() {

        this.filter = new  EmailConfigFilter();
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
        this.filter = new  EmailConfigFilter();
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
                    localText: this.translate.instant('EMAILCONFIG.' + x.translate)
                }));

        }


        this.emailConfigService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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

        let emptyData = new EmailConfigData();

        emptyData.emailConfigId = 0;
emptyData.configName = '';
emptyData.host = '';
emptyData.port = '';
emptyData.enableSsl = false;
emptyData.userName = '';
emptyData.password = '';
emptyData.isActive = false;
emptyData.createdDate = new Date();
emptyData.createdUser = 0;
emptyData.updatedDate = new Date();
emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: EmailConfigData) {
        this.openDialog(row, false);
    }

    openDialog(row: EmailConfigData, isNew: boolean) {

        this.dialog.open(EmailConfigDialogComponent, {
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


    delete(data: EmailConfigData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.emailConfigService.delete(deleteRow).subscribe((response: any) => {
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
   prop: 'emailConfigId',
   translate: 'EMAILCONFIGID',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'configName',
   translate: 'CONFIGNAME',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'host',
   translate: 'HOST',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'port',
   translate: 'PORT',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'enableSsl',
   translate: 'ENABLESSL',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'userName',
   translate: 'USERNAME',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'password',
   translate: 'PASSWORD',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'isActive',
   translate: 'ISACTIVE',
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



export class EmailConfigData {

    emailConfigId : number;
configName : string;
host : string;
port : string;
enableSsl : boolean;
userName : string;
password : string;
isActive : boolean;
   
    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class EmailConfigFilter {

    emailConfigId : number;
configName : string;
host : string;
port : string;
enableSsl : boolean;
userName : string;
password : string;
isActive : boolean;
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;

    constructor() {

    }
}


