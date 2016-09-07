import ReactDOM from 'react-dom';
import React from 'react';
import App from 'components/App';
import $ from 'jquery';

$(() => {
  ReactDOM.render(<App />, $('#app')[0]);
});
