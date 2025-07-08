import React from "react";
import RegistroForm from "../components/RegistroForm"; // ajustá la ruta si es necesario
import { useNavigate } from "react-router-dom";

const RegistroPage = () => {
  const navigate = useNavigate();

  const handleSubmitSuccess = () => {
    navigate('/login');
  };
  return (
    <div className="d-flex justify-content-center align-items-center min-vh-100 bg-light">
      <div className="text-center">
        <h2 className="mb-4">Crear una cuenta</h2>
        <RegistroForm onSubmit={handleSubmitSuccess} />
      </div>
    </div>
  );
};

export default RegistroPage;
