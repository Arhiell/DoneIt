DROP DATABASE IF EXISTS DoneIt;
CREATE DATABASE DoneIt;
USE DoneIt;

-- Tabla de datos personales de los usuarios 
CREATE TABLE Persona (
    id_persona INT AUTO_INCREMENT PRIMARY KEY, 
    nombre VARCHAR(50) NOT NULL,            
    apellido VARCHAR(50) NOT NULL,             
    fecha_nacimiento DATE,                     
    email VARCHAR(150) UNIQUE NOT NULL        
);

-- Usuarios del sistema, enlazados con una persona
CREATE TABLE Usuario (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,    
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,     -- Nombre de usuario (verifiación en el login)
    password_hash VARCHAR(255) NOT NULL,            -- Contraseña cifrada (NO guardar en texto plano, revisar como hacer en el BackEnd)
    salt VARCHAR(255) NOT NULL,                     -- Salt usado para mejorar la seguridad del hash
    fecha_registro DATE,                            -- Fecha en que el usuario se registró
    token_recuperacion VARCHAR(255),                -- Token temporal para recuperación de contraseña
    vencimiento_token DATETIME,                     -- Fecha de vencimiento del token
    id_persona INT NOT NULL,                        -- Clave foránea a Persona
    FOREIGN KEY (id_persona) REFERENCES Persona(id_persona)
);

-- Tabla de estados posibles para una tarea
CREATE TABLE Estados (
    id_estado INT AUTO_INCREMENT PRIMARY KEY,       
    estado ENUM('Pendiente', 'En Proceso', 'Finalizado') NOT NULL 
);

-- Tabla de prioridades para asignar urgencia a una tarea
CREATE TABLE Prioridades (
    id_prioridad INT AUTO_INCREMENT PRIMARY KEY,    
    prioridad ENUM('Bajo', 'Medio', 'Alto') NOT NULL 
);

-- Proyectos, cada uno agrupando un conjunto de tareas
CREATE TABLE Proyectos (
    id_proyecto INT AUTO_INCREMENT PRIMARY KEY,        
    nombre_Proyecto VARCHAR(100) NOT NULL,                      
    descripcion TEXT,         
    fecha_creacion DATE ,             					   -- Fecha de creación
    id_usuario_creador INT NOT NULL,                       -- Usuario que creó el proyecto
    FOREIGN KEY (id_usuario_creador) REFERENCES Usuario(id_usuario)
);

-- Tareas individuales dentro de un proyecto o independientes
CREATE TABLE Tareas (
    id_tarea INT AUTO_INCREMENT PRIMARY KEY,            -- Identificador único de la tarea
    titulo VARCHAR(150) NOT NULL,                       -- Título corto de la tarea
    descripcion TEXT,                                   -- Detalle de la tarea
    fecha_inicio DATE,                                  -- Fecha de inicio de la tarea
    fecha_limite DATE,                                  -- Fecha límite para terminarla, Se puede modificar, para dar mas tiempo
    id_estado INT NOT NULL,                             -- Estado actual de la tarea (FK a Estados)
    id_prioridad INT NOT NULL,                          -- Prioridad asignada (FK a Prioridades)
    id_proyecto INT,                                    -- Proyecto al que pertenece (puede ser NULL)
    id_usuario_creador INT NOT NULL,                    -- Usuario que creó la tarea
    FOREIGN KEY (id_estado) REFERENCES Estados(id_estado),
    FOREIGN KEY (id_prioridad) REFERENCES Prioridades(id_prioridad),
    FOREIGN KEY (id_proyecto) REFERENCES Proyectos(id_proyecto),
    FOREIGN KEY (id_usuario_creador) REFERENCES Usuario(id_usuario)
);

-- Asociación muchos a muchos entre tareas y usuarios asignados
CREATE TABLE Tareas_Usuarios (
    id_tarea INT,                                       -- Clave foránea a Tareas
    id_usuario INT,                                     -- Clave foránea a Usuario
    PRIMARY KEY (id_tarea, id_usuario),                 -- Clave compuesta para evitar duplicados
    FOREIGN KEY (id_tarea) REFERENCES Tareas(id_tarea) ON DELETE CASCADE,
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE
);

-- Códigos QR generados para identificar tareas de forma única (por ejemplo, para escanear y abrir tarea)
-- EN PROCESO A MODIFICACIONES
CREATE TABLE QRCodes (
    id_QRCodes INT AUTO_INCREMENT PRIMARY KEY,          -- Identificador único del código QR
    codigo VARCHAR(255) NOT NULL UNIQUE,                -- Valor codificado en el QR (token o URL)
    id_tarea INT NOT NULL,                              -- Tarea a la que pertenece el QR
    fecha_generacion DATETIME DEFAULT CURRENT_TIMESTAMP,-- Fecha en que se generó el QR
    FOREIGN KEY (id_tarea) REFERENCES Tareas(id_tarea)
);
