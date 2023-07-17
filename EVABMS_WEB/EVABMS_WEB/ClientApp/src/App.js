import React, { useEffect } from 'react';
import { Route, Routes, useNavigate } from 'react-router-dom';
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";

import { clearCurrentUser } from "./redux/user/user.action";
import { selectCurrentUser } from "./redux/user/user.selectors";

import ErrorBoundary from "./base-components/error-boundary/error-boundary.component";

import AppRoutes from './AppRoutes';
import Login from "./page/Login/Login";
import MainPageLayout from './page/MainPageLayout';
import PageNotFound from './page//PageNotFound/PageNotFound';
import './custom.css';

const App = ({ user }) => {
    const navigate = useNavigate()
    useEffect(() => {
        if (user == null) navigate('/');
    }, [user]);

    return (
        <div>
            <ErrorBoundary>
                <Routes>
                    <Route index element={<Login />} />
                    <Route path="/Login" element={<Login />} />
                    <Route element={<MainPageLayout />}>
                        {AppRoutes.map((route, index) => {
                            const { element, ...rest } = route;
                            return <Route key={index} {...rest} element={element} />;
                        })}
                    </Route>
                    <Route path="*" element={<PageNotFound />} />
                </Routes>
            </ErrorBoundary>
        </div>
    );
}

const mapStateToProps = createStructuredSelector({
    user: selectCurrentUser
});

const mapDispatchToProps = (dispatch) => ({
    Close: () => dispatch(clearCurrentUser())
});
export default connect(mapStateToProps, mapDispatchToProps)(App);
