import React from "react";

import UnknownHandle from "../unknow-handle/unknown-handle.component";

class ErrorBoundary extends React.Component {
    constructor() {
        super();

        this.state = {
            hasErrored: false,
            errorMessage: null,
        };
    }

    static getDerivedStateFromError(error) {
        console.log("getDerivedStateFromError");
        // process the error
        return {
            hasErrored: true,
            errorMessage: error.message,
        };
    } // allow us to catch the error before passing to children components

    componentDidCatch(error, errorInfo) {
        // 你也可以把錯誤記錄到一個錯誤回報系統服務
        !process.env?.NODE_ENV && console.log(error);
        !process.env?.NODE_ENV && console.log(errorInfo);
    }

    render() {
        if (this.state.hasErrored) {
            const contentError = {
                code: "Oops!",
                title: "Seems like an error occurred..",
                description:
                    "sorry but unknown error occured while processing your request. Please wait a moment to try again ... or pass on this information to your Tech team..",
                moreInfo: this.state.errorMessage,
            };
            console.log(`%cERROR : %c ${contentError}`, "background-color: #FF0000; color: white; padding: 5px;", "background-color: none; color: auto; padding: none;")
            return <UnknownHandle contentInfo={contentError} />;
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
