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

	public handleRoomSelect = (e: React.MouseEvent<SVGElement>) => {
		this.props.onClick(e.currentTarget.id);
	};

	private getClassName = (prefix: string, roomId: string) => {
		return prefix + ' ' + ((this.props.selectedRoom === roomId) ? 'on' : 'off');
	}

	public render() {

		return (
			<svg width="100%" height="100%" version="1.1" viewBox="25 0 1083 687">
				<g id="property">
					<rect id='W1' className={this.getClassName('property', 'W1')} onClick={this.handleRoomSelect} x="28" y="14" width="1077" height="666" />
					<line className={this.getClassName('roomdoor', 'W1')} x1="680" y1="680" x2="453" y2="680"  />
					<text x="501" y="645">Tor West</text>
				</g>
				<g id="production">
					<text x="733" y="45">Production</text>
					<rect id='200' className={this.getClassName('room','200')} onClick={this.handleRoomSelect} x="567" y="71" width="170" height="156" />
					<rect id='201' className={this.getClassName('room','201')} onClick={this.handleRoomSelect} x="567" y="227" width="170" height="142" />
					<rect id='202' className={this.getClassName('room','202')} onClick={this.handleRoomSelect} x="567" y="369" width="170" height="156" />
					<rect id='204' className={this.getClassName('room','204')} onClick={this.handleRoomSelect} x="737" y="369" width="326" height="156" />
					<rect id='205' className={this.getClassName('room','205')} onClick={this.handleRoomSelect} x="737" y="71" width="326" height="156" />
					<g>
						<line className={this.getClassName('roomdoor', '200')} x1="567" y1="110" x2="567" y2="170" />
						<line className='roomdoor center' x1="567" y1="136" x2="567" y2="144" />
					</g>
					<g>
						<line className={this.getClassName('roomdoor', '201')} x1="567" y1="270" x2="567" y2="330" />
						<line className='roomdoor center' x1="567" y1="296" x2="567" y2="304" />
					</g>
					<g>
						<line className={this.getClassName('roomdoor', '202')} x1="567" y1="420" x2="567" y2="480" />
						<line className='roomdoor center' x1="567" y1="446" x2="567" y2="454" />
					</g>
					<g>
						<line className={this.getClassName('roomdoor', '205')} x1="1063" y1="99" x2="1063" y2="199" />
						<line className='roomdoor center' x1="1063" y1="145" x2="1063" y2="153" />
					</g>
					<line className={this.getClassName('roomdoor', '204')} x1="1063" y1="397" x2="1063" y2="496" />
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
						<line x1="213" y1="99" x2="213" y2="156" opacity="1" stroke="#E6E6E5" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
						<line x1="213" y1="122" x2="213" y2="125" opacity="1" stroke="#999998" strokeOpacity="1" strokeWidth="10" strokeLinecap="butt" strokeLinejoin="miter" strokeDasharray="none" />
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
					<line x1="312" y1="509" x2="312" y2="537" className={this.getClassName('roomdoor', '102')} />
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
