import React, { useEffect, useState } from 'react';
import DataGrid, {
    Column,
    Paging,
    Pager,
    SearchPanel,
} from 'devextreme-react/data-grid';
import { Button, Modal, Form } from 'react-bootstrap';
import { Workbook } from 'exceljs';
import saveAs from 'file-saver';
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';
import 'bootstrap/dist/css/bootstrap.min.css';

import {
    getUsers,
    createUser,
    updateUser,
    deleteUser,
} from '../services/userService';

const UserGrid = () => {
    const [users, setUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [statusFilter, setStatusFilter] = useState('all');
    const [showModal, setShowModal] = useState(false);
    const [isDeleteModal, setIsDeleteModal] = useState(false);
    const [userFormData, setUserFormData] = useState({
        id: '',
        firstName: '',
        lastName: '',
        email: '',
        isActive: false,
    });
    const [userRole, setUserRole] = useState('');

    const loadUsers = async () => {
        try {
            const data = await getUsers();
            setUsers(data.items || []);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        const role = sessionStorage.getItem('user_role');
        setUserRole(role);
        loadUsers();
    }, []);

    useEffect(() => {
        if (statusFilter === 'active') {
            setFilteredUsers(users.filter((u) => u.isActive));
        } else if (statusFilter === 'inactive') {
            setFilteredUsers(users.filter((u) => !u.isActive));
        } else {
            setFilteredUsers(users);
        }
    }, [statusFilter, users]);

    const handleModalClose = () => {
        setShowModal(false);
        setIsDeleteModal(false);
        setUserFormData({
            id: '',
            firstName: '',
            lastName: '',
            email: '',
            isActive: false,
        });
    };

    const handleModalShow = (user = {}, action = 'add') => {
        setUserFormData({
            id: user.id || '',
            firstName: user.firstName || '',
            lastName: user.lastName || '',
            email: user.email || '',
            isActive: user.isActive || false,
        });
        if (action === 'delete') {
            setIsDeleteModal(true);
        } else {
            setShowModal(true);
        }
    };

    const handleFormChange = (e) => {
        const { name, value, type, checked } = e.target;
        setUserFormData((prev) => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (userFormData.id) {
                await updateUser(userFormData.id, userFormData);
            } else {
                await createUser(userFormData);
            }
            handleModalClose();
            await loadUsers();
        } catch (error) {
            console.error(error);
        }
    };

    const handleDelete = async () => {
        try {
            await deleteUser(userFormData.id);
            handleModalClose();
            await loadUsers();
        } catch (error) {
            console.error(error);
        }
    };

    const exportToPDF = () => {
        const doc = new jsPDF();
        const tableColumn = ['Ime', 'Prezime', 'Email', 'Aktivan'];
        const tableRows = filteredUsers.map(user => [
            user.firstName,
            user.lastName,
            user.email,
            user.isActive ? 'Da' : 'Ne'
        ]);

        autoTable(doc, {
            head: [tableColumn],
            body: tableRows,
            startY: 20,
            styles: { fontSize: 10 },
            headStyles: { fillColor: [22, 160, 133] }
        });

        doc.text('Lista korisnika', 14, 15);
        doc.save('korisnici.pdf');
    };

    const exportToExcel = () => {
        const workbook = new Workbook();
        const worksheet = workbook.addWorksheet('Korisnici');

        worksheet.columns = [
            { header: 'Ime', key: 'firstName', width: 20 },
            { header: 'Prezime', key: 'lastName', width: 20 },
            { header: 'Email', key: 'email', width: 30 },
            { header: 'Aktivan', key: 'isActive', width: 10 },
        ];

        filteredUsers.forEach(user => {
            worksheet.addRow({
                firstName: user.firstName,
                lastName: user.lastName,
                email: user.email,
                isActive: user.isActive ? 'Da' : 'Ne',
            });
        });

        workbook.xlsx.writeBuffer().then(buffer => {
            saveAs(new Blob([buffer]), 'korisnici.xlsx');
        });
    };

    const exportToCSV = () => {
        const header = ['Ime', 'Prezime', 'Email', 'Aktivan'];
        const rows = filteredUsers.map(user => [
            user.firstName,
            user.lastName,
            user.email,
            user.isActive ? 'Da' : 'Ne',
        ]);

        const csv = [header, ...rows].map(e => e.join(',')).join('\n');
        const blob = new Blob([csv], { type: 'text/csv' });
        saveAs(blob, 'korisnici.csv');
    };

    if (userRole && userRole !== 'admin') {
        return <div className="alert alert-danger mt-4">Nemate pristup ovoj sekciji.</div>;
    }

    return (
        <div className="container mt-5 user-grid-container">
            <h2 className="mb-4 text-primary">📋 Lista korisnika</h2>
            <div className="d-flex gap-2 flex-wrap mb-3">
                <Button variant="primary" onClick={() => handleModalShow({}, 'add')}>
                    ➕ Dodaj korisnika
                </Button>
                <Button variant="danger" onClick={exportToPDF}>🧾 PDF</Button>
                <Button variant="success" onClick={exportToExcel}>📊 Excel</Button>
                <Button variant="info" onClick={exportToCSV}>📁 CSV</Button>
                <Form.Select className="w-auto" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
                    <option value="all">Svi</option>
                    <option value="active">Aktivni</option>
                    <option value="inactive">Neaktivni</option>
                </Form.Select>
            </div>

            <div className="card shadow-lg">
                <div className="card-body">
                    <DataGrid
                        dataSource={filteredUsers}
                        keyExpr="id"
                        showBorders
                        columnAutoWidth
                        rowAlternationEnabled
                    >
                        <SearchPanel visible highlightCaseSensitive />
                        <Paging defaultPageSize={10} />
                        <Pager showPageSizeSelector allowedPageSizes={[5, 10, 20]} showInfo />
                        <Column dataField="firstName" caption="Ime" alignment="left" />
                        <Column dataField="lastName" caption="Prezime" alignment="left" />
                        <Column dataField="email" caption="Email" alignment="left" />
                        <Column dataField="isActive" caption="Aktivan" dataType="boolean" alignment="center" />
                        <Column
                            type="buttons"
                            width={110}
                            buttons={[
                                {
                                    text: 'Uredi',
                                    icon: 'edit',
                                    onClick: (e) => handleModalShow(e.row.data, 'edit')
                                },
                                {
                                    text: 'Obriši',
                                    icon: 'trash',
                                    onClick: (e) => handleModalShow(e.row.data, 'delete')
                                }
                            ]}
                        />
                    </DataGrid>
                </div>
            </div>

            <Modal show={showModal} onHide={handleModalClose}>
                <Modal.Header closeButton>
                    <Modal.Title>{userFormData.id ? 'Uredi korisnika' : 'Dodaj korisnika'}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={handleSubmit}>
                        <Form.Group className="mb-3">
                            <Form.Label>Ime</Form.Label>
                            <Form.Control
                                type="text"
                                name="firstName"
                                value={userFormData.firstName}
                                onChange={handleFormChange}
                                required
                            />
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>Prezime</Form.Label>
                            <Form.Control
                                type="text"
                                name="lastName"
                                value={userFormData.lastName}
                                onChange={handleFormChange}
                                required
                            />
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>Email</Form.Label>
                            <Form.Control
                                type="email"
                                name="email"
                                value={userFormData.email}
                                onChange={handleFormChange}
                                required
                            />
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Check
                                type="checkbox"
                                label="Aktivan"
                                name="isActive"
                                checked={userFormData.isActive}
                                onChange={handleFormChange}
                            />
                        </Form.Group>

                        <Button variant="primary" type="submit">
                            {userFormData.id ? 'Spremi promjene' : 'Dodaj korisnika'}
                        </Button>
                    </Form>
                </Modal.Body>
            </Modal>

            <Modal show={isDeleteModal} onHide={handleModalClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Obriši korisnika</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>Jeste li sigurni da želite obrisati ovog korisnika?</p>
                    <Button variant="danger" onClick={handleDelete}>Obriši</Button>
                    <Button variant="secondary" onClick={handleModalClose}>Zatvori</Button>
                </Modal.Body>
            </Modal>
        </div>
    );
};

export default UserGrid;
