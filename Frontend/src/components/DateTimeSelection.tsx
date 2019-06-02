import * as React from 'react';

import { DatePicker } from 'material-ui-pickers';
import { TimePicker } from 'material-ui-pickers';

import { messages } from './../translations/messages';

interface DateTimeSelectionProps {
	intl: any,
	selectedDate: Date,
	handleDateChange: any,
	language: string
}

export const DateSelection: React.SFC<DateTimeSelectionProps> = (props) => {
	return (
		<DatePicker className="datePicker"
			format="dd MMM yy"
			cancelLabel={props.intl.formatMessage(messages.cancelLabel)}
			okLabel={props.intl.formatMessage(messages.okLabel)}
			value={props.selectedDate} onChange={props.handleDateChange} />
	);
};

export const TimeSelection: React.SFC<DateTimeSelectionProps> = (props) => {
	return (
		<TimePicker className="timePicker"
			ampm={false}
			cancelLabel={props.intl.formatMessage(messages.cancelLabel)}
			okLabel={props.intl.formatMessage(messages.okLabel)}
			value={props.selectedDate} onChange={props.handleDateChange} />
	);
};

