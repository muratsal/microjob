import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleAuthData, RoleAuthFilter } from "./roleauth.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class RoleAuthService {

    endPointBase = environment.apiURL + "/RoleAuth/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: RoleAuthData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveRoleAuth" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: RoleAuthData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteRoleAuth?session=" + this.authService.getToken(), data);
    }

    getData(filter: RoleAuthFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetRoleAuthList" + "?session=" + this.authService.getToken(), filterData);
    }

}
