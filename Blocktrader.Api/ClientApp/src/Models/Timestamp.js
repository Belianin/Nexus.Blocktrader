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
