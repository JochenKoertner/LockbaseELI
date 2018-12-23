// fetch all the data from the backend or mock it 
import React from 'react';

import messages_de from './../translations/de.json';
import messages_en from './../translations/en.json';
import messages_es from './../translations/es.json';
import messages_fr from './../translations/fr.json';
import messages_it from './../translations/it.json';

// Languages 

export const languages = [
	{ value: 'de', label: 'Deutsch', messages: messages_de },
	{ value: 'en', label: 'English', messages: messages_en },
	{ value: 'fr', label: 'Français', messages: messages_fr  },
	{ value: 'it', label: 'Italiano', messages: messages_it  },
	{ value: 'es', label: 'Español', messages: messages_es }
]

const defaultLanguage = 'de';

export function findLanguage (language) {
	const index = languages.findIndex(x => x.value === language);
	return languages[index];
};

export const LanguageContext = React.createContext(
	{
		language: findLanguage(defaultLanguage),
		switchLanguage: (selected) => { },
	}
);

// persons and departments 

export const persons = [
	{ value: '900-1', label: 'Ahrens, Andrea', department: 'Geschäftsführung', color: 'OliveDrab' },
	{ value: '901-1', label: 'Barthauer, Thomas', department: 'Geschäftsführung', color: 'SandyBrown'  },
	{ value: '103-1', label: 'Fendler, Klaus', department: 'Buchhaltung' },
	{ value: '104-1', label: 'Kistler, Sabine', department: 'Vertrieb' },
	{ value: '105-1', label: 'Kohl, Ulrich', department: 'Vertrieb' },
	{ value: '200-1', label: 'Leinkamp, Sebastian', department: 'Lager' },
	{ value: '201-1', label: 'Mertens, Martina', department: 'Lager' },
	{ value: '202-1', label: 'Sidow, Janin', department: 'Montage' },
	{ value: '203-1', label: 'Walter, Jens', department: 'Montage' },
	{ value: '203-2', label: 'Winter, Sina', department: 'Montage' },
	{ value: '203-3', label: 'Wondraschek, Volker', department: 'Montage' }
]

// doors and locks 

export const doors = [
	{ value: 'W1', label: 'Tor West', building: '-.-', image: 'tor_west', color: 'LightSlateGray' },
	{
		type: 'group', name: 'Verwaltung', items: [
			{ value: '100', label: 'Konferenzraum', building: 'Verwaltung', image: 'konferenzraum' },
			{ value: '101', label: 'Büro Ahrens', building: 'Verwaltung', image: 'buero_ahrens' },
			{ value: '102', label: 'Büro Barthauer', building: 'Verwaltung', image: 'buero_barthauer' },
			{ value: '103', label: 'Buchhaltung', building: 'Verwaltung', image: 'buchhaltung', color: 'Yellow'   },
			{ value: '104', label: 'Büro Vertrieb 1', building: 'Verwaltung', image: 'buero_vertrieb_1', color: 'Yellow'   },
			{ value: '105', label: 'Büro Vertrieb 2', building: 'Verwaltung', image: 'buero_vertrieb_2', color: 'Yellow'   },
			{ value: 'Z1', label: 'Eingang West', building: 'Verwaltung', image: 'eingang_west' }
		]
	},
	{
		type: 'group', name: 'Produktion', items: [
			{ value: '204', label: 'Werkhalle West', building: 'Produktion', image: 'werkhalle_west', color: 'DeepSkyBlue'  },
			{ value: '200', label: 'Metalllager', building: 'Produktion', image: 'metalllager', color: 'DeepSkyBlue'  },
			{ value: '202', label: 'Büro Montage', building: 'Produktion', image: 'buero_montage', color: 'DeepSkyBlue'  },
			{ value: '201', label: 'Warenlager', building: 'Produktion', image: 'warenlager', color: 'DeepSkyBlue'  },
			{ value: '205', label: 'Werkhalle Süd', building: 'Produktion', image: 'werkhalle_sued', color: 'DeepSkyBlue'  }
		]
	}
]


// Time functions 

function* hoursGenerator() {
	var index = 0;
	while (index < 24) {
		yield index++;
	}
}

function* minutesGenerator() {
	var index = 0;
	while (index < 4) {
		yield index * 15;
		index++;
	}
}

function* yearsGenerator() {
	let year = 2018;
	var index = year - 4;
	while (index < year + 4) {
		yield index++;
	}
}

export let hours = Array
	.from(hoursGenerator())
	.map(x => {
		return {
			value: x,
			label: x.toString().padStart(2, '0')
		};
	})

export let years = Array
	.from(yearsGenerator())
	.map(x => {
		return {
			value: x,
			label: x.toString()
		};
	})

export let minutes = Array
	.from(minutesGenerator())
	.map(x => {
		return {
			value: x,
			label: x.toString().padStart(2, '0')
		};
	})

export const findLabel = (roomId) => {
		const onlyDoors = doors
			.map(x => {
				if (x.type === 'group') {
					return x.items
				}
				return [x]
			})
			.reduce((a, b) => a.concat(b), []);
	
		const index = onlyDoors.findIndex(x => x.value === roomId);
	
		return onlyDoors[index].label;
	};
