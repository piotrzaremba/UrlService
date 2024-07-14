const API_URL = 'http://localhost:5237/api/url';

export async function createShortUrl(longUrl, customShortUrl = null) {
    const response = await fetch(`${API_URL}/create`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ longUrl, shortUrl: customShortUrl }),
    });
    return response.text();
}

export async function getStatistics() {
    const response = await fetch(`${API_URL}/stats`);
    return response.json();
}

export async function deleteShortUrl(shortUrl) {
    const response = await fetch(`${API_URL}/${shortUrl}`, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
        },
    });

    if (response.ok) {
        return true;
    } else {
        throw new Error(`Failed to delete short URL: ${shortUrl}`);
    }
}

export async function getLongUrl(shortUrl) {
    const response = await fetch(`${API_URL}/${shortUrl}`, {
        method: 'GET'
    });
    if (response.ok) {
        const longUrl = await response.text();
        const newWindow = window.open(longUrl, '_blank'); 
        if (newWindow) {
            newWindow.focus(); 
        } else {
            throw new Error('Failed to open long URL in a new window');
        }
        return longUrl; 
    }
    throw new Error('Failed to fetch long URL');
}
