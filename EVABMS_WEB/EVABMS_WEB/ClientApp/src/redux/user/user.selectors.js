import { createSelector } from "reselect";

const userSelector = state => state.user;

export const selectCurrentUser = createSelector(
    [userSelector], 
    user => user.currentUser
);

export const selectCurrentUserRole = createSelector(
    [userSelector],
    user => user.userRoles
);

export const selectToken = createSelector(
    [userSelector],
    user => user.token
);