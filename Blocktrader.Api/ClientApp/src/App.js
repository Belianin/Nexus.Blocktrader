import React from 'react';
import logo from './logo.png';
import './App.css';
import {Button} from '@material-ui/core'
import Slider from '@material-ui/core/Slider';
import Grid from '@material-ui/core/Grid';
import {Timestamp, TickerInfo, OrderBook} from './Models/Timestamp'
import {TimestampsTable} from "./Components/TimestampsTable";

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      value: [],
      selectedTimestamp: 0
    };
  }

  componentDidMount() {
    fetch("http://localhost:777/api/v1/timestamps/exchange/Binance/ticker/BtcUsd/year/2020/month/3/day/6?precision=2")
        .then(res => res.arrayBuffer())
        .then(
            (result) => {
              this.setState({
                value: this.convertToTimestamp(result)
              });
            },
            (error) => {
              this.setState({
                value: []
              });
            }
        )
  }
  convertToTimestamp(buffer) {
    const result = [];

    let index = 0;
    const dataView = new DataView(buffer);

    while (index < buffer.byteLength) {
      const date = new Date(Number(dataView.getBigInt64(index, true)));
      index += 8;
      const averagePrice = dataView.getFloat32(index, true);
      index += 4;

      const bidsCount = dataView.getInt32(index, true);
      index += 4;
      const bids = [];
      for (let i = 0; i < bidsCount; i++) {
        bids.push(this.getOrderFromBytes(dataView, index));
        index += 8;
      }

      const asksCount = dataView.getInt32(index, true);
      index += 4;
      const asks = [];
      for (let i = 0; i < asksCount; i++) {
        asks.push(this.getOrderFromBytes(dataView, index));
        index += 8;
      }

      result.push(new Timestamp(date, new TickerInfo(new OrderBook(bids, asks), averagePrice)));
    }

    return result;
  }

  getOrderFromBytes(dataView, index) {
    return {
      price: dataView.getFloat32(index, true),
      amount: dataView.getFloat32(index + 4, true)
    }
  }

  fetchData() {
    return fetch("http://localhost:777/api/v1/timestamps/me")//("http://localhost:777/api/v1/timestamps/exchange/Binance/ticker/BtcUsd/year/2020/month/3/day/5?precision=2")
        .then(r => r.json())
        .then(r => r)
    //  .then(response => response.arrayBuffer())
        //.then(buffer => buffer.byteLength)
  }

  onSliderChange(value) {
    this.setState({
      selectedTimestamp: value
    });
    console.log("Тыкнули слайдер " + value)
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
                  max={this.state.value.length}
                  valueLabelDisplay="auto"
                  onChange={(e, v) => this.onSliderChange(v)}
                  aria-labelledby="discrete-slider-small-steps" />
            </Grid>
            {this.state.value.length > 0 && <TimestampsTable timestamps={[this.state.value[this.state.selectedTimestamp]]}/>}
          </div>
        </div>
    );
  }
}

export default App;
