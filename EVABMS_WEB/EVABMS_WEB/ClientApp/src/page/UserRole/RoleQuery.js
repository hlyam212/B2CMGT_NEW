import React, { useEffect, useState, useRef } from "react";
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";
import Tab from "react-bootstrap/Tab";
import Tabs from "react-bootstrap/Tabs";
import { Tree } from "react-arborist";
import TreeModel from "tree-model-improved";

import PageTitle from "../../base-components/page-title/page-title";
import _Modal from "../../base-components/modal/modal.component";
import MessageCard from "../../base-components/message-card/message-card";

import { selectCurrentUser } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import "./UserRole.css";
import RoleScreenedcolumn from "./RoleScreenedcolumn";

const RoleQuery = ({ Query }) => {
  //useState
  const [UserRoleList, setUserRoleList] = useState([]);
  const [Rolememberlist, setRolememberlist] = useState([]);
  const [treeModel, setTreeModel] = useState([{ id: "-1" }]);
  const [MSG, setMSG] = useState({ show: false });

  //UserRolecolumn
  const UserRolecolumn = [
    {
      id: "checkbox",
      visible: false,
      pinned: true,
      width: "54px",
    },
    { id: 1, field: "user_role", label: "UserRole" },
  ];

  //useEffect
  useEffect(() => {
    DataQuery();
  }, []);

  const DataQuery = () => {
    Query()
      .then((result) => {
        if (!result.Succ) {
          setMSG({
            Succ: false,
            show: true,
            Message: result.Message,
          });
          return;
        }
        let id = 0;
        let temp = result.Data.map((item) => {
          id += 1;
          return { ...item, id: id, user_role: item.Userrole.user_role };
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

  const FunctionTree = ({ treeModel }) => {
    const Node = ({ node, style, dragHandle }) => {
      let result = "👉";
      return (
        <div
          style={style}
          ref={dragHandle}
          onDoubleClick={(e) => node.toggle()}
        >
          {result}
          {node.data.name}
        </div>
      );
    };
    return (
      <Tree
        initialData={treeModel.model?.children}
        disableDrag={true}
        height={800}
      >
        {Node}
      </Tree>
    );
  };

  //render
  return (
    <div className="container">
      <PageTitle text={"RoleQuery"} />
      <MessageCard show={MSG.show} ApiResult={MSG} />
      <div className="row">
        <div className="col-4">
          <GridTable
            className="tableforrolequery"
            columns={UserRolecolumn}
            rows={UserRoleList}
            onRowClick={(
              { rowIndex, data, column, isEdit, event },
              tableManager
            ) => {
              if (
                tableManager.rowSelectionApi.selectedRowsIds.length != 0 &&
                data.id == tableManager.rowSelectionApi.selectedRowsIds[0]
              ) {
                tableManager.rowSelectionApi.setSelectedRowsIds([]);
                setRolememberlist([]);
                let tree = new TreeModel();
                setTreeModel(
                  tree.parse({
                    id: "-1",
                    name: "Root",
                    fkmgauid: 0,
                    levels: -1,
                    children: "",
                  })
                );
                return;
              }
              tableManager.rowSelectionApi.setSelectedRowsIds([data.id]);
              setRolememberlist(data.Rolememberlist);
              let tree = new TreeModel();
              setTreeModel(
                tree.parse({
                  id: "-1",
                  name: "Root",
                  fkmgauid: 0,
                  levels: -1,
                  children: data.Rolefunctionlist,
                })
              );
            }}
          />
        </div>
        <div className="col-8">
          <Tabs
            defaultActiveKey="rolememberlist"
            id="uncontrolled-tab-example"
            className="mb-3"
          >
            <Tab eventKey="rolememberlist" title="權限人員清單">
              <GridTable
                columns={RoleScreenedcolumn()}
                rows={Rolememberlist}
                className="tableforrolelist"
              />
            </Tab>
            <Tab eventKey="rolefunction" title="權限功能清單">
              <FunctionTree treeModel={treeModel} />
            </Tab>
          </Tabs>
        </div>
      </div>
    </div>
  );
};

const mapStateToProps = createStructuredSelector({
  User: selectCurrentUser,
});

const mapDispatchToProps = (dispatch) => ({
  Query: () => requestHelper.get(dispatch, "/UserRole"),
});

export default connect(mapStateToProps, mapDispatchToProps)(RoleQuery);
