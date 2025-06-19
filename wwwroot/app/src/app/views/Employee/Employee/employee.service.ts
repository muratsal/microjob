import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeData, EmployeeFilter } from "./employee.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class EmployeeService {

    endPointBase = environment.apiURL + "/Employee/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: EmployeeData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveEmployee" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: EmployeeData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteEmployee?session=" + this.authService.getToken(), data);
    }

    getData(filter: EmployeeFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetEmployeeList" + "?session=" + this.authService.getToken(), filterData);
    }

    uploadEmployeeImage(file: File): Observable<object> {

        const formData = new FormData();
        formData.append("imageName", file);

        return this.httpClient.post(this.endPointBase + "UploadEmployeeImage?session=" + this.authService.getToken(), formData);
    }

}
