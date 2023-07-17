import React from "react";
import { connect } from "react-redux";


const UnknownHandle = ({ BackLogin, contentInfo }) => (
    <div style={{ backgroundColor: "pink" }} >
        <span>{JSON.stringify(contentInfo)}</span>
    </div>
);

const mapDispatchToProps = (dispatch) => ({
    //BackLogin: (event) => {
    //    event.preventDefault();

    //    dispatch(clearCurrentUser());
    //    dispatch(clearToken());
    //    return Promise.resolve();
    //},
});

export default connect(null, mapDispatchToProps)(UnknownHandle);








//import React from "react";
//import { connect } from "react-redux";

//import * as UI from "./unknown-handle.style";
//import { ReactComponent as LogoIcon } from "../../assets/logotop.svg";

//import { clearToken } from "../../redux/request/request.action";
//import { clearCurrentUser } from "../../redux/user/user.action";

//const UnknownHandle = ({ BackLogin, contentInfo }) => (
//    <UI.Container>
//        <UI.BackgroundCover />
//        <UI.Container position="inner">
//            <UI.Container position="logo">
//                <LogoIcon css={UI.Logo} />
//            </UI.Container>
//            <UI.Container position="body">
//                <UI.Container position="info">
//                    <UI.StatusCode>{contentInfo.code}</UI.StatusCode>
//                    <UI.Title>{contentInfo.title}</UI.Title>
//                    <UI.Description>{contentInfo.description}</UI.Description>
//                    {contentInfo.moreInfo && <UI.MoreInfo>{contentInfo.moreInfo}</UI.MoreInfo>}
//                </UI.Container>
//                <UI.Container position="buttons">
//                    <UI.BackLoginButton onClick={BackLogin}>Back to Login...</UI.BackLoginButton>
//                </UI.Container>
//            </UI.Container>
//        </UI.Container>
//    </UI.Container>
//);

//const mapDispatchToProps = (dispatch) => ({
//    BackLogin: (event) => {
//        event.preventDefault();

//        dispatch(clearCurrentUser());
//        dispatch(clearToken());
//        return Promise.resolve();
//    },
//});

//export default connect(null, mapDispatchToProps)(UnknownHandle);
