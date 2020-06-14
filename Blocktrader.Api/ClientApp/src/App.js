import React from 'react';
import './App.css';
import Slider from '@material-ui/core/Slider';
import Grid from '@material-ui/core/Grid';
import {Timestamp, TickerInfo, OrderBook, Order, timestampFromBytes} from './Models/Timestamp'
import DatePicker from "./Components/DatePicker";
import Container from "@material-ui/core/Container";
import {ExchangeTable} from "./Components/ExchangeTable";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import Typography from "@material-ui/core/Typography";
import Paper from "@material-ui/core/Paper";
import GlobalLoader from "./Components/GlobalLoader";
import Header from "./Components/Header";
import TradingViewWidget from 'react-tradingview-widget'
import {BlocktradesTable} from "./Components/BlocktradesTable";

const exchanges = ["Binance", "Bitfinex", "Bitstamp"];
const backendUrl = "/api/v1/";
const ticker = "BtcUsd";

function addDays(date, days) {
  const copy = new Date(Number(date));
  copy.setDate(date.getDate() + days);
  return copy
}


class App extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      years: {},
      selectedTimestamp: 0,
      selectedDate: new Date(),
      isLoading: false,
      trades: {},
      isLoadingTrades: false
    };
  }

  getBlocktrades(exchange, ticker, year, month, day) {
    return fetch(`${backendUrl}trades/exchange/${exchange}/ticker/${ticker}/year/${year}/month/${month}/day/${day}`)
        .then(response => {
          if (response.ok) {
            return response
          }

          throw Error(response.status.toString())
        })
        .then(r => r.json())
        .then(t => {
          return this.setState((state) => {
            let trades = state.trades;

            if (!trades[year])
              trades[year] = this.createYear();

            const selectedMonth = trades[year][month];
            if (!selectedMonth[day])
              selectedMonth[day] = {};
            selectedMonth[day][exchange.toLowerCase()] = t;

            return {trades: trades};
          }, () => 0)
        })
        .catch(error => {
          console.log(error);

          return error;
        })
  }

  getTimestamps(exchange, ticker, year, month, day) {
    return fetch(`${backendUrl}timestamps/exchange/${exchange}/ticker/${ticker}/year/${year}/month/${month}/day/${day}?precision=2`)
        .then(response => {
          if (response.ok) {
            return response;
          }

          throw Error(response.status.toString());
        })
        .then(response => response.arrayBuffer())
        .then(result => {
          return this.setState((state) => {
            let years = state.years;

            if (!years[year])
              years[year] = this.createYear();

            const selectedMonth = years[year][month];
            if (!selectedMonth[day])
              selectedMonth[day] = {};
            selectedMonth[day][exchange.toLowerCase()] = timestampFromBytes(result);

            return {years: years};
          }, () => 0)
        })
        .catch(error => {
          console.log(error);

          return error;
        })
  }

  loadDay(setSliderMax) {
    const year = this.state.selectedDate.getFullYear();
    const month = this.state.selectedDate.getMonth() + 1;
    const day = this.state.selectedDate.getDate();
    console.log(`Скачиваем данные за ${day}/${month}/${year}`)

    this.setState({isLoading: true}, () => {
      Promise.all(exchanges.map(e => this.getTimestamps(e, "BtcUsd", year, month, day)))
          .then(() => {
        console.log("Обновляем выбранный timestamp")
        this.setState({
          selectedTimestamp: setSliderMax ? this.getSliderLength() - 1 : 0,
          isLoading: false})
      });
    });

    this.setState({isLoadingTrades: true}, () => {
      Promise.all(exchanges.map(e => this.getBlocktrades(e, "BtcUsd", year, month, day)))
          .then(() => {
            this.setState({isLoadingTrades: false})
          })
    })
  }

  componentDidMount() {
    this.loadDay();
  }

  nextDay() {
    if (this.state.selectedTimestamp === this.getSliderLength()) {
      this.setState((state) => {
        return {selectedDate: addDays(state.selectedDate, 1)}
      }, this.loadDay);
    } else if (this.state.selectedTimestamp === -1) {
      this.setState((state) => {
        return {selectedDate: addDays(state.selectedDate, -1)}
      }, () => this.loadDay(true))
    }
  }

  onSliderChange(value) {
    if (this.state.isLoading)
      return;

    this.setState({
      selectedTimestamp: value
    }, this.nextDay);
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

  getTradesFor(exchange) {
    if (!this.state.selectedDate || !this.state.trades[this.state.selectedDate.getFullYear()])
      return undefined;

    const day = this.state.trades[this.state.selectedDate.getFullYear()][this.state.selectedDate.getMonth() + 1][this.state.selectedDate.getDate()];
    if (!day)
      return undefined;

    return day[exchange];
  }

  getTimestampsFor(exchange) {
    if (!this.state.selectedDate || !this.state.years[this.state.selectedDate.getFullYear()])
      return undefined;

    const day = this.state.years[this.state.selectedDate.getFullYear()][this.state.selectedDate.getMonth() + 1][this.state.selectedDate.getDate()];
    if (!day)
      return undefined;

    const timestamps = day[exchange];
    if (!timestamps)
      return undefined;

    return timestamps.map(t =>
        new Timestamp(t.date,
            new TickerInfo(
                new OrderBook(t.tickerInfo.orderBook.bids.map(b =>
                        new Order(b.price, Number(b.amount.toFixed(0)))),
                    t.tickerInfo.orderBook.asks.map(a =>
                        new Order(a.price, Number(a.amount.toFixed(0))))))))
  }

  getCurrentTimestamp(exchange) {
    const timestamps = this.getTimestampsFor(exchange)

    if (!timestamps)
      return undefined;

    return timestamps[this.state.selectedTimestamp]
  }

  onDateChanged(newDate) {
    console.log(newDate);
    this.setState({
      selectedDate: newDate
    }, this.loadDay);
  }

  getSliderLength() {
    const timestamps = this.getTimestampsFor("binance");

    if (!timestamps)
      return 0;

    return timestamps.length;
  }

  renderControlPanel() {
    return (
        <Container style={{width: 320, display: 'inline-block'}}>
          <Grid container direction={"column"}>
              <Grid item style={{width: 'auto'}}>
                <Container>
                  <Typography id="range-slider" gutterBottom>
                  Пресижн
                  </Typography>
                </Container>
                <Slider
                    disabled={this.state.isLoading}
                    style={{width: 256}}
                    step={1}
                    min={-1}
                    marks
                    max={3}
                    value={1}
                    valueLabelDisplay="auto"
                    key={'timestamp-slider'}
                />
              </Grid>
              <Grid item>
                <FormControl>
                  <InputLabel id="demo-simple-select-label">Тикер</InputLabel>
                  <Select
                      labelId="demo-simple-select-label"
                      id="demo-simple-select"
                      value={10}
                      onChange={console.log}
                      disabled={this.state.isLoading}
                  >
                    <MenuItem value={10}>BtcUsd</MenuItem>
                    <MenuItem value={20}>EthBtc</MenuItem>
                    <MenuItem value={30}>BtcXrp</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
            <Grid item>
              <DatePicker onChange={(e) => this.onDateChanged(e)} defaultValue={this.state.selectedDate} disabled={this.state.isLoading}/>
            </Grid>
          </Grid>
        </Container>
    )
  }

  renderBlockTrades() {
    const exchanges = ['binance', 'bitfinex', 'bitstamp']
        .map(e => {return {title: e, trades: this.getTradesFor(e)}})

    return <BlocktradesTable exchanges={exchanges} isLoading={this.state.isLoadingTrades}/>
  }

  renderExchange(exchange) {
    const timestamp = this.getCurrentTimestamp(exchange);

    return <ExchangeTable
        title={exchange}
        loading={timestamp === undefined}
        bids={timestamp === undefined ? [] : timestamp.tickerInfo.orderBook.bids}
        asks={timestamp === undefined ? [] : timestamp.tickerInfo.orderBook.asks}
        width={256}
    />
  }

  getTimestampDate() {
    const timestamp = this.getCurrentTimestamp('binance')
    if (timestamp)
      return timestamp.date.toLocaleString('ru-RU')

    return "Нет времени на раскачку";
  }

  renderTable() {
    return (
        <Container style={{width: 256 * 3, display: 'inline-block'}}>
          <Grid container direction={'column'}>
            <Grid item>
              <Container justify={'center'}>
                <Typography>
                  {this.getTimestampDate()}
                </Typography>
                <Slider
                    step={1}
                    min={-1}
                    marks
                    max={this.getSliderLength()}
                    //disabled={this.state.isLoading}
                    value={this.state.selectedTimestamp}
                    valueLabelDisplay="auto"
                    onChange={(e, v) => this.onSliderChange(v)}
                    aria-labelledby="discrete-slider-small-steps" />
              </Container>
            </Grid>
            <Grid item>
                <Grid container direction={'row'}>
                  <Grid item xs={4}>
                    {this.renderExchange('binance')}
                  </Grid>
                  <Grid item xs={4}>
                    {this.renderExchange('bitfinex')}
                  </Grid>
                  <Grid item xs={4}>
                    {this.renderExchange('bitstamp')}
                  </Grid>
                </Grid>
            </Grid>
          </Grid>
        </Container>
    )
  }

  render() {
    return (
        <>
          <Header/>
          <Grid container>
            <Grid item>
              {this.renderControlPanel()}
            </Grid>
            <Grid item>
              {this.renderTable()}
            </Grid>
            <Grid item>
              {this.renderBlockTrades()}
            </Grid>
            <Grid item>
              <Paper style={{width: 256, height: 800}}>
                <TradingViewWidget
                    symbol={"BTCUSDT"}
                    locale={"ru"}
                    autosize
                />
              </Paper>
            </Grid>
          </Grid>
          {this.state.isLoading && <GlobalLoader/>}
        </>
    );
  }
}

export default App;
