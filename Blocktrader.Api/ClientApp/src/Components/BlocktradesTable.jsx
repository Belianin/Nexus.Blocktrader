import React from "react";
import TableContainer from "@material-ui/core/TableContainer";
import Paper from "@material-ui/core/Paper/Paper";
import {Table} from "@material-ui/core";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TableCell from "@material-ui/core/TableCell/TableCell";
import TableBody from "@material-ui/core/TableBody";
import PropTypes, {object} from "prop-types";
import CircularProgress from "@material-ui/core/CircularProgress";
import {addHours} from "../Models/Timestamp";
import Container from "@material-ui/core/Container";

export class BlocktradesTable extends React.Component {
    constructor(props){
        super(props);
    }

    getContent() {
        if (this.props.isLoading)
            return <CircularProgress />

        const filtered = this.props.exchanges.filter(e => e.trades)
            .flatMap(e => e.trades.map(t => {return {
                exchange: e.title,
                amount: t.amount,
                price: t.price,
                isSale: t.isSale,
                time: t.time}}))
            .sort((a, b) => a.time - b.time);

        if (filtered.length === 0)
            return <p>Крупных сделок не было</p>

        return (
            <TableContainer component={Paper} style={{maxHeight: 800}}>
                <Table size="small">
                    <TableHead>
                        <TableRow>
                            <TableCell align="left" size="small">Цена</TableCell>
                            <TableCell align="right" size="small" >Количество</TableCell>
                            <TableCell align="right" size="small" >Время</TableCell>
                            <TableCell align="right" size="small" >Биржа</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {filtered.map(t => (
                            <TableRow >
                                <TableCell align="left" size="small" >{t.price.toFixed(0)}</TableCell>
                                <TableCell align="right" size="small" style={{color: t.isSale ? '#DC143C' : '#228B22'}}><b title={t.amount}>{t.amount.toFixed(0)}</b></TableCell>
                                <TableCell align="right" size="small" >{new Date(t.time).toLocaleTimeString('ru-RU')}</TableCell>
                                <TableCell align="right" size="small" >{t.exchange.toUpperCase()}</TableCell>
                            </TableRow>))}
                    </TableBody>
                </Table>
            </TableContainer>
        );
    }

    render() {
        return <Container style={{textAlign: 'center', width: 128 * 4, display: 'inline-block'}}>
            <h1>BLOCKTRADES</h1>
            {this.getContent()}
        </Container>
    }
}

BlocktradesTable.propTypes = {
    exchanges: PropTypes.arrayOf(object),
    isLoading: PropTypes.bool
};
