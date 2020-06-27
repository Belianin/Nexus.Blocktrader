import React from "react";
import PropTypes from 'prop-types';
import { makeStyles } from '@material-ui/core/styles';
import TableContainer from "@material-ui/core/TableContainer";
import {Table} from "@material-ui/core";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TableCell from "@material-ui/core/TableCell/TableCell";
import TableBody from "@material-ui/core/TableBody";
import {Order} from '../Models/Timestamp'
import Paper from '@material-ui/core/Paper'

const classes = makeStyles({
    table: {
        minWidth: 650,
    },
});

const width = 256;
const height = 600;

function OrderRow(order) {
    return (
        <div style={{width: "auto", height: 24}}>
            <span style={{float: "left"}}>{order.price}</span>
            <span style={{float: "right"}}>{order.amount}</span>
        </div>
    )
}

export class OrdersTable extends React.Component {
    constructor(props){
        super(props);
    }

    renderRows() {
        const style = this.props.alignBottom
            ? {bottom: 0, position: "absolute", width: width, backgroundColor: "#42f5aa"}
            : {position: "absolute", width: width, backgroundColor: "#f54242"};

        return (
            <div className={"OrdersContent"} style={{height: height, maxHeight: height, overflow: "scroll", position: "relative", width: width}}>
                <div style={style}>
                    {this.props.orders.map(o => OrderRow(o))}
                </div>
            </div>
        )
    }

    render() {
        const style = this.props.alignBottom ? {
            backgroundColor: "red",
            //position: "absolute",
            height: height,
            width: 300
        } : {
            backgroundColor: "blue",
            height: height,
            width: 300
        };

        //return this.renderRows()

        return (
            <Paper>
                {this.renderRows()}
            </Paper>
        )

        return (
            <TableContainer component={Paper}>
                <Table className={classes.table} size="small">
                    <TableHead>
                        <TableRow>
                            <TableCell align="left" size="small">Цена</TableCell>
                            <TableCell align="right" size="small" >Колличество</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody style={style}>
                        {this.props.orders.map(o => (
                            <TableRow>
                                <TableCell align="left" size="small" >{o.price}</TableCell>
                                <TableCell align="right" size="small" >{o.amount}</TableCell>
                            </TableRow>))}
                    </TableBody>
                </Table>
            </TableContainer>
        )
    }
}

OrdersTable.propTypes = {
    orders: PropTypes.arrayOf(Order),
    alignBottom: PropTypes.bool
};
