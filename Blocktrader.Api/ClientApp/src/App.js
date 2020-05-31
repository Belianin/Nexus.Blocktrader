import React from 'react';
import logo from './logo.png';
import './App.css';
import {Button} from '@material-ui/core'
import Slider from '@material-ui/core/Slider';
import Grid from '@material-ui/core/Grid';
import {Timestamp, TickerInfo, OrderBook, timestampFromBytes} from './Models/Timestamp'
import {TimestampsTable} from "./Components/TimestampsTable";
import DatePicker from "./Components/DatePicker";
import Container from "@material-ui/core/Container";

const exchanges = ["Binance", "Bitfinex", "Bitstamp"];
const backendUrl = "/api/v1/";
const ticker = "BtcUsd";

class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      years: {},
      selectedTimestamp: 0,
      selectedDate: new Date()
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
            this.setState((state) => {
                let years = state.years;

                if (!years[year])
                  years[year] = this.createYear();

                const selectedMonth = years[year][month];
                if (!selectedMonth[day])
                  selectedMonth[day] = {};
                selectedMonth[day][exchange.toLowerCase()] = timestampFromBytes(result);

                return {years: years};
              })
            })
        .catch(error => console.log(error))
  }

  loadDay() {
    for (let exchange of exchanges) {
      const year = this.state.selectedDate.getFullYear();
      const month = this.state.selectedDate.getMonth() + 1;
      const day = this.state.selectedDate.getDate();

      this.getTimestamps(exchange, "BtcUsd", year, month, day);
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

  createYear() {
    return {
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
    };
  }

  getCurrentTimestamp(exchange) {
    if (!this.state.selectedDate || !this.state.years[this.state.selectedDate.getFullYear()])
      return [];

    const day = this.state.years[this.state.selectedDate.getFullYear()][this.state.selectedDate.getMonth() + 1][this.state.selectedDate.getDate()];
    if (!day)
      return [];

    const timestamps = day[exchange];
    if (!timestamps)
      return [];

    return timestamps;
  }

  onDateChanged(newDate) {
    console.log(newDate);
    this.setState({
      selectedDate: newDate
    }, this.loadDay);
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
                  max={this.getCurrentTimestamp("binance").length}
                  valueLabelDisplay="auto"
                  onChange={(e, v) => this.onSliderChange(v)}
                  aria-labelledby="discrete-slider-small-steps" />
              <Container jusity={"center"}>
                <DatePicker onChange={(e) => this.onDateChanged(e)} defaultValue={this.state.selectedDate}/>
              </Container>
            </Grid>
            <TimestampsTable
                pointer={this.state.selectedTimestamp}
                bitfinex={this.getCurrentTimestamp("bitfinex")}
                bitstamp={this.getCurrentTimestamp("bitstamp")}
                binance={this.getCurrentTimestamp("binance")}
            />
          </div>
        </div>
    );
  }
}

export default App;
