import React from 'react';
import { Col } from 'react-bootstrap';

// https://hackernoon.com/react-js-a-better-introduction-to-the-most-powerful-ui-library-ever-created-ecd96e8f4621

const GroundPlan = ({ doorName }) => (
	<Col xs={4} className="col-content-center" >
		<p>Verwaltung</p>
		<p>Produktion</p>
	</Col>
);

export default GroundPlan;
