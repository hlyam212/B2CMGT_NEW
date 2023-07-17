import React, { useEffect, useState, useRef } from "react";
import GridTable from "@nadavshaar/react-grid-table";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";

import PageTitle from "../../base-components/page-title/page-title";
import Select from "../../base-components/select/select";

import { selectCurrentUserRole } from "../../redux/user/user.selectors";
import { requestHelper } from "../../redux/request/request.helper";

import "./Query.css";
import getColumns from "./QueryTableColumns";

const Query = ({ FileName, FileData, FileUpload, UserRole }) => {
  let initialState = "Drag and drop your file here or";

  //useState
  const [Getfilename, setGetfilename] = useState([]);
  const [Getfiledata, setGetfiledata] = useState([]);
  const [Uploadname, setUploadname] = useState(initialState);
  const [selected, SetSelected] = useState("ConnectString.ini");

  //useRef
  const inputRef = useRef(null);

  //drag state
  const [dragActive, setDragActive] = useState(false);

  //useEffect
  useEffect(() => {
    FileName()
      .then((result) => {
        let temp = result.Data.map((item) => ({ id: item, name: item }));
        temp.reverse();
        setGetfilename(temp);
      })
      .catch((err) => {
        console.log(err);
      });
  }, []);

  useEffect(() => {
    DataQuery();
    setUploadname(initialState);
  }, [selected]);

  //show screened
  const DataQuery = () => {
    FileData(selected)
      .then((result) => {
        let temp = [];
        let No = 0;
        UserRole.push("");
        result.Data.map((item) => {
          if (UserRole.includes(item.UserRole)) {
            No += 1;
            item = { ...item, No: No };
            temp.push(item);
          }
        });
        setGetfiledata(temp);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  //show nonscreened
  const DropDataQuery = (filedata) => {
    FileUpload(filedata)
      .then((result) => {
        let No = 0;
        result.Data = result.Data.map((item) => {
          No += 1;
          return { ...item, No: No };
        });
        setGetfiledata(result.Data);
      })
      .catch((err) => {
        console.log(err);
      });
  };

  //DragDropFile components
  const DragDropFile = () => {
    // handle drag events
    const handleDrag = (e) => {
      e.preventDefault();
      e.stopPropagation();
      if (e.type === "dragenter" || e.type === "dragover") {
        setDragActive(true);
      } else if (e.type === "dragleave") {
        setDragActive(false);
      }
    };

    // triggers when file is dropped
    const handleDrop = function (e) {
      e.preventDefault();
      e.stopPropagation();
      setDragActive(false);
      if (e.dataTransfer.files && e.dataTransfer.files[0]) {
        // at least one file has been dropped so do something
        // handleFiles(e.dataTransfer.files);
        try {
          const file = e.dataTransfer.files[0];
          const reader = new FileReader();

          setUploadname(e.dataTransfer.files[0].name);

          reader.onload = (e) => {
            DropDataQuery(e.target.result);
          };

          reader.readAsText(file);
        } catch (err) {
          console.log(err);
        }
      }
    };

    // triggers when file is selected with click
    const handleChange = function (e) {
      e.preventDefault();
      if (e.target.files && e.target.files[0]) {
        // at least one file has been selected so do something
        // handleFiles(e.target.files);
        try {
          const file = e.target.files[0];
          const reader = new FileReader();

          setUploadname(e.target.files[0].name);

          reader.onload = (e) => {
            DropDataQuery(e.target.result);
          };

          reader.readAsText(file);
        } catch (err) {
          console.log(err);
        }
      }
    };

    // triggers the input when the button is clicked
    const onButtonClick = () => {
      inputRef.current.click();
    };

    return (
      <form
        id="form-file-upload"
        onDragEnter={handleDrag}
        onSubmit={(e) => e.preventDefault()}
      >
        <input
          ref={inputRef}
          type="file"
          id="input-file-upload"
          multiple={true}
          onChange={handleChange}
          accept=".ini"
        />
        <label id="label-file-upload" htmlFor="input-file-upload">
          <div>
            <p className="pname">{Uploadname}</p>
            <button className="upload-button" onClick={onButtonClick}>
              Upload a file
            </button>
          </div>
        </label>
        {dragActive && (
          <div
            id="drag-file-element"
            onDragEnter={handleDrag}
            onDragLeave={handleDrag}
            onDragOver={handleDrag}
            onDrop={handleDrop}
          ></div>
        )}
      </form>
    );
  };

  //Render Component
  return (
    <div className="container">
          <PageTitle text={"Data Base Connect String List"} />
          <div className="row">
              <div className="col-6">
                  <label className="selectlabel">Version Select : </label>
                  <Select
                      defaultValue={selected}
                      options={Getfilename}
                      onChangeCallBack={(e) => {
                          SetSelected(e.target.value);
                      }}
                  />
              </div>
              <div className="col-6">
                  <DragDropFile />
              </div>
          </div>
      <GridTable
        className="GridTable"
        columns={getColumns()}
        rows={Getfiledata}
      />
    </div>
  );
};

const mapStateToProps = createStructuredSelector({
  UserRole: selectCurrentUserRole,
});

//API Route
const mapDispatchToProps = (dispach) => ({
  FileName: () => requestHelper.get(dispach, "/ConnectingStringQuery/FileName"),
  FileData: (fileName) =>
    requestHelper.get(dispach, `/ConnectingStringQuery/FileData/${fileName}`),
  FileUpload: (content) =>
    requestHelper.post(dispach, `/ConnectingStringQuery/FileData`, {
      filecontent: content,
    }),
});

export default connect(mapStateToProps, mapDispatchToProps)(Query);
