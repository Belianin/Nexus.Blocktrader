import React from "react";
import PropTypes from 'prop-types';
import {Order} from "../Models/Timestamp";
import CircularProgress from "@material-ui/core/CircularProgress/CircularProgress";
import Container from "@material-ui/core/Container";
import {OrdersTable} from "./OrdersTable";

const height = 800;

export class ExchangeTable extends React.Component {
   getContent() {
       if (this.props.loading)
           return (
               <CircularProgress />
           );

       return (
           <div>
               <OrdersTable orders={this.props.asks} alignBottom={true}/>
               <br />
               <OrdersTable orders={this.props.bids} alignBottom={false}/>
           </div>
       )
   }

    render() {
        return (
            <Container jusity={"center"} style={{textAlign: 'center'}}>
                <h1>{this.props.title.toUpperCase()}</h1>
                {this.getContent()}
            </Container>
        );
    }
}

ExchangeTable.propTypes = {
    title: PropTypes.string,
    bids: PropTypes.arrayOf(Order),
    asks: PropTypes.arrayOf(Order),
    loading: PropTypes.bool,
    width: PropTypes.number
};
