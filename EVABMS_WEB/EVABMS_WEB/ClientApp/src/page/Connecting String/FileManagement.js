import GridTable from "@nadavshaar/react-grid-table";
import React, { useEffect, useState } from "react";
import { connect } from "react-redux";
import { nanoid } from "nanoid";
import { Button } from "react-bootstrap";
import { createStructuredSelector } from "reselect";

import PageTitle from "../../base-components/page-title/page-title";
import _Modal from "../../base-components/modal/modal.component";
import MessageCard from "../../base-components/message-card/message-card";
import Select from "../../base-components/select/select";

import { requestHelper } from "../../redux/request/request.helper";
import {
    selectCurrentUser,
    selectCurrentUserRole,
} from "../../redux/user/user.selectors";

import "./FileManagement.css";

const FileManagement = ({ FileData, Save, UserRole, UserRoleList, User }) => {
    //useStste
    const [Getfiledata, setGetfiledata] = useState([]);
    const [NoshowData, setNoshowData] = useState([]);
    const [UserRoleOption, setUserRoleOption] = useState([]);
    const [MSG, setMSG] = useState({ show: false });
    const [DataNo, setDataNo] = useState(null);
    const [popup, setpopup] = useState({
        isShow: false,
        title: "Delete Confirmation",
        descriptions: "Are you sure to delete",
        confirmLabel: "Delete",
        rowdata: "",
    });

    //useEffect
    useEffect(() => {
        console.log(UserRole);
        FetchLatestData();
        FetchUserRoleList();
    }, []);

    const FetchUserRoleList = () => {
        UserRoleList()
            .then((result) => {
                let temp = [];
                result.Data.authorizedto.map((item) => {
                    if (UserRole.includes(item.user_role)) {
                        item = {
                            id: item.user_role,
                            name: item.user_role,
                        };
                        temp.push(item);
                    }
                });
                temp.unshift({ id: "", name: "" });
                setUserRoleOption(temp);
            })
            .catch((err) => {
                console.log(err);
            });
    };

    //取最新版本
    const FetchLatestData = () => {
        FileData()
            .then((result) => {
                let temp = [];
                let noshow = [];
                let No = 0;
                UserRole.push("");
                result.Data.map((item) => {
                    if (UserRole.includes(item.UserRole)) {
                        No += 1;
                        item = { ...item, genpassword: item.Password.length, No: No };
                        temp.push(item);
                    } else {
                        noshow.push(item);
                    }
                });
                setGetfiledata(temp);
                setNoshowData(noshow);
            })
            .catch((err) => {
                console.log(err);
            });
    };

    //SaveData
    const SaveData = () => {
        const temp = Getfiledata.map(({ genpassword, ...rest }) => {
            return rest;
        });
        const combined = [...temp, ...NoshowData];
        let data = { userID: User, newmodel: combined };
        Save(data)
            .then((result) => {
                setMSG({
                    Succ: result.Succ,
                    show: true,
                    Message: result.Message,
                });
            })
            .catch((err) => {
                console.log(err);
                setMSG({
                    Succ: false,
                    show: true,
                    Message: "Save Error!Please try again later.",
                });
            });
    };

    //Add Event
    const AddrowData = () => {
        let rowsClone = [...Getfiledata];
        if (rowsClone[0].Name == "") {
            rowsClone.splice(0, 1);
        }
        setDataNo(Math.max(...rowsClone.map((p) => p.No)) + 1);
        let newdata = {
            No: Math.max(...rowsClone.map((p) => p.No)) + 1,
            Name: "",
            DataSource: "",
            UserId: "",
            Password: "",
            Other: "",
            genpassword: 0,
        };
        rowsClone.unshift(newdata);
        setGetfiledata(rowsClone);
    };

    //Delete event
    const DeleteCheckModal = () => {
        let rowsClone = [...Getfiledata];
        let deleteRowIndex = rowsClone.findIndex((r) => r.No === popup.rowdata);
        let deleterow = rowsClone.splice(deleteRowIndex, 1);
        setGetfiledata(rowsClone);
        setpopup({ ...popup, isShow: false });
    };

    //GridTable Component
    const EditableTable = ({ Getfiledata }) => {
        //pwdGenerator
        const Genpwd = (length) => {
            let s = "";
            s = nanoid(length);
            return s;
        };

        const columns = [
            {
                id: 1,
                field: "No",
                label: "No",
                width: "70px",
                editable: false,
            },
            {
                id: 2,
                field: "Name",
                label: "Name",
            },
            {
                id: 3,
                field: "DataSource",
                label: "Datasource",
                width: "max-content",
            },
            {
                id: 4,
                field: "UserId",
                label: "UserId",
                width: "max-content",
            },
            {
                id: 5,
                field: "Password",
                label: "Password",
                width: "max-content",
                editorCellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                    onChange,
                }) => (
                    <textarea
                        className="form-control"
                        aria-label="With textarea"
                        value={data.Password}
                        onChange={(e) => {
                            onChange({ ...data, [column.field]: e.target.value });
                        }}
                    ></textarea>
                ),
            },
            {
                id: 6,
                field: "Other",
                label: "Other",
                width: "max-content",
                editorCellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                    onChange,
                }) => (
                    <textarea
                        className="form-control"
                        aria-label="With textarea"
                        value={data.Other}
                        onChange={(e) => {
                            onChange({ ...data, [column.field]: e.target.value });
                        }}
                    ></textarea>
                ),
            },
            {
                id: 7,
                field: "UserRole",
                label: "UserRole",
                editorCellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                    onChange,
                }) => (
                    <div className="celldiv">
                        <Select
                            defaultValue={value}
                            options={UserRoleOption}
                            onChangeCallBack={(e) =>
                                onChange({ ...data, [column.field]: e.target.value })
                            }
                        />
                    </div>
                ),
            },
            {
                id: 8,
                field: "genpassword",
                label: "GenPassword",
                cellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                }) => (
                    <div className="celldiv">
                        <p className="genpassword">{data.genpassword}</p>
                    </div>
                ),
                editorCellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                    onChange,
                }) => (
                    <div className="celldiv">
                        <input
                            type="text"
                            pattern="[0-9]*"
                            className="input"
                            value={data.genpassword}
                            onChange={(e) => {
                                onChange({
                                    ...data,
                                    [column.field]: e.target.validity.valid
                                        ? e.target.value
                                        : value,
                                });
                            }}
                            onInput={(e) => ({
                                ...data,
                                [column.field]: e.target.validity.valid
                                    ? e.target.value
                                    : value,
                            })}
                        ></input>
                        <Button
                            variant="outline-secondary"
                            size="sm"
                            className="button"
                            onClick={(e) => {
                                let newPw = Genpwd(data.genpassword);
                                onChange({ ...data, Password: newPw });
                            }}
                        >
                            Generate
                        </Button>
                    </div>
                ),
            },
            {
                id: 9,
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
                        <button
                            onClick={(e) => {
                                tableManager.rowEditApi.setEditRowId(data.No);
                            }}
                        >
                            &#x270E;
                        </button>

                        <button
                            onClick={() => {
                                setpopup({ ...popup, isShow: true, rowdata: data.No });
                            }}
                        >
                            &#x1F5D1;
                        </button>
                    </div>
                ),
                editorCellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                    onChange,
                }) => (
                    <div style={{ display: "inline-flex" }}>
                        <button
                            onClick={(e) => {
                                if (data.Name != null && data.Name != "") {
                                    tableManager.rowEditApi.setEditRowId(null);
                                    setDataNo(null);
                                } else {
                                    let rowsClone = [...Getfiledata];
                                    rowsClone.splice(0, 1);
                                    setGetfiledata(rowsClone);
                                    setDataNo(null);
                                }
                            }}
                        >
                            &#x2716;
                        </button>
                        <button
                            onClick={(e) => {
                                let rowsClone = [...tableManager.rowsApi.rows];
                                let updatedRowIndex = rowsClone.findIndex(
                                    (r) => r.No === data.No
                                );
                                rowsClone[updatedRowIndex] = data;
                                if (data.Name != null && data.Name != "") {
                                    setGetfiledata(rowsClone);
                                    tableManager.rowEditApi.setEditRowId(null);
                                    setDataNo(null);
                                } else {
                                    alert("The Name field cannot be Null or Empty");
                                }
                            }}
                        >
                            &#x2714;
                        </button>
                    </div>
                ),
            },
        ];
        return (
            <GridTable
                columns={columns}
                rows={Getfiledata}
                rowIdField={"No"}
                editRowId={DataNo}
                pageSizes={[200, 100, 50, 20]}
            />
        );
    };

    //Render Component
    return (
        <div className="container">
            <PageTitle text={"Connecting String File Management"} />
            <Button variant="outline-dark" size="lg" onClick={SaveData}>
                Export
            </Button>
            <Button variant="outline-dark" size="lg" onClick={AddrowData}>
                Add
            </Button>
            <MessageCard show={MSG.show} ApiResult={MSG} />
            <EditableTable Getfiledata={Getfiledata} />
            {popup.isShow && (
                <_Modal
                    isShow={popup.isShow}
                    title={popup.title}
                    descriptions={popup.descriptions}
                    confirmLabel={popup.confirmLabel}
                    confirmCallbackFn={DeleteCheckModal}
                    abortCallbackFn={() => {
                        setpopup({ ...popup, isShow: false });
                    }}
                />
            )}
        </div>
    );
};

const mapStateToProps = createStructuredSelector({
    UserRole: selectCurrentUserRole,
    User: selectCurrentUser,
});

//API Route
const mapDispatchToProps = (dispatch) => ({
    FileData: () =>
        requestHelper.get(
            dispatch,
            `/ConnectingStringQuery/FileData/${"ConnectString.ini"}`
        ),
    UserRoleList: () =>
        requestHelper.get(dispatch, "Authorization/QueryANode/Query"),
    Save: (data) =>
        requestHelper.post(dispatch, "/ConnectingStringQuery/Update", data),
});

export default connect(mapStateToProps, mapDispatchToProps)(FileManagement);
