import { combineReducers } from "redux";

import requestReducer from "./request/request.reducer";
import popupReducer from "./popup/popup.reducer";
import userReducer from "./user/user.reducer";

export default combineReducers({
    request: requestReducer,
    popup: popupReducer,
    user: userReducer,
});
