
export const fetchEntities = async () => {
    const response = await fetch('api/entities');
    const entities = await response.json();
    return entities;
}