"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) {
            if (e.indexOf(p[i]) < 0 && Object.prototype.propertyIsEnumerable.call(s, p[i]))
                t[p[i]] = s[p[i]];
        }
    return t;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var react_1 = __importDefault(require("react"));
var clsx_1 = __importDefault(require("clsx"));
var styles_1 = require("@material-ui/core/styles");
var TableCell_1 = __importDefault(require("@material-ui/core/TableCell"));
var Paper_1 = __importDefault(require("@material-ui/core/Paper"));
var react_virtualized_1 = require("react-virtualized");
var MuiVirtualizedTable = /** @class */ (function (_super) {
    __extends(MuiVirtualizedTable, _super);
    function MuiVirtualizedTable() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.getRowClassName = function (_a) {
            var _b;
            var index = _a.index;
            var _c = _this.props, classes = _c.classes, onRowClick = _c.onRowClick;
            return clsx_1.default(classes.tableRow, classes.flexContainer, (_b = {},
                _b[classes.tableRowHover] = index !== -1 && onRowClick != null,
                _b));
        };
        _this.cellRenderer = function (_a) {
            var _b;
            var cellData = _a.cellData, columnIndex = _a.columnIndex;
            var _c = _this.props, columns = _c.columns, classes = _c.classes, rowHeight = _c.rowHeight, onRowClick = _c.onRowClick;
            return (<TableCell_1.default component="div" className={clsx_1.default(classes.tableCell, classes.flexContainer, (_b = {},
                _b[classes.noClick] = onRowClick == null,
                _b))} variant="body" style={{ height: rowHeight }} align={(columnIndex != null && columns[columnIndex].numeric) || false ? 'right' : 'left'} sortDirection={"asc"}>
                {cellData}
            </TableCell_1.default>);
        };
        _this.headerRenderer = function (_a) {
            var label = _a.label, columnIndex = _a.columnIndex;
            var _b = _this.props, headerHeight = _b.headerHeight, columns = _b.columns, classes = _b.classes;
            return (<TableCell_1.default component="div" className={clsx_1.default(classes.tableCell, classes.flexContainer, classes.noClick)} variant="head" style={{ height: headerHeight }} align={columns[columnIndex].numeric || false ? 'right' : 'left'}>
                <span>{label}</span>
            </TableCell_1.default>);
        };
        return _this;
    }
    MuiVirtualizedTable.prototype.onRowClick = function (info) {
        console.log(info.event);
    };
    MuiVirtualizedTable.prototype.render = function () {
        var _this = this;
        var _a = this.props, classes = _a.classes, columns = _a.columns, rowHeight = _a.rowHeight, headerHeight = _a.headerHeight, tableProps = __rest(_a, ["classes", "columns", "rowHeight", "headerHeight"]);
        return (<react_virtualized_1.AutoSizer>
                {function (_a) {
            var height = _a.height, width = _a.width;
            return (<react_virtualized_1.Table 
            //onRowClick={this.onRowClick}
            rowCount={columns.length} height={height} width={width} rowHeight={rowHeight} headerHeight={headerHeight}>
                        {columns.map(function (_a, index) {
                var dataKey = _a.dataKey, other = __rest(_a, ["dataKey"]);
                return (<react_virtualized_1.Column key={dataKey} headerRenderer={function (headerProps) {
                    return _this.headerRenderer(__assign(__assign({}, headerProps), { columnIndex: index }));
                }} className={classes.flexContainer} cellRenderer={_this.cellRenderer} dataKey={dataKey} {...other}/>);
            })}
                    </react_virtualized_1.Table>);
        }}
            </react_virtualized_1.AutoSizer>);
    };
    MuiVirtualizedTable.defaultProps = {
        headerHeight: 48,
        rowHeight: 24
    };
    return MuiVirtualizedTable;
}(react_1.default.PureComponent));
var VirtualizedTable = styles_1.withStyles({
    flexContainer: {
        display: 'flex',
        alignItems: 'center',
        boxSizing: 'border-box',
    },
    tableRow: {
        cursor: 'pointer',
    },
    // tableRowHover: {
    //     '&:hover': {
    //         backgroundColor: theme.palette.grey[200],
    //     },
    // },
    tableCell: {
        flex: 1
    },
    noClick: {
        cursor: 'initial',
    }
})(MuiVirtualizedTable);
// ---
function VirtualizedOrdersTable(props) {
    return (<Paper_1.default style={{ height: 400, width: 400 }}>
            <VirtualizedTable 
    //rowCount={props.orders.length}
    //rowGetter={({ index }: any) => props.orders[index]}
    onRowClick={undefined} columns={[
        {
            width: 200,
            label: 'Цена',
            dataKey: 'price',
            numeric: true
        },
        {
            width: 200,
            label: 'Обьем',
            dataKey: 'amount',
            numeric: true
        }
    ]}/>
        </Paper_1.default>);
}
exports.default = VirtualizedOrdersTable;
