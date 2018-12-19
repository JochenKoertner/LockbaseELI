import React, { Component } from 'react';
import { Col, Grid, Row, Button, Label } from 'react-bootstrap';
import Dropdown from 'react-dropdown';
import { injectIntl } from 'react-intl';

// https://codepen.io/ecurbelo/pen/GKjAx


import Door from './Door';
import DoorCaption from './DoorCaption';
import GroundPlan from './GroundPlan';
import InfoBox from './InfoBox';
import ColorInfoBox from './ColorInfoBox';

import { arrowClosed, arrowOpen } from './Constants';
import { persons, doors, minutes, hours } from './../services/BackendAdapter';

import { messages } from './../translations/messages';


class Home extends Component {
	displayName = Home.name

	constructor(props) {
		super(props);
		this.state = {
			isOpen: false,
			person: persons[0],
			door: doors[1].items[2],
			hour: hours[10],
			minute: minutes[3]
		};

		this.toggleDoor = this.toggleDoor.bind(this);
		this.onSelectDoor = this.onSelectDoor.bind(this);
		this.onSelectPerson = this.onSelectPerson.bind(this);
		this.onSelectHour = this.onSelectHour.bind(this);
		this.onSelectMinute = this.onSelectMinute.bind(this);
	}

	toggleDoor() {
		this.setState({
			isOpen: !this.state.isOpen
		});
	}

	onSelectDoor(selected) {
		const onlyDoors = doors
			.map(x => {
				if (x.type === 'group') {
					return x.items
				}
				return [x]
			})
			.reduce((a, b) => a.concat(b), []);

		const index = onlyDoors.findIndex(x => x.value === selected.value);
		this.setState({ door: onlyDoors[index] });
	}

	onSelectPerson(selected) {
		const index = persons.findIndex(x => x.value === selected.value);
		this.setState({ person: persons[index] });
	}

	onSelectHour(selected) {
		const index = hours.findIndex(x => x.value === selected.value);
		this.setState({ hour: hours[index] });
	}

	onSelectMinute(selected) {
		const index = minutes.findIndex(x => x.value === selected.value);
		this.setState({ minute: minutes[index] });
	}

	render() {

		return (

			<Grid>
				<Door doorId={this.state.door.image} isOpen={this.state.isOpen}></Door>

				<DoorCaption doorName={this.state.door.label}></DoorCaption>

				<Row className="grid-content">
					<Col xs={4} className="col-content-aside col-content-left">

						<Label>
							{this.props.intl.formatMessage(messages.homeLabelPerson)}
						</Label>
						<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
							options={persons} onChange={this.onSelectPerson} value={this.state.person} />

						<InfoBox label={this.props.intl.formatMessage(messages.homeLabelDepartment)}>
							{this.state.person.department}
						</InfoBox>

						<ColorInfoBox label={this.props.intl.formatMessage(messages.homeLabelKeyId)}
							color={this.state.person.color}>
							{this.state.person.value}
						</ColorInfoBox>

						<Label>{this.props.intl.formatMessage(messages.homeLabelDoor)}</Label>
						<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
							options={doors} onChange={this.onSelectDoor} value={this.state.door} />

						<InfoBox label={this.props.intl.formatMessage(messages.homeLabelBuilding)}>
							{this.state.door.building}
						</InfoBox>

						<ColorInfoBox label={this.props.intl.formatMessage(messages.homeLabelLockId)}
							color={this.state.door.color} >
							{this.state.door.value}
						</ColorInfoBox>
					</Col>
					
					<GroundPlan/>

					<Col xs={4} className="col-content-aside col-content-right">

						<InfoBox label={this.props.intl.formatMessage(messages.homeLabelDoorState)}
							icon={this.state.isOpen ? "lock-open" : "lock"} >
							{this.state.isOpen ? 
								this.props.intl.formatMessage(messages.homeDoorOpenState): 
								this.props.intl.formatMessage(messages.homeDoorCloseState)}
						</InfoBox>

						<Label>{this.props.intl.formatMessage(messages.homeLabelTime)}</Label>
						<Row>
							<Col xs={6}>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
									options={hours} onChange={this.onSelectHour} value={this.state.hour} />
							</Col>
							<Col xs={6}>
								<Dropdown arrowClosed={arrowClosed} arrowOpen={arrowOpen}
									options={minutes} onChange={this.onSelectMinute} value={this.state.minute} />
							</Col>
						</Row>
						<div>
							<Button bsStyle="warning" bsSize="large" onClick={this.toggleDoor}>
								{this.props.intl.formatMessage(messages.homeButtonCheck)}
							</Button>
						</div>
					</Col>
				</Row>
			</Grid>
		);
	}
};

export default injectIntl(Home, {withRef:true});

