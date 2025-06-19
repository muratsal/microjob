import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmailData, EmailFilter } from "./email.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class EmailService {

    endPointBase = environment.apiURL + "/Email/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: EmailData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveEmail" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: EmailData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteEmail?session=" + this.authService.getToken(), data);
    }

    getData(filter: EmailFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetEmailList" + "?session=" + this.authService.getToken(), filterData);
    }

}
