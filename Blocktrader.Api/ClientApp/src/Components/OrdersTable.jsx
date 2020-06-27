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

const width = 256 + 64;
const height = 512;

function OrderRow(order) {
    return (
        <div style={{width: "auto", height: 24, borderBottom: "1px double lightgray"}}>
            <span style={{float: "left", marginLeft: 24}}>{order.price}</span>
            <span style={{float: "right", marginRight: 24}}>{order.amount}</span>
        </div>
    )
}

export class OrdersTable extends React.Component {
    constructor(props){
        super(props);
        this.scrollRef = React.createRef()
    }

    componentDidMount() {
        if (this.props.alignBottom)
            this.scrollRef.current.scrollTop = this.scrollRef.current.scrollHeight //.scrollIntoView({ behavior: 'smooth' })
    }

    renderRows() {
        const style = this.props.alignBottom
            ? {position: "absolute", width: width} //bottom: 0, backgroundColor: "#42f5aa"
            : {position: "absolute", width: width}; //  backgroundColor: "#f54242"

        const spacerHeight = height - this.props.orders.length * 24

        return (
            <div ref={this.scrollRef} className={"OrdersContent"} style={{height: height, minHeight: height, maxHeight: height, overflowY: "scroll", overflowX: "hidden", position: "relative", width: width}}>
                <div>
                    {this.props.alignBottom && spacerHeight > 0 && <div style={{width: width, height: spacerHeight - 1}}> </div>}
                    <div style={style}>
                        {this.props.orders.map(o => OrderRow(o))}
                    </div>
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

        return (
            <Paper style={{width: width}}>
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
