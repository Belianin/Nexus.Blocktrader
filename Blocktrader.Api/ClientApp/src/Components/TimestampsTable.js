import React from "react";
import PropTypes from 'prop-types';
import {Timestamp} from "../Models/Timestamp";
import Paper from '@material-ui/core/Paper';
import Typography from "@material-ui/core/Typography";
import Grid from "@material-ui/core/Grid";
import {OrdersTable} from "./OrdersTable";

export class TimestampsTable extends React.Component {
    render() {
        const date = this.props.timestamps[0].date;

        return (
            <div>
                <Paper>
                    <Typography variant="h6" id="tableTitle" component="div">
                        {date.getDay()}/{date.getMonth() + 1}/{date.getFullYear()}
                    </Typography>
                    <Typography variant="h6" id="tableTitle" component="div">
                        Аски
                    </Typography>
                    <Grid item xs>
                        <Grid container>
                            {[0, 1, 2].map((value) => (
                                <Grid key={value} item>
                                    <OrdersTable orders={this.props.timestamps[0].tickerInfo.orderBook.asks}/>
                                </Grid>
                            ))}
                        </Grid>
                    </Grid>
                    <Typography variant="h6" id="tableTitle" component="div">
                        Биды
                    </Typography>
                    <Grid item xs>
                        <Grid container>
                            {[0, 1, 2].map((value) => (
                                <Grid key={value} item>
                                    <OrdersTable orders={this.props.timestamps[0].tickerInfo.orderBook.bids}/>
                                </Grid>
                            ))}
                        </Grid>
                    </Grid>
                </Paper>
            </div>
        )
    }
}

TimestampsTable.propTypes = {
    timestamps: PropTypes.arrayOf(Timestamp)
};
