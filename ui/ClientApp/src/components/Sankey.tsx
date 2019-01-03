import * as PropTypes from 'prop-types';
import * as React from 'react';

export interface SankeyProps {

}

export class Sankey extends React.PureComponent<SankeyProps> {
	public static propTypes: any = {
	};

	public static defaultProps = {
	};

	public render() {
		return (
			<svg width="100%" height="100%" version="1.1" viewBox="25 0 1083 687">
				<text x={100} y={100} >Sankey Diagramm</text>
			</svg>
		)
	};
};

export default Sankey;
