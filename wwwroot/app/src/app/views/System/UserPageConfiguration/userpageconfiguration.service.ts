import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserPageConfigurationData, UserPageConfigurationFilter } from "./userpageconfiguration.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class UserPageConfigurationService {

    endPointBase = environment.apiURL + "/UserPageConfiguration/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: UserPageConfigurationData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveUserPageConfiguration" + "?session=" + this.authService.getToken(), data);
    }

    saveUserConfig(data: UserPageConfigurationData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveUserConfig" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: UserPageConfigurationData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteUserPageConfiguration?session=" + this.authService.getToken(), data);
    }

    getData(filter: UserPageConfigurationFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetUserPageConfigurationList" + "?session=" + this.authService.getToken(), filterData);
    }

    getConfigDataByPageAndUser(userId :number, confPage : string) {

        let filter: UserPageConfigurationFilter = new UserPageConfigurationFilter();
        filter.userId = userId;
        filter.confPage = confPage;

        return this.getData(filter, null, null, false);
    }

}
