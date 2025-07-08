import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FaProjectDiagram, FaEdit, FaUser, FaSignOutAlt } from 'react-icons/fa';

const Sidebar = () => {
  const nombre = localStorage.getItem("nombreUsuario");
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('nombreUsuario');
    navigate('/login');
  };

  return (
    <div style={{
      width: '200px',
      height: '100vh',
      backgroundColor: '#2a3535',
      padding: '1rem',
      boxSizing: 'border-box'
    }}>
      <h4 style={{ color: 'white' }}>Menú</h4>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        <li>
          <Link to="/dashboard/proyectos" className="d-flex align-items-center gap-2" style={{ color: 'white' }}>
            <FaProjectDiagram /> Ver Proyectos
          </Link>
        </li>
        <li>
          <Link to="/dashboard/editar" className="d-flex align-items-center gap-2" style={{ color: 'white' }}>
            <FaEdit /> Editar Proyectos
          </Link>
        </li>
        <li>
          <Link to="/dashboard/usuario" className="d-flex align-items-center gap-2" style={{ color: 'white' }}>
            <FaUser /> {nombre || "Usuario"}
          </Link>
        </li>
        <li style={{ marginTop: '2rem' }}>
          <button
            onClick={handleLogout}
            className="btn btn-danger d-flex align-items-center gap-2"
            style={{ width: '100%' }}
          >
            <FaSignOutAlt /> Cerrar Sesión
          </button>
        </li>
      </ul>
    </div>
  );
};

export default Sidebar;
