import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CompanyData, CompanyFilter } from "./company.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class CompanyService {

    endPointBase = environment.apiURL + "/Company/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: CompanyData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveCompany" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: CompanyData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteCompany?session=" + this.authService.getToken(), data);
    }

    getData(filter: CompanyFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetCompanyList" + "?session=" + this.authService.getToken(), filterData);
    }

}
