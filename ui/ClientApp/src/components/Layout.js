import React, { Component } from 'react';

export class Layout extends Component {
  displayName = Layout.name

  render() {
    return (
		<div>
			<header>
			</header>
			
			{this.props.children}
			
			<footer>
				Copyright (c) by km 2018
			</footer>
	  </div>
    );
  }
}
