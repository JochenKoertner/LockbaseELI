import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';
import Dropdown from 'react-dropdown';

import {arrowClosed, arrowOpen} from './Constants';

import {languages} from './../services/BackendAdapter';

const Layout = (props) => (
	<div>
		<header>
			<Row className="logos">
				<Col sm={2}>
					<img src="images/lockbase.png" alt="Lockbase" width="85%"></img>
				</Col>
				<Col sm={8}>
				
				</Col>
				<Col sm={2}>
					<img src="images/km.png" width="80%" alt="Logo"></img>
					<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
									options={languages} />
							
				</Col>
			</Row>
		</header>
		<main>
			{props.children}
		</main>
		<footer>
		</footer>
	</div>
);

export default Layout;
