import React, { useState, useEffect } from "react";
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";
import { Button } from "react-bootstrap";

import PageTitle from "../../base-components/page-title/page-title";
import _Modal from "../../base-components/modal/modal.component";
import MessageCard from "../../base-components/message-card/message-card";

import { selectCurrentUser } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import LotteryDetail from "./LotteryDetail";
import "./Lottery.css";

const Lottery = ({ Query, Delete }) => {
  let selectedparam = { id: 0, subject: "" };

  //usestate
  const [QueryData, setQueryData] = useState([]);
  const [SelectedData, setSelectedData] = useState(selectedparam);
  const [State, setState] = useState("start");
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
  }, [State == "start"]);

  //Event
  const Add = () => {
    setState("Detail");
    setSelectedData(selectedparam);
  };

  const DeleteCheckModal = () => {
    Delete(popup.rowid)
      .then((result) => {
        DataQuery();
        setMSG({
          Succ: result.Succ,
          show: true,
          Message: result.Message,
        });
        setTimeout(() => {
          setMSG({ ...MSG, show: false });
        }, 5000);
      })
      .catch((err) => {
        setMSG({
          Succ: false,
          show: true,
          Message: err,
        });
      });
    setpopup({ ...popup, isShow: false });
  };

  const DataQuery = () => {
    if (State == "Detail") {
      setMSG({ ...MSG, show: false });
      return;
    }
    Query().then((result) => {
      result.Data.reverse();
      setQueryData(result.Data);
      if (MSG.Message == "Query Success") return;
      setMSG({
        Succ: result.Succ,
        show: true,
        Message: result.Succ ? "Query Success" : result.Message,
      });
      setTimeout(() => {
        setMSG({ ...MSG, show: false });
      }, 5000);
    });
  };

  const Select = () => {
    //tablecoulmns
    const columns = [
      { id: 1, field: "id", label: "ID" },
      { id: 2, field: "subject", label: "SUBJECT" },
      { id: 3, field: "last_updated_timestamp", label: "LastUpdatedTime" },
      { id: 4, field: "last_updated_userid", label: "USERID" },
      { id: 5, field: "ENV", label: "ENV" },
      {
        id: 6,
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
              onClick={() => {
                setSelectedData({ id: data.id, subject: data.subject });
                setState("Detail");
              }}
            >
              &#x1F58A;
            </button>
            <button
              onClick={() => {
                setpopup({
                  ...popup,
                  isShow: true,
                  rowid: data.id,
                  descriptions: `Are you sure to delete ${data.subject} ?`,
                });
              }}
            >
              &#x274C;
            </button>
          </div>
        ),
      },
    ];
    return (
      <div>
        <Button onClick={Add}>Add</Button>
        <div className="tableforlottery">
          <GridTable
            key={QueryData?.id}
            columns={columns}
            rows={QueryData}
            showSearch={false}
          />
        </div>
      </div>
    );
  };

  //render component
  return (
    <div className="container">
      <PageTitle text={"Lottery"} />
      <MessageCard show={MSG.show} ApiResult={MSG} />
      {State == "start" && <Select />}
      {State == "Detail" && (
        <LotteryDetail
          setState={setState}
          SelectedData={SelectedData}
          setSelectedData={setSelectedData}
        />
      )}
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
  user: selectCurrentUser,
});

const mapDispatchToProps = (dispatch) => ({
  Query: () => requestHelper.get(dispatch, "/Lottery"),
  Save: (input) => requestHelper.post(dispatch, "/Lottery/Update", input),
  Queryone: (id) => requestHelper.get(dispatch, `/Lottery/${id}`),
  Delete: (id) => requestHelper.get(dispatch, `/Lottery/Delete/${id}`),
});

export default connect(mapStateToProps, mapDispatchToProps)(Lottery);
