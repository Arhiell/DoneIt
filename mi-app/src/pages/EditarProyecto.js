import React, { useState, useEffect } from "react";
import { Button, Form, Accordion, ListGroup, InputGroup, Modal } from "react-bootstrap";

// declaracion de la página de proyectos y tareas
const ProyectosTareasPage = () => {
const [proyectos, setProyectos] = useState([]);
const [tareasPorProyecto, setTareasPorProyecto] = useState({});
const [nuevoProyecto, setNuevoProyecto] = useState({ nombre: "", descripcion: "" });
const [nuevaTarea, setNuevaTarea] = useState({ titulo: "", fecha_inicio: "", fecha_fin: "", estado: "", prioridad: "" });
const [tareasTmp, setTareasTmp] = useState([]);
const [proyectoEditando, setProyectoEditando] = useState(null); // para editar
const [mostrarModal, setMostrarModal] = useState(false);
const [tareaEditando, setTareaEditando] = useState(null); // para editar tareas
const [mostrarModalTarea, setMostrarModalTarea] = useState(false);
const [mostrarModalNuevaTarea, setMostrarModalNuevaTarea] = useState(false);
const [idProyectoParaNuevaTarea, setIdProyectoParaNuevaTarea] = useState(null);
const [nuevaTareaModal, setNuevaTareaModal] = useState({
  titulo: "",
  fecha_inicio: "",
  fecha_fin: "",
  estado: "",
  prioridad: "",
});

  useEffect(() => {
    cargarProyectos();
  }, []);

// Carga los proyectos del usuario
  const cargarProyectos = async () => {
    const res = await fetch("http://localhost:5119/api/Proyectos/mis-proyectos", {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    if (res.ok) {
      const data = await res.json();
      setProyectos(data.$values || []);
    }
  };
// Carga las tareas de un proyecto específico

  const cargarTareas = async (idProyecto) => {
    const res = await fetch(`http://localhost:5119/api/Tareas/proyecto/${idProyecto}`, {
      headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
    });
    if (res.ok) {
      const data = await res.json();
      setTareasPorProyecto((prev) => ({ ...prev, [idProyecto]: data.$values || [] }));
    }
  };
// Elimina un proyecto y sus tareas asociadas

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
// Elimina una tarea del proyecto

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

// Agrega una tarea temporal a la lista de tareas antes de crear el proyecto
  const agregarTareaTmp = () => {
    setTareasTmp([...tareasTmp, nuevaTarea]);
    setNuevaTarea({ titulo: "", fecha_inicio: "", fecha_fin: "", estado: "", prioridad: "" });
  };

  // Crea un nuevo proyecto y asocia las tareas temporales
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
  // Edita un proyecto existente
const handleGuardarProyecto = async (e) => {
  e.preventDefault();
  if (!proyectoEditando) return;

  try {
    // Cloná y eliminá las propiedades extra
    const proyectoSanitizado = { ...proyectoEditando };
    delete proyectoSanitizado["$id"];
    delete proyectoSanitizado["id"]; // por si también hay un `id`

    const response = await fetch(`http://localhost:5119/api/Proyectos/${proyectoSanitizado.id_proyecto}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify(proyectoSanitizado),
    });
    console.log("Enviando:", proyectoSanitizado);
    if (!response.ok) throw new Error("Error al actualizar");

    setMostrarModal(false);
    setProyectoEditando(null);
    await cargarProyectos();

  } catch (err) {
    console.error("Error actualizando proyecto:", err);
  }
};

// Maneja la edición de una tarea existente
const handleGuardarTarea = async (e) => {
  e.preventDefault();

  if (!tareaEditando) return;
  const idProyecto = tareaEditando.id_proyecto; // guarda antes
   // Crear copia limpia sin propiedades no deseadas
    const {
      $id,
      id_proyectoNavigation,
      ...tareaLimpia
    } = tareaEditando;
  try {
    // Clonamos y limpiamos cualquier propiedad extra

    const response = await fetch(`http://localhost:5119/api/Tareas/${tareaEditando.id_tarea}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify(tareaLimpia),
    });

    if (!response.ok) {
      const mensaje = await response.text();
      throw new Error(mensaje || "Error al actualizar tarea");
    }

    setMostrarModalTarea(false);       // cerrar modal
    setTareaEditando(null);       // limpiar estado
    await cargarTareas(idProyecto);         // refrescar tareas
  } catch (err) {
    console.error("Error actualizando tarea:", err);
  }
};

// Abre el modal para agregar una nueva tarea a un proyecto específico
const abrirModalNuevaTarea = (idProyecto) => {
  setIdProyectoParaNuevaTarea(idProyecto);
  setNuevaTareaModal({
    titulo: "",
    fecha_inicio: "",
    fecha_fin: "",
    estado: "",
    prioridad: "",
  });
  setMostrarModalNuevaTarea(true);
};

// Maneja el guardado de una nueva tarea en el proyecto seleccionado
const handleGuardarNuevaTarea = async (e) => {
  e.preventDefault();

  if (!idProyectoParaNuevaTarea) return alert("Proyecto no seleccionado");

  try {
    // Crear tarea en API ligada al proyecto
    const response = await fetch("http://localhost:5119/api/Tareas", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify({ ...nuevaTareaModal, id_proyecto: idProyectoParaNuevaTarea }),
    });

    if (!response.ok) {
      const mensaje = await response.text();
      throw new Error(mensaje || "Error al crear tarea");
    }

    setMostrarModalNuevaTarea(false);
    setNuevaTareaModal({
      titulo: "",
      fecha_inicio: "",
      fecha_fin: "",
      estado: "",
      prioridad: "",
    });

    await cargarTareas(idProyectoParaNuevaTarea); // refrescar tareas
  } catch (error) {
    console.error("Error creando tarea:", error);
  }
};

// Renderiza la página de proyectos y tareas

  return (
    <div  className="d-flex gap-4 p-4">
      <div style={{
    flex: 1,
    padding: 20,
    borderRadius: 5,
    background: "linear-gradient(135deg, #f2f2f2, #e6f7ff)",
    boxShadow: "0 4px 8px rgba(0,0,0,0.1)"
  }}>
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
              value={nuevaTarea.fecha_inicio}
              onChange={(e) => setNuevaTarea({ ...nuevaTarea, fecha_inicio: e.target.value })}
            />
            <Form.Control
              type="datetime-local"
              value={nuevaTarea.fecha_fin}
              onChange={(e) => setNuevaTarea({ ...nuevaTarea, fecha_fin: e.target.value })}
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
        <Modal show={mostrarModal} onHide={() => setMostrarModal(false)}>
  <Modal.Header closeButton>
    <Modal.Title>Editar Proyecto</Modal.Title>
  </Modal.Header>
  <Modal.Body>
    {proyectoEditando && (
      <Form onSubmit={handleGuardarProyecto}>
        <Form.Group className="mb-3">
          <Form.Label>Nombre</Form.Label>
          <Form.Control
            type="text"
            value={proyectoEditando.nombre}
            onChange={(e) =>
              setProyectoEditando({ ...proyectoEditando, nombre: e.target.value })
            }
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Descripción</Form.Label>
          <Form.Control
            as="textarea"
            value={proyectoEditando.descripcion}
            onChange={(e) =>
              setProyectoEditando({ ...proyectoEditando, descripcion: e.target.value })
            }
          />
        </Form.Group>
        <Button variant="primary" type="submit">
          Guardar cambios
        </Button>
      </Form>
    )}
    {/* Modal para editar Proyecto */}
  </Modal.Body>
</Modal>
<Accordion>
          {proyectos.map((proyecto) => (
            <Accordion.Item eventKey={proyecto.id_proyecto.toString()} key={proyecto.id_proyecto}>
              <Accordion.Header onClick={() => cargarTareas(proyecto.id_proyecto)}>{proyecto.nombre}</Accordion.Header>
              <Accordion.Body>
                <Button // Botón para editar proyecto
                  variant="warning"
                  size="sm"
                  className="me-2"
                  onClick={() => {
                    setProyectoEditando(proyecto);
                    setMostrarModal(true);
                  }}
                >
                  Editar
                </Button>
                <Button variant="danger" size="sm" onClick={() => eliminarProyecto(proyecto.id_proyecto)}>
                  Eliminar 
                </Button> 
                <Button
                  style={{ marginLeft: "10px" }}
                  variant="success"
                  size="sm"
                  className="me-2"
                  onClick={() => abrirModalNuevaTarea(proyecto.id_proyecto)}
                >
                  Agregar Tarea
                </Button>


                <h6 className="mt-3">Tareas</h6>
                <ListGroup>
                  {(tareasPorProyecto[proyecto.id_proyecto] || []).map((tarea) => (
                    <ListGroup.Item key={tarea.id_tarea} className="d-flex justify-content-between align-items-center">
                      <span>
                        {tarea.titulo} ({tarea.fecha_inicio} - {tarea.fecha_fin}) <br />
                        Estado: {tarea.estado} | Prioridad: {tarea.prioridad}
                      </span>
                      <div>
                        <Button    // Botón para editar tarea
                          variant="warning"
                          size="sm"
                          className="me-2"
                          onClick={() => {
                            setTareaEditando({ ...tarea, id_proyecto: proyecto.id_proyecto });
                            setMostrarModalTarea(true);
                          }}
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
        {/* Modal para editar tarea */}
        <Modal show={mostrarModalTarea} onHide={() => setMostrarModalTarea(false)}>
  <Modal.Header closeButton>
    <Modal.Title>Editar Tarea</Modal.Title>
  </Modal.Header>
  <Modal.Body>
    {tareaEditando && (
      <Form onSubmit={handleGuardarTarea}>
        <Form.Group className="mb-2">
          <Form.Label>Título</Form.Label>
          <Form.Control
            type="text"
            value={tareaEditando.titulo}
            onChange={(e) => setTareaEditando({ ...tareaEditando, titulo: e.target.value })}
          />
        </Form.Group>
        <Form.Group className="mb-2">
          <Form.Label>Fecha Inicio</Form.Label>
          <Form.Control
            type="datetime-local"
            value={tareaEditando?.fecha_inicio?.slice(0, 16) ?? ""}
            onChange={(e) => setTareaEditando({ ...tareaEditando, fecha_inicio: e.target.value })}
          />
        </Form.Group>
        <Form.Group className="mb-2">
          <Form.Label>Fecha Fin</Form.Label>
          <Form.Control
            type="datetime-local"
            value={tareaEditando?.fecha_fin?.slice(0, 16) ?? ""}
            onChange={(e) => setTareaEditando({ ...tareaEditando, fecha_fin: e.target.value })}
          />
        </Form.Group>
        <Form.Group className="mb-2">
          <Form.Label>Estado</Form.Label>
          <Form.Select
            value={tareaEditando.estado}
            onChange={(e) => setTareaEditando({ ...tareaEditando, estado: e.target.value })}
          >
            <option value="Pendiente">Pendiente</option>
            <option value="En Proceso">En proceso</option>
            <option value="Finalizado">Completa</option>
          </Form.Select>
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Prioridad</Form.Label>
          <Form.Select
            value={tareaEditando.prioridad}
            onChange={(e) => setTareaEditando({ ...tareaEditando, prioridad: e.target.value })}
          >
            <option value="Bajo">Baja</option>
            <option value="Medio">Media</option>
            <option value="Alto">Alta</option>
          </Form.Select>
        </Form.Group>
        <Button variant="primary" type="submit">
          Guardar cambios
        </Button>
      </Form>
    )}
  </Modal.Body>
</Modal>
{/* Modal para agregar nueva tarea */}
<Modal show={mostrarModalNuevaTarea} onHide={() => setMostrarModalNuevaTarea(false)}>
  <Modal.Header closeButton>
    <Modal.Title>Agregar nueva tarea</Modal.Title>
  </Modal.Header>
  <Modal.Body>
    <Form onSubmit={handleGuardarNuevaTarea}>
      <Form.Group className="mb-2">
        <Form.Label>Título</Form.Label>
        <Form.Control
          type="text"
          value={nuevaTareaModal.titulo}
          onChange={(e) => setNuevaTareaModal({ ...nuevaTareaModal, titulo: e.target.value })}
          required
        />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Label>Fecha Inicio</Form.Label>
        <Form.Control
          type="datetime-local"
          value={nuevaTareaModal.fecha_inicio}
          onChange={(e) => setNuevaTareaModal({ ...nuevaTareaModal, fecha_inicio: e.target.value })}
          required
        />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Label>Fecha Fin</Form.Label>
        <Form.Control
          type="datetime-local"
          value={nuevaTareaModal.fecha_fin}
          onChange={(e) => setNuevaTareaModal({ ...nuevaTareaModal, fecha_fin: e.target.value })}
          required
        />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Label>Estado</Form.Label>
        <Form.Select
          value={nuevaTareaModal.estado}
          onChange={(e) => setNuevaTareaModal({ ...nuevaTareaModal, estado: e.target.value })}
          required
        >
          <option value="">Seleccionar estado</option>
          <option value="Pendiente">Pendiente</option>
          <option value="En Proceso">En proceso</option>
          <option value="Finalizado">Completa</option>
        </Form.Select>
      </Form.Group>
      <Form.Group className="mb-3">
        <Form.Label>Prioridad</Form.Label>
        <Form.Select
          value={nuevaTareaModal.prioridad}
          onChange={(e) => setNuevaTareaModal({ ...nuevaTareaModal, prioridad: e.target.value })}
          required
        >
          <option value="">Seleccionar prioridad</option>
          <option value="Bajo">Baja</option>
          <option value="Medio">Media</option>
          <option value="Alto">Alta</option>
        </Form.Select>
      </Form.Group>
      <Button variant="primary" type="submit" disabled={
        !nuevaTareaModal.titulo ||
        !nuevaTareaModal.fecha_inicio ||
        !nuevaTareaModal.fecha_fin ||
        !nuevaTareaModal.estado ||
        !nuevaTareaModal.prioridad
      }>
        Agregar tarea
      </Button>
    </Form>
  </Modal.Body>
</Modal>
      </div>
    </div>
  );
};

export default ProyectosTareasPage;
