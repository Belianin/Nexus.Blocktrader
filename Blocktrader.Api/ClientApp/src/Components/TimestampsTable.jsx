import React from "react";
import PropTypes from 'prop-types';
import {Timestamp} from "../Models/Timestamp";
import Paper from '@material-ui/core/Paper';
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";
import {OrdersTable} from "./OrdersTable";
import Container from "@material-ui/core/Container";
import VirtualizedOrdersTable from "./VirtualizedOrdersTable";

const exchanges = ["binance", "bitfinex", "bitstamp"];

export class TimestampsTable extends React.Component {

    renderAsks(exchange) {
        return (
            <div>
                <Grid key={exchange + 1} item>
                    <h1>{exchange.toUpperCase()}</h1>
                </Grid>
                {this.renderOrders(exchange, "asks")}
            </div>
        )
    }

    renderOrders(exchange, ordersType) {
        const data = this.props[exchange];
        console.log(data);
        if (!data)
            return <Paper style={{ height: 400, width: 400 }}><h4>Нет биржи</h4></Paper>;

        const day = data[this.props.pointer];
        if (!day)
            return <Paper style={{ height: 400, width: 400 }}><h4>Нет дня</h4></Paper>;

        const tickerInfo = day.tickerInfo;
        if (!tickerInfo)
            return <Paper style={{ height: 400, width: 400 }}><h4>Нет тикер инфо</h4></Paper>;

        const orderBook = tickerInfo.orderBook;
        if (!orderBook)
            return <Paper style={{ height: 400, width: 400 }}><h4>Нет ордер бука</h4></Paper>;

        return (
            <Grid key={exchange} item>
                <VirtualizedOrdersTable orders={orderBook[ordersType]}/>
            </Grid>
        )
    }

    render() {
        let date = "Нет времени на раскачку";// this.props.bitfinex[this.props.pointer].date;
        if (this.props.binance && this.props.binance[this.props.pointer]) {
            const dateTime = this.props.binance[this.props.pointer].date;
            date = `${dateTime.getFullYear()}/${dateTime.getMonth() + 1}/${dateTime.getDate()}`
        }

        return (
            <Container>
                    <Typography variant="h6" id="tableTitle" component="div">
                        Аски {date}
                    </Typography>
                    <Grid item xs={12}>
                        <Grid container spacing={12}>
                            {exchanges.map(e => this.renderAsks(e))}
                        </Grid>
                    </Grid>
                    <Typography variant="h6" id="tableTitle" component="div">
                        Биды
                    </Typography>
                    <Grid item xs={12}>
                        <Grid container spacing={12}>
                            {exchanges.map(e => this.renderOrders(e, "bids"))}
                        </Grid>
                    </Grid>
            </Container>
        )
    }
}

TimestampsTable.propTypes = {
    pointer: PropTypes.number,
    binance: PropTypes.arrayOf(Timestamp),
    bitfinex: PropTypes.arrayOf(Timestamp),
    bitstamp: PropTypes.arrayOf(Timestamp)
};
