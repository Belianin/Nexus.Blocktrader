import React from "react";
import PropTypes from 'prop-types';
import {Timestamp} from "../Models/Timestamp";
import Paper from '@material-ui/core/Paper';
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";
import {OrdersTable} from "./OrdersTable";
import Container from "@material-ui/core/Container";
import VirtualizedOrdersTable from "./VirtualizedOrdersTable";
import {NexusOrdersTable} from "./NexusOrdersTable";

const exchanges = ["binance", "bitfinex", "bitstamp"];

interface TimestampsTableProps {
    pointer: number,
    exchanges: {
        [exchange: string]: Timestamp[]
    }
}

interface TimestampsTableState {

}

export class TimestampsTable extends React.Component<TimestampsTableProps, TimestampsTableState> {

    renderAsks(exchange: string) {
        return (
            <div>
                <Grid key={exchange + 1} item>
                    <h1>{exchange.toUpperCase()}</h1>
                </Grid>
                {this.renderOrders(exchange, false)}
            </div>
        )
    }

    renderOrders(exchange: string, isBids: boolean) {
        console.log(this.props.pointer);
        const data = this.props.exchanges[exchange];
        if (!data)
            return <h4>Нет биржи</h4>;

        console.log("Data: " + this.props.exchanges[exchange]);
        const day = data[this.props.pointer];
        if (!day)
            return <h4>Нет дня</h4>;

        const tickerInfo = day.tickerInfo;
        if (!tickerInfo)
            return <h4>Нет тикер инфо</h4>;

        const orderBook = tickerInfo.orderBook;
        if (!orderBook)
            return <h4>Нет ордер бука</h4>;

        const orders = isBids ? orderBook.bids : orderBook.asks;

        return (
            <Grid key={exchange} item>
                {VirtualizedOrdersTable(orders)}
            </Grid>
        )
    }

    render() {
        let date = "Нет времени на раскачку";// this.props.bitfinex[this.props.pointer].date;
        const binance = this.props.exchanges["binance"];
        if (binance && binance[this.props.pointer]) {
            const dateTime = binance[this.props.pointer].date;
            date = `${dateTime.getFullYear()}/${dateTime.getMonth() + 1}/${dateTime.getDate()}`
        }

        return (
            <Container>
                    <Typography variant="h6" id="tableTitle" component="div">
                        Аски {date}
                    </Typography>
                    <Grid item xs={12}>
                        <Grid container>
                            {exchanges.map(e => this.renderAsks(e))}
                        </Grid>
                    </Grid>
                    <Typography variant="h6" id="tableTitle" component="div">
                        Биды
                    </Typography>
                    <Grid item xs={12}>
                        <Grid container>
                            {exchanges.map(e => this.renderOrders(e, true))}
                        </Grid>
                    </Grid>
            </Container>
        )
    }
}
