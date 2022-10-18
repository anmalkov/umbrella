
export const fetchAreas = async () => {
    const response = await fetch('api/areas');
    const areas = await response.json();
    return areas;
}