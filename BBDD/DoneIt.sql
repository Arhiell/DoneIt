-- Crear usuario con plugin mysql_native_password
CREATE USER IF NOT EXISTS'doneit_user'@'localhost' IDENTIFIED WITH mysql_native_password BY 'doneit123';

-- Otorga permisos sobre la BD
GRANT ALL PRIVILEGES ON DoneIt.* TO 'doneit_user'@'localhost';
FLUSH PRIVILEGES;

-- Crear la base de datos y usarla
CREATE DATABASE IF NOT EXISTS DoneIt;
USE DoneIt;

-- Tabla Usuario (sin el campo 'salt')
CREATE TABLE Usuario (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    fecha_nacimiento DATE,
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL, -- Contiene hash + salt
    fecha_registro DATE,
    token_recuperacion VARCHAR(255),
    vencimiento_token DATETIME
);

-- Tabla Proyecto
CREATE TABLE Proyectos (
    id_proyecto INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    fecha_creacion DATE,
    id_usuario INT NOT NULL,
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario)
);

-- Tabla Tarea
CREATE TABLE Tareas (
    id_tarea INT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(150) NOT NULL,
    descripcion TEXT,
    fecha_inicio DATETIME,
    fecha_fin DATETIME,
    estado ENUM('Pendiente', 'En Proceso', 'Finalizado') NOT NULL,
    prioridad ENUM('Bajo', 'Medio', 'Alto') NOT NULL,
    id_proyecto INT NOT NULL,
    FOREIGN KEY (id_proyecto) REFERENCES Proyectos(id_proyecto)
);

-- Datos de prueba (password_hash incluye hash + salt)
INSERT INTO Usuario (nombre, apellido, email, fecha_nacimiento, nombre_usuario, password_hash, fecha_registro) VALUES 
('María', 'López', 'maria@example.com', '1992-03-15', 'maria92', 'hashsalt_maria', CURDATE()),
('Carlos', 'Fernández', 'carlos@example.com', '1988-09-22', 'carlos88', 'hashsalt_carlos', CURDATE());

INSERT INTO Proyectos (nombre, descripcion, fecha_creacion, id_usuario) VALUES 
('Proyecto Personal', 'Organización de tareas personales', CURDATE(), 1),
('Estudios y Cursos', 'Seguimiento de cursos online', CURDATE(), 1),
('Trabajo Freelance', 'Actividades laborales como freelancer', CURDATE(), 2);

INSERT INTO Tareas (titulo, descripcion, fecha_inicio, fecha_fin, estado, prioridad, id_proyecto) VALUES 
('Ordenar habitación', 'Limpiar, ordenar y tirar cosas viejas.', NOW(), NOW() + INTERVAL 1 DAY, 'Pendiente', 'Medio', 1),
('Revisión médica', 'Turno con el clínico el lunes.', NOW(), NOW() + INTERVAL 2 DAY, 'En Proceso', 'Bajo', 1),
('Terminar curso de SQL', 'Completar módulo 5 y hacer el examen final.', NOW(), NOW() + INTERVAL 3 DAY, 'Pendiente', 'Alto', 2),
('Leer capítulo de redes', 'Leer capítulo 3 del libro de redes.', NOW(), NOW() + INTERVAL 2 DAY, 'En Proceso', 'Medio', 2),
('Practicar ejercicios C#', 'Resolver al menos 3 ejercicios de listas.', NOW(), NOW() + INTERVAL 1 DAY, 'Finalizado', 'Alto', 2),
('Diseñar landing page', 'Hacer mockup y diseño en Figma.', NOW(), NOW() + INTERVAL 4 DAY, 'Pendiente', 'Alto', 3);
