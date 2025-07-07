import React, { useEffect, useState } from 'react';

const MisProyectosPage = () => {
  const [proyectos, setProyectos] = useState([]);
  const [error, setError] = useState(null);

  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchProyectos = async () => {
      try {
        const res = await fetch("http://localhost:5119/api/proyecto/mis-proyectos", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!res.ok) {
          throw new Error("Error al obtener proyectos");
        }

        const data = await res.json();
        setProyectos(data);
      } catch (err) {
        setError(err.message);
      }
    };

    fetchProyectos();
  }, [token]);

  return (
    <div className="p-4">
      <h2>Mis Proyectos</h2>
      {error && <div className="alert alert-danger">{error}</div>}
      {proyectos.length === 0 ? (
        <p>No tienes proyectos todavía.</p>
      ) : (
        <ul className="list-group">
          {proyectos.map((proyecto) => (
            <li key={proyecto.id_proyecto} className="list-group-item">
              <strong>{proyecto.nombre}</strong><br />
              {proyecto.descripcion}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default MisProyectosPage;