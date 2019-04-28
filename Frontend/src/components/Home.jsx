import React, { Component } from 'react';
import { Col, Grid, Row, Label } from 'react-bootstrap';
import Dropdown from 'react-dropdown';

import DateFnsUtils from '@date-io/date-fns'
import format from 'date-fns/format'
import moment from 'moment'

import Button from '@material-ui/core/Button';
import Snackbar from '@material-ui/core/Snackbar';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';

import { MuiPickersUtilsProvider } from 'material-ui-pickers';

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
import { DateSelection, TimeSelection } from './DateTimeSelection';
import { LanguageContext } from './../services/BackendAdapter';

import { messages } from './../translations/messages';

const localeMap = {
	de: deLocale,
	es: esLocale,
	en: enLocale,
	fr: frLocale,
	it: itLocale,
};

class ExtDateFnsUtils extends DateFnsUtils {
    startOfMonth(date) {
        return moment(date).startOf('month').toDate();
    }
    getDatePickerHeaderText(date) {
        return format(date, 'd MMMM', {locale: this.locale})
    }
}

const dfns = new DateFnsUtils();

class Home extends Component {
	displayName = Home.name

	
		
	constructor(props) {
		super(props);

		// https://www.robinwieruch.de/react-fetching-data/


		// Zeit auf 15 Minuten Intervall abrunden
		var date = new Date();
		date = dfns.setMinutes(date, (Math.round(dfns.getMinutes(date) / 15)) * 15);

		this.state = {
			isLoading: false,
			personList: null,
			doorList: null,
			isOpen: null,
			person: null,
			door: null,
			selectedDate: date,
			open: null,
			transition: 0
		};

		this.toggleDoor = this.toggleDoor.bind(this);
		this.onSelectDoor = this.onSelectDoor.bind(this);
		this.onSelectPerson = this.onSelectPerson.bind(this);
		this.handleDateChange = this.handleDateChange.bind(this);
		this.onSelectRoom = this.onSelectRoom.bind(this);

	}

	componentDidMount() {
		this.setState({isLoading: true})
		fetch('api/data/persons')
			.then(response => response.json())
			.then(data => this.setState({
					personList: data,
					person: data[0]
				 }))
			.then( () => fetch('api/data/doors')
				 .then(response => response.json())
				 .then(data => this.setState({
					 isLoading: false,
					 doorList: data,
					 door: data[1].items[1]
				 }))
			);
	}
	
	handleClose = (event, reason) => {
		if (reason === 'clickaway') {
			return;
		}

		this.setState({ open: null });
	};

	toggleDoor() {
		console.log(`Toggle Door ${this.state.door.lockId} - ${this.state.person.keyId} - ${this.state.selectedDate}`)
		var dateString = dfns.format(this.state.selectedDate, "yyyy-MM-dd'T'HH:mmZ")   
		console.log(dateString)
		fetch(`api/data/check?keyId=${this.state.person.keyId}&lockId=${this.state.door.lockId}&dateTime=${dateString}`)
			.then( response => response.json())
			.then( ok => {
				console.log(` response with ${ok}`)
				if (ok) {
					this.setState(function(state) {
						let transition;
						if (typeof state.isOpen !== 'undefined') {
							if (state.isOpen) {
								transition = 1;
							} else
							{
								transition = 2;
							}
						}
						else
						{
							transition = 0;
						}
						return {
							isOpen: !state.isOpen,
							open: true,
							transition: transition,
						}
					})
					//var audio = document.getElementById('door-sound');
					//audio.play();
				}
				else {
					this.setState(function(state) {
						return {
							open: false
						}
					})
				}
			});
	}

	handleDateChange = date => {
		this.setState({selectedDate: dfns.setMinutes(date, (Math.round(dfns.getMinutes(date) / 5)) * 5)});
	}

	onSelectDoor(selected) {
		const onlyDoors = this.state.doorList
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
		
		const index = this.state.personList.findIndex(x => x.value === selected.value);
		this.setState({ person: this.state.personList[index] });
	}

	onSelectRoom(roomId) {
		this.onSelectDoor({value: roomId});
	}

	render() {

		if (this.state.isLoading) {
			return <p>Loading ...</p>;
		}

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
							message={<span id="message-id">{(this.state.door) ? this.state.door.label : "xxx" } kann geöffnet werden!</span>}
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

						<Door doorId={(this.state.door) ? this.state.door.image : "nothing"} isOpen={this.state.isOpen} transition={this.state.transition}/>
						
						<DoorCaption doorName={(this.state.door) ? this.state.door.label : "nothing"}></DoorCaption>

					<Row className="grid-content">
						<Col xs={4} className="col-content-aside col-content-left">
							<Label>
								{this.props.intl.formatMessage(messages.homeLabelPerson)}
							</Label>
							<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
								options={this.state.personList} onChange={this.onSelectPerson} value={this.state.person} />

							<InfoBox label={this.props.intl.formatMessage(messages.homeLabelDepartment)}>
								{(this.state.person) ? this.state.person.department : "nothing"}
							</InfoBox>

							<ColorInfoBox label={this.props.intl.formatMessage(messages.homeLabelKeyId)}
								color={(this.state.person) && (this.state.person.color) ? this.state.person.color : undefined}>
								{(this.state.person) ? `${this.state.person.value} (${this.state.person.keyId})` : null} 
							</ColorInfoBox>

							<Label>{this.props.intl.formatMessage(messages.homeLabelDoor)}</Label>
							<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
								options={this.state.doorList} onChange={this.onSelectDoor} value={this.state.door} />

							<InfoBox label={this.props.intl.formatMessage(messages.homeLabelBuilding)}>
								{(this.state.door) ? this.state.door.building : null}
							</InfoBox>

							<ColorInfoBox label={this.props.intl.formatMessage(messages.homeLabelLockId)}
								color={(this.state.door) && (this.state.door.color) ? this.state.door.color : undefined}>
								{(this.state.door) ? `${this.state.door.value} (${this.state.door.lockId})` : null}
							</ColorInfoBox>
						</Col>
						
						<Col xs={4} className="col-content-center" >
							<GroundPlan 
								selectedRoom={(this.state.door) ? this.state.door.value: null} 
								onClick={this.onSelectRoom}
								doors={this.state.doorList} />
						</Col>

						<Col xs={4} className="col-content-aside col-content-right">

							<InfoBox label={this.props.intl.formatMessage(messages.homeLabelDoorState)}
								icon={this.state.isOpen ? "lock-open" : "lock"} >
								{this.state.isOpen ? 
									this.props.intl.formatMessage(messages.homeDoorOpenState): 
									this.props.intl.formatMessage(messages.homeDoorCloseState)}
							</InfoBox>
							<MuiPickersUtilsProvider utils={ExtDateFnsUtils} locale={localeMap[language.language.value]}>
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
							
								<Button variant="contained" color="primary" onClick={this.toggleDoor}>
									{this.props.intl.formatMessage(messages.homeButtonCheck)}
								</Button>
						</Col>
					</Row>
				</Grid>
			)}
			</LanguageContext.Consumer>
		);
	}
};

export default injectIntl(Home, {withRef:true});

