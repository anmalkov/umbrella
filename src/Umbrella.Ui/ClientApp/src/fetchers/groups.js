
export const fetchGroups = async () => {
    const response = await fetch('api/groups');
    const groups = await response.json();
    return groups;
}