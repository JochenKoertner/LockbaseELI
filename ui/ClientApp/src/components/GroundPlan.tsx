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

	private getClassName = (prefix: string, roomId: string) => {
		return prefix + ' ' + ((this.state.room === roomId) ? 'on' : 'off');
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
			<svg width="100%" height="100%" version="1.1" viewBox="25 0 1083 687">
				<g id="property">
					<text x="501" y="645">Tor West</text>
					<rect id='W1' className={this.getClassName('property', 'W1')} onClick={this.handleRoomSelect} x="28" y="14" width="1077" height="666" />
					<line x1="680" y1="680" x2="453" y2="680" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
				</g>
				<g id="production">
					<text x="733" y="45">Production</text>
					<rect id='200' className={this.getClassName('room','200')} onClick={this.handleRoomSelect} x="567" y="71" width="170" height="156" />
					<rect id='201' className={this.getClassName('room','201')} onClick={this.handleRoomSelect} x="567" y="227" width="170" height="142" />
					<rect id='202' className={this.getClassName('room','202')} onClick={this.handleRoomSelect} x="567" y="369" width="170" height="156" />
					<rect id='204' className={this.getClassName('room','204')} onClick={this.handleRoomSelect} x="737" y="369" width="326" height="156" />
					<rect id='205' className={this.getClassName('room','205')} onClick={this.handleRoomSelect} x="737" y="71" width="326" height="156" />
					<g>
						<line x1="566.9291" y1="113.3858" x2="566.9291" y2="170.0787" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="566.9291" y1="136.063" x2="566.9291" y2="138.8976" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="566.9291" y1="269.2914" x2="566.9291" y2="325.9843" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="566.9291" y1="291.9685" x2="566.9291" y2="294.8031" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="566.9291" y1="425.1968" x2="566.9291" y2="481.8898" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="566.9291" y1="447.874" x2="566.9291" y2="450.7086" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="1062.992" y1="99.2126" x2="1062.992" y2="198.4252" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="1062.992" y1="138.8976" x2="1062.992" y2="143.8583" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<line x1="1062.992" y1="396.8504" x2="1062.992" y2="496.063" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
				</g>
				<g id="administration">
					<text x="192" y="45">Verwaltung</text>
					<rect id='Z1' className={this.getClassName('room','Z1')} onClick={this.handleRoomSelect} x="213" y="71" width="99" height="482" />
					<rect id='100' className={this.getClassName('room','100')} onClick={this.handleRoomSelect} x="85" y="270" width="128" height="340"  />
					<rect id='101' className={this.getClassName('room','101')} onClick={this.handleRoomSelect} x="312" y="255" width="156" height="156" />
					<rect id='103' className={this.getClassName('room','103')} onClick={this.handleRoomSelect} x="312" y="411" width="156" height="85" />
					<rect id='104' className={this.getClassName('room','104')} onClick={this.handleRoomSelect} x="312" y="71" width="156" height="184" />
					<rect id='105' className={this.getClassName('room','105')} onClick={this.handleRoomSelect} x="85" y="71" width="128" height="198" />
					<path id='102' className={this.getClassName('room','102')} onClick={this.handleRoomSelect} 
						d="M311.811 496.063 L467.7165 496.063 L467.7165 595.2756 L311.811 609.4488 Z " />

					<g>
						<line x1="212.5984" y1="99.2126" x2="212.5984" y2="155.9055" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="212.5984" y1="121.8897" x2="212.5984" y2="124.7244" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="311.8813" y1="99.28291" x2="311.8813" y2="155.9758" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="311.8813" y1="121.9601" x2="311.8813" y2="124.7947" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="212.5984" y1="311.811" x2="212.5984" y2="368.5039" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="212.5984" y1="334.4882" x2="212.5984" y2="337.3228" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="311.811" y1="311.811" x2="311.811" y2="368.5039" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="311.811" y1="334.4882" x2="311.811" y2="337.3228" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<g>
						<line x1="311.811" y1="425.1968" x2="311.811" y2="481.8898" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="311.811" y1="447.874" x2="311.811" y2="450.7086" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
					<line x1="311.7993" y1="508.8788" x2="311.7993" y2="537.2252" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					<g transform="rotate(90 262.2685 552.5645)">
						<line x1="262.2685" y1="519.9661" x2="262.2685" y2="585.1629" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="262.2685" y1="546.0449" x2="262.2685" y2="549.3046" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
					</g>
				</g>
			</svg>
		)
	};
};

export default GroundPlan;
