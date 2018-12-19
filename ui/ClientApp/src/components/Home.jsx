import React, { Component } from 'react';
import { Col, Grid, Row, Button, Label } from 'react-bootstrap';
import Dropdown from 'react-dropdown';

import DateFnsUtils from '@date-io/date-fns';
import { MuiPickersUtilsProvider } from 'material-ui-pickers';
import { TimePicker } from 'material-ui-pickers';
import { DatePicker } from 'material-ui-pickers';

import frLocale from 'date-fns/locale/fr';
import deLocale from 'date-fns/locale/de';
import esLocale from 'date-fns/locale/es';
import itLocale from 'date-fns/locale/it';
import enLocale from 'date-fns/locale/en-US';

import { injectIntl } from 'react-intl';

// https://codepen.io/ecurbelo/pen/GKjAx


import Door from './Door';
import DoorCaption from './DoorCaption';
import GroundPlan from './GroundPlan';
import InfoBox from './InfoBox';
import ColorInfoBox from './ColorInfoBox';

import { arrowClosed, arrowOpen } from './Constants';
import { persons, doors, minutes, hours } from './../services/BackendAdapter';

import { messages } from './../translations/messages';

const localeMap = {
	de: deLocale,
	es: esLocale,
	en: enLocale,
	fr: frLocale,
	it: itLocale,
};

class Home extends Component {
	displayName = Home.name

	constructor(props) {
		super(props);
		this.state = {
			isOpen: false,
			person: persons[0],
			door: doors[1].items[2],
			hour: hours[10],
			minute: minutes[3],
			selectedDate: new Date()
		};

		this.toggleDoor = this.toggleDoor.bind(this);
		this.onSelectDoor = this.onSelectDoor.bind(this);
		this.onSelectPerson = this.onSelectPerson.bind(this);
		this.onSelectHour = this.onSelectHour.bind(this);
		this.onSelectMinute = this.onSelectMinute.bind(this);
	}

	toggleDoor() {
		this.setState({
			isOpen: !this.state.isOpen
		});
	}

	handleDateChange = date => {
		this.setState({selectedDate: date});
	}

	onSelectDoor(selected) {
		const onlyDoors = doors
			.map(x => {
				if (x.type === 'group') {
					return x.items
				}
				return [x]
			})
			.reduce((a, b) => a.concat(b), []);

		const index = onlyDoors.findIndex(x => x.value === selected.value);
		this.setState({ door: onlyDoors[index] });
	}

	onSelectPerson(selected) {
		const index = persons.findIndex(x => x.value === selected.value);
		this.setState({ person: persons[index] });
	}

	onSelectHour(selected) {
		const index = hours.findIndex(x => x.value === selected.value);
		this.setState({ hour: hours[index] });
	}

	onSelectMinute(selected) {
		const index = minutes.findIndex(x => x.value === selected.value);
		this.setState({ minute: minutes[index] });
	}

	render() {

		const { selectedDate } = this.state;

		const locale = localeMap['de'];

		return (

			<Grid>
				<Door doorId={this.state.door.image} isOpen={this.state.isOpen}></Door>

				<DoorCaption doorName={this.state.door.label}></DoorCaption>

				<Row className="grid-content">
					<Col xs={4} className="col-content-aside col-content-left">


						<Label>
							{this.props.intl.formatMessage(messages.homeLabelPerson)}
						</Label>
						<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
							options={persons} onChange={this.onSelectPerson} value={this.state.person} />

						<InfoBox label={this.props.intl.formatMessage(messages.homeLabelDepartment)}>
							{this.state.person.department}
						</InfoBox>

						<ColorInfoBox label={this.props.intl.formatMessage(messages.homeLabelKeyId)}
							color={this.state.person.color}>
							{this.state.person.value}
						</ColorInfoBox>

						<Label>{this.props.intl.formatMessage(messages.homeLabelDoor)}</Label>
						<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
							options={doors} onChange={this.onSelectDoor} value={this.state.door} />

						<InfoBox label={this.props.intl.formatMessage(messages.homeLabelBuilding)}>
							{this.state.door.building}
						</InfoBox>

						<ColorInfoBox label={this.props.intl.formatMessage(messages.homeLabelLockId)}
							color={this.state.door.color} >
							{this.state.door.value}
						</ColorInfoBox>
					</Col>
					
					<GroundPlan/>

					<Col xs={4} className="col-content-aside col-content-right">

						<InfoBox label={this.props.intl.formatMessage(messages.homeLabelDoorState)}
							icon={this.state.isOpen ? "lock-open" : "lock"} >
							{this.state.isOpen ? 
								this.props.intl.formatMessage(messages.homeDoorOpenState): 
								this.props.intl.formatMessage(messages.homeDoorCloseState)}
						</InfoBox>
						<MuiPickersUtilsProvider utils={DateFnsUtils} locale={locale}>
							<Row>
							
								<Col xs={6}>
									<Label>{this.props.intl.formatMessage(messages.homeLabelTime)}</Label>
						
									<DatePicker value={selectedDate} onChange={this.handleDateChange} />
								</Col>
								<Col xs={6}>
									<Label>{this.props.intl.formatMessage(messages.homeLabelTime)}</Label>
						
									<TimePicker value={selectedDate} onChange={this.handleDateChange} />
		
								</Col>
							</Row>
						</MuiPickersUtilsProvider>
						<div>
							<Button bsStyle="warning" bsSize="large" onClick={this.toggleDoor}>
								{this.props.intl.formatMessage(messages.homeButtonCheck)}
							</Button>
						</div>
					</Col>
				</Row>
			</Grid>
		);
	}
};

export default injectIntl(Home, {withRef:true});

