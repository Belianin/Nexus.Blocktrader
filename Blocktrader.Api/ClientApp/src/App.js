import React from 'react';
import logo from './logo.png';
import './App.css';

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      value: false
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
                value: error.toString()
              });
            }
        )
  }
  convertToTimestamp(buffer) {
    const result = [];

    let index = 0;
    const dataView = new DataView(buffer);

    while (index < buffer.byteLength) {
      console.log(index);
      const date = new Date(Number(dataView.getBigInt64(index, true)) / 1000);
      index += 8;
      const averagePrice = dataView.getFloat32(index, true);
      index += 4;

      const bidsCount = dataView.getInt32(index, true);
      index += 4;
      const bids = [];
      console.log(dataView.byteLength);
      console.log(bidsCount);
      for (let i = 0; i < bidsCount; i++)
      {
        console.log(index);
        bids.push(this.getOrderFromBytes(dataView, index));
        index += 8;
      }

      const asksCount = dataView.getInt32(index, true);
      index += 4;
      const asks = [];
      for (let i = 0; i < asksCount; i++)
      {
        asks.push(this.getOrderFromBytes(dataView, index));
        index += 8;
      }

      result.push({
        date: date,
        tickerInfo: {
          averagePrice: averagePrice,
          orderBook: {
            bids: bids,
            asks: asks
          },
        }
      });
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

  convertToHtml(timestamp) {
    return (
        <div>
          <p>Цена на {timestamp.date.getDay()}:{timestamp.date.getMonth()}: {timestamp.tickerInfo.averagePrice}</p>
          <p>Биды</p>
          <table>
            {timestamp.tickerInfo.orderBook.bids.map(b => <tr><td>{b.price}</td><td>{b.amount}</td></tr>)}
          </table>
        </div>
    )
  }

  render() {
    return (
        <div>
          <header>
            <img src={logo} alt="logo"/>
          </header>
          <div>
            {this.state.value && this.state.value.map(t => this.convertToHtml(t))}
          </div>
        </div>
    );
  }
}

export default App;
