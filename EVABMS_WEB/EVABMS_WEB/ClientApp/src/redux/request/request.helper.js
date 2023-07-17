import axios from "axios";

import store from "../store";

import { setToken, requestStarted, requestFinished, requestFailed } from "./request.action";

const defaultConfig = {
    baseURL: `${process.env.REACT_APP_API_URL}`,
    headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
        "Access-Control-Allow-Origin": process.env.REACT_APP_API_URL,
        "Access-Control-Allow-Methods": "POST",
    },
    withCredentials: true,
    credentials: "same-origin",
};

const axiosProvider = axios.create(defaultConfig);

axiosProvider.interceptors.request.use(
    async (config) => {
        let _token = store.getState()?.request?.token
        if (_token !== null && _token !== undefined && _token !== "") {
            config.headers = {
                ...config.headers,
                Authorization: `Bearer ${_token}`,
            };
        }
        return config;
    },
    async (error) => Promise.reject(error)
);

axiosProvider.interceptors.response.use(
    (response) => {
        let resObjs = response.data;
        if (resObjs.Maintenance !== undefined) {
            //RedirectMaintenance();
            return;
        }
        if (resObjs.ErrMsg !== undefined) {
            throw new Error(resObjs.ErrMsg);
        }

        return resObjs;
    },
    async (error) => Promise.reject(error)
);

const responseErrorHandling = (error) => {
    //const userStore = useUserStore();
    //const modalStore = useModalStore();

    //const { status } = error.response;
    //const { message } = error.response?.data;

    //if (status === 400 || status === 404 || status === 409) {
    //    return `${message || "Data Error."}`;
    //}

    //if (status === 401) {
    //    userStore.LOGOUT(false);

    //    return `${message || "Your session has expired. Please login again."}`;
    //}

    //if (error.response?.status === 408 && !error.response?.data?.message?.accessToken) {
    //    modalStore.SET_REQUEST_TIMEOUT_ALERT(
    //        error.response?.data?.message || error.message || "Session Timeout! Please login again."
    //    );
    //    return;
    //} else if (error.name === "timeout" || (error.response?.status && _error.response?.status !== 200)) {
    //    modalStore.SET_REQUEST_TIMEOUT_ALERT(error.response?.data?.message || error.message);
    //    return;
    //}

    //if (status === 500) {
    //    return "An unexpected error occurred. If this issue continues, please contact us.";
    //}
};

export const requestHelper = {
    get: async (dispatch, requestTarget, requestObj, ownErrorControl = false) => {
        //const requestStore = useRequestStore();
        dispatch(requestStarted(requestTarget));

        try {
            // Start requesting
            const result = await axiosProvider.get(requestTarget, { params: requestObj });

            dispatch(requestFinished(requestTarget))
            return result;
        } catch (error) {
            dispatch(
                requestFailed({
                    requestName: requestTarget,
                    error: error.response?.data?.message || error.message,
                    ownErrorControl,
                })
            );

            let errorMsg =
                responseErrorHandling(error, ownErrorControl) || error.response?.data?.message || error.message;

            if (error.response?.status === 408 && error.response?.data?.message?.accessToken) {
                dispatch(setToken(error.response?.data?.message?.accessToken));
                dispatch(requestStarted(requestTarget));

                try {
                    const retryResult = await axiosProvider.get(requestTarget, { params: requestObj });

                    dispatch(requestFinished(requestTarget));
                    return retryResult;
                } catch (_error) {
                    dispatch(
                        requestFailed({
                            requestName: requestTarget,
                            error: _error.response?.data?.message || _error.message,
                            ownErrorControl,
                        })
                    );

                    errorMsg =
                        responseErrorHandling(_error, ownErrorControl) ||
                        _error.response?.data?.message ||
                        _error.message;
                }
            }
            return Promise.reject(errorMsg)
            //if (modalStore.needBackLogin) {
            //    return;
            //}

            //return ownErrorControl ? Promise.reject(errorMsg) : modalStore.SET_REQUEST_ERROR_ALERT(errorMsg);
        }
    },
    post: async (dispatch, requestTarget, requestObj, ownErrorControl = false) => {
        //const modalStore = useModalStore();
        //const requestStore = useRequestStore();
        dispatch(requestStarted(requestTarget));

        try {
            // Start requesting
            const result = await axiosProvider.post(requestTarget, requestObj);
            dispatch(requestFinished(requestTarget))
            return result;
        } catch (error) {
            dispatch(
                requestFailed({
                    requestName: requestTarget,
                    error: error.response?.data?.message || error.message,
                    ownErrorControl,
                })
            );

            let errorMsg =
                responseErrorHandling(error, ownErrorControl) || error.response?.data?.message || error.message;

            if (error.response?.status === 408 && error.response?.data?.message?.accessToken) {
                dispatch(setToken(error.response?.data?.message?.accessToken));
                dispatch(requestStarted(requestTarget));

                try {
                    const retryResult = await axiosProvider.post(requestTarget, requestObj);

                    dispatch(requestFinished(requestTarget));
                    return retryResult;
                }
                catch (_error) {
                    dispatch(
                        requestFailed({
                            requestName: requestTarget,
                            error: _error.response?.data?.message || _error.message,
                            ownErrorControl,
                        })
                    );

                    errorMsg =
                        responseErrorHandling(_error, ownErrorControl) ||
                        _error.response?.data?.message ||
                        _error.message;
                }
            }
            return Promise.reject(errorMsg)
            //if (modalStore.needBackLogin) {
            //    return;
            //}

            //return ownErrorControl ? Promise.reject(errorMsg) : modalStore.SET_REQUEST_ERROR_ALERT(errorMsg);
        }
    },
};
