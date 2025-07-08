import React from 'react';
import { Routes, Route } from 'react-router-dom';
import Sidebar from '../components/Sidebar';
import EditarProyecto from './EditarProyecto';
import Usuario from './Usuario';
import MisProyectosPage from './MisProyectosPage'; // Asegúrate de que esta ruta sea correcta

const HomePage = () => {
  return (
      <div style={{ display: 'flex' }}>
        <Sidebar />
      <div style={{ padding: '1rem', flexGrow: 1 }}>
        <Routes>
          <Route index element={<MisProyectosPage />} />  
          <Route path="proyectos" element={<MisProyectosPage />} />
          <Route path="editar" element={<EditarProyecto />} />
          <Route path="usuario" element={<Usuario />} />
        </Routes>
      </div>
    </div>
  );
};

export default HomePage;
