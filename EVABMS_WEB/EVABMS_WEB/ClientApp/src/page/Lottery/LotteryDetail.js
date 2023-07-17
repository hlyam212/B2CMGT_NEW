import React, { useState, useEffect, useRef } from "react";
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";
import { Button } from "react-bootstrap";
import Tab from "react-bootstrap/Tab";
import Tabs from "react-bootstrap/Tabs";

import _Modal from "../../base-components/modal/modal.component";
import MessageCard from "../../base-components/message-card/message-card";

import { selectCurrentUser } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import "./Lottery.css";

const LotteryDetail = ({
  User,
  Queryone,
  setState,
  SelectedData,
  setSelectedData,
  Draw,
  Save,
  Savenamelist,
  Saveprizelist,
  Deletenamelist,
  Deleteprizelist,
}) => {
  //param
  let LotteryData = {
    id: 0,
    subject: "",
    list_column: "",
    process_status: "",
    last_updated_userid: User,
    ENV: "",
  };

  let LotteryModel = {
    query_data: LotteryData,
    file_list: [],
    file_prize: [],
    data_list: [],
    data_prize: [],
    winner_list: [],
  };

  const columnPrizelist = [
    { id: 1, field: "id", label: "ID", visible: false },
    { id: 2, field: "name", label: "獎品" },
    { id: 3, field: "numbers", label: "獎品數" },
  ];

  //useState
  const [input_list, setinput_list] = useState("");
  const [input_prize, setinput_prize] = useState("");
  const [NameList, setNameList] = useState([]);
  const [PrizeList, setPrizeList] = useState([]);
  const [WinnerList, setWinnerList] = useState([]);
  const [ColumnNameList, setColumnNameList] = useState([]);
  const [ColumnWinnerList, setColumnWinnerList] = useState([]);
  const [Datamodel, setDatamodel] = useState({ ...LotteryModel });
  const [MSG, setMSG] = useState({ show: false });
  const [popup, setpopup] = useState({
    isShow: false,
    title: "Delete Confirmation",
    descriptions: "Are you sure to delete?",
    confirmLabel: "Delete",
    buttonid: "",
  });

  //useRef
  const listRef = useRef(null);
  const prizeRef = useRef(null);

  //useEffect
  useEffect(() => {
    if (SelectedData.id == 0) {
      return setDatamodel({
        ...Datamodel,
        query_data: { ...Datamodel.query_data, subject: "" },
      });
    }
    DataQuery(SelectedData.id);
  }, [SelectedData.id]);

  useEffect(() => {
    if (Datamodel.file_list == null || Datamodel.file_list.length == 0) return;
    CallApi_List_Insert();
  }, [Datamodel.file_list]);

  useEffect(() => {
    if (Datamodel.file_prize == null || Datamodel.file_prize.length == 0)
      return;
    CallApi_Prize_Insert();
  }, [Datamodel.file_prize]);

  //function
  const timeout = () => {
    setTimeout(() => {
      setMSG({ ...MSG, show: false });
    }, 5000);
  };

  const genColumn = (columnstring) => {
    let listcolumn = columnstring.split(";");
    let namecolumn = [];
    let i = 1;
    listcolumn.map((item) => {
      namecolumn.push({ id: i, field: item, label: item });
      i++;
    });
    let result = { listcolumn, namecolumn };
    return result;
  };

  const liststringTogrid = (string) => {
    let content = string.split(/\r?\n/);
    let initial_content = string.split(/\r?\n/);
    setDatamodel({ ...Datamodel, file_list: initial_content });
    let listcolumn = genColumn(content[0]).listcolumn;
    let namecolumn = genColumn(content[0]).namecolumn;
    setColumnNameList(namecolumn);
    content.shift();
    const rows = content.reduce((accumulator, currentValue) => {
      let cols = currentValue.split(";");
      let result = {};
      cols.map((item, index) => {
        result = { ...result, [listcolumn[index]]: item };
      });
      accumulator.push(result);
      return accumulator;
    }, []);
    setNameList(rows);
  };

  const prizestringTogrid = (string) => {
    let content = string.split(/\r?\n/);
    let initial_content = string.split(/\r?\n/);
    setDatamodel({ ...Datamodel, file_prize: initial_content });
    let rows = [];
    content.map((item) => {
      let row = item.split(";");
      rows.push({ name: row[0], numbers: row[1] });
    });
    setPrizeList(rows);
  };

  const uploadmseeage = () => {
    setMSG({
      Succ: false,
      show: true,
      Message: "資料尚未儲存!如需儲存請按右上角的Save",
    });
  };

  const errorMessage = (err) => {
    setMSG({
      Succ: false,
      show: true,
      Message: err,
    });
  };

  //DataEvent
  const DataQuery = (id) => {
    Queryone(id).then((result) => {
      setDatamodel(result.Data);
      let listcolumn = genColumn(result.Data.query_data.list_column).listcolumn;
      let namecolumn = genColumn(result.Data.query_data.list_column).namecolumn;
      let winnercolumn = genColumn(
        result.Data.query_data.list_column
      ).namecolumn;
      winnercolumn.push({
        id: winnercolumn.length + 1,
        field: "name",
        label: "獎品",
      });
      setColumnNameList(namecolumn);
      setColumnWinnerList(winnercolumn);

      let prizeArray = result.Data.data_prize;
      const handleresultlist = (resultlist) => {
        const rows = resultlist.reduce((accumulator, currentValue) => {
          let cols = currentValue.list_content_enc.split(";");
          let result = {};
          cols.map((item, index) => {
            result = { ...result, [listcolumn[index]]: item };
          });
          if (currentValue.fk_mlop_id != null) {
            let prizename = prizeArray.find(
              (r) => r.id == currentValue.fk_mlop_id
            ).name;
            result = { ...result, name: prizename };
          }
          accumulator.push(result);
          return accumulator;
        }, []);
        return rows;
      };

      let namelist = handleresultlist(result.Data.data_list);
      setNameList(namelist);

      setPrizeList(result.Data.data_prize);

      let winnerlist = handleresultlist(result.Data.winner_list);
      setWinnerList(winnerlist);
    });
  };

  const CallApi_List_Insert = () => {
    if (SelectedData.id == 0) return;
    Savenamelist(Datamodel)
      .then((result) => {
        setMSG({
          Succ: result.Succ,
          show: true,
          Message: result.Message,
        });
        setinput_list("");
        DataQuery(SelectedData.id);
        timeout();
      })
      .catch((err) => {
        errorMessage(err);
      });
  };

  const CallApi_Prize_Insert = () => {
    if (SelectedData.id == 0) return;
    Saveprizelist(Datamodel)
      .then((result) => {
        setMSG({
          Succ: result.Succ,
          show: true,
          Message: result.Message,
        });
        setinput_prize("");
        DataQuery(SelectedData.id);
        timeout();
      })
      .catch((err) => {
        errorMessage(err);
      });
  };

  const Back = () => {
    setState("start");
  };

  //SaveAll
  const SaveData = () => {
    if (Datamodel.winner_list.length != 0) {
      Back();
      return;
    }
    Save(Datamodel)
      .then((result) => {
        setSelectedData({ ...SelectedData, id: result.Data });
        setinput_list("");
        setinput_prize("");
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

  //Delete
  const Delete = () => {
    if (popup.buttonid == "delete-list") {
      Deletenamelist(SelectedData.id)
        .then((result) => {
          setMSG({
            Succ: result.Succ,
            show: true,
            Message: result.Message,
          });
          setNameList(result.Succ ? [] : NameList);
        })
        .catch((err) => {
          console.log(err);
        });
    } else {
      Deleteprizelist(SelectedData.id)
        .then((result) => {
          setMSG({
            Succ: result.Succ,
            show: true,
            Message: result.Message,
          });
          setPrizeList(result.Succ ? [] : PrizeList);
        })
        .catch((err) => {
          console.log(err);
        });
    }
    setpopup({ ...popup, isShow: false });
    DataQuery(SelectedData.id);
  };

  //Upload
  const handlelistChange = (e) => {
    e.preventDefault();
    if ((e.target.files && e.target.files[0]) == false) return;
    try {
      const file = e.target.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        liststringTogrid(e.target.result);
        if (SelectedData.id == 0) uploadmseeage();
      };
      reader.readAsText(file);
    } catch (err) {
      errorMessage(err);
    }
  };

  const handleprizeChange = (e) => {
    e.preventDefault();
    if ((e.target.files && e.target.files[0]) == false) return;
    try {
      const file = e.target.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        prizestringTogrid(e.target.result);
        if (SelectedData.id == 0) uploadmseeage();
      };
      reader.readAsText(file);
    } catch (err) {
      errorMessage(err);
    }
  };

  const onButtonClick = (e) => {
    if (e.currentTarget.id == "name-list") {
      if (input_list) {
        liststringTogrid(input_list);
        uploadmseeage();
      } else {
        listRef.current.click();
      }
    } else {
      if (input_prize) {
        prizestringTogrid(input_prize);
        uploadmseeage();
      } else {
        prizeRef.current.click();
      }
    }
  };

  //Draw
  const onDrawClick = () => {
    if (!Datamodel.data_list || !Datamodel.data_prize) {
      return setMSG({
        Succ: false,
        show: true,
        Message: "缺少抽獎名單或獎品!",
      });
    }
    let namecount = Datamodel.data_list.length;
    let prizecount;
    Datamodel.data_prize.map((item) => {
      prizecount += item.numbers;
    });
    if (namecount < prizecount) {
      return setMSG({
        Succ: false,
        show: true,
        Message: "獎品數大於抽獎人數!",
      });
    }
    Draw(SelectedData.id)
      .then((result) => {
        if (result.Succ == false) {
          setMSG({
            Succ: result.Succ,
            show: true,
            Message: result.Message,
          });
          return;
        }
        DataQuery(SelectedData.id);
        setMSG({
          Succ: result.Succ,
          show: true,
          Message: "恭喜以下中獎人!",
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

  //ModifyNameListComponet
  const ModifyNameList = React.useMemo(() => {
    return (
      <div className="container">
        <div className="row">
          <div className="col">
            <input
              ref={listRef}
              type="file"
              id="input-file-upload"
              multiple={true}
              onChange={handlelistChange}
              accept=".txt"
            />
            <Button
              disabled={Datamodel.winner_list.length != 0}
              id="name-list"
              variant="primary"
              onClick={onButtonClick}
            >
              Upload
            </Button>
            <Button
              disabled={Datamodel.winner_list.length != 0}
              id="delete-list"
              variant="danger"
              onClick={(e) => {
                setpopup({
                  ...popup,
                  isShow: true,
                  buttonid: e.currentTarget.id,
                });
              }}
            >
              Delete
            </Button>
          </div>
        </div>
        <div className="row">
          <div className="col">
            <textarea
              value={input_list}
              onChange={(e) => setinput_list(e.target.value)}
              className="form-control"
              aria-label="With textarea"
              placeholder=" 姓名;部門
              AAA;P13
              XXX;P14"
            />
          </div>
        </div>
      </div>
    );
  });

  //ModifyPrizeListComponet
  const ModifyPrizeList = React.useMemo(() => {
    return (
      <div className="container">
        <div className="row">
          <div className="col">
            <input
              ref={prizeRef}
              type="file"
              id="input-file-upload"
              multiple={true}
              onChange={handleprizeChange}
              accept=".txt"
            />
            <Button
              disabled={Datamodel.winner_list.length != 0}
              id="prize-list"
              variant="primary"
              onClick={onButtonClick}
            >
              Upload
            </Button>
            <Button
              disabled={Datamodel.winner_list.length != 0}
              id="delete-prize"
              variant="danger"
              onClick={(e) => {
                setpopup({
                  ...popup,
                  isShow: true,
                  buttonid: e.currentTarget.id,
                });
              }}
            >
              Delete
            </Button>
          </div>
        </div>
        <div className="row">
          <div className="col">
            <textarea
              value={input_prize}
              onChange={(e) => setinput_prize(e.target.value)}
              className="form-control"
              aria-label="With textarea"
              placeholder="車子;10
手機;20"
            />
          </div>
        </div>
      </div>
    );
  });

  const ShowGridView = (props) => {
    const { rows, columns } = props;
    return (
      <GridTable
        pageSize={10}
        key={columns?.id}
        columns={columns}
        rows={rows}
        showSearch={false}
      />
    );
  };

  return (
    <div className="container">
      <div className="row">
        <div className="col">
          <Button
            disabled={!Datamodel.query_data.subject}
            className="float-right"
            variant="success"
            onClick={SaveData}
          >
            Save
          </Button>
          <Button onClick={Back} className="float-right" variant="secondary">
            Back
          </Button>

          {popup.isShow && (
            <_Modal
              isShow={popup.isShow}
              title={popup.title}
              descriptions={popup.descriptions}
              confirmLabel={popup.confirmLabel}
              confirmCallbackFn={Delete}
              abortCallbackFn={() => {
                setpopup({ ...popup, isShow: false });
              }}
            />
          )}
        </div>
      </div>
      <div className="row">
        <MessageCard show={MSG.show} ApiResult={MSG} />
        <div className="col">
          Subject:
          <input
            type="text"
            className="form-control"
            size="50"
            value={Datamodel.query_data.subject}
            onChange={(e) => {
              setDatamodel({
                ...Datamodel,
                query_data: {
                  ...Datamodel.query_data,
                  subject: e.target.value,
                },
              });
            }}
          ></input>
        </div>
      </div>
      <div className="row">
        <div className="col">
          <Tabs
            defaultActiveKey="list"
            id="uncontrolled-tab-example"
            className="mb-3"
          >
            <Tab eventKey="list" title="名單">
              <div className="container">
                <div className="row">
                  <div className="col-4">
                    {Datamodel.winner_list.length == 0 && ModifyNameList}
                  </div>
                  <div className="col-8">
                    <ShowGridView columns={ColumnNameList} rows={NameList} />
                  </div>
                </div>
              </div>
            </Tab>
            <Tab eventKey="prize" title="獎品">
              <div className="container">
                <div className="row">
                  <div className="col-4">
                    {Datamodel.winner_list.length == 0 && ModifyPrizeList}
                  </div>
                  <div className="col-8">
                    <ShowGridView columns={columnPrizelist} rows={PrizeList} />
                  </div>
                </div>
              </div>
            </Tab>
            {SelectedData.id != 0 && (
              <Tab
                eventKey="result"
                title={
                  Datamodel.winner_list.length == 0 ? "開始抽獎" : "中獎名單"
                }
              >
                <div className="container">
                  <div className="row">
                    <div className="col-4">
                      {Datamodel.winner_list.length == 0 && (
                        <Button
                          disabled={Datamodel.winner_list.length != 0}
                          id="Draw"
                          variant="primary"
                          onClick={onDrawClick}
                        >
                          Draw
                        </Button>
                      )}
                    </div>
                    <div className="col-8">
                      <label className="selectlabel">中獎名單:</label>
                      <ShowGridView
                        columns={ColumnWinnerList}
                        rows={WinnerList}
                      />
                    </div>
                  </div>
                </div>
              </Tab>
            )}
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
  //Query: () => requestHelper.get(dispatch, "/Lottery"),
  Queryone: (id) => requestHelper.get(dispatch, `/Lottery/${id}`),
  Save: (input) => requestHelper.post(dispatch, "/Lottery/Update", input),
  Savenamelist: (input) =>
    requestHelper.post(dispatch, "/Lottery/List/Insert", input),
  Saveprizelist: (input) =>
    requestHelper.post(dispatch, "/Lottery/Prize/Insert", input),
  Deletenamelist: (id) =>
    requestHelper.post(dispatch, `/Lottery/List/Delete/${id}`),
  Deleteprizelist: (id) =>
    requestHelper.post(dispatch, `/Lottery/Prize/Delete/${id}`),
  Draw: (id) => requestHelper.post(dispatch, `/Lottery/Draw/${id}`),
});

export default connect(mapStateToProps, mapDispatchToProps)(LotteryDetail);
