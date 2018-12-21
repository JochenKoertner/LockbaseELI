import React from 'react';
import { Col } from 'react-bootstrap';

// https://hackernoon.com/react-js-a-better-introduction-to-the-most-powerful-ui-library-ever-created-ecd96e8f4621

// http://www.petercollingridge.co.uk/tutorials/svg/interactive/mouseover-effects/

const GroundPlan = ({ selectedRoom, onClick }) => {

	const roomStyle = {
		stroke: 'black',
		strokeOpacity: 1,
		strokeWidth: 8,
		strokeLinecap: 'butt',
		strokeLinejoin: 'miter',
		strokeDasharray: 'none',
	}

	return (
	<Col xs={4} className="col-content-center" >
		<svg
			width="100%" height="100%" version="1.1" viewBox="138.7323 110.3858 629.6221 289.4646" 
			xmlns="http://www.w3.org/2000/svg"
			xlink="http://www.w3.org/1999/xlink"
		>
  <g id="layer1" style={roomStyle}  >
    <rect id='100' className={'room ' + ((selectedRoom === '100') ? 'on' : 'off') } onClick={onClick} x="141.7323" y="113.3858" width="113.3858" height="170.0787" />
    <rect id='101' className={'room ' + ((selectedRoom === '101') ? 'on' : 'off') } onClick={onClick} x="255.1181" y="113.3858" width="113.3858" height="170.0787" />
    <rect id='102' className={'room ' + ((selectedRoom === '102') ? 'on' : 'off') } onClick={onClick} x="368.5039" y="113.3858" width="113.3858" height="170.0787" />
    <rect id='103' className={'room ' + ((selectedRoom === '103') ? 'on' : 'off') } onClick={onClick}x="141.7323" y="283.4646" width="340.1575" height="113.3858" />
    <ellipse id='104' className={'room ' + ((selectedRoom === '104') ? 'on' : 'off') } onClick={onClick} cx="694.4882" cy="226.7717" rx="70.86614" ry="70.86614"  />
  </g>		
  </svg>
	</Col>
);
	};

export default GroundPlan;
