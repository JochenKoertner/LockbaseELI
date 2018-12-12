import React, { Component } from 'react';
import { Col, Grid, Row, Button, Label } from 'react-bootstrap';

import Dropdown from 'react-dropdown';

import { Door } from './Door';
import { DoorCaption } from './DoorCaption';
import { InfoBox } from './InfoBox';

const arrowClosed = (
	<span className="arrow-closed" />
)
const arrowOpen = (
	<span className="arrow-open" />
)

const persons = [
	{ value: 4711, label: 'Ahrens; Andrea', section: 'Geschäftsleitung', 
		summary: 'keine zeitliche Einschränkung', keyId: '900-1' },
	{ value: 4712, label: 'Müller; Bernd', section: 'Werkstattsleitung', 
		summary:'keine zeitliche Einschränkung', keyId: '900-7' },
	{ value: 4713, label: 'Schmidt; Helga', section: 'Sekretariat', 
		summary: 'normale Öffnungszeiten', keyId: '100-99' }
]

const doors = [
	{ value: 'torwest', label: 'Tor West' },
	{ type: 'group', name: 'Verwaltung', items: [
		 { value: 'konferenzraum', label: 'Konferenzraum' },
		 { value: 'buero_ahrens', label: 'Büro Ahrens' },
		 { value: 'buero_barthauer', label: 'Büro Barthauer' },
		 { value: 'buchhaltung', label: 'Buchhaltung' },
		 { value: 'buero_vertrieb1', label: 'Büro Vertrieb 1' },
		 { value: 'buero_vertrieb2', label: 'Büro Vertrieb 2' },
		 { value: 'eingang_west', label: 'Eingang West' }
	 ]
	},
	{
	 type: 'group', name: 'Produktion', items: [
		 { value: 'werkhalle_west', label: 'Werkhalle West' },
		 { value: 'metalllager', label: 'Metalllager' },
		 { value: 'buero_montage', label: 'Büro Montage' },
		 { value: 'warenlager', label: 'Warenlager' },
		 { value: 'werkhalle_sued', label: 'Werkhalle Süd' }
	 ]
	}
]

function* hoursGenerator() {
  var index = 0;
  while(index < 24) {
    yield index++;
  }
}

function* minutesGenerator() {
  var index = 0;
  while(index < 4) {
    yield index * 15;
    index++;
  }
}

let hours = Array
	.from(hoursGenerator())
	.map( x => { 
		return {
			value : x ,
			label : x.toString().padStart(2,'0')
		}; 
	})

let minutes = Array
	.from(minutesGenerator())
	.map( x => { 
		return {
			value : x ,
			label : x.toString().padStart(2,'0')
		}; 
	})

export class Home extends Component {
	displayName = Home.name

	constructor(props) {
		super(props);
		this.state = { 
			isOpen: false, 
			person: persons[0], 
			door: doors[0], 
			hour: hours[10], 
			minute: minutes[3] };

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

	_onSelect() {

	}

	onSelectDoor(selected) {
		const onlyDoors = doors
			.map( x => {
					if (x.type === 'group') {
						return x.items	
					} 
					return [x]
				})
			.reduce( (a, b) => a.concat(b),[]);

		const index = onlyDoors.findIndex( x => x.value === selected.value);
		this.setState({door: onlyDoors[index]});
	}

	onSelectPerson(selected) {
		const index = persons.findIndex( x => x.value === selected.value);
		this.setState({person: persons[index]});
	}

	onSelectHour(selected) {
		const index = hours.findIndex( x => x.value === selected.value);
		this.setState({hour: hours[index]});
	}

	onSelectMinute(selected) {
		const index = minutes.findIndex( x => x.value === selected.value);
		this.setState({minute: minutes[index]});
	}

	render() {

		const doorId = 'buero_barthauer';

		return (
			<div>
				<Grid>

					<Door doorId={doorId} isOpen={this.state.isOpen}></Door>

					<DoorCaption doorName={this.state.door.label}></DoorCaption>

					<Row className="grid-content">
						<Col lg={1}></Col>
						<Col lg={3}>
								<InfoBox label="Gewählte Person">
									{this.state.person.label} 
									<br/>
									{this.state.person.section}
									<br/>
									{this.state.person.summary}
								</InfoBox>
								<Label>Personen</Label>
								
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} 
								options={persons} onChange={this.onSelectPerson} value={this.state.person} />
								<Label>Türen/Tore</Label>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} 
								options={doors} onChange={this.onSelectDoor} value={this.state.door} />

						</Col>
						<Col lg={4} className="col-content-center" >
							Übersichtsplan Werkhalle West
					</Col>
						<Col lg={3}>
							<InfoBox label="Türstatus">
								{this.state.isOpen ? "Geöffnet" : "Geschlossen" }
							</InfoBox>
							<InfoBox label="Schlüssel ID">
								{this.state.person.keyId}
							</InfoBox>

							<Label>Uhrzeit</Label>
							<Row>
								<Col lg={6}>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} 
									options={hours} onChange={this.onSelectHour} value={this.state.hour}/>
								</Col>
								<Col lg={6}>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} 
									options={minutes} onChange={this.onSelectMinute} value={this.state.minute} />
								</Col>
							</Row>

							<Button bsStyle="warning" bsSize="large" onClick={this.toggleDoor}>Jetzt testen</Button>
						</Col>
						<Col lg={1}></Col>
					</Row>
				</Grid>
			</div>
		);
	}
}
