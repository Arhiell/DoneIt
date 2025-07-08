import React, { useEffect, useState } from 'react';

// Página para mostrar los proyectos del usuario
const MisProyectosPage = () => {
  const [proyectos, setProyectos] = useState([]);
  const [error, setError] = useState(null);
  const [expandedProyectoId, setExpandedProyectoId] = useState(null);
  const token = localStorage.getItem("token"); 
  
  // Verifica si el token existe
  useEffect(() => {
    const fetchProyectos = async () => {
      try {
        const res = await fetch("http://localhost:5119/api/Proyectos/mis-proyectos", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
// Verifica si la respuesta es exitosa
        if (!res.ok) {
          throw new Error("Error al obtener proyectos");
        }
// Convierte la respuesta a JSON
        const data = await res.json();
        setProyectos(data.$values || []);
      } catch (err) {
        setError(err.message);
      }
    };
// Llama a la función para obtener los proyectos
    fetchProyectos();
  }, [token]);

  // Renderiza la lista de proyectos 

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
              <br />
              <button
                className="btn btn-sm btn-sm mt-2"
                onClick={() => setExpandedProyectoId(
                  expandedProyectoId === proyecto.id_proyecto ? null : proyecto.id_proyecto
                )}
              >
                {expandedProyectoId === proyecto.id_proyecto ? "Ocultar Tareas" : "Ver Tareas"}
              </button>
               {expandedProyectoId === proyecto.id_proyecto && proyecto.tareas?.$values && (
              <ul className="mt-2">
                {proyecto.tareas.$values.length === 0 ? (
                  <li>No hay tareas.</li>
                ) : (
                  proyecto.tareas.$values.map((tarea) => (
                    <li key={tarea.id_tarea}>
                      <strong>{tarea.titulo}</strong>: {tarea.descripcion} <br />
                      Fecha Inicio: {tarea.fechaInicio} <br />
                      Fecha Fin: {tarea.fechaFin} <br />
                      Estado: {tarea.estado} : {tarea.prioridad} <br />
                    </li>
                   ))
                  )}
                </ul>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default MisProyectosPage;