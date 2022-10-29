# Forge.Yoda.Apps.Web.React
An example of JWT client side implementation and usage

## Run the solution
To run the solution, you have to configure which apps start. Right-click on the solution and select the 'Set Startup Projects'.
Choose the 'Multiple startup projects' and select 'Start' for:
a, Forge.Yoda.Apps.Web.React
b, Forge.Yoda.Services.Authentication

Please check authentication service configuration (appSettings.json) and modify the database connection string as you need on your side.

Now you can start the company in debug mode. Two windows need to appear, the first one is the authentication service (server) with a SwaggerUI.
The second one is a basic web app, with a login screen. The default username and password is 'Admin' and 'Passw0rd12345',
feel free to use it for testing.


## Under the hood

### Configure the client services
For the complete reference, please visit: https://github.com/JZO001/Forge.Security.Jwt.Client.Web

Apps.tsx
I put here the initialization logic. It is important, the initialization must have happen only once.

```c#
import 'devextreme/dist/css/dx.common.css';
import 'devextreme/dist/css/dx.light.css';
import { Configuration, Options, ServiceStore } from 'forge-security-jwt-client-web';
import { AuthorizeView } from 'forge-security-jwt-client-web-react';
import { Guid } from 'jzo-library';

import * as React from 'react';
import { Route, Routes } from 'react-router-dom';

import './App.css';
import AppRoutes from './components/AppRoutes';
import { Layout } from './components/Layout';
import { UserContext } from './components/UserContext';

Options.getJwtClientAuthenticationCoreOptions.baseAddress = "https://localhost:7253/";
Options.getJwtClientAuthenticationCoreOptions.refreshTokenBeforeExpirationInMilliseconds = 50000;
Configuration.addLocalStorage();

// generate a unique device id for this browser and store it into the storage
const deviceId_Key = "__deviceId";
if (ServiceStore.storage.containsKey(deviceId_Key)) {
	ServiceStore.additionalData.secondaryKeys.push({ key: deviceId_Key, value: ServiceStore.storage.getAsString(deviceId_Key) });
} else {
	const guid: string = Guid.CreateNewAsString();
	ServiceStore.storage.setAsString(deviceId_Key, guid);
	ServiceStore.additionalData.secondaryKeys.push({ key: deviceId_Key, value: guid });
}

Configuration.configureServices();

UserContext.instance = new UserContext(ServiceStore.authenticationService);

export default class App extends React.Component<{}> {

	render() {
		return (
			<AuthorizeView>
				<Layout>
					<Routes>
						{AppRoutes.map((route, index) => {
							const { element, ...rest } = route;
							return <Route key={index} {...rest} element={element} />;
						})}
					</Routes>
				</Layout>
			</AuthorizeView>
		)
	}

}
```

In the options, I override the baseAddress from empty to a localhost address. The provided value points to the authentication service address.
'refreshTokenBeforeExpirationInMilliseconds' overrided from 15000 ms to 50000 ms for demonstration purposes. The client will refresh the existing token
more frequently before the expiry time. Normally the 15000 ms is enough in not heavilly loaded system.

'Configuration.addLocalStorage();' - means, I add browser's localStorage as a service to store data, more exactly the given token from the auth service.
If you restart the webapp, the user will be authentication and it does not necessary to display the login screen again.
You can also use 'Configuration.addSessionStorage();' or 'Configuration.addMemoryStorage();' (this is the default).

In next step, I just generate a unique identifier for the client instance. This helps to make the accessToken more unique, which has given from the auth service.

The final step is to configure the client side authentication services with the options and the selected storage.


### Authentication context
In the render() method, there is a component <AuthorizeView>, which provides a context for the entire app, and inform the descendants,
if the authentication state changing. See the next section.


### Usage
For the complete reference, please visit: https://github.com/JZO001/Forge.Security.Jwt.Client.Web.Reach

AccountMenu.tsx:
```c#
import "./AccountMenu.css";

import React, { Component, Fragment } from "react";
import { NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { Navigate } from 'react-router';

import LoadingPopup from "./LoadingPopup";
import { UserContext } from "./UserContext";
import { EventArgs } from "jzo-library";
import { ServiceStore } from "forge-security-jwt-client-web";
import { Authorized, NotAuthorized } from "forge-security-jwt-client-web-react";

type AccountMenuState = {
    isErrorPopupVisible: boolean,
    showLoading: boolean,
    isRedirectNeed: boolean
}

class AccountMenu extends Component<{}, AccountMenuState> {

    state = {
        isErrorPopupVisible: false,
        showLoading: false,
        isRedirectNeed: false
    }

    componentDidMount(): void {
        UserContext.instance.onUserChanged.addEventHandler(this.onUserChangedEventHandler);
    }

    componentWillUnmount(): void {
        UserContext.instance.onUserChanged.removeEventHandler(this.onUserChangedEventHandler);
    }

    private onUserChangedEventHandler = (sender: any, e: EventArgs) => {
        this.forceUpdate();
    }

    private logoutClickEventHandler = (e: React.MouseEvent<HTMLAnchorElement, MouseEvent>) => {
        e.preventDefault();
        this.setState({ showLoading: true });

        (async () => await ServiceStore.authenticationService.logoutUserAsync())();

        this.setState({ showLoading: false, isRedirectNeed: true });
    }

    private authMenu = () => {
        const name: string = UserContext.instance.currentUser.givenname;
        return (
            <NavItem className="dropdown">
                <NavLink className="dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false" to="#" href="#" title={name}>{name.length > 12 ? name.substring(0, 10) + "..." : name}</NavLink>
                <div className="dropdown-menu accountMenu">
                    <a className="dropdown-item text-dark nav-link" href="#" onClick={(e) => { this.logoutClickEventHandler(e); return false; }}>Sign Out</a>
                </div>
            </NavItem>
        );
    }

    private unAuthMenu = () => {
        return (
            <NavItem>
                <NavLink tag={Link} className="navbar-dark" to={'/login'}>Sign In</NavLink>
            </NavItem>
        );
    }

    render() {
        if (this.state.isRedirectNeed) {
            setTimeout(() => {
                this.setState({ isRedirectNeed: false });
            }, 1);
        }
        return (
            <Fragment>
                <ul className="navbar-nav">
                    <Authorized>
                        {this.authMenu()}
                    </Authorized>
                    <NotAuthorized>
                        {this.unAuthMenu()}
                    </NotAuthorized>
                </ul>
                <LoadingPopup isVisible={this.state.showLoading} />
                {this.state.isRedirectNeed ? <Navigate replace to={'/login'} /> : null}
            </Fragment>
        );
    }

}

export default AccountMenu;
```

The interesting part is in the render() method. <Authorized> and <NotAuthorized> components here can displays the content, depends on the authentication state.

In my example project, I added a UserContent.tx file, which contains two classes:

a, User
Presents the user data. The fields are empty, if no one authenticated, or contains the information which are come from the accessToken as a Claim.

b, UserContext
Handling the authentication events and make a user instance from the given accessToken
