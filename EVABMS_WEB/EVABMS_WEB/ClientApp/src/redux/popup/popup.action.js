import { PopupActionTypes } from "./popup.types";

export const setWindowAlert = (message, callbackFn, btnLabel = "OK", status = "error") => ({
    type: PopupActionTypes.SET_WINDOW_ALERT,
    payload: {
        type: "alert",
        message,
        callbackFn,
        btnLabel,
        status,
    },
});

export const setWindowConfirm = (
    message,
    confirmCallbackFn,
    confirmLabel = "Yes",
    abortCallbackFn,
    abortLabel = "No"
) => ({
    type: PopupActionTypes.SET_WINDOW_CONFIRM,
    payload: {
        type: "confirm",
        message,
        confirmCallbackFn,
        confirmLabel,
        abortCallbackFn,
        abortLabel,
    },
});

export const setRequestTimeoutAlert = (error) => ({
    type: PopupActionTypes.SET_REQUEST_TIMEOUT_ALERT,
    payload: {
        type: "alert",
        error: error,
    },
});

export const setRequestErrorAlert = (error) => ({
    type: PopupActionTypes.SET_REQUEST_ERROR_ALERT,
    payload: {
        type: "alert",
        error: error,
    },
});
export const setWarningAlert = (warningWord) => ({
    type: PopupActionTypes.SET_WARNING_ALERT,
    payload: {
        type: "alert",
        warningWord: warningWord,
    },
});

export const setQuestionConfirm = ({
    question,
    confirmCallbackFn,
    abortCallbackFn,
    confirmLabel = "Yes",
    abortLabel = "No",
}) => ({
    type: PopupActionTypes.SET_QUESTION_CONFIRM,
    payload: {
        type: "confirm",
        question,
        confirmCallbackFn,
        confirmLabel,
        abortCallbackFn,
        abortLabel,
    },
});

export const closePopup = () => ({ type: PopupActionTypes.CLOSE_POPUP });
