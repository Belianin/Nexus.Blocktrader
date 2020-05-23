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

const classes = {
    minWidth: 650
};

interface OrdersTableProps {
    orders: Order[]
}

export class OrdersTable extends React.Component<OrdersTableProps> {
    render() {
        return (
            <TableContainer component={Paper}>
                <Table style={classes} size="small">
                    <TableHead>
                        <TableRow>
                            <TableCell align="left" size="small">Цена</TableCell>
                            <TableCell align="right" size="small" >Колличество</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
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
