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
