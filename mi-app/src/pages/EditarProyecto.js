import React, { useState, useEffect } from "react";
import { Button, Form, Accordion, ListGroup, InputGroup } from "react-bootstrap";

const ProyectosTareasPage = () => {
  const [proyectos, setProyectos] = useState([]);
  const [tareasPorProyecto, setTareasPorProyecto] = useState({});
  const [nuevoProyecto, setNuevoProyecto] = useState({ nombre: "", descripcion: "" });
  const [nuevaTarea, setNuevaTarea] = useState({ titulo: "", fechaInicio: "", fechaFin: "", estado: "", prioridad: "" });
  const [tareasTmp, setTareasTmp] = useState([]);

  useEffect(() => {
    cargarProyectos();
  }, []);

  const cargarProyectos = async () => {
    const res = await fetch("http://localhost:5119/api/Proyectos/mis-proyectos", {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    if (res.ok) {
      const data = await res.json();
      setProyectos(data.$values || []);
    }
  };

  const cargarTareas = async (idProyecto) => {
    const res = await fetch(`http://localhost:5119/api/Tareas/proyecto/${idProyecto}`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    if (res.ok) {
      const data = await res.json();
      setTareasPorProyecto((prev) => ({ ...prev, [idProyecto]: data.$values || [] }));
    }
  };

  const eliminarProyecto = async (id) => {
    if (!window.confirm("¿Seguro que querés eliminar este proyecto?")) return;

    const res = await fetch(`http://localhost:5119/api/Proyectos/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });

    if (res.ok) {
      setProyectos((prev) => prev.filter((p) => p.id_proyecto !== id));
      setTareasPorProyecto((prev) => {
        const copy = { ...prev };
        delete copy[id];
        return copy;
      });
    } else {
      alert("Error al eliminar proyecto");
    }
  };

  const eliminarTarea = async (idTarea, idProyecto) => {
    if (!window.confirm("¿Seguro que querés eliminar esta tarea?")) return;

    const res = await fetch(`http://localhost:5119/api/Tareas/${idTarea}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });

    if (res.ok) {
      setTareasPorProyecto((prev) => ({
        ...prev,
        [idProyecto]: prev[idProyecto].filter((t) => t.id_tarea !== idTarea),
      }));
    } else {
      alert("Error al eliminar tarea");
    }
  };

  const agregarTareaTmp = () => {
    setTareasTmp([...tareasTmp, nuevaTarea]);
    setNuevaTarea({ titulo: "", fechaInicio: "", fechaFin: "", estado: "", prioridad: "" });
  };

  const crearProyecto = async () => {
    const res = await fetch("http://localhost:5119/api/Proyectos", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify(nuevoProyecto),
    });

    if (res.ok) {
      const proyectoCreado = await res.json();
      for (const tarea of tareasTmp) {
        await fetch("http://localhost:5119/api/Tareas", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify({ ...tarea, id_proyecto: proyectoCreado.id_proyecto }),
        });
      }
      setNuevoProyecto({ nombre: "", descripcion: "" });
      setTareasTmp([]);
      cargarProyectos();
    } else {
      alert("Error al crear proyecto");
    }
  };

  return (
    <div className="d-flex gap-4 p-4">
      <div style={{ flex: 1, border: "1px solid #ddd", padding: 20, borderRadius: 5 }}>
        <h4>Crear nuevo proyecto</h4>
        <Form>
          <Form.Group className="mb-3" controlId="nombreProyecto">
            <Form.Label>Nombre del proyecto</Form.Label>
            <Form.Control
              type="text"
              value={nuevoProyecto.nombre}
              onChange={(e) => setNuevoProyecto({ ...nuevoProyecto, nombre: e.target.value })}
            />
          </Form.Group>
          <Form.Group className="mb-3" controlId="descProyecto">
            <Form.Label>Descripción</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              value={nuevoProyecto.descripcion}
              onChange={(e) => setNuevoProyecto({ ...nuevoProyecto, descripcion: e.target.value })}
            />
          </Form.Group>

          <h5>Agregar tareas</h5>
          <InputGroup className="mb-2">
            <Form.Control
              placeholder="Nombre tarea"
              value={nuevaTarea.titulo}
              onChange={(e) => setNuevaTarea({ ...nuevaTarea, titulo: e.target.value })}
            />
          </InputGroup>
          <InputGroup className="mb-2">
            <Form.Control
              type="datetime-local"
              value={nuevaTarea.fechaInicio}
              onChange={(e) => setNuevaTarea({ ...nuevaTarea, fechaInicio: e.target.value })}
            />
            <Form.Control
              type="datetime-local"
              value={nuevaTarea.fechaFin}
              onChange={(e) => setNuevaTarea({ ...nuevaTarea, fechaFin: e.target.value })}
            />
            <Form.Select
                type="select"
                value={nuevaTarea.estado || ""}
                onChange={(e) => setNuevaTarea({ ...nuevaTarea, estado: e.target.value })}
            >
              <option value="">Seleccionar estado</option>
              <option value="Pendiente">Pendiente</option>
              <option value="En Proceso">En progreso</option>
              <option value="Finalizado">Completa</option>
            </Form.Select>
            <Form.Select
                value={nuevaTarea.prioridad || ""}
                onChange={(e) => setNuevaTarea({ ...nuevaTarea, prioridad: e.target.value })}
            >
              <option value="">Seleccionar prioridad</option>
              <option value="Bajo">Baja</option>
              <option value="Medio">Media</option>
              <option value="Alto">Alta</option>
            </Form.Select>
          </InputGroup>
          <Button variant="secondary" size="sm" onClick={agregarTareaTmp} disabled={!nuevaTarea.titulo || !nuevaTarea.fechaInicio || !nuevaTarea.fechaFin || !nuevaTarea.estado || !nuevaTarea.prioridad}>
            Agregar tarea
          </Button>

          <hr />
          <h6>Tareas agregadas:</h6>
          <ul>
            {tareasTmp.map((t, i) => (
              <li key={i}>
                {t.nombre} ({t.fechaInicio} - {t.fechaFin})
              </li>
            ))}
          </ul>

          <Button variant="primary" onClick={crearProyecto} disabled={!nuevoProyecto.nombre}>
            Crear proyecto con tareas
          </Button>
        </Form>
      </div>

      <div style={{ flex: 2 }}>
        <h4>Mis proyectos</h4>
        <Accordion>
          {proyectos.map((proyecto) => (
            <Accordion.Item eventKey={proyecto.id_proyecto.toString()} key={proyecto.id_proyecto}>
              <Accordion.Header onClick={() => cargarTareas(proyecto.id_proyecto)}>{proyecto.nombre}</Accordion.Header>
              <Accordion.Body>
                <Button
                  variant="warning"
                  size="sm"
                  className="me-2"
                  onClick={() => alert("Editar proyecto - próximamente")}
                >
                  Editar
                </Button>
                <Button variant="danger" size="sm" onClick={() => eliminarProyecto(proyecto.id_proyecto)}>
                  Eliminar
                </Button>

                <h6 className="mt-3">Tareas</h6>
                <ListGroup>
                  {(tareasPorProyecto[proyecto.id_proyecto] || []).map((tarea) => (
                    <ListGroup.Item key={tarea.id_tarea} className="d-flex justify-content-between align-items-center">
                      <span>
                        {tarea.titulo} ({tarea.fechaInicio} - {tarea.fechaFin}) <br />
                        Estado: {tarea.estado} | Prioridad: {tarea.prioridad}
                      </span>
                      <div>
                        <Button
                          variant="warning"
                          size="sm"
                          className="me-2"
                          onClick={() => alert("Editar tarea - próximamente")}
                        >
                          Editar
                        </Button>
                        <Button variant="danger" size="sm" onClick={() => eliminarTarea(tarea.id_tarea, proyecto.id_proyecto)}>
                          Eliminar
                        </Button>
                      </div>
                    </ListGroup.Item>
                  ))}
                </ListGroup>
              </Accordion.Body>
            </Accordion.Item>
          ))}
        </Accordion>
      </div>
    </div>
  );
};

export default ProyectosTareasPage;
