import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { connect } from "react-redux";
import { createStructuredSelector } from "reselect";

import { setCurrentUser, clearCurrentUser } from "../../redux/user/user.action";
import { requestHelper } from "../../redux/request/request.helper";
import { setToken } from "../../redux/request/request.action";

import './Login.css';

const Login = ({ Login, LoginSuccess, LoginFailure, AAMSLinkGet }) => {
    const [aamsLink, setaamsLink] = useState("");
    const [errorMSG, setErrorMSG] = useState(undefined);
    const navigate = useNavigate()

    useEffect(() => {
        AAMSLinkGet().then((result) => {
            setaamsLink(result)
        }).catch((err) => {
        })
    }, [])

    const loginkeypress = (e) => {
        if ((e.key === 'Enter') == false) {
            return;
        }
        LoginQuery();
    }

    const [userInfo, setUserInfo] = useState({
        userCompany: "EVAAIR",
        userId: "",
        userPwd: "",
    });

    const changeFormInput = (event) => {
        event.preventDefault();
        const { value, name } = event.target;
        setUserInfo({ ...userInfo, [name]: value });
    };

    const LoginQuery = () => {
        Login({
            UserAccount: userInfo.userId,
            Password: userInfo.userPwd,
        }).then((result) => {
            if (result.Succ == false) {
                setErrorMSG(result.Message);
                return;
            }
            LoginSuccess(result);
            navigate('/Home');
        }).catch((error) => {
            setErrorMSG(error);
            LoginFailure()
        });
    }

    return (<div>
        <section className="vh-100 gradient-custom">
            <div className="container py-5 h-100">
                <div className="row d-flex justify-content-center align-items-center h-100">
                    <div className="col-12 col-md-8 col-lg-6 col-xl-5">
                        <div className="text-center">
                            <h1>EC BACKEND MANAGEMENT SYSTEM</h1>
                        </div>
                        <div className="card bg-dark text-white" style={{ borderRadius: "1rem" }}>
                            <div className="card-body p-5 text-center">
                                <div className="mb-md-5 mt-md-4 pb-5">
                                    <h2 className="fw-bold mb-2 text-uppercase">Login</h2>
                                    <p className="text-white-50 mb-5">Please enter your AD and password</p>
                                    <div className="form-outline form-white mb-4">
                                        <input type="text"
                                            className="form-control form-control-lg"
                                            placeholder="AD"
                                            name="userId"
                                            value={userInfo.userId}
                                            onKeyDown={loginkeypress}
                                            onChange={changeFormInput} />
                                    </div>
                                    <div className="form-outline form-white mb-4">
                                        <input type="password"
                                            id="typePasswordX"
                                            className="form-control form-control-lg"
                                            placeholder="Password"
                                            name="userPwd"
                                            value={userInfo.userPwd}
                                            onKeyDown={loginkeypress}
                                            onChange={changeFormInput} />
                                    </div>
                                    <button className="btn btn-outline-light btn-lg px-5" onClick={LoginQuery} >Login</button>
                                    {errorMSG == undefined ? "" : <p className="mt-3 errorText">{errorMSG}</p>}
                                    <p className="text-white-50 mt-5">If you're a new user, please apply for authorization from <a href={aamsLink} target="_blank" >here</a></p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
    );
}

const mapStateToProps = createStructuredSelector({

});

const mapDispatchToProps = (dispatch) => ({
    AAMSLinkGet: () => requestHelper.get(dispatch, 'Login/AAMSLinkGet'),
    Login: (inpudata) => requestHelper.post(dispatch, 'Login/Query', inpudata),
    LoginSuccess: (result) => {
        dispatch(setToken(result.Data.auth.AccessToken))
        dispatch(setCurrentUser({
            currentUser: result.Data.auth.UserId,
            userRoles: result.Data.userRoles
        }))
    },
    LoginFailure: () => dispatch(clearCurrentUser()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Login);