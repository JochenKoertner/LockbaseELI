import React from 'react';
import { Col } from 'react-bootstrap';

// https://hackernoon.com/react-js-a-better-introduction-to-the-most-powerful-ui-library-ever-created-ecd96e8f4621

const GroundPlan = ({ doorName }) => (
	<Col xs={4} className="col-content-center" >
		<p>Verwaltung</p>
		<p>Produktion</p>
		<svg
			width="100%" height="100%" version="1.1" viewBox="138.7323 110.3858 629.6221 289.4646" 
			xmlns="http://www.w3.org/2000/svg"
			xlink="http://www.w3.org/1999/xlink"
		>
  <g id="layer1">
    <rect x="141.7323" y="113.3858" width="113.3858" height="170.0787" opacity="1" stroke="#000000" strokeOpacity="1" strokeWidth="4" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" fill="#FFFFFF" fillOpacity="1" />
    <rect x="255.1181" y="113.3858" width="113.3858" height="170.0787" opacity="1" stroke="#000000" strokeOpacity="1" strokeWidth="4" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" fill="#FFFFFF" fillOpacity="1" />
    <rect x="368.5039" y="113.3858" width="113.3858" height="170.0787" opacity="1" stroke="#000000" strokeOpacity="1" strokeWidth="4" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" fill="#FFFFFF" fillOpacity="1" />
    <rect x="141.7323" y="283.4646" width="340.1575" height="113.3858" opacity="1" stroke="#000000" strokeOpacity="1" strokeWidth="4" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" fill="#FFFFFF" fillOpacity="1" />
    <ellipse cx="694.4882" cy="226.7717" rx="70.86614" ry="70.86614" opacity="1" stroke="#000000" strokeOpacity="1" strokeWidth="4" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" fill="#FFFFFF" fillOpacity="1" />
  </g>		
  </svg>
	</Col>
);

export default GroundPlan;
