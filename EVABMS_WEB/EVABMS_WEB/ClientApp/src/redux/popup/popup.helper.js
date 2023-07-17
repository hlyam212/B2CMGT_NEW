import store from "../store";

import { setWindowAlert, setWindowConfirm } from "./popup.action";

export const alertHelper = (message, callbackFn, btnLabel = "OK", status = "error") => {
    store.dispatch(setWindowAlert(message, callbackFn, btnLabel, status));
};

export const confirmHelper = (message, confirmCallbackFn, abortCallbackFn, confirmLabel = "Yes", abortLabel = "No") => {
    store.dispatch(setWindowConfirm(message, confirmCallbackFn, confirmLabel, abortCallbackFn, abortLabel));
};
