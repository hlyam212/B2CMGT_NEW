import React, { useState } from 'react';
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";

import { selectCurrentUser, } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import _Modal from "../../base-components/modal/modal.component"
import Select from "../../base-components/select/select"

import './ParameterSetting.css';

const Modify = ({ auth, ddlUserRoles, selectedData, setSelectedData, setMSG, user, Update }) => {
    const [_Data, _setData] = useState({ ...selectedData });

    let isFullControl = auth == "CPU.ParameterSetting.FullControl";
    let canModify = auth != "CPU.ParameterSetting.ReadOnly";

    const handleChange = (e) => {
        const name = e.target.name;
        const value = e.target.value;
        _setData({ ..._Data, [name]: value });
    };

    const Save = () => {
        _Data.lastupdateduserid = user
        Update(_Data).then((result) => {
            setMSG({ ...result, show: true })
            setSelectedData(undefined)
        }).catch((err) => {
            setMSG({ show: true, Succ: false, Message: err })
        });
    }

    const Cancel = () => {
        setSelectedData(undefined)
    }

    return (
        <div className="row" >
            <div className="col">
                {canModify && <button id="btnSave" className="btn btn-primary" onClick={Save}>Save</button>}
                <button id="btnCancel" className="btn btn-primary" onClick={Cancel}>Cancel</button>
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th scope="col">Function Name</th>
                            <th scope="col">
                                {isFullControl
                                    ? <input type="text"
                                        className="form-control"
                                        placeholder="Function Name"
                                        aria-label="Function Name"
                                        name={"functionname"}
                                        value={_Data?.functionname}
                                        onChange={(e) => handleChange(e)} />
                                    : <span>{_Data?.functionname}</span>}
                            </th>
                        </tr>
                        <tr>
                            <th scope="col">Sub Function Name</th>
                            <th scope="col">
                                {isFullControl
                                    ? <input type="text"
                                        className="form-control"
                                        placeholder="Sub Function Name"
                                        aria-label="Sub Function Name"
                                        name={"subfunctionname"}
                                        value={_Data?.subfunctionname}
                                        onChange={(e) => handleChange(e)} />
                                    : <span>{_Data?.subfunctionname}</span>}
                            </th>
                        </tr>
                        <tr>
                            <th scope="col">Setting Name</th>
                            <th scope="col">
                                {isFullControl
                                    ? <input type="text"
                                        className="form-control"
                                        placeholder="Setting Name"
                                        aria-label="Setting Name"
                                        name={"settingname"}
                                        value={_Data?.settingname}
                                        onChange={(e) => handleChange(e)} />
                                    : <span>{_Data?.settingname}</span>}
                            </th>
                        </tr>
                        <tr>
                            <th scope="col">Value</th>
                            <th scope="col">
                                {canModify
                                    ? <textarea
                                        className="form-control"
                                        placeholder="Value"
                                        aria-label="Value"
                                        name={"value"}
                                        value={_Data?.value}
                                        onChange={(e) => handleChange(e)} />
                                    : <span>{_Data?.value}</span>}
                            </th>
                        </tr>
                        <tr>
                            <th scope="col">Description</th>
                            <th scope="col">
                                {isFullControl
                                    ? <textarea
                                        className="form-control"
                                        placeholder="Description"
                                        aria-label="Description"
                                        name={"description"}
                                        value={_Data?.description}
                                        onChange={(e) => handleChange(e)} />
                                    : <span>{_Data?.description}</span>}
                            </th>
                        </tr>
                        {isFullControl &&
                            <tr>
                                <th scope="col">Authorize to :</th>
                                <th scope="col">

                                    <Select defaultValue={_Data?.fk_mgau_id}
                                        options={ddlUserRoles}
                                        onChangeCallBack={(e) => {
                                            let result = parseInt(e.target.value)
                                            result = result == NaN ? 0 : result
                                            _setData({ ..._Data, fk_mgau_id: result });
                                        }} />
                                </th>
                            </tr>}
                    </thead>
                </table>
            </div>
        </div>
    )
}

const mapStateToProps = createStructuredSelector({
    user: selectCurrentUser
});

const mapDispatchToProps = (dispatch) => ({
    Update: (data) => requestHelper.post(dispatch, 'ParameterSetting/Update', data),
});

export default connect(mapStateToProps, mapDispatchToProps)(Modify);
