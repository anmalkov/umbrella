
export const fetchEntities = async () => {
    const response = await fetch('api/entities');
    const entities = await response.json();
    return entities;
}

export const setLightState = async (id, turnedOn, brightness, colorTemperature) => {
    const params = {
        turnedOn,
        brightness: brightness != null ? Number(brightness) : null,
        colorTemperature: colorTemperature != null ? Number(colorTemperature) : null
    };
    await updateEntityState(id, params);
}

const updateEntityState = async (id, params) => {
    console.log("updateState", params);
    const request = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ state: params })
    };
    const response = await fetch(`api/entities/${id}/state`, request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}