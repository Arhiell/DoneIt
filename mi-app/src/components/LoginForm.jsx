
import React, { useState } from 'react';
import { Link } from 'react-router-dom';


const LoginForm = ({ onLoginSuccess }) => {
  const [usuario, setUsuario] = useState('');
  const [contrasena, setContrasena] = useState('');
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(false);

const handleSubmit = async (e) => {
  e.preventDefault();
  setCargando(true);
  setError(null);

  const inicio = Date.now();

  try {
    const response = await fetch("https://localhost:5119/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ usuario, contrasena }),
    });

    const delayMinimo = 1500; // milisegundos
    const tiempoTranscurrido = Date.now() - inicio;
    const esperaExtra = Math.max(delayMinimo - tiempoTranscurrido, 0);

    setTimeout(async () => {
      if (!response.ok) {
        setError("Credenciales incorrectas");
        setCargando(false);
        return;
      }

      const data = await response.json();
      localStorage.setItem("token", data.token);
      window.location.href = "/dashboard";
    }, esperaExtra);

  } catch (err) {
    setTimeout(() => {
      setError("Error al conectar con el servidor");
      setCargando(false);
    }, 1500);
  }
};

  return (
    <form onSubmit={handleSubmit} className="p-4 border rounded shadow w-100" style={{ maxWidth: 400 }}>
      <h3>Iniciar sesión</h3>
      {error && <div className="alert alert-danger">{error}</div>}
      <div className="mb-3">
        <label className="form-label">Usuario</label>
        <input type="text" className="form-control" value={usuario} onChange={(e) => setUsuario(e.target.value)} required />
      </div>
      <div className="mb-3">
        <label className="form-label">Contraseña</label>
        <input type="password" className="form-control" value={contrasena} onChange={(e) => setContrasena(e.target.value)} required />
      </div>
      <p className="text-center mt-3">¿No tienes cuenta?{" "}
        <Link to="/registro" className="text-primary text-decoration-none fw-semibold">Regístrate</Link>
      </p>
    <button type="submit" className="btn btn-primary w-100" disabled={cargando}>
      {cargando ? (<span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>) : ("Ingresar")}
    </button>
    </form>
  );
};
export default LoginForm;
