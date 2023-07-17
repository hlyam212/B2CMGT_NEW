import { RequestActionTypes } from "./request.types";

export const setToken = (token) => ({
    type: RequestActionTypes.SET_TOKEN,
    payload: { token: token },
});
export const clearToken = () => ({
    type: RequestActionTypes.CLEAR_TOKEN,
});

export const requestStarted = (requestName) => ({
    type: RequestActionTypes.REQUEST_STARTED,
    payload: {
        request: {
            name: requestName,
            inProgress: true,
        },
    },
});

export const requestFinished = (requestName) => ({
    type: RequestActionTypes.REQUEST_FINISHED,
    payload: {
        request: {
            name: requestName,
            inProgress: false,
        },
    },
});

export const requestFailed = ({ requestName, error, ownErrorControl }) => ({
    type: RequestActionTypes.REQUEST_FAILED,
    payload: {
        request: {
            name: requestName,
            inProgress: false,
            error,
            ownErrorControl,
        },
    },
});
