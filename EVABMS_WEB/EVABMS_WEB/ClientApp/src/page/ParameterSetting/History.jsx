import React, { useState, useEffect } from 'react';
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { format } from 'date-fns'

import { requestHelper } from "../../redux/request/request.helper";

import MessageCard from "../../base-components/message-card/message-card"
import _Modal from "../../base-components/modal/modal.component"

import './ParameterSetting.css';

const History = ({ searchHistory, setSearchHistory, setHide, HistoryQuery }) => {
    const [searchModel, setSearchModel] = useState({ ...searchHistory });
    const [historyResult, setHistoryResult] = useState([]);
    const [MSG, setMSG] = useState({ show: false });

    //Events
    const DataQuery = () => {
        HistoryQuery(searchModel).then((result) => {
            setMSG({ ...result, show: true })
            setHistoryResult(result.Data);
        }).catch((err) => {
            setMSG({ show: true, Succ: false, Message: err })
        });
    }
    const Back = () => {
        setSearchHistory(undefined)
        setHide()
    }
    const handleChange = (e) => {
        const name = e.target.name;
        const value = e.target.value;
        setSearchModel({ ...searchModel, [name]: value });
    };

    useEffect(() => {
        if (searchModel == undefined) return
        if (searchModel.functionname ||
            searchModel.subfunctionname ||
            searchModel.settingname) {
            DataQuery()
        }

    }, [])

    //Components
    const HistoryResult = ({ historyResult }) => {
        const columns = [
            {
                id: 1,
                field: "id",
                label: "ID",
                visible: false
            }, {
                id: 2,
                field: "functionname",
                label: "Function Name"
            }, {
                id: 3,
                field: "subfunctionname",
                label: "Sub Function Name"
            }, {
                id: 4,
                field: "settingname",
                label: "Setting Name"
            }, {
                id: 5,
                field: "value",
                label: "Value"
            }, {
                id: 6,
                field: "action",
                label: "Action"
            }, {
                id: 9,
                field: "authorizeto",
                label: "Authorize To"
            }, {
                id: 7,
                field: "lastupdatedtimestamp",
                label: "Last Updated Timestamp",
                cellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex }) => {
                    return `${format(new Date(data.lastupdatedtimestamp), 'yyyy-MM-dd HH:mm:ss')}`
                },
            }, {
                id: 8,
                field: "lastupdateduserid",
                label: "Last Updated Userid"
            }
        ]
        return (<GridTable columns={columns} rows={historyResult} />)
    }

    return (
        <div>
            <MessageCard show={MSG.show} ApiResult={MSG} />
            <div className="row">
                <div className="col">
                    <div className="card">
                        <div className="card-body">
                            <h2 className="card-title">History Search</h2>
                            <div className="input-group">
                                <input type="text"
                                    className="form-control"
                                    placeholder="Function Name"
                                    aria-label="Function Name"
                                    name={"functionname"}
                                    value={searchModel?.functionname}
                                    onChange={(e) => handleChange(e)} />
                                <input type="text"
                                    className="form-control"
                                    placeholder="Sub Function Name"
                                    aria-label="Sub Function Name"
                                    name={"subfunctionname"}
                                    value={searchModel?.subfunctionname}
                                    onChange={(e) => handleChange(e)} />
                                <input type="text"
                                    className="form-control"
                                    placeholder="Setting Name"
                                    aria-label="Setting Name"
                                    name={"settingname"}
                                    value={searchModel?.settingname}
                                    onChange={(e) => handleChange(e)} />
                                <div className="input-group-append">
                                    <button type="button" className="btn btn-outline-secondary" onClick={DataQuery}>Search</button>
                                </div>
                                <div className="input-group-append">
                                    <button type="button" className="btn btn-outline-secondary" onClick={Back}>Back</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col">
                    {historyResult.length > 0 && <HistoryResult historyResult={historyResult} />}
                </div>
            </div>
        </div>
    )
}

const mapDispatchToProps = (dispatch) => ({
    HistoryQuery: (data) => requestHelper.post(dispatch, 'ParameterSetting/HistoryQuery', data)
});

export default connect(null, mapDispatchToProps)(History);
