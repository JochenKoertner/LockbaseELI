import React from 'react';
import { connect } from 'react-redux';

const Home = props => (
  <div>
    <h1>Hello, world!</h1>
    <p>Welcome to your new single-page application</p>
  </div>
);

export default connect()(Home);
