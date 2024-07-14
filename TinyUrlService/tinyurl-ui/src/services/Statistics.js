import React, { useEffect, useState } from 'react';
import { getStatistics, getLongUrl } from '../services/UrlService';

function Statistics() {
    const [stats, setStats] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            const result = await getStatistics();
            setStats(result);
        };
        fetchData();
    }, []);

    const handleClick = async (shortUrl) => {
        try {
            const response = await getLongUrl(shortUrl);
            if (response.ok) {
                window.location.href = response.url; // Redirect to the long URL
            } else {
                console.error('Failed to fetch long URL');
            }
        } catch (error) {
            console.error('Error fetching long URL', error);
        }
    };

    return (
        <div>
            <h2>Statistics</h2>
            <ul>
                {Object.entries(stats).map(([shortUrl, data]) => (
                    <li key={shortUrl}>
                        <a href="#" onClick={() => handleClick(shortUrl)}>
                            {shortUrl}
                        </a>: {data.longUrl} (Clicks: {data.clickCount})
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default Statistics;
