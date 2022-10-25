
export const fetchEntities = async () => {
    const response = await fetch('api/entities');
    const entities = await response.json();
    return entities;
}

export const setLightState = async (id, turnedOn, brightness, colorTemperature) => {
    await setLightsStates([{ id, turnedOn, brightness, colorTemperature }]);
}

export const setLightsStates = async (states) => {
    const params = states.map(s => ({
        key: s.id,
        value: {
            turnedOn: s.turnedOn,
            brightness: s.brightness != null ? Number(s.brightness) : null,
            colorTemperature: s.colorTemperature != null ? Number(s.colorTemperature) : null
        }
    }));
    await updateEntitiesStates(params);
}

const updateEntitiesStates = async (params) => {
    const request = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ states: params })
    };
    const response = await fetch(`api/entities/states`, request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}
