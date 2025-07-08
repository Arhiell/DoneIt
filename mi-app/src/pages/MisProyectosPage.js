import React, { useEffect, useState } from 'react';
import { Button, ListGroup, Badge } from "react-bootstrap";
import { FaProjectDiagram, FaTasks, FaCheckCircle, FaHourglassHalf, FaRegClock } from "react-icons/fa";
// Página para mostrar los proyectos del usuario
const MisProyectosPage = () => {
  const [proyectos, setProyectos] = useState([]);
  const [error, setError] = useState(null);
  const [expandedProyectoId, setExpandedProyectoId] = useState(null);
  const [tareasPorProyecto, setTareasPorProyecto] = useState({});

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
    const fetchTareas = async (idProyecto) => {
      try {
        const res = await fetch(`http://localhost:5119/api/Tareas/proyecto/${idProyecto}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        // Verifica si la respuesta es exitosa
        if (!res.ok) {
          throw new Error("Error al obtener tareas del proyecto");
        }
        // Convierte la respuesta a JSON
        const data = await res.json();
        return data.$values || [];
      } catch (err) {
        setError(err.message);
        return [];
      }
    };
  
  const handleExpandProyecto = async (idProyecto) => {
    if (expandedProyectoId === idProyecto) {
      setExpandedProyectoId(null);
      return;
    } 
// Si no tenemos las tareas, las buscamos
    if (!tareasPorProyecto[idProyecto]) {
      const tareas = await fetchTareas(idProyecto);
      setTareasPorProyecto(prev => ({ ...prev, [idProyecto]: tareas }));
    }
    setExpandedProyectoId(idProyecto);
  };

  // Renderiza la lista de proyectos
const estadoEstilos = {
  pendiente: { backgroundColor: "#fff3cd", color: "#856404" },      // amarillo claro
  "en progreso": { backgroundColor: "#cce5ff", color: "#004085" },  // azul claro
  completada: { backgroundColor: "#d4edda", color: "#155724" },    // verde claro
};

  return (
    <div className="p-4">
      <h2
        style={{
          background: "linear-gradient(45deg, #6a11cb, #2575fc)",
          color: "white",
          padding: "10px 20px",
          borderRadius: "8px",
          display: "inline-block",
          marginBottom: "20px",
        }}
      >
        <FaProjectDiagram style={{ marginRight: 8 }} />
        Mis Proyectos
      </h2>

      {error && <div className="alert alert-danger">{error}</div>}

      {proyectos.length === 0 ? (
        <p>No tienes proyectos todavía.</p>
      ) : (
        <ListGroup>
          {proyectos.map((proyecto) => (
            <ListGroup.Item key={proyecto.id_proyecto}>
              <h5 style={{ color: "#2575fc" }}>
                <FaTasks style={{ marginRight: 6 }} />
                {proyecto.nombre}
              </h5>
              <p>{proyecto.descripcion}</p>

              <Button
                variant="primary"
                size="sm"
                onClick={() => handleExpandProyecto(proyecto.id_proyecto)}
              >
                {expandedProyectoId === proyecto.id_proyecto ? "Ocultar Tareas" : "Ver Tareas"}
              </Button>

              {expandedProyectoId === proyecto.id_proyecto && (
                <ListGroup className="mt-3">
                  {tareasPorProyecto[proyecto.id_proyecto] ? (
                    tareasPorProyecto[proyecto.id_proyecto].length === 0 ? (
                      <ListGroup.Item>No hay tareas.</ListGroup.Item>
                    ) : (
                      tareasPorProyecto[proyecto.id_proyecto].map((tarea) => {
                        const estadoKey = tarea.estado.toLowerCase();
                        const estiloEstado = estadoEstilos[estadoKey] || {
                          backgroundColor: "#f8f9fa",
                          color: "#6c757d",
                        };

                        let IconEstado = FaRegClock;
                        if (estadoKey === "pendiente") IconEstado = FaHourglassHalf;
                        else if (estadoKey === "en progreso") IconEstado = FaHourglassHalf;
                        else if (estadoKey === "completada") IconEstado = FaCheckCircle;

                        return (
                          <ListGroup.Item
                            key={tarea.id_tarea}
                            style={{
                              ...estiloEstado,
                              borderRadius: "6px",
                              marginBottom: "8px",
                            }}
                          >
                            <strong>{tarea.titulo}</strong> <br />
                            {tarea.descripcion} <br />
                            Fecha Inicio: {tarea.fecha_inicio} <br />
                            Fecha Fin: {tarea.fecha_fin} <br />
                            Estado:{" "}
                            <Badge
                              pill
                              style={{
                                backgroundColor: estiloEstado.color,
                                color: "white",
                                marginRight: 6,
                              }}
                            >
                              <IconEstado /> {tarea.estado}
                            </Badge>
                            Prioridad: {tarea.prioridad}
                          </ListGroup.Item>
                        );
                      })
                    )
                  ) : (
                    <ListGroup.Item>Cargando tareas...</ListGroup.Item>
                  )}
                </ListGroup>
              )}
            </ListGroup.Item>
          ))}
        </ListGroup>
      )}
    </div>
  );
}

export default MisProyectosPage;