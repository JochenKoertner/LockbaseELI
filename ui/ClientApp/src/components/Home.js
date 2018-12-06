import React, { Component } from 'react';
import { Col, Grid, Row, Button } from 'react-bootstrap';


export class Home extends Component {
  displayName = Home.name

  render() {
    return (
      <div>
		  	<Grid>
			  	<Row className="grid-image">
				  	<Col lg={2}>
						<img src="images/lockbase.png" alt="Lockbase"></img>
					</Col>
					<Col lg={7}></Col>
					<Col lg={2}>
						<img src="images/km.png" width="240" height="66" alt="Logo"></img>
					</Col>
				</Row>

				<Row className="grid-header">
					<Col lg={4}>
						&nbsp;
					</Col>
					<Col lg={4} className="col-header-center" >
						Büro Barthauser
					</Col>
					<Col lg={4}>
						&nbsp;
					</Col>
				</Row>

				<Row className="grid-content">
					<Col lg={4}>
						<div class="container-main">
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
						<div class="container-main">&nbsp;
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
