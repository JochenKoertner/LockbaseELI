import React, { Component } from 'react';
import { Col, Row } from 'react-bootstrap';

/*
export class Layout extends Component {
  displayName = Layout.name

  render() {
    return (
		<div>
			<header>
			  	<Row className="logos">
				  	<Col lg={2}>
						<img src="images/lockbase.png" alt="Lockbase"></img>
					</Col>
					<Col lg={7}>&nbsp;</Col>
					<Col lg={2}>
						<img src="images/km.png" width="240" height="66" alt="Logo"></img>
					</Col>
				</Row>
			</header>

			{this.props.children}
	  </div>
    );
  }
} */

const Layout = (props) => (
	<div>
			<header>
				<Row className="logos">
					<Col lg={2}>
						<img src="images/lockbase.png" alt="Lockbase"></img>
					</Col>
					<Col lg={7}>&nbsp;</Col>
					<Col lg={2}>
						<img src="images/km.png" width="240" height="66" alt="Logo"></img>
					</Col>
				</Row>
			</header>
			{props.children}
	</div>
); 

export default Layout;
