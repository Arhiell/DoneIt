import React from 'react';
import { Link } from 'react-router-dom';
import { FaProjectDiagram, FaEdit, FaUser } from 'react-icons/fa';

const Sidebar = () => {
  const nombre = localStorage.getItem("nombreUsuario");


  return (
    <div style={{
      width: '200px',
      height: '100vh',
      backgroundColor: '#f8f9fa',
      padding: '1rem',
      boxSizing: 'border-box'
    }}>
      <h4>Menú</h4>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        <li>
          <Link to="/dashboard/proyectos" className="d-flex align-items-center gap-2">
            <FaProjectDiagram /> Ver Proyectos
          </Link>
        </li>
        <li>
          <Link to="/dashboard/editar" className="d-flex align-items-center gap-2">
            <FaEdit /> Editar Proyectos
          </Link>
        </li>
        <li>
          <Link to="/dashboard/usuario" className="d-flex align-items-center gap-2">
            <FaUser /> {nombre || "Usuario"}
          </Link>
        </li>
      </ul>
    </div>
  );
};

export default Sidebar;
