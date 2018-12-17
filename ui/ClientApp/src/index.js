import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap/dist/css/bootstrap-theme.css';
import 'react-dropdown/style.css';
import './index.scss';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

import { IntlProvider, addLocaleData } from 'react-intl';
import de from 'react-intl/locale-data/de';
import en from 'react-intl/locale-data/en';
import es from 'react-intl/locale-data/es';
import fr from 'react-intl/locale-data/fr';
import it from 'react-intl/locale-data/it';

import App from './App';
import registerServiceWorker from './registerServiceWorker';

import messages_de from './translations/de.json';
import messages_en from './translations/en.json';

addLocaleData([...de, ...en, ...es, ...fr, ...it]);


const messages = {
	'de': messages_de,
	'en': messages_en
};

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const language = 'de';

ReactDOM.render(
	<BrowserRouter basename={baseUrl}>
		<IntlProvider locale={language} messages={messages_en}>
			<App />
		</IntlProvider>
	</BrowserRouter>,
	rootElement);

registerServiceWorker();
