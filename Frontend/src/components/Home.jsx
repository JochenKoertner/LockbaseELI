import React, { useState, useEffect } from 'react';

import { Col, Grid, Row, Label } from 'react-bootstrap';
import Dropdown from 'react-dropdown';

import DateFnsAdapter from '@date-io/date-fns'

import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

import Button from '@material-ui/core/Button';
import Snackbar from '@material-ui/core/Snackbar';
import IconButton from '@material-ui/core/IconButton';
import CloseIcon from '@material-ui/icons/Close';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';


import frLocale from 'date-fns/locale/fr';
import deLocale from 'date-fns/locale/de';
import esLocale from 'date-fns/locale/es';
import itLocale from 'date-fns/locale/it';
import enLocale from 'date-fns/locale/en-US';

import { injectIntl } from 'react-intl';
import {useIntl} from 'react-intl';

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

class ExtDateFnsUtils extends DateFnsAdapter {
    getDatePickerHeaderText(date) {
			  const dateFns = new DateFnsAdapter({ locale: this.locale});
				return dateFns.format(date, 'd MMMM')
    }
}

const dfns = new DateFnsAdapter();

function Home(props) {

	const [isLoading,setIsLoading] = useState(false);
	const [isOpen,setIsOpen] = useState(null);
	const [personList,setPersonList] = useState(null);
	const [person,setPerson] = useState(null);
	const [doorList,setDoorList] = useState(null);
	const [door,setDoor] = useState(null);
	const [selectedDate,setSelectedDate] = useState(dfns.setMinutes(new Date(), (Math.round(dfns.getMinutes(new Date()) / 15)) * 15));
	const [open,setOpen] = useState(null);
	const [transition,setTransition] = useState(0);
	const [hubConnection,setHubConnection] = useState(
			new HubConnectionBuilder()
				.withUrl('/signalr')
				.configureLogging(LogLevel.Information)
				.build());


	/*
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
			transition: 0,
			hubConnection: null,
		};

		this.toggleDoor = this.toggleDoor.bind(this);
		this.onSelectDoor = this.onSelectDoor.bind(this);
		this.onSelectPerson = this.onSelectPerson.bind(this);
		this.handleDateChange = this.handleDateChange.bind(this);
		this.onSelectRoom = this.onSelectRoom.bind(this);

	}
	*/

	useEffect( () => {
		setIsLoading(true)
		console.log('component mounted')
		fetch('api/data/persons')
			.then(response => response.json())
			.then(data => {
					setPersonList(data);
					setPerson(data[0]);
				 })
			.then( () => fetch('api/data/doors')
				 .then(response => response.json())
				 .then(data => {
					 setIsLoading(false);
					 setDoorList(data);
					 setDoor(null);
				 })
			);

			// useEffect( () => {
			// 	const connection = props.hubConnection
			// 		.start()
			// 		.then( () => console.log('Connection started'))
			// 		.catch( err => console.log('Error while establishing connection'));
				
			// 	return () => {
			// 		console.log('Connection closed');
			// 	}
			// }, [props.hubConnection])

		
	/*	hubConnection.on('BroadcastMessage', (receivedMessage) => {
			console.log(receivedMessage.message);

			fetch('api/data/persons')
			.then(response => response.json())
			.then(data => this.setState({
					personList: data,
					person: data[0]
				 }))
			.then( () => fetch('api/data/doors')
				 .then(response => response.json())
				 .then(data => this.setState({
					 doorList: data,
					 door: data[1].items[1]
				 }))
			);
		}); */
	},[])
	
	const handleClose = (event, reason) => {
		if (reason === 'clickaway') {
			return;
		}

		setOpen(null);
	};

	const toggleDoor = () => {
		console.log(`Toggle Door ${door.lockId} - ${person.keyId} - ${selectedDate}`)
		var dateString = dfns.format(selectedDate, "yyyy-MM-dd'T'HH:mm")
		console.log(dateString)
		fetch(`api/data/check?keyId=${person.keyId}&lockId=${door.lockId}&dateTime=${dateString}`)
			.then( response => response.json())
			.then( ok => {
				console.log(` response with ${ok}`)
				if (ok) {
					let newTransition;
					if (typeof isOpen !== 'undefined') {
						if (isOpen) {
							newTransition = 1;
						} else {
							newTransition = 2;
						}
					}	else {
						newTransition = 0;
					}
					setIsOpen(!isOpen);
					setOpen(true);
					setTransition(newTransition);

					//var audio = document.getElementById('door-sound');
					//audio.play();
				} else {
					setOpen(false)
				}
			});
	}

	const handleDateChange = (date) => {
		setSelectedDate(dfns.setMinutes(date, (Math.round(dfns.getMinutes(date) / 5)) * 5));
	}

	const onSelectDoor = (selected) => {
		const onlyDoors = doorList
			.map(x => {
				if (x.type === 'group') {
					return x.items
				}
				return [x]
			})
			.reduce((a, b) => a.concat(b), []);

		const index = onlyDoors.findIndex(x => x.value === selected.value);
		setDoor(onlyDoors[index]);
	}

	const onSelectPerson = (selected) => {
		const index = personList.findIndex(x => x.value === selected.value);
		setPerson(personList[index]);
	}

	const onSelectRoom = (roomId) => {
		onSelectDoor({value: roomId});
	}

	return (
		isLoading ? (<p>Loading ...</p>) :
		(
			<LanguageContext.Consumer>
				{ language => (
					<Grid>
						<Snackbar
							anchorOrigin={{
							vertical: 'bottom',
							horizontal: 'center',
							}}
							open={open}
							autoHideDuration={2000}
							onClose={handleClose}
							ContentProps={{
							'aria-describedby': 'message-id',
							}}
							message={<span id="message-id">{(door) ? door.label : "xxx" } kann {open} geöffnet werden!</span>}
							action={[
								<IconButton
									key="close"
									aria-label="Close"
									color="inherit"
									onClick={handleClose}
								>
								<CloseIcon />
							</IconButton>,
							]}
						/>

						<Door doorId={(door) ? door.image : "nothing"} isOpen={isOpen} transition={transition}/>
						
						<DoorCaption doorName={(door) ? door.label : "nothing"}></DoorCaption>

					<Row className="grid-content">
						<Col xs={4} className="col-content-aside col-content-left">
							<Label>
								{props.intl.formatMessage(messages.homeLabelPerson)}
							</Label>
							<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
								options={personList} onChange={onSelectPerson} value={person} />

							<InfoBox label={props.intl.formatMessage(messages.homeLabelDepartment)}>
								{(person) ? person.department : "nothing"}
							</InfoBox>

							<ColorInfoBox label={props.intl.formatMessage(messages.homeLabelKeyId)}
								color={(person) && (person.color) ? person.color : undefined}>
								{(person) ? `${person.value} (${person.keyId})` : null} 
							</ColorInfoBox>

							<Label>{props.intl.formatMessage(messages.homeLabelDoor)}</Label>
							<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
								options={doorList} onChange={onSelectDoor} value={door} />

							<InfoBox label={props.intl.formatMessage(messages.homeLabelBuilding)}>
								{(door) ? door.building : null}
							</InfoBox>

							<ColorInfoBox label={props.intl.formatMessage(messages.homeLabelLockId)}
								color={(door) && (door.color) ? door.color : undefined}>
								{(door) ? `${door.value} (${door.lockId})` : null}
							</ColorInfoBox>
						</Col>
						
						<Col xs={4} className="col-content-center" >
							<GroundPlan 
								selectedRoom={(door) ? door.value: null} 
								onClick={onSelectRoom}
								doors={doorList} />
						</Col>

						<Col xs={4} className="col-content-aside col-content-right">

							<InfoBox label={props.intl.formatMessage(messages.homeLabelDoorState)}
								icon={isOpen ? "lock-open" : "lock"} >
								{isOpen ? 
									props.intl.formatMessage(messages.homeDoorOpenState): 
									props.intl.formatMessage(messages.homeDoorCloseState)}
							</InfoBox>
							<MuiPickersUtilsProvider utils={ExtDateFnsUtils} locale={localeMap[language.language.value]}>
								<Row>
									<Col xs={6}>
										<Label>{props.intl.formatMessage(messages.homeLabelDate)}</Label>
									</Col>
									<Col xs={6}>
										<Label>{props.intl.formatMessage(messages.homeLabelTime)}</Label>
									</Col>
								</Row>
								<Row>
									<Col xs={6}>
										<DateSelection intl={props.intl} selectedDate={selectedDate} 
										handleDateChange={handleDateChange} />
									</Col>
									<Col xs={6}>
										<TimeSelection intl={props.intl} selectedDate={selectedDate} 
											handleDateChange={handleDateChange} /> 
									</Col>
								</Row>
							</MuiPickersUtilsProvider>
							
							<Button variant="contained" color="primary" onClick={toggleDoor}>
								{props.intl.formatMessage(messages.homeButtonCheck)}
							</Button>
						</Col>
					</Row>
				</Grid>
			)}
			</LanguageContext.Consumer>
		));
};

Home.displayName = 'Home'
export default injectIntl(Home); 

/*export const withHooksHOC = (Component: any) => {
  return (props: any) => {
    
    return <Home width={screenWidth} {...props} />;
  };
};*/

