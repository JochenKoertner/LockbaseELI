import React, { Component } from 'react';
import { Col, Grid, Row, Button } from 'react-bootstrap';

import { Door } from './Door';
import { DoorCaption } from './DoorCaption';

export class Home extends Component {
	displayName = Home.name

	constructor(props) {
		super(props);
		this.state = { isOpen: false };
		this.toggleDoor = this.toggleDoor.bind(this);
	}

	toggleDoor() {
		this.setState({
			isOpen: !this.state.isOpen
		});
	}

	render() {

		const doorName = 'Büro Barthauer';
		const doorId = 'buero_barthauer';

		return (
			<div>
				<Grid>

					<Door doorId={doorId} isOpen={this.state.isOpen}></Door>

					<DoorCaption doorName={doorName}></DoorCaption>

					<Row className="grid-content">
						<Col lg={4}>
							<div className="container-main">
								<p>GEWÄHLTE PERSON</p>
								<p>
									Ahrens; Andrea
									Geschäftsführung
									keine zeitliche Einschränkung
							</p>
								<p>Personen</p>
								<p>Wählen Sie</p>
								<p>Türen/Tore</p>
								<p>Büro Barthauer</p>

							</div>
						</Col>
						<Col lg={4} className="col-content-center" >
							Übersichtsplan Werkhalle West
					</Col>
						<Col lg={4}>
							<div className="container-main">&nbsp;
							<p>Türstatus</p>
								<p>{this.state.isOpen ? "Geöffnet" : "Geschlossen" }</p>
								<p>Schlüssel ID</p>
								<p>900-1</p>
								<p>Uhrzeit</p>
								<p>10 : 45</p>
								<Button bsStyle="warning" bsSize="large" onClick={this.toggleDoor}>Jetzt testen</Button>
							</div>
						</Col>
					</Row>
				</Grid>
			</div>
		);
	}
}
