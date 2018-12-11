import React, { Component } from 'react';
import { Row, Col } from 'react-bootstrap';

export class DoorCaption extends React.Component {
	displayName = DoorCaption.name

	render() {
		return (
			<Row className="door-caption">
				<Col lg={4} />

				<Col lg={4} >
					{this.props.doorName}
				</Col>
				<Col lg={4} />
			</Row>
		);
	}
}
