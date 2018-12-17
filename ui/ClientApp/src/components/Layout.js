import React from 'react';
import { Col, Row } from 'react-bootstrap';
import Dropdown from 'react-dropdown';

import { arrowClosed, arrowOpen } from './Constants';

import { languages, LanguageContext, findLanguage } from './../services/BackendAdapter';

const Layout = (props) => (
	<div>
		<header>
			<Row className="logos">
				<Col xs={2}>
					<img src="images/lockbase.png" alt="Lockbase" width="85%"></img>
				</Col>
				<Col xs={8}>

				</Col>
				<Col xs={2}>
					<img src="images/km.png" width="80%" alt="Logo"></img>
					<LanguageContext.Consumer>
						{({ language, switchLanguage }) => (
							<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen} onChange={switchLanguage}
								value={findLanguage(language.value)} options={languages} />
					)}
					</LanguageContext.Consumer>
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
