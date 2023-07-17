import { PopupActionTypes } from "./popup.types";

const INITIAL_STATE = {
    isShow: false,
    type: null,
    status: null,
    body: {
        icon: null,
        title: null,
        description: [],
        children: null,
    },
    buttons: {
        confirmCallbackFn: null,
        confirmLabel: null,
        abortCallbackFn: null,
        abortLabel: null,
    },
    backLogin: false,
};

const popupReducer = (state = INITIAL_STATE, action) => {
    switch (action.type) {
        case PopupActionTypes.SET_WINDOW_ALERT:
            return {
                ...INITIAL_STATE,
                isShow: true,
                type: action.payload.type,
                status: action.payload.status ?? null,
                body: {
                    ...state.body,
                    children: action.payload.message,
                },
                buttons: {
                    ...state.buttons,
                    confirmCallbackFn: action.payload.callbackFn,
                    confirmLabel: action.payload.btnLabel,
                },
            };
        case PopupActionTypes.SET_WINDOW_CONFIRM:
            return {
                ...INITIAL_STATE,
                isShow: true,
                type: action.payload.type,
                body: {
                    ...state.body,
                    children: action.payload.message,
                },
                buttons: {
                    ...state.buttons,
                    confirmCallbackFn: action.payload.confirmCallbackFn,
                    confirmLabel: action.payload.confirmLabel,
                    abortCallbackFn: action.payload.abortCallbackFn,
                    abortLabel: action.payload.abortLabel,
                },
            };
        case PopupActionTypes.SET_REQUEST_TIMEOUT_ALERT:
            return {
                ...INITIAL_STATE,
                isShow: true,
                type: action.payload.type,
                status: "error",
                body: {
                    ...state.body,
                    icon: "alert",
                    title: "SOMETHING WENT ERROR!",
                    description: [action.payload.error],
                },
                buttons: {
                    ...state.buttons,
                    confirmLabel: "Back to Login...",
                },
                backLogin: true,
            };
        case PopupActionTypes.SET_REQUEST_ERROR_ALERT:
            const { error } = action.payload;
            return {
                ...INITIAL_STATE,
                isShow: true,
                type: action.payload.type,
                status: "error",
                body: {
                    ...state.body,
                    icon: "alert",
                    title: "SOMETHING WENT ERROR!",
                    description: state.body.description.length ? [...state.body.description, error] : [error],
                },
                buttons: {
                    ...state.buttons,
                    confirmLabel: "OK",
                },
            };
        case PopupActionTypes.SET_WARNING_ALERT:
            const { warningWord } = action.payload;
            return {
                ...INITIAL_STATE,
                isShow: true,
                type: action.payload.type,
                status: "warning",
                body: {
                    ...state.body,
                    icon: "warning",
                    title: "WARNING NOTIFICATIONS!",
                    description:
                        !warningWord.prototype.isReactComponent && state.body.description.length
                            ? [...state.body.description, warningWord]
                            : [warningWord],
                    children: warningWord.prototype.isReactComponent ? warningWord : state.body.children,
                },
                buttons: {
                    ...state.buttons,
                    confirmLabel: "OK",
                },
            };
        case PopupActionTypes.SET_QUESTION_CONFIRM:
            return {
                ...INITIAL_STATE,
                isShow: true,
                type: action.payload.type,
                status: "question",
                body: {
                    ...state.body,
                    icon: "question",
                    title: action.payload.question.title || "Please...",
                    description: action.payload.question.description || action.payload.question,
                },
                buttons: {
                    ...state.buttons,
                    confirmCallbackFn: action.payload.confirmCallbackFn,
                    confirmLabel: action.payload.confirmLabel,
                    abortCallbackFn: action.payload.abortCallbackFn,
                    abortLabel: action.payload.abortLabel,
                },
            };
        case PopupActionTypes.CLOSE_POPUP:
            return {
                ...INITIAL_STATE,
                isShow: false,
            };
        default:
            return state;
    }
};

export default popupReducer;
