import React, { useState, useRef, useEffect } from 'react';
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";
import { Tree } from "react-arborist";
import TreeModel from "tree-model-improved";
import { Button, Accordion, ButtonGroup } from 'react-bootstrap';
import { format } from 'date-fns'

import { selectCurrentUser } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import _Modal from "../../base-components/modal/modal.component"
import MessageCard from "../../base-components/message-card/message-card"
import CheckBox from "../../base-components/checkbox/checkbox"
import PageTitle from '../../base-components/page-title/page-title'
import Select from "../../base-components/select/select"

import { AuthorizationModel } from "./AuthorizationModel";
import { AuthToModel } from "./AuthToModel";
import './Authorization.css';

const Authorization = ({ user, Query, UserRoleGet, Save, Delete }) => {
    const [userRole, setUserRole] = useState([]);
    const treeRef = useRef();
    const [treeModel, setTreeModel] = useState([{ id: '-1' }]);
    const [selectedData, setSelectedData] = useState({ ...AuthorizationModel });
    const [saveDisable, setSaveDisable] = useState(true);
    const [MSG, setMSG] = useState({
        show: false
    });
    const [popup, setPopup] = useState({
        isShow: false,
        title: "Delete Confirmation",
        descriptions: "Are you sure to delete",
        confirmLabel: "Delete"
    });

    //Events
    useEffect(() => {
        Search()
        UserRoleGet().then((result) => {
            if (result.Succ === false)
                setMSG({ ...result, show: true })
            result.Data.splice(0, 0, "")
            let options = result.Data.map((item) => {
                return { id: item, name: item }
            })
            setUserRole(options)
        }).catch((err) => {
            setMSG({ show: true, Succ: false, Message: err })
        })
    }, [])

    const Search = () => {
        Query().then((result) => {
            if (result.Succ === false)
                setMSG({ ...result, show: true })
            let tree = new TreeModel();
            setTreeModel(tree.parse({
                id: '-1',
                name: 'Root',
                fkmgauid: 0,
                levels: -1,
                children: result.Data
            }))
        }).catch((err) => {
            setMSG({ show: true, Succ: false, Message: err })
        })
    }
    const Add = () => {
        const thisTreeID = treeRef.current.selectedIds.values().next().value
        let newID = `temp-${new Date().getHours()}${new Date().getMinutes()}${new Date().getMilliseconds()}`
        treeModel.walk(function (node) {
            if (node.model.id != thisTreeID) {
                return
            }

            let tree = new TreeModel();
            let newNode = tree.parse({
                ...AuthorizationModel,
                id: newID,
                name: '(new)',
                fkmgauid: node.model.id,
                levels: node.model.levels + 1,
                lastupdateduserid: user
            })
            node.addChild(newNode);
            return false;
        });
        setTreeModel(treeModel)
        for (let i = 0; i <= 1; i++) {
            treeRef.current.selectedNodes[0].toggle()
        }
    }
    const DeleteData = () => {
        Delete({ ...selectedData, lastupdateduserid: user }).then((result) => {
            setMSG({ ...result, show: true })

            const thisTreeID = treeRef.current.selectedIds.values().next().value
            treeModel.walk(function (node) {
                if ((node.model.id === thisTreeID) == false) {
                    return
                }
                node.drop();
                return false;
            });
            setTreeModel(treeModel)
            for (let i = 0; i <= 1; i++) {
                treeRef.current.selectedNodes[0].toggle()
            }

            setPopup({ ...popup, isShow: false })

        }).catch((err) => {
            setMSG({ show: true, Succ: false, Message: err })
        })
    }
    const AddAuthUser = () => {
        let temp = { ...selectedData }
        let newData = { ...AuthToModel }
        newData.id = `temp-${new Date().getHours()}${new Date().getMinutes()}${new Date().getMilliseconds()}`
        newData.lastupdateduserid = user
        temp.newauthorizedto = temp.newauthorizedto ?? []
        temp.newauthorizedto.push(newData)
        setSelectedData(temp);
    }
    const Modify = (node) => {
        if (node.length == 0) {
            return;
        }
        setSelectedData(node[0].data);
        setSaveDisable(false)
    }
    const SaveData = () => {
        if (String(selectedData.id).indexOf("temp") > -1)
            selectedData.id = "0";
        selectedData.newauthorizedto = selectedData.newauthorizedto.map((data, index) => {
            if (String(data.id).indexOf("temp") > -1)
                data.id = 0
            return data
        })

        selectedData.lastupdateduserid = user
        Save(selectedData).then((result) => {
            setMSG({ ...result, show: true })
            setSaveDisable(true)
            Search()
        }).catch((err) => {
            setMSG({ show: true, Succ: false, Message: err })
        });
    }
    const handleChange = (e) => {
        const name = e.target.name;
        const value = e.target.value;
        setSelectedData({ ...selectedData, [name]: value });
    }
    const dateFormat = (input) => {
        let result = ""
        if (input == null) result = "";
        else if (new Date(input) == "Invalid Date") result = "";
        else result = `${format(new Date(input), 'yyyy-MM-dd')}`
        return result;
    }

    //Components
    const Node = ({ node, style, dragHandle }) => {
        let result = "🗀";
        if (node.isSelected) result = "👉"
        else if (node.data.name == "(new)") result = "➕"

        return (
            <div style={style} ref={dragHandle} onDoubleClick={(e) => node.toggle()}>
                {result}
                {node.data.name}
            </div>
        );
    }
    const AuthorizedDetail = (props) => {
        const { selectedData, setSelectedData } = props
        const [dateRange, setDateRange] = useState([null, null]);
        const [startDate, endDate] = dateRange;
        const columns = [
            { id: 1, field: "id", label: "ID", visible: false },
            {
                id: 2,
                field: "userrole",
                label: "User Role",
                editorCellRenderer: ({
                    tableManager,
                    value,
                    data,
                    column,
                    colIndex,
                    rowIndex,
                    onChange
                }) => (
                    <Select defaultValue={value}
                        options={userRole}
                        onChangeCallBack={(e) =>
                            onChange({ ...data, [column.field]: e.target.value })
                        }
                        optionsStrList={userRole} />
                )
            },
            {
                id: 3,
                field: "effectivestart",
                label: "Effective Start",
                width: '170px',
                cellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex }) => {
                    return dateFormat(data.effectivestart)
                },
                editorCellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex, onChange }) => {
                    return (
                        <div>
                            <input type="date"
                                name="trip-start"
                                value={`${dateFormat(data.effectivestart) }`}
                                onChange={(e) => {
                                    onChange({ ...data, [column.field]: e.target.value })
                                }
                                }
                            />
                        </div>)
                }
            },
            {
                id: 4,
                field: "effectiveend",
                label: "Effective End",
                width: '170px',
                cellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex }) => {
                    return dateFormat(data.effectiveend);
                },
                editorCellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex, onChange }) => {
                    return (
                        <div>
                            <input type="date"
                                name="trip-start"
                                value={`${dateFormat(data.effectiveend) }`}
                                onChange={(e) =>
                                    onChange({ ...data, [column.field]: e.target.value })
                                }
                            />
                        </div>)
                }
            },
            {
                id: 5,
                label: "State",
                width: '110px',
                editable: false,
                cellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex }) => (
                    <span
                        style={{
                            color: data.state ? "blue" : "red",
                        }}
                    >
                        {data.state ? "Activate" : "Deactivate"}
                    </span>
                )
            },
            {
                id: 6,
                width: "max-content",
                pinned: true,
                sortable: false,
                resizable: false,
                cellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex }) => (
                    <button
                        onClick={(e) => {
                            tableManager.rowEditApi.setEditRowId(data.id);
                            setSaveDisable(false);
                        }}
                    >&#x270E;</button>
                ),
                editorCellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex, onChange }) => (
                    <div style={{ display: 'inline-flex' }}>
                        <button onClick={(e) => { tableManager.rowEditApi.setEditRowId(null) }}>&#x2716;</button>
                        <button onClick={e => {
                            const rowsClone = { ...selectedData };
                            let updatedRowIndex = rowsClone.authto.findIndex((r) => r.id === data.id);
                            rowsClone.authto[updatedRowIndex] = data;
                            setSelectedData(rowsClone);
                            tableManager.rowEditApi.setEditRowId(null);
                        }}>&#x2714;</button>
                    </div>
                )
            },
        ];
        return (
            <GridTable key={selectedData?.id} columns={columns} rows={selectedData?.authto} showSearch={false} />
        )
    }
    const HistoryDetail = (props) => {
        const { data } = props
        const columns = [
            { id: 1, field: "name", label: "Name" },
            { id: 2, field: "description", label: "Description" },
            { id: 3, field: "action", label: "Action", width: '100px' },
            {
                id: 4,
                field: "lastupdatedtimestamp",
                label: "TimeStamp",
                cellRenderer: ({ tableManager, value, data, column, colIndex, rowIndex }) => (
                    `${format(new Date(data.lastupdatedtimestamp), 'yyyy/MM/dd HH:mm:ss')}`
                ),
            },
            { id: 5, field: "lastupdateduserid", label: "Last Updated UserID" },
        ];
        return (
            <GridTable columns={columns} rows={data} showSearch={false} />
        )
    }
    return (
        <div className="container" id="AuthArea">
            <PageTitle text={"Authorization"} />
            <div className="row">
                <div className="col">
                    <MessageCard show={MSG.show} ApiResult={MSG} />
                    {popup.isShow && (<_Modal isShow={popup.isShow}
                        isShow={popup.isShow}
                        title={popup.title}
                        descriptions={popup.descriptions}
                        confirmLabel={popup.confirmLabel}
                        confirmCallbackFn={DeleteData}
                    />)}
                </div>
            </div>
            <div className="row">
                <div className="col-3">
                    <Tree data={treeModel.model?.children} disableDrag={true} height={1000} onSelect={Modify} ref={treeRef}>{Node}</Tree>
                </div>
                <div className="col-9" style={{ display: saveDisable === false ? "" : "none" }}>
                    <div className="row">
                        <div className="col">
                            <ButtonGroup aria-label="btnFunctions">
                                <Button variant="primary" disabled={saveDisable} onClick={SaveData}>Save</Button>
                                <Button variant="primary" disabled={saveDisable} onClick={Add}>Add</Button>
                                <Button variant="primary" disabled={saveDisable} onClick={() => {
                                    setPopup({ ...popup, isShow: true })
                                }}>Delete</Button>
                            </ButtonGroup>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col">
                            <Accordion defaultActiveKey={['0', '1', '2']} alwaysOpen>
                                <Accordion.Item eventKey="0">
                                    <Accordion.Header><h4>Function Detail</h4></Accordion.Header>
                                    <Accordion.Body>
                                        <div>
                                            <table id="AU00Table" className="table table-bordered">
                                                <tbody>
                                                    <tr>
                                                        <td>Function Name :</td>
                                                        <td>
                                                            <input type="text"
                                                                className="form-control"
                                                                name={"name"}
                                                                value={selectedData.name ?? ''}
                                                                onChange={(e) => handleChange(e)}
                                                            />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>AAMS System Code :</td>
                                                        <td><input type="text"
                                                            className="form-control"
                                                            name={"aams_syscode"}
                                                            value={selectedData.aams_syscode ?? ''}
                                                            onChange={(e) => handleChange(e)}
                                                        /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Description :</td>
                                                        <td><textarea className="form-control"
                                                            aria-label="With textarea"
                                                            name={"description"}
                                                            value={selectedData.description ?? ''}
                                                            onChange={(e) => handleChange(e)}
                                                        ></textarea></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Show on menu :</td>
                                                        <td>
                                                            <CheckBox name="menu"
                                                                value="Y"
                                                                checked={selectedData.menu == "Y"}
                                                                onChangeCallBack={(e) => {
                                                                    const name = e.target.name;
                                                                    const value = e.target.checked ? e.target.value : '';
                                                                    setSelectedData({ ...selectedData, [name]: value });
                                                                }}
                                                                label={"Show on Menu"} />
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </Accordion.Body>
                                </Accordion.Item>
                                <Accordion.Item eventKey="1">
                                    <Accordion.Header><h4>Authorized Detail</h4></Accordion.Header>
                                    <Accordion.Body>
                                        <ButtonGroup aria-label="btnAuthDetailFunctions">
                                            <Button variant="primary" disabled={saveDisable} onClick={AddAuthUser}>Add</Button>
                                        </ButtonGroup>
                                    </Accordion.Body>
                                    <Accordion.Body>
                                        <AuthorizedDetail selectedData={selectedData} setSelectedData={setSelectedData} />
                                    </Accordion.Body>
                                </Accordion.Item>
                                <Accordion.Item eventKey="2">
                                    <Accordion.Header><h4>History</h4></Accordion.Header>
                                    <Accordion.Body>
                                        <HistoryDetail data={selectedData.authorsethistory} />
                                    </Accordion.Body>
                                </Accordion.Item>
                            </Accordion>
                        </div>
                    </div>
                </div>
            </div>
        </div>)
}

const mapStateToProps = createStructuredSelector({
    user: selectCurrentUser
});

const mapDispatchToProps = (dispatch) => ({
    Query: () => requestHelper.get(dispatch, 'Authorization'),
    UserRoleGet: () => requestHelper.post(dispatch, 'Authorization/UserRoleGet', {}),
    Save: (inpudata) => requestHelper.post(dispatch, 'Authorization/Update', inpudata),
    Delete: (inpudata) => requestHelper.post(dispatch, 'Authorization/Delete', inpudata),
});

export default connect(mapStateToProps, mapDispatchToProps)(Authorization);