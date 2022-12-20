import { Providers } from '@microsoft/mgt-element';
import { Msal2Provider } from '@microsoft/mgt-msal2-provider';
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

Providers.globalProvider = new Msal2Provider({
  clientId: '',
  scopes: ['Mail.ReadBasic', 'Mail.Read', 'calendars.read', 'user.read', 'openid', 'profile', 'people.read', 'user.read.all', 'files.read']
})



const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

