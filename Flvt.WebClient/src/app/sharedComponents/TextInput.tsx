import React from 'react';
import {
    TextField,
    FormControl,
    FilledInputProps,
    InputProps,
    OutlinedInputProps, FormHelperText
} from '@mui/material';
import {useField} from 'formik';
import {observer} from 'mobx-react-lite';

interface Props {
    placeholder: string;
    name: string;
    label?: string;
    type?: string;
    maxValue?: number;
    minValue?: number;
    inputProps?: Partial<FilledInputProps> | Partial<OutlinedInputProps> | Partial<InputProps>
    style?: React.CSSProperties
    disabled?: boolean
    maxLength?: number
    capitalize?: boolean
    color?: "error" | "primary" | "secondary" | "info" | "success" | "warning" | undefined
    autoComplete?: string
    fullwidth?: boolean
    errorMessage?: string
}

const TextInput: React.FC<Props> = ({
                                        disabled,
                                        capitalize,
                                        maxLength,
                                        color,
                                        maxValue,
                                        minValue,
                                        inputProps,
                                        type,
                                        style,
                                        autoComplete,
                                        fullwidth,
                                        errorMessage,
                                        ...props
                                    }) => {
    const [field, meta, helpers] = useField(props.name);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        let value = e.target.value.replace(',', '.');

        if (type === 'number') {
            value = value.replace(/[^0-9.-]/g, '')

            const dots = value.split('.').length - 1

            if (dots > 1) {
                value = value.slice(0, value.lastIndexOf('.'))
            }

            if (value !== '0') {
                value = value.replace(/^0+/, '');
            }

            if (maxValue && parseFloat(value) > maxValue) {
                value = maxValue.toString();
            }

            if (minValue && parseFloat(value) < minValue) {
                value = minValue.toString();
            }
        }

        if (maxLength && value.length > maxLength) {
            value = value.slice(0, maxLength)
        }

        if (capitalize) {
            value = value.toUpperCase()
        }

        helpers.setValue(value);
    };

    return (
        <FormControl error={meta.touched && !!meta.error} fullWidth style={style}>
            <TextField
                {...field}
                {...props}
                disabled={disabled}
                fullWidth={fullwidth}
                autoComplete={autoComplete}
                onChange={handleChange}
                type={type}
                label={props.label ? props.label : props.placeholder}
                variant="outlined"
                error={meta.touched && !!meta.error}
                InputProps={inputProps}
                color={color || 'primary'}
            />
            {errorMessage && (
                <FormHelperText>{errorMessage}</FormHelperText>
            )}
        </FormControl>
    );
}

export default observer(TextInput);
