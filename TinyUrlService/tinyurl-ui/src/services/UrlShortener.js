import React, { useState } from 'react';
import { createShortUrl } from '../services/UrlService';

function UrlShortener() {
    const [longUrl, setLongUrl] = useState('');
    const [shortUrl, setShortUrl] = useState('');
    const [customShortUrl, setCustomShortUrl] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        const result = await createShortUrl(longUrl, customShortUrl);
        setShortUrl(result);
    };

    return (
        <div>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    value={longUrl}
                    onChange={(e) => setLongUrl(e.target.value)}
                    placeholder="Enter long URL"
                    required
                />
                <input
                    type="text"
                    value={customShortUrl}
                    onChange={(e) => setCustomShortUrl(e.target.value)}
                    placeholder="Enter custom short URL (optional)"
                />
                <button type="submit">Shorten URL</button>
            </form>
            {shortUrl && <p>Short URL: {shortUrl}</p>}
        </div>
    );
}

export default UrlShortener;

