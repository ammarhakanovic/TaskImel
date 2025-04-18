const API_URL = 'https://localhost:6001/api/Users';

const authHeader = () => {
    const token = sessionStorage.getItem('access_token');
    if (!token) throw new Error('Nema pristupnog tokena.');
    return {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    };
};

export const getUsers = async (page = 1, pageSize = 100, search = '', isActive = null) => {
    const params = new URLSearchParams({ Page: page, PageSize: pageSize });
    if (search) params.append('Search', search);
    if (isActive !== null) params.append('IsActive', isActive);

    const response = await fetch(`${API_URL}/getusers?${params.toString()}`, {
        method: 'GET',
        headers: {
            ...authHeader().headers
        }
    });

    if (!response.ok) throw new Error('Greška prilikom dohvaćanja korisnika.');
    return await response.json();
};

export const deleteUser = async (id) => {
    const response = await fetch(`${API_URL}/deleteuser/${id}`, {
        method: 'DELETE',
        headers: {
            ...authHeader().headers
        }
    });

    if (!response.ok) throw new Error('Greška prilikom brisanja korisnika.');
};

export const createUser = async (data) => {
    const response = await fetch(`${API_URL}/createuser`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            ...authHeader().headers
        },
        body: JSON.stringify(data)
    });

    if (!response.ok) throw new Error('Greška prilikom kreiranja korisnika.');
};

export const updateUser = async (id, data) => {
    const response = await fetch(`${API_URL}/updateuser/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            ...authHeader().headers
        },
        body: JSON.stringify(data)
    });

    if (!response.ok) throw new Error('Greška prilikom ažuriranja korisnika.');
};