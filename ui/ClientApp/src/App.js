import React, { Component } from 'react';
import { Route } from 'react-router';
import { IntlProvider, addLocaleData } from 'react-intl';

import de from 'react-intl/locale-data/de';
import en from 'react-intl/locale-data/en';
import es from 'react-intl/locale-data/es';
import fr from 'react-intl/locale-data/fr';
import it from 'react-intl/locale-data/it';

import { library } from '@fortawesome/fontawesome-svg-core';
import { faSquare, faLockOpen, faLock, faCalendarAlt, faClock } from '@fortawesome/free-solid-svg-icons';

import Layout from './components/Layout';
import Home from './components/Home';

import { LanguageContext, findLanguage } from './services/BackendAdapter';

addLocaleData([...de, ...en, ...es, ...fr, ...it]);

library.add(faSquare, faLockOpen, faLock, faCalendarAlt, faClock);

class App extends Component {
	displayName = App.name

	constructor(props) {
		super(props);

		this.switchLanguage = (selected) => {
			this.setState(state => ({
			  language: findLanguage(selected.value)
			}));
		  };

		this.state = {
			language: findLanguage('de'),
			switchLanguage: this.switchLanguage,
		};
	}

	render() {
		return (
			<LanguageContext.Provider value={this.state}>
				<IntlProvider key={this.state.language.value} locale={this.state.language.value} messages={this.state.language.messages}>
					<Layout>
						<Route exact path='/' component={Home} />
					</Layout>
				</IntlProvider>
			</LanguageContext.Provider>
		);
	}
}

App.contextType = LanguageContext;

export default App;
