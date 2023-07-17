import React, { useEffect } from 'react';
import './select.css';

const Select = ({
    defaultValue,
    options,
    onChangeCallBack,
    ...otherProps
}) => {
    const onChangeAbort = (event) => {
        if (onChangeCallBack == undefined) {
            return;
        }
        typeof onChangeCallBack === "function" && onChangeCallBack(event);
    };
    return (
        <div>
            <div className="select">
                <select id="standard-select" value={defaultValue} onChange={onChangeAbort} >
                    {options.map((route) => {
                        return <option key={route.id} value={route.id}>{route.name}</option>
                    })}
                </select>
                <span className="focus"></span>
            </div>
        </div>
    )
};

export default Select;