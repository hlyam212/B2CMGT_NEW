import { createSelector } from "reselect";

//get request part of the state
const popupState = (state) => state.popup;

export const selectPopupInfo = createSelector([popupState], (popup) => {
    const { icon, title, description, children } = popup.body;
    return {
        ...popup,
        body: {
            ...popup.body,
            icon: children?.icon || icon,
            title: children?.title || title,
            description: children?.description || description,
            children: children?.title ? null : children,
        },
    };
});
