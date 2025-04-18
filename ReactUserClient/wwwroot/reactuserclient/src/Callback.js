//import React, { useEffect } from 'react';
//import { signinRedirectCallback } from './services/authService';
//import { useNavigate } from 'react-router-dom';


//const Callback = () => {
//    const navigate = useNavigate();

//    useEffect(() => {
//        signinRedirectCallback().then(user => {
//            if (user && user.profile && user.profile.role) {
//                sessionStorage.setItem('user_role', user.profile.role); 
//            }
//            navigate('/');
//        }).catch(err => {
//            console.error('Login callback greška', err);
//        });
//    }, [navigate]);

//    return <div>Obrađujem prijavu...</div>;
//};

//export default Callback;
