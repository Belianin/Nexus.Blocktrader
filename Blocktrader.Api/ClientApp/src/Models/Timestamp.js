export class Timestamp {
    constructor(date, tickerInfo){
        this.date = date;
        this.tickerInfo = tickerInfo
    }
}

export class OrderBook {
    constructor(bids, asks){
        this.bids = bids;
        this.asks = asks;
    }
}

export class TickerInfo {
    constructor(orderBook, currentPrice) {
        this.orderBook = orderBook;
        this.currentPrice = currentPrice;
    }
}

export class Order {
    constructor(price, amount){
        this.price = price;
        this.amount = amount;
    }
}

export function timestampFromBytes(buffer) {
    const result = [];

    let index = 0;
    const dataView = new DataView(buffer);

    console.log(buffer.byteLength);
    while (index < buffer.byteLength) {
        const date = new Date(Number(dataView.getBigInt64(index, true)));
        index += 8;
        const averagePrice = dataView.getFloat32(index, true);
        index += 4;

        const bidsCount = dataView.getInt32(index, true);
        index += 4;
        const bids = [];
        for (let i = 0; i < bidsCount; i++) {
            bids.push(getOrderFromBytes(dataView, index));
            index += 8;
        }

        const asksCount = dataView.getInt32(index, true);
        index += 4;
        const asks = [];
        for (let i = 0; i < asksCount; i++) {
            asks.push(getOrderFromBytes(dataView, index));
            index += 8;
        }

        result.push(new Timestamp(date, new TickerInfo(new OrderBook(bids, asks), averagePrice)));
    }

    return result;
}

function getOrderFromBytes(dataView, index) {
    return new Order(
        dataView.getFloat32(index, true),
        dataView.getFloat32(index + 4, true));
}
