import React from 'react';
import { Label } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const InfoBox = (props) => {

	let icon;
	if (props.icon !== undefined) {
		icon = <FontAwesomeIcon icon={props.icon}/>
	};

	return (<div>
		<Label>{props.label}</Label>
		<p className="info-box">
			{props.children}
			{icon}
		</p>
	</div>)
};

export default InfoBox;
