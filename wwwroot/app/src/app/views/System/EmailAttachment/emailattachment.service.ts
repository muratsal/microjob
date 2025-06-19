import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmailAttachmentData, EmailAttachmentFilter } from "./emailattachment.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class EmailAttachmentService {

    endPointBase = environment.apiURL + "/EmailAttachment/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: EmailAttachmentData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveEmailAttachment" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: EmailAttachmentData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteEmailAttachment?session=" + this.authService.getToken(), data);
    }

    getData(filter: EmailAttachmentFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetEmailAttachmentList" + "?session=" + this.authService.getToken(), filterData);
    }

}
