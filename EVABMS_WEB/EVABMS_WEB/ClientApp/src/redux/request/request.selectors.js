import { createSelector } from "reselect";

//get request part of the state
const requestState = (state) => state.request;

export const currentToken = createSelector([requestState], (request) => request.token);

export const requestsInProgress = createSelector(
    [requestState],
    ({ requests }) => requests?.filter((request) => request.inProgress).length > 0
);

//get requests in progress either by single requestName or by requestNames array
export const namedRequestsInProgress = createSelector([requestState], ({ requests }, requestName) => {
    const singleNamedRequestInProgress = (singleRequestName) =>
        requests.find((request) => request.name === singleRequestName && request.inProgress) !== undefined;

    if (Array.isArray(requestName)) {
        return requestName.some(singleNamedRequestInProgress);
    }

    return singleNamedRequestInProgress(requestName);
});

export const namedAllRequestsError = createSelector([requestState], ({ requests }) =>
    requests?.filter((request) => request.error !== null)
);

export const namedRequestsError = createSelector([requestState], ({ requests }) =>
    requests?.filter((request) => request.error !== null && !request.ownErrorControl)
);

export const namedOwnRequestsError = createSelector([requestState], ({ requests }) =>
    requests?.filter((request) => request.error !== null && request.ownErrorControl)
);

export const namedSpecificRequestError = createSelector(
    [requestState],
    ({ requests }, requestName) =>
        requests.find((request) => request.name === requestName && request.error !== null)?.error
);
