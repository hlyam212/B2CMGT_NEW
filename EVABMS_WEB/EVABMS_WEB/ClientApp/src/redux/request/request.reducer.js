import { RequestActionTypes } from "./request.types";

const INITIAL_STATE = {
    token: null,
    requests: [],
};

const requestReducer = (state = INITIAL_STATE, action) => {
    switch (action.type) {
        case RequestActionTypes.SET_TOKEN:
            return {
                ...state,
                token: action.payload.token,
            };
        case RequestActionTypes.CLEAR_TOKEN:
            return {
                ...state,
                token: null,
            };
        case RequestActionTypes.REQUEST_STARTED:
            const existingCall = state.requests.find((request) => request.requestName === action.payload.request.name);
            if (existingCall) {
                return {
                    ...state,
                    requests: state.requests.map((request) =>
                        request.name === action.payload.request.name
                            ? { ...request, inProgress: true, error: null }
                            : request
                    ),
                };
            }

            return {
                ...state,
                requests: [...state.requests, action.payload.request],
            };
        case RequestActionTypes.REQUEST_FINISHED:
            return {
                ...state,
                requests: state.requests.filter((request) => request.name !== action.payload.request.name),
            };
        case RequestActionTypes.REQUEST_FAILED:
            return {
                ...state,
                requests: state.requests.map((request) =>
                    request.name === action.payload.request.name
                        ? {
                              ...request,
                              error: action.payload.request.error,
                              ownErrorControl: action.payload.request.ownErrorControl,
                              inProgress: false,
                          }
                        : request
                ),
            };
        default:
            return state;
    }
};

export default requestReducer;
