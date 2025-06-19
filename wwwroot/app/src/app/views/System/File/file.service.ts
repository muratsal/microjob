import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FileData, FileFilter } from "./file.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class FileService {

    endPointBase = environment.apiURL + "/File/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: FileData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveFile" + "?session=" + this.authService.getToken(), data);
    }

    bulkSave(data:FileData[]): Observable<object> {
        return this.httpClient.post(this.endPointBase + "BulkSaveFile" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: FileData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteFile?session=" + this.authService.getToken(), data);
    }

    bulkDelete(data: FileData[]): Observable<object> {
        return this.httpClient.post(this.endPointBase + "BulkDeleteFile?session=" + this.authService.getToken(), data);
    }

    

    getData(filter: FileFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetFileList" + "?session=" + this.authService.getToken(), filterData);
    }

    uploadFile(files: File[], data: FileData,path:string): Observable<object> {

        const formData = new FormData();
      
        for (var i = 0; i < files.length; i++) {

            var file = files[i];
            formData.append("file", file);
        }

        formData.append("path",!path ? "\\AppFiles\\PricingResearchItem":path);
        formData.append("tableNo", data.tableNo.toString());
        formData.append("tableReferenceId", data.tableReferenceId.toString());
        formData.append("description", data.description);
        formData.append("displayByCustomer", data.displayByCustomer.toString());

        return this.httpClient.post(this.endPointBase + "UploadFile?session=" + this.authService.getToken(), formData);

    }

}
