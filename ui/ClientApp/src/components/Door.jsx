import React, { Component } from 'react';
import { Row } from 'react-bootstrap';

export class Door extends React.Component {
	displayName = Door.name

	render() {
		let doorState = this.props.isOpen ? 'open' : 'close'

		var imgUrl = '/images/doors/' + this.props.doorId + '_' + doorState + '.png'

		return (
			<Row className="door" style={{backgroundImage: `url(${imgUrl})`}}>
			</Row>
		);
	}
}
