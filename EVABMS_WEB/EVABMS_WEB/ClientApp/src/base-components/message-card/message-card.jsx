import './message-card.css';

const MessageCard = ({ show, ApiResult }) => {
    return(
        <div className={ApiResult?.Succ == false ? "alert alert-danger" : "alert alert-success"} role="alert" style={{ display: show === true ? "" : "none" }}>
            {String(ApiResult?.Message)}
        </div>
    )
}

export default MessageCard

export const MessageTypes = {
    info: "info",
    success: "success",
    error: "error"
};

