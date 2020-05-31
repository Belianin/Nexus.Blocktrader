import React from 'react';
import 'date-fns';
import DateFnsUtils from '@date-io/date-fns';
import {
    MuiPickersUtilsProvider,
    KeyboardDatePicker,
} from '@material-ui/pickers';

export default function DatePicker(props) {
    return (
        <MuiPickersUtilsProvider utils={DateFnsUtils}>
                <KeyboardDatePicker
                    variant="inline"
                    format="dd/MM/yyyy"
                    margin="normal"
                    id="date-picker"
                    label="Выбор даты"
                    value={props.defaultValue}
                    onChange={props.onChange}
                    KeyboardButtonProps={{
                        'aria-label': 'change date',
                    }}
                />
        </MuiPickersUtilsProvider>
    );
}
