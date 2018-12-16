// fetch all the data from the backend or mock it 

// Languages 

export const languages = [
	{ value: 'DE', label: 'Deutsch' },
	{ value: 'EN', label: 'English' },
	{ value: 'FR', label: 'Français' },
	{ value: 'IT', label: 'Italiano' },
	{ value: 'ES', label: 'Español' }
]

// persons and departments 

export const persons = [
	{
		value: 4711, label: 'Ahrens, Andrea', department: 'Geschäftsführung',
		summary: 'keine zeitliche Einschränkung', keyId: '900-1'
	},
	{
		value: 4712, label: 'Barthauer, Thomas', department: 'Geschäftsführung',
		summary: 'keine zeitliche Einschränkung', keyId: '901-1'
	},
	{
		value: 4713, label: 'Fendler, Klaus', department: 'Buchhaltung',
		summary: 'Jeden Tag 6-21 Uhr', keyId: '103-1'
	}
]

// doors and locks 

export const doors = [
	{ value: 'W1', label: 'Tor West', image: 'torwest'},
	{
		type: 'group', name: 'Verwaltung', items: [
			{ value: '100', label: 'Konferenzraum', image: 'konferenzraum' },
			{ value: '101', label: 'Büro Ahrens', image: 'buero_ahrens'},
			{ value: '102', label: 'Büro Barthauer', image: 'buero_barthauer' },
			{ value: '103', label: 'Buchhaltung', image: 'buchhaltung' },
			{ value: '104', label: 'Büro Vertrieb 1', image: 'buero_vertrieb1' },
			{ value: '105', label: 'Büro Vertrieb 2', image: 'buero_vertrieb2' },
			{ value: 'Z1', label: 'Eingang West', image: 'eingang_west' }
		]
	},
	{
		type: 'group', name: 'Produktion', items: [
			{ value: '204', label: 'Werkhalle West', image: 'werkhalle_west' },
			{ value: '200', label: 'Metalllager', image: 'metalllager' },
			{ value: '202', label: 'Büro Montage', image: 'buero_montage' },
			{ value: '201', label: 'Warenlager', image: 'warenlager' },
			{ value: '205', label: 'Werkhalle Süd', image: 'werkhalle_sued' }
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

export let hours = Array
	.from(hoursGenerator())
	.map(x => {
		return {
			value: x,
			label: x.toString().padStart(2, '0')
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
