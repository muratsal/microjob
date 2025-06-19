import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserData, UserFilter } from "./user.component";
import { UserRoleInfo } from "./user-dialog.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class UserService {

    endPointBase = environment.apiURL + "/User/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: UserData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveUser" + "?session=" + this.authService.getToken(), data);
    }

    setUserLanguage(languageCode): Observable<object> {
        return this.httpClient.get(this.endPointBase + "SetUserLanguage" + "?session=" + this.authService.getToken() + "&languageCode=" + languageCode);
    }

    saveWithRoles(data: UserData, roleData: UserRoleInfo[]): Observable<object> {

        let userWithRole = {
            user: data,
            roleInfos: roleData,
           
        }
        return this.httpClient.post(this.endPointBase + "SaveUserWithRoles" + "?session=" + this.authService.getToken(), userWithRole);
    }

    delete(data: UserData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteUser?session=" + this.authService.getToken(), data);
    }

    getData(filter: UserFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetUserList" + "?session=" + this.authService.getToken(), filterData);
    }

    getRoleInfo(id: number): Observable<any>{

        return this.httpClient.post(this.endPointBase + "GetRoleInfo" + "?session=" + this.authService.getToken(), { userId: id });

    }

    getUserListForCombo(search: string, value: number) {
        return this.httpClient.post(this.endPointBase + "GetListForCombo" + "?session=" + this.authService.getToken(), { userName: search , userId : value });
    }

    uploadProfileImage(file: File): Observable<object> {

        const formData = new FormData();
        formData.append("imageName", file);

        return this.httpClient.post(this.endPointBase + "UploadProfileImage?session=" + this.authService.getToken(), formData);
    }


    getData2() {
        return this.httpClient.get<any>('https://api.icndb.com/jokes/random/5');
    }

}


