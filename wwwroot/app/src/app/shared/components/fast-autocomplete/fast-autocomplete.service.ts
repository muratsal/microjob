import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class FastAutoCompleteService {

    endPointBase = environment.apiURL ;

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }
   
    getFilterDataList(search: string, value: number, endpoint: string) {
        let queryAnd = endpoint.indexOf('?') == -1 ? '?' : '&';
        return this.httpClient.post(this.endPointBase + endpoint + queryAnd+ "session=" + this.authService.getToken(), { search: search, id: value });
    }

}


