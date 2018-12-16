import React, { Component } from 'react';
import { Col, Grid, Row, Button, Label } from 'react-bootstrap';
import Dropdown from 'react-dropdown';


import Door from './Door';
import DoorCaption from './DoorCaption';
import InfoBox from './InfoBox';

import { arrowClosed, arrowOpen } from './Constants';
import { persons, doors, minutes, hours } from './../services/BackendAdapter';


export class Home extends Component {
	displayName = Home.name

	constructor(props) {
		super(props);
		this.state = {
			isOpen: false,
			person: persons[0],
			door: doors[1].items[2],
			hour: hours[10],
			minute: minutes[3]
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

		return (

			<Grid>

				<Door doorId={this.state.door.image} isOpen={this.state.isOpen}></Door>

				<DoorCaption doorName={this.state.door.label}></DoorCaption>

				<Row className="grid-content">
					<Col xs={4} className="col-content-aside col-content-left">

						<Label>Gewählte Person</Label>
						<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
							options={persons} onChange={this.onSelectPerson} value={this.state.person} />

						<InfoBox label="Abteilung">
							{this.state.person.department}
						</InfoBox>

						<InfoBox label="Schlüssel ID">
							{this.state.person.value}
						</InfoBox>

					</Col>
					<Col xs={4} className="col-content-center" >
						<p>Verwaltung</p>
						<p>Produktion</p>
					</Col>

					<Col xs={4} className="col-content-aside col-content-right">
						<Label>Türen/Tore</Label>
						<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
							options={doors} onChange={this.onSelectDoor} value={this.state.door} />

						<InfoBox label="Gebäude">
							{this.state.door.building}
						</InfoBox>

						<InfoBox label="Schloss ID">
							{this.state.door.value}
						</InfoBox>

						<InfoBox label="Türstatus">
							{this.state.isOpen ? "Geöffnet" : "Geschlossen"}
						</InfoBox>

						<Label>Uhrzeit</Label>
						<Row>
							<Col xs={6}>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
									options={hours} onChange={this.onSelectHour} value={this.state.hour} />
							</Col>
							<Col xs={6}>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
									options={minutes} onChange={this.onSelectMinute} value={this.state.minute} />
							</Col>
						</Row>
						<div>
							<Button bsStyle="warning" bsSize="large" onClick={this.toggleDoor}>Jetzt testen</Button>
						</div>
					</Col>
				</Row>
			</Grid>
		);
	}
}
