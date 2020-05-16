import React from 'react';
import logo from './logo.png';
import './App.css';
import {Button} from '@material-ui/core'
import Slider from '@material-ui/core/Slider';
import Grid from '@material-ui/core/Grid';
import {Timestamp, TickerInfo, OrderBook, timestampFromBytes} from './Models/Timestamp'
import {TimestampsTable} from "./Components/TimestampsTable";

const exchanges = ["Binance", "Bitfinex", "Bitstamps"];
const backendUrl = "http://localhost:777/api/v1/";
const ticker = "BtcUsd";

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      month: [{}],
      selectedTimestamp: 0
    };
  }

  getTimestamps(exchange, ticker, year, month, day) {
    fetch(`${backendUrl}timestamps/exchange/${exchange}/ticker/${ticker}/year/2020/month/3/day/6?precision=2`)
        .then(response => {
          if (response.ok) {
            return response;
          }

          throw Error(response.status.toString());})
        .then(response => response.arrayBuffer())
        .then(result => {
            month = this.state.month;
            month[0][exchange] = timestampFromBytes(result);

            this.setState({
              month: month
            })
          })
        .catch(error => console.log(error))
  }

  componentDidMount() {
    for (let exchange of exchanges) {
      this.getTimestamps(exchange, "BtcUsd", 2020, 3, 6);
    }
  }

  onSliderChange(value) {
    this.setState({
      selectedTimestamp: value
    });
    console.log("Тыкнули слайдер " + value)
  }

  getTimestampsCount() {
    if (!this.state.month[0].Bitfinex)
      return 0;

    return this.state.month[0].Bitfinex.length;
  }

  render() {
    return (
        <div>
          <header>
            <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" />
            <img src={logo} style={{height: 128, width: "auto"}} alt="logo"/>
          </header>
          <div>
            <Button variant="contained">>>></Button>
            <Grid item xs>
              <Slider
                  step={1}
                  min={0}
                  marks
                  max={this.getTimestampsCount()}
                  valueLabelDisplay="auto"
                  onChange={(e, v) => this.onSliderChange(v)}
                  aria-labelledby="discrete-slider-small-steps" />
            </Grid>
            <TimestampsTable
                pointer={this.state.selectedTimestamp}
                bitfinex={this.state.month[0].Bitfinex}
                bitstamp={this.state.month[0].Bitstamp}
                binance={this.state.month[0].Binance}
            />
          </div>
        </div>
    );
  }
}

export default App;
