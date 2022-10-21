
export const fetchStates = async () => {
    const response = await fetch('api/entities/states');
    const states = await response.json();
    return states;
}
