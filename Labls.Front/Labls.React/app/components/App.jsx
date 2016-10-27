"use strict";
import React from "react";

const Line = (props) =>
    <tr>
        <th scope="row">{props.data.n}</th>
        <td>{props.data.name}</td>
        <td>{props.data.price}</td>
        <td>{props.data.country}</td>
        <td>{props.data.pcs}</td>
    </tr>;
const LineList = (props) =>
    <table className="table">
        <thead>
        <tr>
            <th>#</th>
            <th>Товар</th>
            <th>Цена</th>
            <th>Производитель</th>
            <th>Количество</th>
        </tr>
        </thead>
        <tbody>
        {props.lines.map(l => <Line data={l}/>)}
        </tbody>
    </table>;

class App extends React.Component {
    constructor() {
        super();
        this.state = { lines: [{ n: 1, name: "Свитер женский", price: 7900, country: "Италия", pcs: 7 }] };
    }

    render() {
        return <div className="container" id="content">
                   <LineList lines={this.state.lines}/>
               </div>;
    }
}

export default App