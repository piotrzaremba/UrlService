import React from 'react';
import UrlShortener from './services/UrlShortener';
import Statistics from './services/Statistics';
import DeleteShortUrlButton from './services/DeleteShortUrlButton';

function App() {
    return (
        <div className="App">
            <h1>TinyURL POC</h1>
            <UrlShortener />
            <DeleteShortUrlButton />
            <Statistics />
        </div>
    );
}

export default App;
