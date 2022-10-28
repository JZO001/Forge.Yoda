import React, { Component, Fragment } from "react";

import { LoadPanel } from 'devextreme-react/load-panel';

export type LoadingPopupProps = {
    isVisible: boolean
}

export default class LoadingPopup extends Component<LoadingPopupProps> {
    static displayName = LoadingPopup.name;

    private _timeoutId: number = -1;

    componentDidMount() {
        if (this._timeoutId < 0 && this.props.isVisible) {
            this._timeoutId = setTimeout(() => {
                this.forceUpdate();
            }, 1000) as unknown as number;
        }
    }

    componentWillUnmount() {
        this.clearMyTimeout();
    }

    private clearMyTimeout = (): void => {
        if (this._timeoutId !== -1) {
            clearTimeout(this._timeoutId);
            this._timeoutId = -1;
        }
    }

    render() {
        //console.log("LoadingPopup.render, isVisible: " + this.props.isVisible);

        if (this._timeoutId < 0 && this.props.isVisible) {
            return <Fragment></Fragment>;
        }

        this.clearMyTimeout();

        return (
            <LoadPanel
                shadingColor="rgba(0,107,185,0.4)"
                visible={this.props.isVisible}
                position={{ my: 'center', at: 'center', of: 'body' }}
                showIndicator={true}
                shading={true}
                showPane={true}
                hideOnOutsideClick={false} />
        );
    }

}
