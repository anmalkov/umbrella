
export const fetchExtensions = async () => {
    const response = await fetch('api/extensions');
    const entities = await response.json();
    return entities;
}

export const registerExtension = async (id, parameters) => {
    const request = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ parameters: parameters })
    };
    const response = await fetch(`api/extensions/${id}`, request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}

export const unregisterExtension = async (id) => {
    const request = {
        method: 'DELETE'
    };
    const response = await fetch(`api/extensions/${id}`, request);
    if (response.status !== 200) {
        try {
            const result = await response.json();
            throw Error(result.detail);
        }
        catch (e) {
            throw Error(`[${response.status} ${response.statusText}] ${e}`);
        }
    }
}
