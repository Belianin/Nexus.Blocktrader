import React from 'react';
import logo from './logo.png';
import './App.css';
import {Button} from '@material-ui/core'
import Slider from '@material-ui/core/Slider';
import Grid from '@material-ui/core/Grid';
import {Timestamp, TickerInfo, OrderBook, timestampFromBytes} from './Models/Timestamp'
import {TimestampsTable} from "./Components/TimestampsTable";
import DatePicker from "./Components/DatePicker";

const exchanges = ["Binance", "Bitfinex", "Bitstamp"];
const backendUrl = "http://localhost:777/api/v1/";
const ticker = "BtcUsd";

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      years: {
        2020: {
          1: {},
          2: {},
          3: {},
          4: {},
          5: {},
          6: {},
          7: {},
          8: {},
          9: {},
          10: {},
          11: {},
          12: {}
        }
      },
      selectedTimestamp: 0,
      selectedDate: new Date(2020, 2, 5)
    };
  }

  getTimestamps(exchange, ticker, year, month, day) {
    fetch(`${backendUrl}timestamps/exchange/${exchange}/ticker/${ticker}/year/${year}/month/${month}/day/${day}?precision=2`)
        .then(response => {
          if (response.ok) {
            return response;
          }

          throw Error(response.status.toString());})
        .then(response => response.arrayBuffer())
        .then(result => {
            let years = this.state.years;
            years[2020][month][exchange.toLowerCase()] = timestampFromBytes(result);

            this.setState({
              years: years
            })
          })
        .catch(error => console.log(error))
  }

  loadDay() {
    for (let exchange of exchanges) {
      this.getTimestamps(exchange, "BtcUsd", this.state.selectedDate.getFullYear(), this.state.selectedDate.getMonth() + 1, this.state.selectedDate.getDate());
    }
  }

  componentDidMount() {
    this.loadDay()
  }

  onSliderChange(value) {
    this.setState({
      selectedTimestamp: value
    });
    console.log("Тыкнули слайдер " + value)
  }

  getTimestampsCount() {
    if (!this.state.selectedDate)
      return 0;
    const timestamps = this.state.years[2020][this.state.selectedDate.getMonth() + 1].bitfinex;
    if (!timestamps)
      return 0;

    return timestamps.length;
  }

  onDateChanged(newDate) {
    this.setState({
      selectedDate: new Date(newDate.target.value)
    });

    this.loadDay();
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
            <DatePicker onChange={(e) => this.onDateChanged(e)}/>
            <TimestampsTable
                pointer={this.state.selectedTimestamp}
                bitfinex={this.state.years[2020][this.state.selectedDate.getMonth() + 1].bitfinex}
                bitstamp={this.state.years[2020][this.state.selectedDate.getMonth() + 1].bitstamp}
                binance={this.state.years[2020][this.state.selectedDate.getMonth() + 1].binance}
            />
          </div>
        </div>
    );
  }
}

export default App;
