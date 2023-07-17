import React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Outlet } from 'react-router-dom';
import { createStructuredSelector } from "reselect";
import { connect } from "react-redux";

import { requestsInProgress } from "../redux/request/request.selectors";

import Loading from '../base-components/Loading/Loading';
import ErrorBoundary from "../base-components/error-boundary/error-boundary.component";

import './MainPageLayout.css';

const MainPageLayout = ({ loading }) => {
    return (
        <div id="outer-container">
            <NavMenu />
            <main id="page-wrap">
                <ErrorBoundary>
                    <Container>
                        <Outlet />
                    </Container>
                </ErrorBoundary>
            </main>
            {loading && <Loading />}
        </div>
    );
}

const mapStateToProps = createStructuredSelector({
    loading: requestsInProgress
});

export default connect(mapStateToProps, null)(MainPageLayout);
