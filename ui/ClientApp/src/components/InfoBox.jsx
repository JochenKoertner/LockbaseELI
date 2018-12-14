import React from 'react';
import { Label } from 'react-bootstrap';

const InfoBox = (props) => (
	<div>
		<Label>{props.label}</Label>
		<p className="info-box">
			{props.children}
		</p>
	</div>
)

export default InfoBox;

