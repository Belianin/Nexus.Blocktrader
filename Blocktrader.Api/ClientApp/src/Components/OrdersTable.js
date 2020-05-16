import React from "react";
import PropTypes from 'prop-types';
import TableContainer from "@material-ui/core/TableContainer";
import {Table} from "@material-ui/core";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TableCell from "@material-ui/core/TableCell/TableCell";
import TableBody from "@material-ui/core/TableBody";
import {Order} from '../Models/Timestamp'

export class OrdersTable extends React.Component {
    constructor(props){
        super(props);
    }

    render() {
        return (
            <div>
                <TableContainer>
                    <Table size="small">
                        <TableHead>
                            <TableRow>
                                <TableCell align="left">Цена</TableCell>
                                <TableCell align="right">Колличество</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {this.props.orders.map(o => (
                                <TableRow>
                                    <TableCell align="left">{o.price}</TableCell>
                                    <TableCell align="right">{o.amount}</TableCell>
                                </TableRow>))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </div>
        )
    }
}

OrdersTable.propTypes = {
    orders: PropTypes.arrayOf(Order)
};
