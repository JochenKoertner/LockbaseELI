import React from 'react';

export const arrowClosed = (
	<span className="arrow-closed" />
)
export const arrowOpen = (
	<span className="arrow-open" />
)

// Constructs a Svg Mask depending of media query size 

export const SvgMask = ({ size, imgUrl, isOpen, transition, doorId }) => {
	var width = 1400;
	var height = 700;

	var sign_x = 387;
	var sign_y = 268; 
	var sign_width = 140; 
	var sign_height = 124;

	if (doorId === 'buchhaltung') {
		sign_x = 387;
		sign_y = 268; 
		sign_width = 140; 
		sign_height = 124;
	} else if (doorId === 'eingang_west') {
		sign_x = 260;
		sign_y = 273; 
		sign_width = 140; 
		sign_height = 124;
	}	else if (doorId === 'tor_west') {
		sign_x = 100;
		sign_y = 230; 
		sign_width = 310; 
		sign_height = 250;
		// Perspektive 
		// https://stackoverflow.com/questions/12919398/perspective-transform-of-svg-paths-four-corner-distort
	}

	var factor = 1.0;

	if (size === "l") {
		width = 1170;
		height = 585;
		factor = 0.8571;
	} else if (size === "m") {
		width = 962;
		height = 481;
		factor = 0.7086;
	} else if (size === "s") {
		width = 738;
		height = 369;
		factor = 0.5486;
	}

	sign_x = sign_x * factor;
	sign_y = sign_y * factor;
	sign_height = sign_height * factor;
	sign_width = sign_width * factor;

	// console.log("Transition " + transition );

	let getDoorOpenClassName = () => {
		if (transition === 0) 
			return "door-hidden"
		else if (transition === 1)
			return "door-show"
		else if (transition === 2)
			return "door-hide"
		return "door-visible"
	};

	let getDoorCloseClassName = () => {
		if (transition === 0) 
			return "door-visible"
		else if (transition === 1)
			return "door-hide"
		else if (transition === 2)
			return "door-show"
		return "door-hidden"
	};

	return (
		<svg
			width={width} height={height}
			xmlns="http://www.w3.org/2000/svg"
			xlink="http://www.w3.org/1999/xlink"
		>
			<filter id="filter2">
				<feGaussianBlur stdDeviation="30"></feGaussianBlur>
			</filter>

			<mask id="myMask" x="0" y="0" width={width} height={height}>
				<ellipse cx={width / 2} cy={height / 2} rx={width / 2.1} ry={height / 1.5} fill="white" filter="url(#filter2)" />
			</mask>
			
			<image className={getDoorOpenClassName()} x={0} y={0} width={width} height={height} xlinkHref={imgUrl+"_open_" + size + ".png"} mask="url(#myMask)" />
			<image className={getDoorCloseClassName()} x={0} y={0} width={width} height={height} xlinkHref={imgUrl+"_closed_" + size + ".png"} mask="url(#myMask)" />
			<rect x={sign_x} y={sign_y} width={sign_width} height={sign_height} opacity="1" stroke="#FFFFFF" strokeOpacity="1" 
			strokeWidth="0" strokeLinecap="butt" strokeLinejoin="miter" 
			strokeDasharray="none" fill="#000000" fillOpacity=".3" />

			<text x={sign_x+5} y={sign_y+5} width={sign_width-10} height={sign_height-10}>{doorId}</text>
    
		</svg>
	);
};
