import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { EmailService } from './email.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { EmailDialogComponent } from './email-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'



@Component({
    selector: 'email-table',
    templateUrl: './email.component.html',
    styleUrls: ['./email.component.scss']
})
export class EmailComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: EmailFilter;
    pageEvent: PageEvent;


    constructor(private emailService:  EmailService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) { }

    ngOnInit() {

        this.filter = new  EmailFilter();
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
        this.filter = new  EmailFilter();
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
                    localText: this.translate.instant('EMAIL.' + x.translate)
                }));

        }


        this.emailService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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

        let emptyData = new EmailData();

        emptyData.emailId = 0;
emptyData.emailFrom = '';
emptyData.emailTo = '';
emptyData.emailToCc = '';
emptyData.emailToBcc = '';
emptyData.subject = '';
emptyData.body = '';
emptyData.isHtml = false;
emptyData.isSend = false;
emptyData.isSuccess = false;
emptyData.sendDate = new Date();
emptyData.createdDate = new Date();
emptyData.createdUser = 0;
emptyData.updatedDate = new Date();
emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: EmailData) {
        this.openDialog(row, false);
    }

    openDialog(row: EmailData, isNew: boolean) {

        this.dialog.open(EmailDialogComponent, {
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


    delete(data: EmailData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.emailService.delete(deleteRow).subscribe((response: any) => {
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
   prop: 'emailId',
   translate: 'EMAILID',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'emailFrom',
   translate: 'EMAILFROM',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'emailTo',
   translate: 'EMAILTO',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'emailToCc',
   translate: 'EMAILTOCC',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'emailToBcc',
   translate: 'EMAILTOBCC',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'subject',
   translate: 'SUBJECT',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'body',
   translate: 'BODY',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'isHtml',
   translate: 'ISHTML',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'isSend',
   translate: 'ISSEND',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'isSuccess',
   translate: 'ISSUCCESS',
   showColumn: true,
   isExport: true,
   localText: ''
},
{ 
   prop: 'sendDate',
   translate: 'SENDDATE',
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



export class EmailData {

    emailId : number;
emailFrom : string;
emailTo : string;
emailToCc : string;
emailToBcc : string;
subject : string;
body : string;
isHtml : boolean;
isSend : boolean;
isSuccess : boolean;
sendDate : Date;
   
    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class EmailFilter {

    emailId : number;
emailFrom : string;
emailTo : string;
emailToCc : string;
emailToBcc : string;
subject : string;
body : string;
isHtml : boolean;
isSend : boolean;
isSuccess : boolean;
sendDate : Date;
sendDate2 : Date
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;

    constructor() {

    }
}


