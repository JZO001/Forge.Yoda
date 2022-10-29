import { Claim, ClaimTypes, IAuthenticationService, ParsedTokenData, UserDataEventArgs } from "forge-security-jwt-client-web";
import { EventArgs, GenericEvent } from "jzo-library";

export class UserContext {

    public static instance: UserContext = null;
    private readonly _authenticationService: IAuthenticationService;
    private _currentUser: User = new User();

    public readonly onUserChanged: GenericEvent<EventArgs> = new GenericEvent<EventArgs>();

    constructor(authenticationService: IAuthenticationService) {
        this._authenticationService = authenticationService;
        authenticationService.onUserAuthenticationStateChanged.addEventHandler(this.onUserAuthenticationStateChangedEventHandler);
    }

    public get currentUser() { return this._currentUser; }

    private onUserAuthenticationStateChangedEventHandler = (sender: any, e: UserDataEventArgs) => {
        if (e.userId !== null && e.userId !== "") {
            this.propagateUser();
        } else {
            this.resetUser();
        }
    }

    private propagateUser = (): void => {
        (async () => {
            const parsedTokenData: ParsedTokenData = await this._authenticationService.getCurrentUserInfoAsync();
            if (parsedTokenData !== null) {
                this._currentUser = new User(parsedTokenData);
                this.onUserChanged.raiseEvent(this, new EventArgs());
            }
        })();
    }

    private resetUser = (): void => {
        this._currentUser = new User();
        this.onUserChanged.raiseEvent(this, new EventArgs());
    }

}

export class User {

    private readonly _userId: string = "";
    private readonly _email: string = "";
    private readonly _surname: string = "";
    private readonly _givenname: string = "";
    private readonly _role: string = "";

    constructor(parsedTokenData?: ParsedTokenData) {
        if (parsedTokenData !== undefined && parsedTokenData !== null) {
            this._userId = this.find(ClaimTypes.NameIdentifier, parsedTokenData.claims);
            this._email = this.find(ClaimTypes.Email, parsedTokenData.claims);
            this._surname = this.find(ClaimTypes.Surname, parsedTokenData.claims);
            this._givenname = this.find(ClaimTypes.GivenName, parsedTokenData.claims);
            this._role = this.find(ClaimTypes.Role, parsedTokenData.claims);
        }
    }

    public get userId() { return this._userId; }
    public get email() { return this._email; }
    public get surname() { return this._surname; }
    public get givenname() { return this._givenname; }
    public get role() { return this._role; }

    private find(type: string, claims: Array<Claim>) {
        const claim: Claim = claims.find(item => item.type === type);
        return claim === undefined ? "" : claim.value;
    }

}
