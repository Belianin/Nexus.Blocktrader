import React, {ChangeEvent} from 'react';
import logo from "./logo.png";
import './App.css';
import {Button} from '@material-ui/core'
import Slider from '@material-ui/core/Slider';
import Grid from '@material-ui/core/Grid';
import {Timestamp, TickerInfo, OrderBook, timestampFromBytes} from './Models/Timestamp'
import {TimestampsTable} from "./Components/TimestampsTable";
import DatePicker from "./Components/DatePicker";
import Container from "@material-ui/core/Container";
import {number} from "prop-types";

const exchanges = ["Binance", "Bitfinex", "Bitstamp"];
const backendUrl = "/api/v1/";
const ticker = "BtcUsd";

interface AppProps {

}

interface AppState {
  years: Years,
  selectedTimestamp: number,
  selectedDate: Date
}

interface Years {
   [year: number] : {
     [month: number] : {
       [day: number] : {
         [exchange: string]: Timestamp[]
       }
     }
   };
}

class App extends React.Component<AppProps, AppState> {
  constructor(props: AppProps) {
    super(props);

    this.state = {
      years: {},
      selectedTimestamp: 0,
      selectedDate: new Date()
    };
  }

  getTimestamps(exchange: string, ticker: string, year: number, month: number, day: number) {
    fetch(`${backendUrl}timestamps/exchange/${exchange}/ticker/${ticker}/year/${year}/month/${month}/day/${day}?precision=2`)
        .then(response => {
          if (response.ok) {
            return response;
          }

          throw Error(response.status.toString());})
        .then(response => response.arrayBuffer())
        .then(result => {
            let years = this.state.years;

            if (!years[year])
              years[year] = this.createYear();

            const selectedMonth = years[year][month];
            if (!selectedMonth[day])
              selectedMonth[day] = {};
          selectedMonth[day][exchange.toLowerCase()] = timestampFromBytes(result);

            this.setState({
              years: years
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

  onSliderChange(value: number | number[]) {
    this.setState({
      selectedTimestamp: (typeof value === "number") ? value : value[0]
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

  getCurrentTimestamp(exchange: string) {
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

  onDateChanged(newDate: any) {
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
            <Button variant="contained">&gt;&gt;&gt;</Button>
            <Grid item xs style={{width: 300}}>
              <Slider
                  step={1}
                  min={0}
                  marks
                  max={this.getCurrentTimestamp("binance").length}
                  valueLabelDisplay="auto"
                  onChange={(e: object, v: number | number[]) => this.onSliderChange(v)}
                  aria-labelledby="discrete-slider-small-steps"
              />
              <DatePicker onChange={(e: any) => this.onDateChanged(e)} defaultValue={"2020-05-17"}/>
            </Grid>
            <TimestampsTable
                pointer={this.state.selectedTimestamp}
                exchanges={{
                  "bitfinex": this.getCurrentTimestamp("bitfinex"),
                  "bitstamp": this.getCurrentTimestamp("bitstamp"),
                  "binance": this.getCurrentTimestamp("binance")
                }}
            />
          </div>
        </div>
    );
  }
}

export default App;
