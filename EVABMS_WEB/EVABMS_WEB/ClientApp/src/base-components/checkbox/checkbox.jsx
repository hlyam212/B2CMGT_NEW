import './checkbox.css';

const CheckBox = ({
    name,
    value,
    checked,
    onChangeCallBack,
    label,
    ...otherProps
}) => {
    const onChangeAbort = (event) => {
        if (onChangeCallBack == undefined) {
            return;
        }
        typeof onChangeCallBack === "function" && onChangeCallBack(event);
    };
    return (
        <div className="checkbox-wrapper-4" >
            <input className="inp-cbx" id={name} name={name} value={value} checked={checked} onChange={onChangeAbort} type="checkbox" />
            <label className="cbx" htmlFor={name} >
                <span >
                    <svg width="12px" height="10px" >
                        <use xlinkHref="#check-4" />
                    </svg>
                </span>
                <span>{label}</span>
            </label>
            <svg className="inline-svg" >
                <symbol id="check-4" viewbox="0 0 12 10" >
                    <polyline points="1.5 6 4.5 9 10.5 1" />
                </symbol>
            </svg>
        </div>
    )
};

export default CheckBox;