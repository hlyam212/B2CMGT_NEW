import React, { Component } from 'react';
import pic from "../../img/mushroom.gif"
import './Home.css';

export class Home extends Component {
    constructor(props) {
        super(props);
        this.state = {}
    }

    render() {
        return (
            <div id="homeArea">
                <h1>Welcome</h1>
                <h1>To</h1>
                <h1>EVABMS</h1>
                <img src={pic} />
            </div>
        );
    }
}
