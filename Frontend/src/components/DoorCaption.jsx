import React from 'react';
import { Row, Col } from 'react-bootstrap';

// https://hackernoon.com/react-js-a-better-introduction-to-the-most-powerful-ui-library-ever-created-ecd96e8f4621

const DoorCaption = ({ doorName }) => (
	<Row className="door-caption">
		<Col xs={4} />
		<Col xs={4} >
			{doorName}
		</Col>
		<Col xs={4} />
	</Row>
);

export default DoorCaption;
