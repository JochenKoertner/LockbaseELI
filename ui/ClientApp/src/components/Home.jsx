import React, { Component } from 'react';
import { Col, Grid, Row, Label } from 'react-bootstrap';
import Dropdown from 'react-dropdown';

import DateFnsUtils from '@date-io/date-fns';
import { withStyles } from '@material-ui/core/styles';

import Button from '@material-ui/core/Button';
import Snackbar from '@material-ui/core/Snackbar';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';
import CheckCircleIcon from '@material-ui/icons/CheckCircle';
import green from '@material-ui/core/colors/green';

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

import { arrowClosed, arrowOpen, DateSelection, TimeSelection } from './Constants';
import { LanguageContext, persons, doors } from './../services/BackendAdapter';

import { messages } from './../translations/messages';

const localeMap = {
	de: deLocale,
	es: esLocale,
	en: enLocale,
	fr: frLocale,
	it: itLocale,
};

const dfns = new DateFnsUtils();

class Home extends Component {
	displayName = Home.name

	
		
	constructor(props) {
		super(props);

		

		// Zeit auf 15 Minuten Intervall abrunden
		var date = new Date();
		date = dfns.setMinutes(date, (Math.round(dfns.getMinutes(date) / 15)) * 15);

		this.state = {
			isOpen: false,
			person: persons[0],
			door: doors[1].items[2],
			selectedDate: date,
			open: false
		};

		this.toggleDoor = this.toggleDoor.bind(this);
		this.onSelectDoor = this.onSelectDoor.bind(this);
		this.onSelectPerson = this.onSelectPerson.bind(this);
		this.handleDateChange = this.handleDateChange.bind(this);
	}
	
	handleClose = (event, reason) => {
		if (reason === 'clickaway') {
			return;
		}

		this.setState({ open: false });
	};

	toggleDoor() {
		this.setState({
			isOpen: !this.state.isOpen,
			open: true
		});
	}

	handleDateChange = date => {
		this.setState({selectedDate: dfns.setMinutes(date, (Math.round(dfns.getMinutes(date) / 5)) * 5)});
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

	render() {

		const { selectedDate } = this.state;

		return (
			<LanguageContext.Consumer>
				{ language => (
					<Grid>
						<Snackbar
							anchorOrigin={{
							vertical: 'bottom',
							horizontal: 'center',
							}}
							open={this.state.open}
							autoHideDuration={2000}
							onClose={this.handleClose}
							ContentProps={{
							'aria-describedby': 'message-id',
							}}
							message={<span id="message-id">{this.state.door.label} kann geöffnet werden!</span>}
							action={[
								<IconButton
									key="close"
									aria-label="Close"
									color="inherit"
									onClick={this.handleClose}
								>
								<CloseIcon />
							</IconButton>,
							]}
						/>

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
							<MuiPickersUtilsProvider utils={DateFnsUtils} locale={localeMap[language.language.value]}>
							<Row>
									<Col xs={6}>
										<Label>{this.props.intl.formatMessage(messages.homeLabelDate)}</Label>
									</Col>
									<Col xs={6}>
										<Label>{this.props.intl.formatMessage(messages.homeLabelTime)}</Label>
									</Col>
								</Row>
								<Row>
									<Col xs={6}>
										<DateSelection intl={this.props.intl} selectedDate={selectedDate} 
										handleDateChange={this.handleDateChange} />
									</Col>
									<Col xs={6}>
										<TimeSelection intl={this.props.intl} selectedDate={selectedDate} 
											handleDateChange={this.handleDateChange} />
									</Col>
								</Row>
							</MuiPickersUtilsProvider>
							<div>
								<Button variant="contained" color="primary" onClick={this.toggleDoor}>
									{this.props.intl.formatMessage(messages.homeButtonCheck)}
								</Button>
							</div>
						</Col>
					</Row>
				</Grid>
			)}
			</LanguageContext.Consumer>
		);
	}
};

export default injectIntl(Home, {withRef:true});

