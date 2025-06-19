export interface UserInfo {
    userId?: string;
    userName?: string;
    firstName?: string;
    lastName?: string;
    employeeId?: number;
    authInfos?: AuthInfo[];

}

export interface AuthInfo {
    AuthCode: string;
    AuthDesc: string;
    AuthType: number;
    AuthId: number;
}
