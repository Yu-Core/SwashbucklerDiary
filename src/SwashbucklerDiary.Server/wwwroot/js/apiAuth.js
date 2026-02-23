export async function setCookie(apiKey) {
    try {
        await fetch('api/apiAuth/cookie', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apiKey })  // 将 apiKey 放入 JSON 对象
        });
    } catch (error) {
        console.error('Request failed', error);
    }
}