import React from 'react';
import { Label } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const ColorInfoBox = (props) => {

	let icon;
	if (props.color !== undefined) {
		let iconStyle = {
			color: props.color
		};
		icon = <FontAwesomeIcon icon="square" style={iconStyle}/>
	};

	return (<div>
		<Label>{props.label}</Label>
		<p className="info-box">
			{props.children}
			{icon}
		</p>
	</div>)
};

export default ColorInfoBox;
