import React from 'react'

const Email = (props) => {
    const email = props.dataContext
    
    return (
        <ul className="list-group my-3">
            <li className="list-group-item">
                <div className="d-flex justify-content-between align-items-center">
                    <mgt-person person-query={email.sender.emailAddress.address} view="oneLine"></mgt-person>
                    <span>{new Date(email.receivedDateTime).toLocaleString()}</span>
                </div>
                <div className="my-2">
                    <strong>{email.subject}</strong>
                </div>
                <div>
                    {email.bodyPreview}
                </div>
            </li>
            
        </ul>
    )
}

export default Email