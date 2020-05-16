export class TimestampWithInfo {
    constructor(timestamp, exchange, ticker, date) {
        this.timestamp = timestamp;
        this.exchange = exchange;
        this.ticker = ticker;
        this.date = date;
    }
}

class MonthTimestamp {
    constructor(dayTimestamps, date) {
        this.dayTimestamps = dayTimestamps;
        this.date = date;
    }
}

class DayTimestamp {
    constructor(exchangesTimestamp, date) {
        this.exchangesTimestamp = exchangesTimestamp;
        this.date = date;
    }
}

class ExchangesTimestamp {

}
