import React, { Component } from 'react';

export const arrowClosed = (
	<span className="arrow-closed" />
)
export const arrowOpen = (
	<span className="arrow-open" />
)

// Constructs a Svg Mask depending of media query size 

export const SvgMask = ({ size, imgUrl }) => {
	var width = 1400;
	var height = 700;

	var sign_x = 230;
	var sign_y = 205; 
	var sign_width = 200; 
	var sign_height = 170;

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
			
			<image width={width} height={height} xlinkHref={imgUrl+"_" + size + ".png"} mask="url(#myMask)" />
			<rect x={sign_x} y={sign_y} width={sign_width} height={sign_height} opacity="1" stroke="#FFFFFF" strokeOpacity="1" 
			strokeWidth="4" strokeLinecap="butt" strokeLinejoin="miter" 
			strokeDasharray="none" fill="#000000" fillOpacity=".3" />
    
		</svg>
	);
};





