import React from 'react';
import PropTypes from 'prop-types';
import clsx from 'clsx';
import {makeStyles, withStyles} from '@material-ui/core/styles';
import TableCell from '@material-ui/core/TableCell';
import Paper from '@material-ui/core/Paper';
import {AutoSizer, Column, RowMouseEventHandlerParams, Table} from 'react-virtualized';
import {Order} from "../Models/Timestamp";

class MuiVirtualizedTable extends React.PureComponent<MuiVirtualizedTableProps> {
    static defaultProps = {
        headerHeight: 48,
        rowHeight: 24
    };

    getRowClassName = ({ index }: {index: number}) => {
        const { classes, onRowClick } = this.props;

        return clsx(classes.tableRow, classes.flexContainer, {
            [classes.tableRowHover]: index !== -1 && onRowClick != null,
        });
    };

    cellRenderer = ({ cellData, columnIndex }: any) => {
        const { columns, classes, rowHeight, onRowClick } = this.props;
        return (
            <TableCell
                component="div"
                className={clsx(classes.tableCell, classes.flexContainer, {
                    [classes.noClick]: onRowClick == null,
                })}
                variant="body"
                style={{ height: rowHeight }}
                align={(columnIndex != null && columns[columnIndex].numeric) || false ? 'right' : 'left'}
                sortDirection={"asc"}
            >
                {cellData}
            </TableCell>
        );
    };

    headerRenderer = ({ label, columnIndex }: {label: string, columnIndex: number}) => {
        const { headerHeight, columns, classes } = this.props;

        return (
            <TableCell
                component="div"
                className={clsx(classes.tableCell, classes.flexContainer, classes.noClick)}
                variant="head"
                style={{ height: headerHeight }}
                align={columns[columnIndex].numeric || false ? 'right' : 'left'}
            >
                <span>{label}</span>
            </TableCell>
        );
    };

    onRowClick(info: RowMouseEventHandlerParams){
        console.log(info.event)
    }

    render() {
        const { classes, columns, rowHeight, headerHeight, ...tableProps } = this.props;
        return (
            <AutoSizer>
                {({ height, width }: {height: number, width: number}) => (
                    <Table
                        //onRowClick={this.onRowClick}
                        rowCount={columns.length}
                        height={height}
                        width={width}
                        rowHeight={rowHeight}
                        headerHeight={headerHeight}
                        // gridStyle={{
                        //     direction: 'inherit'
                        // }}
                        // headerHeight={headerHeight}
                        // // className={classes.table}
                        // {...tableProps}
                        //rowClassName={this.getRowClassName}
                    >
                        {columns.map(({ dataKey, ...other }, index) => {
                            return (
                                <Column
                                    key={dataKey}
                                    headerRenderer={(headerProps: any) =>
                                        this.headerRenderer({
                                            ...headerProps,
                                            columnIndex: index,
                                        })
                                    }
                                    className={classes.flexContainer}
                                    cellRenderer={this.cellRenderer}
                                    dataKey={dataKey}
                                    {...other}
                                />
                            );
                        })}
                    </Table>
                )}
            </AutoSizer>
        );
    }
}

interface MuiVirtualizedTableProps {
    classes: any,
    columns: {
        dataKey: string,
        label: string,
        numeric: boolean | null,
        width: number,
    }[],
    headerHeight: number,
    onRowClick: ((x: RowMouseEventHandlerParams) => void) | undefined,
    rowHeight: number,
}

const VirtualizedTable = withStyles({
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

export default function VirtualizedOrdersTable(props: {orders: Order[]}) {
    return (
        <Paper style={{ height: 400, width: 400 }}>
            <VirtualizedTable
                //rowCount={props.orders.length}
                //rowGetter={({ index }: any) => props.orders[index]}
                onRowClick={undefined}
                columns={[
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
                ]}
            />
        </Paper>
    );
}
