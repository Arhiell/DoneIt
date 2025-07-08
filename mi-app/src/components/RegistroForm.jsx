import React, { useState } from "react";

const RegistroForm = ({ onSubmit }) => {
  const [formData, setFormData] = useState({
    nombre: "",
    apellido: "",
    email: "",
    fechaNacimiento: "",
    nombreUsuario: "",
    contrasena: "",
  });

  const [error, setError] = useState(null);
  const [exito, setExito] = useState(null);
  const [cargando, setCargando] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setCargando(true);
    setError(null);
    setExito(null);

    try {
      const res = await fetch("http://localhost:5119/api/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });

      if (!res.ok) {
        const body = await res.json();
        throw new Error(body.error || "Error en el registro");
      }
      onSubmit(); // Llama a la función onSubmit pasada como prop
      setExito("Cuenta creada correctamente. Ya puedes iniciar sesión.");
      setFormData({
        nombre: "",
        apellido: "",
        email: "",
        fechaNacimiento: "", // Formato YYYY-MM-DD
        nombreUsuario: "",
        contrasena: "",
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setCargando(false);
    }
    console.log("Enviando datos:", formData);
  };

  return (
    <form onSubmit={handleSubmit} className="p-4 border rounded shadow w-100" style={{ maxWidth: 500 }}>
      <h3>Registro de usuario</h3>

      {error && <div className="alert alert-danger">{error}</div>}
      {exito && <div className="alert alert-success">{exito}</div>}

      <div className="mb-3">
        <label className="form-label">Nombre</label>
        <input name="nombre" value={formData.nombre} onChange={handleChange} className="form-control" required />
      </div>

      <div className="mb-3">
        <label className="form-label">Apellido</label>
        <input name="apellido" value={formData.apellido} onChange={handleChange} className="form-control" required />
      </div>

      <div className="mb-3">
        <label className="form-label">Email</label>
        <input type="email" name="email" value={formData.email} onChange={handleChange} className="form-control" required />
      </div>

      <div className="mb-3">
        <label className="form-label">Fecha de nacimiento</label>
        <input type="date" name="fechaNacimiento" value={formData.fechaNacimiento} onChange={handleChange} className="form-control" required />
      </div>

      <div className="mb-3">
        <label className="form-label">Nombre de usuario</label>
        <input name="nombreUsuario" value={formData.nombreUsuario} onChange={handleChange} className="form-control" required />
      </div>

      <div className="mb-3">
        <label className="form-label">Contraseña</label>
        <input type="password" name="contrasena" value={formData.contrasena} onChange={handleChange} className="form-control" required />
      </div>

      <button type="submit" className="btn btn-success w-100" disabled={cargando}>
        {cargando ? (
          <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
        ) : (
          "Registrarse"
        )}
      </button>
    </form>
  );
};

export default RegistroForm;
