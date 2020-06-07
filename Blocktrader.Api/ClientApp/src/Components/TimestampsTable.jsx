import React from "react";
import PropTypes from 'prop-types';
import {Timestamp} from "../Models/Timestamp";
import Paper from '@material-ui/core/Paper';
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";
import {OrdersTable} from "./OrdersTable";
import Container from "@material-ui/core/Container";
import CircularProgress from '@material-ui/core/CircularProgress';
import VirtualizedOrdersTable from "./VirtualizedOrdersTable";

const exchanges = ["binance", "bitfinex", "bitstamp"];

export class TimestampsTable extends React.Component {

    renderAsks(exchange) {
        if (exchange === undefined)
            return <Paper style={{ height: 400, width: 400 }}><CircularProgress /></Paper>;
        return (
            <div>
                <Grid key={exchange.title + 1} item>
                    <h1>{exchange.title}</h1>
                </Grid>
                {this.renderOrders(exchange.orders, "asks")}
            </div>
        )
    }

    renderOrders(exchange, ordersType) {
        if (exchange === undefined)
            return <Paper style={{ height: 400, width: 400 }}><CircularProgress /></Paper>;

        const data = exchange.orders;
        if (!data)
            return <Paper style={{ height: 400, width: 400 }}><CircularProgress /><h4>Нет биржи</h4></Paper>;

        const day = data[this.props.pointer];
        if (!day)
            return <Paper style={{ height: 400, width: 400 }}><CircularProgress /><h4>Нет дня</h4></Paper>;

        const tickerInfo = day.tickerInfo;
        if (!tickerInfo)
            return <Paper style={{ height: 400, width: 400 }}><CircularProgress /><h4>Нет тикер инфо</h4></Paper>;

        const orderBook = tickerInfo.orderBook;
        if (!orderBook)
            return <Paper style={{ height: 400, width: 400 }}><CircularProgress /><h4>Нет ордер бука</h4></Paper>;

        return (
            <Grid key={exchange.title} item>
                <VirtualizedOrdersTable orders={orderBook[ordersType]}/>
            </Grid>
        )
    }

    render() {
        let date = "Нет времени на раскачку";// this.props.bitfinex[this.props.pointer].date;
        if (this.props.binance && this.props.binance[this.props.pointer]) {
            const dateTime = this.props.binance[this.props.pointer].date;
            date = dateTime.toLocaleString('ru-RU')// `${dateTime.getFullYear()}/${dateTime.getMonth() + 1}/${dateTime.getDate()}`
        }

        return (
            <Container jusity={"center"} style={{textAlign: 'center'}}>
                <Typography variant="h2" component="div">
                    {date}
                </Typography>
                    <Typography variant="h4" id="tableTitle" component="div">
                        ASKS
                    </Typography>
                    <Grid item xs={12}>
                        <Grid container spacing={12}>
                            {this.props.exchanges.map(e => this.renderAsks(e))}
                        </Grid>
                    </Grid>
                    <Grid item xs={12}>
                        <Grid container spacing={12}>
                            {this.props.exchanges.map(e => this.renderOrders(e, "bids"))}
                        </Grid>
                    </Grid>
                <Typography variant="h4" id="tableTitle" component="div">
                    BIDS
                </Typography>
            </Container>
        )
    }
}

TimestampsTable.propTypes = {
    pointer: PropTypes.number,
    exchanges: PropTypes.arrayOf(PropTypes.object)
};
