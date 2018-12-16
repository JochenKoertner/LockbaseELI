import React from 'react';
import { Row } from 'react-bootstrap';

import MediaQuery from 'react-responsive';

import { SvgMask } from './Constants';

// <Row className="door" style={{backgroundImage: `url(${imgUrl})`}}>
// </Row> 

const Door = ({ doorId, isOpen }) => {
	let doorState = isOpen ? 'open' : 'close'

	var imgUrl = '/images/doors/' + doorId + '_' + doorState;

	return (
		<div>
			<MediaQuery minWidth={1430}>
				<SvgMask size="xl" imgUrl={imgUrl} />
			</MediaQuery>
			<MediaQuery minWidth={1200} maxWidth={1429}>
				<SvgMask size="l" imgUrl={imgUrl} />
			</MediaQuery>
			<MediaQuery minWidth={992} maxWidth={1199}>
				<SvgMask size="m" imgUrl={imgUrl} />
			</MediaQuery>
			<MediaQuery maxWidth={991}>
				<SvgMask size="s" imgUrl={imgUrl} />
			</MediaQuery>
		</div>

	);
};

export default Door;
