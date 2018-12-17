import React, { Component } from 'react';
import { Route } from 'react-router';
import { IntlProvider, addLocaleData } from 'react-intl';

import de from 'react-intl/locale-data/de';
import en from 'react-intl/locale-data/en';
import es from 'react-intl/locale-data/es';
import fr from 'react-intl/locale-data/fr';
import it from 'react-intl/locale-data/it';

import Layout from './components/Layout';
import Home from './components/Home';

import { i18nConfig, LanguageContext } from './services/BackendAdapter';

addLocaleData([...de, ...en, ...es, ...fr, ...it]);

class App extends Component {
	displayName = App.name

	constructor(props) {
		super(props);
		this.state = {
			language: i18nConfig,
		};
	}

	render() {
		return (
			<IntlProvider key={this.state.language.locale} locale={this.state.language.locale} messages={this.state.language.messages}>
				<Layout>
					<Route exact path='/' component={Home} />
				</Layout>
			</IntlProvider>
		);
	}
}

export default App;
