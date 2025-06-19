import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmailConfigData, EmailConfigFilter } from "./emailconfig.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class EmailConfigService {

    endPointBase = environment.apiURL + "/EmailConfig/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: EmailConfigData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveEmailConfig" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: EmailConfigData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteEmailConfig?session=" + this.authService.getToken(), data);
    }

    getData(filter: EmailConfigFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetEmailConfigList" + "?session=" + this.authService.getToken(), filterData);
    }

}
