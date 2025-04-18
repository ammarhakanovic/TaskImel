import React, { useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom';
import { UserManager } from 'oidc-client';
import UserGrid from './components/UserGrid';
import './App.css';


const oidcSettings = {
    authority: 'https://localhost:5443',
    redirect_uri: 'https://localhost:3000/signin-oidc',
    response_type: 'code',
    client_id: 'bff',
    scope: 'openid profile roles userApi',
    post_logout_redirect_uri: 'https://localhost:3000/',
};

const userManager = new UserManager(oidcSettings);

const LoginButton = () => {
    const handleLogin = () => {
        userManager.signinRedirect();
    };

    return (
        <div className="container">
            <div className="content">
                <h2 className="welcome-message">Dobrodošli! Prijavite se da nastavite.</h2>
                <button onClick={handleLogin} className="login-button">Prijava</button>
            </div>
        </div>
    ); };

const LogoutButton = () => {
    const handleLogout = () => {
        sessionStorage.removeItem('access_token');
        userManager.signoutRedirect();
    };

    return <button onClick={handleLogout}>Logout</button>;
};

const SigninOidc = () => {
    const navigate = useNavigate();

    useEffect(() => {
        userManager.signinRedirectCallback().then((user) => {
            console.log('User after signin callback:', user);
            if (user && user.access_token) {
                sessionStorage.setItem('access_token', user.access_token);
                navigate('/admin');
            }
        }).catch(error => {
            console.error('Error during signin callback:', error);
        });
    }, [navigate]);

    return <h2>Processing login...</h2>;
};

const AdminPanel = () => {
    return (
        <div>
            <h2>Admin Panel</h2>
            <LogoutButton />
            <UserGrid />
        </div>
    );
};

const App = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LoginButton />} />
                <Route path="/signin-oidc" element={<SigninOidc />} />
                <Route path="/admin" element={<AdminPanel />} />
            </Routes>
        </Router>
    );
};

export default App;
