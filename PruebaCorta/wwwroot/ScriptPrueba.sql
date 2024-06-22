CREATE DATABASE PruebaCortaTec

USE PruebaCortaTec

CREATE TABLE Usuarios (
Id INT IDENTITY(1,1) PRIMARY KEY,
NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
Contrasena VARCHAR(100) NOT NULL,

   
);

CREATE TABLE Preguntas(
Id INT IDENTITY(1,1) PRIMARY KEY,
Texto VARCHAR(300)	 NOT NULL,
FechaCreacion DATETIME NOT NULL,
Estatus BIT NOT NULL,
UsuarioId INT NOT NULL,
CONSTRAINT FK_preguntas_usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(id)
);

CREATE TABLE Respuestas (
Id INT IDENTITY(1,1) PRIMARY KEY,
FechaCreacion DATETIME NOT NULL,

Texto VARCHAR(300)	 NOT NULL,
PreguntaId INT NOT NULL,
UsuarioId INT NOT NULL,

CONSTRAINT FK_respuestas_preguntas FOREIGN KEY (PreguntaId) REFERENCES Preguntas(id),
CONSTRAINT FK_respuestas_usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(id)
);
