import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthData, AuthFilter } from "./auth.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    endPointBase = environment.apiURL + "/Auth/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: AuthData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveAuth" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: AuthData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteAuth?session=" + this.authService.getToken(), data);
    }

    getData(filter: AuthFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetAuthList" + "?session=" + this.authService.getToken(), filterData);
    }

}
