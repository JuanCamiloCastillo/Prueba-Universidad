-- 01_schema.sql: Tablas del sistema de inscripcion estudiantil
-- Nombres en espanol para coincidir con entidades C#

CREATE TABLE Profesores (
    Id     INT           IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Email  NVARCHAR(200) NOT NULL
);
GO

CREATE TABLE Estudiantes (
    Id             INT           IDENTITY(1,1) PRIMARY KEY,
    Nombre         NVARCHAR(100) NOT NULL,
    Apellido       NVARCHAR(100) NOT NULL,
    Email          NVARCHAR(200) NOT NULL,
    HashContrasena NVARCHAR(500) NOT NULL,
    FechaCreacion  DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    VersionFila    ROWVERSION    NOT NULL
);
GO

CREATE TABLE Asignaturas (
    Id              INT           IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(150) NOT NULL,
    Creditos        INT           NOT NULL DEFAULT 3,
    IdProfesor      INT           NOT NULL,
    CapacidadMaxima INT           NOT NULL DEFAULT 30,
    CONSTRAINT FK_Asignaturas_Profesores FOREIGN KEY (IdProfesor) REFERENCES Profesores(Id)
);
GO

CREATE TABLE Inscripciones (
    Id               INT       IDENTITY(1,1) PRIMARY KEY,
    IdEstudiante     INT       NOT NULL,
    IdAsignatura     INT       NOT NULL,
    FechaInscripcion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Inscripciones_Estudiantes FOREIGN KEY (IdEstudiante) REFERENCES Estudiantes(Id),
    CONSTRAINT FK_Inscripciones_Asignaturas FOREIGN KEY (IdAsignatura) REFERENCES Asignaturas(Id),
    CONSTRAINT UQ_Inscripcion UNIQUE (IdEstudiante, IdAsignatura)
);
GO

CREATE UNIQUE INDEX IX_Estudiantes_Email          ON Estudiantes(Email);
CREATE INDEX        IX_Inscripciones_IdEstudiante ON Inscripciones(IdEstudiante);
CREATE INDEX        IX_Inscripciones_IdAsignatura ON Inscripciones(IdAsignatura);
CREATE INDEX        IX_Asignaturas_IdProfesor     ON Asignaturas(IdProfesor);
GO
