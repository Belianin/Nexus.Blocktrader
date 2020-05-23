import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import TextField from '@material-ui/core/TextField';

const useStyles = makeStyles((theme) => ({
    container: {
        display: 'flex',
        flexWrap: 'wrap',
    },
    textField: {
        marginLeft: theme.spacing(1),
        marginRight: theme.spacing(1),
        width: 200,
    },
}));

interface DatePickerProps {
    onChange: (event: any) => void,
    defaultValue: string
}

export default function DatePickers(props: DatePickerProps) {
    const classes = useStyles();

    return (
        <form className={classes.container} noValidate>
            <TextField
                id="date"
                label="Дата"
                type="date"
                defaultValue={props.defaultValue}
                className={classes.textField}
                onChange={props.onChange}
                InputLabelProps={{
                    shrink: true,
                }}
            />
        </form>
    );
}
