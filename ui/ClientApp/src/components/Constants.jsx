import React, { Component } from 'react';

export const arrowClosed = (
	<span className="arrow-closed" />
)
export const arrowOpen = (
	<span className="arrow-open" />
)

// Constructs a Svg Mask depending of media query size 

// <ellipse cx={width/2} cy={height/2} rx={width/2} ry={height/1.5} fill="black" filter="url(#filter2)"/>

export const SvgMask = ({ size, imgUrl }) => {
	var width = 1400; 
	var height = 700;
	
	if (size === "l") {
		width = 1170; 
		height = 585;
	} else if (size === "m") {
		width = 962; 
		height = 481;
	} else if (size === "s") {
		width = 738; 
		height = 369;
	}
	 return (
			 <svg
				width={width} height={height} 
					 xmlns="http://www.w3.org/2000/svg"
					 xlink="http://www.w3.org/1999/xlink"
			 >
					 <defs>
						 <filter id="filter2">
					 <feGaussianBlur stdDeviation="20"></feGaussianBlur>
				 </filter>
					 </defs>
			
				<image width={width} height={height} xlinkHref={imgUrl} > */}
				</image>
			 </svg>
	 );
};





