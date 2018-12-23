import React from 'react';
import { Row } from 'react-bootstrap';

import MediaQuery from 'react-responsive';

import { SvgMask } from './Constants';


const Door = ({ doorId, isOpen, transition }) => {
	
	var imgUrl = '/images/doors/' + doorId;

	return (
		<Row className="door">
			<MediaQuery minWidth={1430}>
				<SvgMask size="xl" imgUrl={imgUrl} isOpen={isOpen} transition={transition} />
			</MediaQuery>
			<MediaQuery minWidth={1200} maxWidth={1429}>
				<SvgMask size="l" imgUrl={imgUrl} isOpen={isOpen} transition={transition}/>
			</MediaQuery>
			<MediaQuery minWidth={992} maxWidth={1199}>
				<SvgMask size="m" imgUrl={imgUrl} isOpen={isOpen} transition={transition} />
			</MediaQuery>
			<MediaQuery maxWidth={991}>
				<SvgMask size="s" imgUrl={imgUrl} isOpen={isOpen} transition={transition}/>
			</MediaQuery>
		</Row>

	);
};

export default Door;
