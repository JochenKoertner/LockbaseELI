import React, { Component } from 'react';
import { Route } from 'react-router';
import { IntlProvider } from 'react-intl';


import { library } from '@fortawesome/fontawesome-svg-core';
import { faSquare, faLockOpen, faLock, faCalendarAlt, faClock } from '@fortawesome/free-solid-svg-icons';

import { MuiThemeProvider, createMuiTheme } from '@material-ui/core/styles';

import Layout from './components/Layout';
import Home from './components/Home';

import { LanguageContext, findLanguage } from './services/BackendAdapter';

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
		const THEME = createMuiTheme({
			typography: {
				useNextVariants: true, 
				fontFamily: 'Berlin, sans-serif',
				fontSize: 28,
				fontWeightLight: 300,
				fontWeightRegular: 400,
				fontWeightMedium: 500
			},
			palette: {
				primary: {
				  main: '#f59c00',
				  contrastText: '#fff',
				},
				secondary: {
				  main: '#565656',
				  contrastText: '#000',
				},
			  }
		 });

		return (
			<MuiThemeProvider theme={THEME}>
				<LanguageContext.Provider value={this.state}>
					<IntlProvider key={this.state.language.value} locale={this.state.language.value} messages={this.state.language.messages}>
						<Layout>
							<Route exact path='/' component={Home} />
						</Layout>
					</IntlProvider>
				</LanguageContext.Provider>
			</MuiThemeProvider>
		);
	}
}

App.contextType = LanguageContext;

export default App;
