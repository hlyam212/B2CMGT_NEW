import React, { useState, useMemo, useEffect } from "react";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";

import {
    selectCurrentUser,
    selectCurrentUserRole,
} from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import MessageCard from "../../base-components/message-card/message-card";
import _Modal from "../../base-components/modal/modal.component";

import Search from "./Search";
import Modify from "./Modify";
import History from "./History";
import "./ParameterSetting.css";
import PageTitle from "../../base-components/page-title/page-title";

const ParameterSetting = ({ user, AuthGet }) => {
    const [selectedData, setSelectedData] = useState(undefined);

    const [searchHistory, setSearchHistory] = useState(undefined);

    const [MSG, setMSG] = useState({ show: false });

    const [ddlUserRoles, setddlUserRoles] = useState([{ id: "", name: "" }]);
    const [auth, setAuth] = useState(false);

    //Events
    useEffect(() => {
        AuthGet(user)
            .then((result) => {
                if (!result.Succ) {
                    setMSG({ ...result, show: true });
                    return;
                }

                let temp = [...ddlUserRoles].concat(result.Data.ddl);
                setddlUserRoles(temp);
                setAuth(result.Data.auth);
            })
            .catch((err) => {
                setMSG({ show: true, Succ: false, Message: err });
            });
    }, []);
    useMemo(() => {
        if (selectedData != undefined) {
            setMSG({ show: false });
            setSearchHistory(undefined);
        }
    }, [selectedData]);
    useMemo(() => {
        if (searchHistory != undefined) {
            setMSG({ show: false });
        }
    }, [searchHistory]);

    return (
        <div className="container">
            <PageTitle text={"Parameter Setting"} />
            <MessageCard show={MSG.show} ApiResult={MSG} />
            {selectedData == undefined && searchHistory == undefined && (
                <Search
                    auth={auth}
                    setSelectedData={setSelectedData}
                    setSearchHistory={setSearchHistory}
                    setMSG={setMSG}
                />
            )}
            {selectedData != undefined && (
                <Modify
                    auth={auth}
                    ddlUserRoles={ddlUserRoles}
                    selectedData={selectedData}
                    setSelectedData={setSelectedData}
                    setMSG={setMSG}
                />
            )}
            {searchHistory != undefined && (
                <History
                    searchHistory={searchHistory}
                    setSearchHistory={setSearchHistory}
                    setHide={() => {
                        setSearchHistory(undefined);
                    }}
                />
            )}
        </div>
    );
};

const mapStateToProps = createStructuredSelector({
    user: selectCurrentUser,
});

const mapDispatchToProps = (dispatch) => ({
    AuthGet: (userId) =>
        requestHelper.get(dispatch, `ParameterSetting/Auth/${userId}`),
});

export default connect(mapStateToProps, mapDispatchToProps)(ParameterSetting);
