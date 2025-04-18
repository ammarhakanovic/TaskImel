import React from 'react';
import userManager from '../../oidcConfig';

const LoginButton = () => {
    const handleLogin = () => {
        userManager.signinRedirect();
    };

    return (
        <div style={styles.container}>
            <div style={styles.content}>
                <h2 style={styles.welcomeMessage}>Dobrodošli! Prijavite se da nastavite.</h2>
                <button onClick={handleLogin} style={styles.button}>Prijava</button>
            </div>
        </div>
    );
};

const styles = {
    container: {
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        backgroundColor: '#f0f0f0',
    },
    content: {
        textAlign: 'center',
        padding: '20px',
        borderRadius: '8px',
        backgroundColor: '#fff',
        boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
    },
    welcomeMessage: {
        marginBottom: '20px',
        fontSize: '18px',
        color: '#333',
    },
    button: {
        padding: '10px 20px',
        fontSize: '16px',
        backgroundColor: '#007bff',
        color: '#fff',
        border: 'none',
        borderRadius: '4px',
        cursor: 'pointer',
        transition: 'background-color 0.3s',
    },
};

export default LoginButton;