import React, { Component } from 'react';
import { Col, Grid, Row, Button } from 'react-bootstrap';


export class Home extends Component {
  displayName = Home.name

  render() {

	const doorName = 'Büro Barthauer';
	const doorId = 'buero_barthauer';

	var imgUrl = '/images/doors/' + doorId + '_open.png'
	// '_close.png';
	
    return (
      <div>
		  	<Grid>
				<Row className="grid-image" style={{backgroundImage: `url(${imgUrl})`}}>
				</Row>

				<Row className="grid-header">
					<Col lg={4} />
				
					<Col lg={4} className="col-header-center" >
						{doorName}
					</Col>
					<Col lg={4} />
				</Row>

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
							<p>Geschlossen</p>
							<p>Schlüssel ID</p>
							<p>900-1</p>
							<p>Uhrzeit</p>
							<p>10 : 45</p>
							<Button bsStyle="warning" bsSize="large">Jetzt testen</Button>
						</div>
					</Col>
				</Row>
			</Grid>
       </div>
    );
  }
}
