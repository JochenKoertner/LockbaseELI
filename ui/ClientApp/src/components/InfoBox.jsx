import React from 'react';
import { Label } from 'react-bootstrap';

export class InfoBox extends React.Component {
	displayName = InfoBox.name

	render() {
		return (
			<div>
				<Label>{this.props.label}</Label>
				<p className="info-box">
					{this.props.children}
				</p>
			</div>
		);
	}
}
