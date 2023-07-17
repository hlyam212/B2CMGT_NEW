import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";
import { bubble as Menu } from "react-burger-menu";
import { Tree, NodeRenderer } from "react-arborist";
import TreeModel from "tree-model-improved";

import { selectCurrentUser } from "../redux/user/user.selectors";
import { requestHelper } from "../redux/request/request.helper";

import "./NavMenu.css";
import AppRoutes from "../AppRoutes";

const NavMenu = ({ user, MenuQuery }) => {
    const navigate = useNavigate();
    const [treeModel, setTreeModel] = useState([{ id: "-1" }]);

    useEffect(() => {
        if (user == null) {
            navigate("/");
            return;
        }

        MenuQuery(user).then((result) => {
            let resultObj = JSON.parse(result)
            if (resultObj.succ == false) navigate("/error");
            let tree = new TreeModel();
            setTreeModel(
                tree.parse({
                    id: "-1",
                    name: "Root",
                    fkmgauid: 0,
                    levels: -1,
                    children: resultObj.data,
                })
            );
        })
            .catch((err) => {
                navigate("/error");
            });
    }, []);

    const MenuTree = ({ treeModel }) => {
        const Node = ({ node, style, dragHandle }) => {
            let result = "👉";
            const appRouteElm = AppRoutes.find(function (y) {
                return y.nav == node.data.setting.name;
            });

            if (appRouteElm == undefined) {
                return (
                    <div style={style}>
                        <span className="menu-item">
                            {result}
                            {node.data.setting.name}
                        </span>
                    </div>
                );
            }
            return (
                <div
                    style={style}
                    ref={dragHandle}
                    onDoubleClick={(e) => node.toggle()}
                >
                    <Link
                        key={node.id}
                        to={appRouteElm.path}
                        role="button"
                        className="menu-item"
                    >
                        {result}
                        {node.data.setting.name}
                    </Link>
                </div>
            );
        };
        return (
            <Tree
                initialData={treeModel.model?.children}
                disableDrag={true}
                height={800}
                children={({ style, node, tree, dragHandle, preview }) => { }}
            >
                {Node}
            </Tree>
        );
    };

    return (
        <div>
            <Menu
                id="bubble"
                pageWrapId={"page-wrap"}
                outerContainerId={"outer-container"}
            >
                <h2>EC後臺管理系統</h2>
                <div className="Menu-Tree-Area">
                    <MenuTree treeModel={treeModel} />
                </div>
                <h2>AD : {user}</h2>
            </Menu>
        </div>
    );
};

const mapStateToProps = createStructuredSelector({
    user: selectCurrentUser,
});

const mapDispatchToProps = (dispatch) => ({
    MenuQuery: (userid) => requestHelper.get(dispatch, `Authorization/${userid}`),
});

export default connect(mapStateToProps, mapDispatchToProps)(NavMenu);
