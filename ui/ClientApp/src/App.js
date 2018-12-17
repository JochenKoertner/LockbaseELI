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

import { LanguageContext, i8n_de, i8n_en } from './services/BackendAdapter';

addLocaleData([...de, ...en, ...es, ...fr, ...it]);

class App extends Component {
	displayName = App.name

	constructor(props) {
		super(props);

		this.switchLanguage = (selected) => {
			console.log("language : " + selected.value);
			this.setState(state => ({
			  language:
				state.language === i8n_de
				  ? i8n_en
				  : i8n_de,
			}));
		  };

		this.state = {
			language: i8n_de,
			switchLanguage: this.switchLanguage,
		};
	}

	render() {
		let language = this.state.language;
		return (
			<LanguageContext.Provider value={this.state}>
				<IntlProvider key={language.locale} locale={language.locale} messages={language.messages}>
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
