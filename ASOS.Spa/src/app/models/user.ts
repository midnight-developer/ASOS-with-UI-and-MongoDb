export class User {
    id: string;
    username: string;
    password: string;
    firstname: string;
    lastname: string;
    token: string;
}
export class LoggedInTokenModel{
    scope: string;
    token_type: string;
    access_token: string;
    refresh_token: string;
    firstname: string;
    lastname: string;
    username: string;
}
