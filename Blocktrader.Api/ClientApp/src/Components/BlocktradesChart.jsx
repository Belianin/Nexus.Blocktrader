import React from "react";
import Paper from "@material-ui/core/Paper";

const width = 400;
const height = 160;

export class BlocktradesChart extends React.Component {
    constructor(props) {
        super(props);
        this.maxBar = (width / 2) - 16;
        this.state = {
            months : {}
        }
    }

    getMonthKey(date) {
        return date.getFullYear().toString() + ":" + (date.getMonth() + 1).toString();
    }

    renderBar(value, max, isBid) {
        const style = {
            backgroundColor: isBid ? "rgba(35, 203, 167, 0.5)" : "rgba(240, 52, 52, 0.5)",
            height: 32,
            width: this.maxBar * (value / max),
            display: "inline-block"
        };

        const margin = this.maxBar - (this.maxBar * (value / max));
        if (isBid)
            style.marginRight = margin;
        else
            style.marginLeft = margin;

        return <div style={style}><b>{value.toFixed(0)}</b></div>
    }

    render() {
        const monthKey = this.getMonthKey(this.props.selectedDate);
        if (!this.state.months[monthKey]) {
            this.props.monthCallback("BtcUsd", this.props.selectedDate.getFullYear(), this.props.selectedDate.getMonth() + 1).then(s => this.setState(state => {
                state.months[monthKey] = s;

                console.log("Month callback")
                console.log(s);

                return state;
            }))
        }

        const monthAsksCount = this.state.months[monthKey] === undefined
            ? 0
            : this.state.months[monthKey].asksAmount;

        const monthBidsCount = this.state.months[monthKey] === undefined
            ? 0
            : this.state.months[monthKey].bidsAmount;
        let monthMax = Math.max(monthAsksCount, monthBidsCount);
        monthMax = monthMax === 0 ? 1 : monthMax;

        const asksCount = this.props.asks.reduce((x, y) => x + y, 0);
        const bidsCount = this.props.bids.reduce((x, y) => x + y, 0);

        let max = Math.max(asksCount, bidsCount);
        max = max === 0 ? 1 : max;
        console.log("Max trade amount: " + max);

        return (
            <Paper style={{width: width, height: height, textAlign: "center", marginLeft: 32}}>
                <div>
                    <p>День</p>
                    {this.renderBar(asksCount, max, false)}
                    {this.renderBar(bidsCount, max, true)}
                    <div style={{height: 4}}></div>
                    {this.renderBar(monthAsksCount, monthMax, false)}
                    {this.renderBar(monthBidsCount, monthMax, true)}
                    <p>Месяц</p>
                </div>
            </Paper>
        )
    }
}
