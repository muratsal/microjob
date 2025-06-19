import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserRoleData, UserRoleFilter } from "./userrole.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class UserRoleService {

    endPointBase = environment.apiURL + "/UserRole/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: UserRoleData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveUserRole" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: UserRoleData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteUserRole?session=" + this.authService.getToken(), data);
    }

    getData(filter: UserRoleFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetUserRoleList" + "?session=" + this.authService.getToken(), filterData);
    }

}
