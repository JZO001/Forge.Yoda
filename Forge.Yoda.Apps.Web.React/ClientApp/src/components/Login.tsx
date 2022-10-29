import "./Login.css";

import * as React from "react";
import { Navigate } from 'react-router';

import { Button } from 'devextreme-react';
import { TextBox } from 'devextreme-react/text-box';
import Validator, { RequiredRule } from 'devextreme-react/validator';
import { Popup } from 'devextreme-react/popup';
import ValidationGroup from 'devextreme-react/validation-group';

import LoadingPopup from "./LoadingPopup";

import { ClickEventArgs, ValueChangedEventArgs } from "./DxTypes";
import dxTextBox from "devextreme/ui/text_box";
import dxButton from "devextreme/ui/button";
import { JwtTokenResult, ServiceStore } from "forge-security-jwt-client-web";

type LoginState = {
    loginName: string,
    password: string,
    isLoginFailedPopupVisible: boolean,
    showLoading: boolean,
    routeToHome: boolean
}

export default class Login extends React.Component<{}, LoginState> {
    static displayName = Login.name;

    private _emailBoxRef = React.createRef<TextBox>();
    private _passwordBoxRef = React.createRef<TextBox>();

    state = {
        loginName: "",
        password: "",
        isLoginFailedPopupVisible: false,
        showLoading: false,
        routeToHome: false
    }

    componentDidMount() {
        this.emailBox.focus();
    }

    private usernameChangeHandler = (e: ValueChangedEventArgs<dxTextBox>) => {
        this.setState({ loginName: e.value });
    }

    private passwordChangeHandler = (e: ValueChangedEventArgs<dxTextBox>) => {
        this.setState({ password: e.value });
    }

    private btnSignInClick = (e: ClickEventArgs<dxButton>) => {
        if (!e.validationGroup.validate().isValid) {
            return;
        }

        this.setState({ showLoading: true });

        ServiceStore.authenticationService.authenticateUserAsync({
            username: this.state.loginName,
            password: this.state.password,
            secondaryKeys: []
        })
            .then((result: JwtTokenResult) => {
                if (result.accessToken === "") {
                    this.setState({ isLoginFailedPopupVisible: true, password: "", showLoading: false });
                } else {
                    this.setState({ routeToHome: true });
                }
            });
    }

    private get emailBox(): dxTextBox {
        return this._emailBoxRef.current.instance;
    }

    private get passwordBox(): dxTextBox {
        return this._passwordBoxRef.current.instance;
    }

    private getLoginScreen = () => {
        return (
            <div className="centeredContentFlex">
                <ValidationGroup>
                    <div className="loginAfterMargin pLeft">
                        <p>Please enter your user name:</p>
                        <TextBox
                            id="emlUN"
                            ref={this._emailBoxRef}
                            onValueChanged={this.usernameChangeHandler}
                            value={this.state.loginName}
                            hint="Your user name"
                            showClearButton={true}
                            width="15rem">
                            <Validator>
                                <RequiredRule message={'User name is required'} />
                            </Validator>
                        </TextBox>
                    </div>
                    <div className="loginAfterMargin pLeft">
                        <p>Your password:</p>
                        <TextBox
                            id="pw"
                            onValueChanged={this.passwordChangeHandler}
                            value={this.state.password}
                            hint="Your password"
                            showClearButton={true}
                            mode="password"
                            width="15rem"
                            ref={this._passwordBoxRef}>
                            <Validator>
                                <RequiredRule message={'Password is required'} />
                            </Validator>
                        </TextBox>
                    </div>
                    <div className="loginAfterMargin">
                        <Button
                            width="15rem"
                            text="Sign In"
                            hint="Proceed sign in"
                            type="default"
                            stylingMode="contained"
                            onClick={(e) => this.btnSignInClick(e)}
                        />
                    </div>
                </ValidationGroup>
            </div>
        );
    }

    private hideLoginFailedPopup = () => {
        this.setState({ isLoginFailedPopupVisible: false });
        this.passwordBox.focus();
    }

    render() {

        if (this.state.routeToHome) {
            return <Navigate replace to={'/'} />
        }

        return (
            <React.Fragment>
                <div className="scrollY height100 pt-4 pb-4">
                    {this.getLoginScreen()}
                    <Popup
                        visible={this.state.isLoginFailedPopupVisible}
                        onHiding={this.hideLoginFailedPopup}
                        dragEnabled={true}
                        hideOnOutsideClick={true}
                        showTitle={true}
                        title="Sign In"
                        width={300}
                        height={200}>
                        <div className="loginPopupCenter">
                            <div className="centeredContent">
                                <p className="pCenter">
                                    Sign in failed. Please try again.
                                </p>
                                <Button
                                    width="14.5rem"
                                    text="OK"
                                    type="default"
                                    stylingMode="contained"
                                    onClick={() => this.hideLoginFailedPopup()}
                                />
                            </div>
                        </div>
                    </Popup>
                </div>
                {this.state.showLoading ? <LoadingPopup isVisible={this.state.showLoading} /> : null}
            </React.Fragment>
        );
    }

}
