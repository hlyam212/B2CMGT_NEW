import React, { useEffect, useState, useRef } from "react";
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";
import { Button } from "react-bootstrap";

import PageTitle from "../../base-components/page-title/page-title";
import MessageCard from "../../base-components/message-card/message-card";
import _Modal from "../../base-components/modal/modal.component";

import { selectCurrentUser } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

const RoleModify = ({ Query, User, Update }) => {
  //useState
  const [UserRoleList, setUserRoleList] = useState([]);
  const [DataNo, setDataNo] = useState(null);
  const [MSG, setMSG] = useState({ show: false });
  const [popup, setpopup] = useState({
    isShow: false,
    title: "Delete Confirmation",
    descriptions: "Are you sure to delete?",
    confirmLabel: "Delete",
    rowid: 0,
  });

  //useEffect
  useEffect(() => {
    DataQuery();
  }, []);

  const DataQuery = () => {
    Query()
      .then((result) => {
        if (!result.Succ) {
          setMSG({
            Succ: result.Succ,
            show: true,
            Message: result.Message,
          });
          return;
        }
        let id = 0;
        let temp = [];
        result.Data.map((item) => {
          id += 1;
          item.Userrole = { ...item.Userrole, id: id };
          temp.push(item.Userrole);
        });
        setUserRoleList(temp);
      })
      .catch((err) => {
        setMSG({
          Succ: false,
          show: true,
          Message: err,
        });
      });
  };

  const Save = () => {
    const temp = UserRoleList.map(({ id, ...rest }) => {
      return rest;
    });
    Update(temp)
      .then((result) => {
        if (result.Succ) DataQuery();
        setMSG({
          Succ: result.Succ,
          show: true,
          Message: result.Message,
        });
      })
      .catch((err) => {
        setMSG({
          Succ: false,
          show: true,
          Message: err,
        });
      });
  };

  const AddrowData = () => {
    let rowsClone = [...UserRoleList];
    if (rowsClone[0].user_role == "") {
      rowsClone.splice(0, 1);
    }
    setDataNo(Math.max(...rowsClone.map((p) => p.id)) + 1);
    let newdata = {
      id: Math.max(...rowsClone.map((p) => p.id)) + 1,
      user_role: "",
      description: "",
      last_updated_timestamp: null,
      last_updated_userid: User,
      editable: true,
    };
    rowsClone.unshift(newdata);
    setUserRoleList(rowsClone);
  };

  const DeleteCheckModal = () => {
    let rowsClone = [...UserRoleList];
    let deleteRowIndex = rowsClone.findIndex((r) => r.id === popup.rowid);
    let deleterow = rowsClone.splice(deleteRowIndex, 1);
    setUserRoleList(rowsClone);
    setpopup({ ...popup, isShow: false });
  };

  //UserRolecolumn
  const EditableTable = ({ UserRoleList }) => {
    const UserRolecolumn = [
      { id: 1, field: "id", label: "ID", width: "70px", editable: false },
      {
        id: 2,
        field: "user_role",
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
          <div>
            {data.editable && (
              <input
                value={data.user_role}
                onChange={(e) => {
                  onChange({ ...data, [column.field]: e.target.value });
                }}
              ></input>
            )}
            {!data.editable && <label>{data.user_role}</label>}
          </div>
        ),
      },
      { id: 3, field: "editable", label: "editable", visible: false },
      {
        id: 4,
        field: "description",
        label: "Description",
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
            value={data.description}
            onChange={(e) => {
              onChange({ ...data, [column.field]: e.target.value });
            }}
          ></textarea>
        ),
      },
      {
        id: 5,
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
                tableManager.rowEditApi.setEditRowId(data.id);
              }}
            >
              &#x270E;
            </button>
            <button
              onClick={() => {
                setpopup({ ...popup, isShow: true, rowid: data.id });
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
                if (data.user_role != null && data.user_role != "") {
                  tableManager.rowEditApi.setEditRowId(null);
                  setDataNo(null);
                } else {
                  let rowsClone = [...UserRoleList];
                  rowsClone.splice(0, 1);
                  setUserRoleList(rowsClone);
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
                  (r) => r.id === data.id
                );
                rowsClone[updatedRowIndex] = data;
                if (data.user_role != null && data.user_role != "") {
                  setUserRoleList(rowsClone);
                  tableManager.rowEditApi.setEditRowId(null);
                  setDataNo(null);
                } else {
                  alert("The UserRole field cannot be Null or Empty");
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
        className="tableforrolemodify"
        columns={UserRolecolumn}
        rows={UserRoleList}
        pageSize={10}
        editRowId={DataNo}
      />
    );
  };

  //render
  return (
    <div className="container">
      <PageTitle text={"RoleModify"} />
      <MessageCard show={MSG.show} ApiResult={MSG} />
      <Button variant="primary" onClick={AddrowData}>
        Add
      </Button>
      <Button className="float-right" variant="success" onClick={Save}>
        Save
      </Button>
      <div className="grid-div">
        <EditableTable UserRoleList={UserRoleList} />
      </div>
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
  User: selectCurrentUser,
});

const mapDispatchToProps = (dispatch) => ({
  Query: () => requestHelper.get(dispatch, "/UserRole"),
  Update: (data) => requestHelper.post(dispatch, "/UserRole/Update", data),
});

export default connect(mapStateToProps, mapDispatchToProps)(RoleModify);
