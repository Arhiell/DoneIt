import React, { useEffect } from 'react';
import LoginForm from '../components/LoginForm';
import { useNavigate } from 'react-router-dom';

const LoginPage = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      navigate("/dashboard", { replace: true }); // redirige si ya está logueado
    }
  }, [navigate]);

  const handleLoginSuccess = () => {
    navigate('/dashboard'); // redirige después de login exitoso
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <LoginForm onLoginSuccess={handleLoginSuccess} />
    </div>
  );
};

export default LoginPage;
// Este componente maneja la página de inicio de sesión
// y redirige al usuario a la página de dashboard si ya está logueado.