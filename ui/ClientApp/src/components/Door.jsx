import React from 'react';
import Row from 'react-bootstrap';

const Door = ({ doorId, isOpen }) => {
	let doorState = isOpen ? 'open' : 'close'

	var imgUrl = '/images/doors/' + doorId + '_' + doorState + '.png'

	return (
		<Row className="door" style={{backgroundImage: `url(${imgUrl})`}}>
		</Row>
	);
}; 

export default Door;
