import React, { Component } from 'react';
import { Col, Grid, Row, Button, Label } from 'react-bootstrap';

import Dropdown from 'react-dropdown';

import { Door } from './Door';
import { DoorCaption } from './DoorCaption';

const arrowClosed = (
	<span className="arrow-closed" />
)
const arrowOpen = (
	<span className="arrow-open" />
)

const persons = [
	{ value: 4711, label: 'Ahrens; Andrea', section: 'Geschäftsleitung', summary: 'keine zeitliche Einschränkung' },
	{ value: 4712, label: 'Müller; Bernd', section: 'Werkstattsleitung', summary:'keine zeitliche Einschränkung'  },
	{ value: 4713, label: 'Schmidt; Helga', section: 'Sekretariat', summary: 'normale Öffnungszeiten' }
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


export class Home extends Component {
	displayName = Home.name

	

	constructor(props) {
		super(props);
		this.state = { isOpen: false, person: persons[0], door: doors[0] };
		this.toggleDoor = this.toggleDoor.bind(this);
		this.onSelectDoor = this.onSelectDoor.bind(this);
		this.onSelectPerson = this.onSelectPerson.bind(this);
	}

	toggleDoor() {
		this.setState({
			isOpen: !this.state.isOpen
		});
	}

	_onSelect() {

	}

	onSelectDoor(selected) {
		const index = doors.findIndex( x => x.value === selected.value);
		console.log(selected.value);
		console.log(index);
		this.setState({door: selected});
	}

	onSelectPerson(selected) {
		const index = persons.findIndex( x => x.value === selected.value);

		this.setState({person: persons[index]});
	}

	render() {

		
		const doorId = 'buero_barthauer';

		

		const minutes = ['00','15','30','45']
		const hours = ['00','01','02','03','04','05','06','07','08','09','10','11',
										'12','13','14','15','16','17','18','19','20','21','22','23']


		return (
			<div>
				<Grid>

					<Door doorId={doorId} isOpen={this.state.isOpen}></Door>

					<DoorCaption doorName={this.state.door.label}></DoorCaption>

					<Row className="grid-content">
						<Col lg={1}></Col>
						<Col lg={3}>
								<Label>GEWÄHLTE PERSON</Label>
								<p className="info-box">
									{this.state.person.label} 
									<br></br>
									{this.state.person.section}
									<br></br>
									{this.state.person.summary}
							</p>
								<Label>Personen</Label>
								
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} options={persons} onChange={this.onSelectPerson} value={this.state.person} placeholder="Wählen Sie" />
								<Label>Türen/Tore</Label>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} options={doors} onChange={this.onSelectDoor} value={this.state.door} placeholder="Wählen Sie" />
						</Col>
						<Col lg={4} className="col-content-center" >
							Übersichtsplan Werkhalle West
					</Col>
						<Col lg={3}>
							<Label>Türstatus</Label>
							<p className="info-box">{this.state.isOpen ? "Geöffnet" : "Geschlossen" }</p>
							<Label>Schlüssel ID</Label>
							<p className="info-box">900-1</p>
						
							<Label>Uhrzeit</Label>
							<Row>
								<Col lg={6}>
								<Dropdown options={hours} onChange={this._onSelect} value={'10'} placeholder="Wählen Sie" />
								</Col>
								<Col lg={6}>
								<Dropdown options={minutes} onChange={this._onSelect} value={'45'} placeholder="Wählen Sie" />
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
