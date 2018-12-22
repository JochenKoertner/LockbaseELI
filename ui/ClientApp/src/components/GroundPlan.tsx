import * as PropTypes from 'prop-types';
import * as React from 'react';

// https://hackernoon.com/react-js-a-better-introduction-to-the-most-powerful-ui-library-ever-created-ecd96e8f4621

// http://www.petercollingridge.co.uk/tutorials/svg/interactive/mouseover-effects/

export interface GroundPlanProps {
	selectedRoom: string;
	onClick: (selected: string) => void;
}
export class GroundPlan extends React.PureComponent<GroundPlanProps> {
	public static propTypes: any = {
		selectedRoom: PropTypes.string.isRequired,
		onClick: PropTypes.func.isRequired
	};

	public static defaultProps = {
	};

	public state = {
		room: this.props.selectedRoom,
	};

	public handleRoomSelect = (e: React.MouseEvent<SVGElement>) => {
		this.props.onClick(e.currentTarget.id);
		this.setState({ room: e.currentTarget.id });
	};

	private getClassName = (roomId: string) => {
		return 'room ' + ((this.state.room === roomId) ? 'on' : '');
	}

	public render() {
		/*const roomStyle = {
			stroke: 'black',
			strokeOpacity: 1,
			strokeWidth: 8,
			strokeLinecap: 'butt',
			strokeLinejoin: 'miter',
			strokeDasharray: 'none',
		}
	
		const textStyle = {
			fontFamily: 'Berlin',
			fontSize: '2.5em',
			stroke: '#dcdcdc',
			fill: 'black'
		}
	*/

		return (
			<svg width="100%" height="100%" version="1.1" viewBox="138.7323 110.3858 629.6221 289.4646">
				<g>
					<rect id='100' className={this.getClassName('100')} onClick={this.handleRoomSelect} x="141.7323" y="113.3858" width="113.3858" height="170.0787" />
					<rect id='101' className={this.getClassName('101')} onClick={this.handleRoomSelect} x="255.1181" y="113.3858" width="113.3858" height="170.0787" />
					<rect id='102' className={this.getClassName('102')} onClick={this.handleRoomSelect} x="368.5039" y="113.3858" width="113.3858" height="170.0787" />
					<rect id='103' className={this.getClassName('103')} onClick={this.handleRoomSelect} x="141.7323" y="283.4646" width="340.1575" height="113.3858" />
					<ellipse id='104' className={this.getClassName('104')} onClick={this.handleRoomSelect} cx="694.4882" cy="226.7717" rx="70.86614" ry="70.86614"  />
				</g>
				<text x="141" y="253.3858">Verwaltung</text>
				<text x="141" y="313.3858">Produktion</text>
			</svg>
		)};
};

export default GroundPlan;
