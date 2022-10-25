
export const fetchDashboards = async () => {
    const response = await fetch('api/dashboards');
    const dashboards = await response.json();
    return dashboards;
}

export const createDashboard = async (dashboard) => {
    const request = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: dashboard.name, widgets: dashboard.widgets })
    };
    const response = await fetch('api/dashboards', request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}

export const updateDashboard = async (dashboard) => {
    const request = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: dashboard.id, name: dashboard.name, widgets: dashboard.widgets })
    };
    const response = await fetch(`api/dashboards/${dashboard.id}`, request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}

export const deleteDashboard = async (id) => {
    const request = {
        method: 'DELETE'
    };
    const response = await fetch(`api/dashboards/${id}`, request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}

