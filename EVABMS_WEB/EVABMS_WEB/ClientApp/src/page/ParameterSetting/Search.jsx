import React, { useState } from "react";
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { Button, Form, InputGroup } from 'react-bootstrap';

import { requestHelper } from "../../redux/request/request.helper";

import MessageCard from "../../base-components/message-card/message-card";
import _Modal from "../../base-components/modal/modal.component";

import "./ParameterSetting.css";

//Components
const Search = ({ auth, setSelectedData, setSearchHistory, setMSG, Query, Delete }) => {
    let ParameterSettingDataModel = {
        id: 0,
        functionname: "",
        subfunctionname: "",
        settingname: "",
        value: "",
        description: "",
        lastupdateduserid: "",
        fk_mgau_id: 0,
    };

    let ParameterSettingHistoryDataModel = {
        id: -1,
        functionname: "",
        subfunctionname: "",
        settingname: "",
        value: "",
        lastupdateduserid: "",
    };

    const [searchModel, setSearchModel] = useState({
        ...ParameterSettingDataModel,
    });
    const [searchResult, setSearchResult] = useState([]);
    const [DelData, setDelData] = useState({ ...ParameterSettingDataModel });
    //const [MSG, setMSG] = useState({ show: false });
    const [popup, setPopup] = useState({
        isShow: false,
        title: "Delete Confirmation",
        descriptions: "Are you sure to delete",
        confirmLabel: "Delete",
    });

    //Events
    const DataQuery = () => {
        let temp = { ...searchModel, Auth: auth };
        Query(temp)
            .then((result) => {
                setMSG({ ...result, show: true });
                setSearchResult(result.Data);
            })
            .catch((err) => {
                setMSG({ show: true, Succ: false, Message: err });
            });
    };
    const Add = () => {
        let temp = { ...ParameterSettingDataModel };
        temp.id = 0;
        setSelectedData(temp);
    };
    const DataDeleteCheck = () => {
        Delete(DelData)
            .then((result) => {
                setMSG({ ...result, show: true });
                setPopup({ ...popup, isShow: false });
            })
            .catch((err) => {
                setMSG({ show: true, Succ: false, Message: err });
            });
    };
    const History = () => {
        setSearchResult([]);
        setSelectedData(undefined);
        setSearchHistory({ ...ParameterSettingHistoryDataModel });
    };
    const handleChange = (e) => {
        const name = e.target.name;
        const value = e.target.value;
        setSearchModel({ ...searchModel, [name]: value });
    };

    //Components
    const SearchResult = ({ searchResult, setSearchHistory, auth }) => {
        const columns = [
            {
                id: 4,
                field: "id",
                label: "ID",
                visible: false,
            },
            {
                id: 5,
                field: "functionname",
                label: "Function Name",
            },
            {
                id: 6,
                field: "subfunctionname",
                label: "Sub Function Name",
            },
            {
                id: 7,
                field: "settingname",
                label: "Setting Name",
            },
            {
                id: 8,
                field: "value",
                label: "Value",
                width: "minmax(30%, 530px)",
                maxResizeWidth: 530,
            },
            {
                id: 1,
                width: "max-content",
                pinned: true,
                sortable: false,
                resizable: false,
                cellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                }) => (
                    <div>
                        <input
                            type="button"
                            value="&#x1F58A;"
                            className="btn btn-link"
                            onClick={() => {
                                setSelectedData(data);
                            }}
                        />
                        {auth != "CPU.ParameterSetting.ReadOnly" && (
                            <input
                                type="button"
                                value="&#x274C;"
                                className="btn btn-link"
                                onClick={() => {
                                    DataDelete(data);
                                }}
                            />
                        )}
                        <input
                            type="button"
                            value="&#x1F4DC;"
                            className="btn btn-link"
                            onClick={() => {
                                setSearchHistory(data);
                            }}
                        />
                    </div>
                ),
            },
        ];
        if (auth == "CPU.ParameterSetting.FullControl") {
            columns.splice(5, 0, {
                //Auth
                id: 9,
                field: "Auth",
                label: "Auth",
            });
        }
        const DataDelete = (rowData) => {
            setDelData(rowData);
            setPopup({ ...popup, isShow: true });
        };

        return <GridTable columns={columns} rows={searchResult} />;
    };

    return (
        <div>
            {/*<MessageCard show={MSG.show} ApiResult={MSG} />*/}
            <div className="row">
                <div className="col">
                    <div className="card">
                        <div className="card-body">
                            <h2 className="card-title">Search</h2>
                            <InputGroup>
                                <Form.Control
                                    placeholder="Function Name"
                                    name={"functionname"}
                                    value={searchModel?.functionname}
                                    onChange={(e) => handleChange(e)}
                                />
                                <Form.Control
                                    placeholder="Sub Function Name"
                                    name={"subfunctionname"}
                                    value={searchModel?.subfunctionname}
                                    onChange={(e) => {
                                        handleChange(e, searchModel, setSearchModel);
                                    }}
                                />
                                <Form.Control
                                    placeholder="Setting Name"
                                    name={"settingname"}
                                    value={searchModel?.settingname}
                                    onChange={(e) => {
                                        handleChange(e, searchModel, setSearchModel);
                                    }}
                                />
                                <Button variant="outline-secondary" onClick={DataQuery}>Search</Button>
                                {auth == "CPU.ParameterSetting.FullControl" && (
                                    <Button variant="outline-secondary" onClick={Add}>Add</Button>
                                )}
                                <Button variant="outline-secondary" onClick={History}>History</Button>
                            </InputGroup>
                        </div>
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col">
                    {searchResult?.length > 0 && (
                        <SearchResult
                            searchResult={searchResult}
                            setSearchHistory={setSearchHistory}
                            auth={auth}
                        />
                    )}
                </div>
            </div>
            {popup.isShow && (
                <_Modal
                    isShow={popup.isShow}
                    isShow={popup.isShow}
                    title={popup.title}
                    descriptions={popup.descriptions}
                    confirmLabel={popup.confirmLabel}
                    confirmCallbackFn={DataDeleteCheck}
                />
            )}
        </div>
    );
};

const mapDispatchToProps = (dispatch) => ({
    Query: (data) => requestHelper.post(dispatch, "ParameterSetting/Query", data),
    Delete: (data) =>
        requestHelper.post(dispatch, "ParameterSetting/Delete", data),
});

export default connect(null, mapDispatchToProps)(Search);
