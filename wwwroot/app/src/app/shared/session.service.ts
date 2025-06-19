import { Injectable } from '@angular/core';

@Injectable()
export class SessionService {

    getSessionId(): string {
        return  localStorage.getItem("appSession");
    }
}

 