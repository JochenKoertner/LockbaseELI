import * as PropTypes from 'prop-types';
import * as React from 'react';
import Tooltip from '@material-ui/core/Tooltip';

import { findLabel } from './../services/BackendAdapter';

// https://hackernoon.com/react-js-a-better-introduction-to-the-most-powerful-ui-library-ever-created-ecd96e8f4621

// http://www.petercollingridge.co.uk/tutorials/svg/interactive/mouseover-effects/

interface BaseProps {
	id: string,
	onClick: (e: React.MouseEvent<SVGElement>) => void,
	getClassName: (prefix: string, roomId: string) => string
}

interface RoomProps extends BaseProps {
	x: number,
	y: number,
	width: number,
	height: number
}

interface PathRoomProps extends BaseProps {
	d: string
}

const Room: React.SFC<RoomProps> = (props) => {
	return (
		<Tooltip className='room-tooltip' title={findLabel(props.id)}>
			<rect id={props.id} className={props.getClassName('room',props.id)} 
			onClick={props.onClick} x={props.x} y={props.y} width={props.width} height={props.height} />
		</Tooltip>
	);
};

const PathRoom: React.SFC<PathRoomProps> = (props) => {
	return (
		<Tooltip className='room-tooltip' title={findLabel(props.id)}>
			<path id={props.id} className={props.getClassName('room',props.id)} 
			onClick={props.onClick} d={props.d} />
		</Tooltip>
	);
};

const Property: React.SFC<RoomProps> = (props) => {
	return (
		<Tooltip className='room-tooltip' title={findLabel(props.id)}>
			<rect id={props.id} className={props.getClassName('property',props.id)} 
			onClick={props.onClick} x={props.x} y={props.y} width={props.width} height={props.height} />
		</Tooltip>
	);
};

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

	private getToolTipTitle = (roomId: string ) => {
		return 'ToolTip for ' + roomId;
	}

	public render() {

		return (
			
				<svg width="100%" height="100%" version="1.1" viewBox="25 0 1083 687">
					<g id="property">
						<Property id='W1' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={28} y={14} width={1077} height={666} />
						<line className={this.getClassName('roomdoor', 'W1')} x1="680" y1="680" x2="453" y2="680"  />
						<text x="490" y="650">Tor West</text>
					</g>
					<g id="production">
						<text x="710" y="50">Production</text>
						
						<Room id='200' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={567} y={71} width={170} height={156} />
						<Room id='201' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={567} y={227} width={170} height={142} />
						<Room id='202' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={567} y={369} width={170} height={156} />
						<Room id='204' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={737} y={369} width={326} height={156} />
						<Room id='205' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={737} y={71} width={326} height={156} />
						
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
						<text x="170" y="50">Verwaltung</text>
						<Room id='Z1' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={213} y={71} width={99} height={482} />
						<Room id='100' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={85} y={270} width={128} height={340}  />
						<Room id='101' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={312} y={255} width={156} height={156} />
						<PathRoom id='102' onClick={this.handleRoomSelect} getClassName={this.getClassName} d="M312 496 L468 496 L468 595 L312 609 Z" />
						<Room id='103' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={312} y={411} width={156} height={85} />
						<Room id='104' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={312} y={71} width={156} height={184} />
						<Room id='105' onClick={this.handleRoomSelect} getClassName={this.getClassName} x={85} y={71} width={128} height={198} />

						<g>
							<line className={this.getClassName('roomdoor', 'Z1')} x1="230" y1="553" x2="290" y2="553" />
							<line className='roomdoor center' x1="256" y1="553" x2="264" y2="553"/>
						</g>
						<g>
							<line className={this.getClassName('roomdoor', '100')} x1="213" y1="310" x2="213" y2="370" />
							<line className='roomdoor center' x1="213" y1="336" x2="213" y2="344" />
						</g>
						<g>
							<line className={this.getClassName('roomdoor', '101')} x1="312" y1="310" x2="312" y2="370" />
							<line className='roomdoor center' x1="312" y1="336" x2="312" y2="344" />
						</g>
						<line x1="312" y1="509" x2="312" y2="537" className={this.getClassName('roomdoor', '102')} />
						<g>
							<line className={this.getClassName('roomdoor', '105')} x1="213" y1="110" x2="213" y2="170" />
							<line className='roomdoor center' x1="213" y1="136" x2="213" y2="144" />
						</g>
						<g>
							<line className={this.getClassName('roomdoor', '104')} x1="312" y1="110" x2="312" y2="170" />
							<line className='roomdoor center' x1="312" y1="136" x2="312" y2="144" />
						</g>
						<g>
							<line className={this.getClassName('roomdoor', '103')} x1="312" y1="420" x2="312" y2="480" />
							<line className='roomdoor center' x1="312" y1="446" x2="312" y2="454"  />
						</g>
					</g>
				</svg>
		)
	};
};

export default GroundPlan;
