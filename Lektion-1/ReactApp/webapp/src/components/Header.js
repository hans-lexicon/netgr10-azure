import React, { useState, useEffect } from 'react';
import { Providers, ProviderState } from '@microsoft/mgt-element';
import { Get, Login } from '@microsoft/mgt-react'
import Email from './Email';

const useIsSignedIn = () => {
    const [isSignedIn, setIsSignedIn] = useState(false);
  
    useEffect(() => {
      const updateState = () => {
        const provider = Providers.globalProvider;
        setIsSignedIn(provider && provider.state === ProviderState.SignedIn);
      };
  
      Providers.onProviderUpdated(updateState);
      updateState();
  
      return () => {
        Providers.removeProviderUpdatedListener(updateState);
      }
    }, []);
  
    return [isSignedIn];
  }



const Header = () => {
    const [isSignedIn] = useIsSignedIn()

    return (
        <div className="container-fluid">
            <Login />
            {isSignedIn && <div className="container">
                <Get resource="me/mailFolders/Inbox/messages" maxPage={1}>
                    <Email template="value" />
                </Get>             
            </div>}
        </div>
    )
}

export default Header