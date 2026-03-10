-- ============================================================
-- Sistema de Inscripción Estudiantil
-- Script completo: tablas, índices, SP y datos semilla
-- Base de datos: PruebaUniversidad (Azure SQL)
-- Servidor: studentenrollmentserver.database.windows.net
-- Compatibilidad: Azure SQL / SQL Server 2019+
-- ============================================================

-- En Azure SQL la BD ya existe, solo nos conectamos a ella
-- (ejecutar este script conectado directamente a PruebaUniversidad)

-- ============================================================
-- TABLAS
-- Los nombres de tabla vienen de los DbSet en ContextoBD.cs
-- Los nombres de columna vienen de las propiedades de la entidad
-- ============================================================

-- Primero las que no tienen dependencias
CREATE TABLE Profesores (
    Id           INT           IDENTITY(1,1) PRIMARY KEY,
    Nombre       NVARCHAR(150) NOT NULL,
    Email        NVARCHAR(200) NOT NULL
);
GO

CREATE TABLE Estudiantes (
    Id             INT            IDENTITY(1,1) PRIMARY KEY,
    Nombre         NVARCHAR(100)  NOT NULL,
    Apellido       NVARCHAR(100)  NOT NULL,
    Email          NVARCHAR(200)  NOT NULL,
    HashContrasena NVARCHAR(MAX)  NOT NULL,
    FechaCreacion  DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    VersionFila    ROWVERSION     NOT NULL   -- manejado automáticamente por SQL Server
);
GO

CREATE TABLE Asignaturas (
    Id              INT           IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(150) NOT NULL,
    Creditos        INT           NOT NULL DEFAULT 3,
    IdProfesor      INT           NOT NULL,
    CapacidadMaxima INT           NOT NULL DEFAULT 30,
    CONSTRAINT FK_Asignaturas_Profesores FOREIGN KEY (IdProfesor)
        REFERENCES Profesores(Id)
);
GO

CREATE TABLE Inscripciones (
    Id               INT       IDENTITY(1,1) PRIMARY KEY,
    IdEstudiante     INT       NOT NULL,
    IdAsignatura     INT       NOT NULL,
    FechaInscripcion DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Inscripciones_Estudiantes FOREIGN KEY (IdEstudiante)
        REFERENCES Estudiantes(Id),
    CONSTRAINT FK_Inscripciones_Asignaturas FOREIGN KEY (IdAsignatura)
        REFERENCES Asignaturas(Id)
);
GO

-- ============================================================
-- ÍNDICES
-- IX_Estudiantes_Email          → EF Core lo genera por HasIndex().IsUnique()
-- IX_Inscripciones_Estudiante_Asignatura → clave única compuesta
-- Los otros dos aceleran los JOINs más frecuentes
-- ============================================================

CREATE UNIQUE INDEX IX_Estudiantes_Email
    ON Estudiantes(Email);

CREATE UNIQUE INDEX IX_Inscripciones_Estudiante_Asignatura
    ON Inscripciones(IdEstudiante, IdAsignatura);

CREATE INDEX IX_Inscripciones_IdEstudiante
    ON Inscripciones(IdEstudiante);

CREATE INDEX IX_Inscripciones_IdAsignatura
    ON Inscripciones(IdAsignatura);

CREATE INDEX IX_Asignaturas_IdProfesor
    ON Asignaturas(IdProfesor);
GO

-- ============================================================
-- STORED PROCEDURE
-- Nombre y parámetros deben coincidir con RepositorioInscripciones.cs:
--   "EXEC sp_EnrollStudent @StudentId, @SubjectId"
-- Códigos de error deben coincidir con los catch del repositorio:
--   50001 → ExcepcionLimiteInscripciones
--   50002 → ExcepcionProfesorDuplicado
--   50003 → ExcepcionAsignaturaLlena
--   50004 → ExcepcionYaInscrito
-- ============================================================

CREATE OR ALTER PROCEDURE sp_EnrollStudent
    @StudentId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    BEGIN TRY

        -- Regla 1: No inscribirse dos veces en la misma materia
        IF EXISTS (
            SELECT 1 FROM Inscripciones
            WHERE IdEstudiante = @StudentId
              AND IdAsignatura = @SubjectId
        )
            THROW 50004, 'Ya estás inscrito en esta materia.', 1;

        -- Regla 2: Máximo 3 materias por estudiante
        IF (
            SELECT COUNT(*) FROM Inscripciones
            WHERE IdEstudiante = @StudentId
        ) >= 3
            THROW 50001, 'No puedes inscribirte en más de 3 materias.', 1;

        -- Regla 3: No repetir profesor
        IF EXISTS (
            SELECT 1
            FROM Inscripciones  i
            INNER JOIN Asignaturas a ON a.Id = i.IdAsignatura
            WHERE i.IdEstudiante = @StudentId
              AND a.IdProfesor = (
                  SELECT IdProfesor FROM Asignaturas WHERE Id = @SubjectId
              )
        )
            THROW 50002, 'Ya tienes una clase con este profesor.', 1;

        -- Regla 4: Verificar cupo disponible
        IF (
            SELECT COUNT(*) FROM Inscripciones
            WHERE IdAsignatura = @SubjectId
        ) >= (
            SELECT CapacidadMaxima FROM Asignaturas
            WHERE Id = @SubjectId
        )
            THROW 50003, 'Esta materia ya alcanzó su cupo máximo.', 1;

        -- Todo bien, inscribir
        INSERT INTO Inscripciones (IdEstudiante, IdAsignatura, FechaInscripcion)
        VALUES (@StudentId, @SubjectId, GETUTCDATE());

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW; -- re-lanza el error para que C# lo atrape
    END CATCH
END;
GO

-- ============================================================
-- DATOS SEMILLA
-- ============================================================

-- Profesores
INSERT INTO Profesores (Nombre, Email) VALUES
    ('Carlos Mendoza',   'c.mendoza@universidad.edu'),
    ('Ana García',       'a.garcia@universidad.edu'),
    ('Roberto Herrera',  'r.herrera@universidad.edu'),
    ('Laura Sánchez',    'l.sanchez@universidad.edu'),
    ('Miguel Torres',    'm.torres@universidad.edu');
GO

-- Asignaturas
-- Créditos y cupo por defecto (3 créditos, 30 estudiantes max)
-- Algunas con cupo reducido para probar la regla de capacidad
INSERT INTO Asignaturas (Nombre, Creditos, IdProfesor, CapacidadMaxima) VALUES
    ('Cálculo Diferencial',    4, 1, 30),
    ('Programación I',         3, 2, 30),
    ('Álgebra Lineal',         3, 3, 30),
    ('Bases de Datos',         3, 2, 25),
    ('Estructuras de Datos',   3, 4, 30),
    ('Física I',               4, 5, 30),
    ('Inglés Técnico',         2, 4, 35),
    ('Estadística',            3, 1, 30);
GO

-- Estudiantes de prueba
-- IMPORTANTE: HashContrasena es un hash BCrypt
-- Contraseña de todos los estudiantes de prueba: Test1234!
-- Para generar el hash: BCrypt.Net.BCrypt.HashPassword("Test1234!", 11)
-- O ejecutar esto en C#: Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Test1234!", 11));
-- El hash cambia cada vez que se genera (es por diseño de BCrypt), reemplaza los valores
-- de abajo por hashes generados en tu entorno o regístralos por la API /api/auth/register
INSERT INTO Estudiantes (Nombre, Apellido, Email, HashContrasena, FechaCreacion) VALUES
    ('Juan',    'Castillo',   'juan.castillo@correo.com',   '$2a$11$REEMPLAZA_CON_HASH_REAL_DE_Test1234!', GETUTCDATE()),
    ('María',   'López',      'maria.lopez@correo.com',     '$2a$11$REEMPLAZA_CON_HASH_REAL_DE_Test1234!', GETUTCDATE()),
    ('Andrés',  'Ramírez',    'andres.ramirez@correo.com',  '$2a$11$REEMPLAZA_CON_HASH_REAL_DE_Test1234!', GETUTCDATE());
GO

-- Inscripciones de ejemplo
-- Juan se inscribe en 3 materias (llega al límite)
EXEC sp_EnrollStudent @StudentId = 1, @SubjectId = 1;  -- Juan → Cálculo Diferencial
EXEC sp_EnrollStudent @StudentId = 1, @SubjectId = 2;  -- Juan → Programación I
EXEC sp_EnrollStudent @StudentId = 1, @SubjectId = 6;  -- Juan → Física I

-- María se inscribe en 2
EXEC sp_EnrollStudent @StudentId = 2, @SubjectId = 2;  -- María → Programación I
EXEC sp_EnrollStudent @StudentId = 2, @SubjectId = 3;  -- María → Álgebra Lineal

-- Andrés se inscribe en 1
EXEC sp_EnrollStudent @StudentId = 3, @SubjectId = 4;  -- Andrés → Bases de Datos
GO

-- ============================================================
-- VERIFICACIÓN
-- ============================================================

SELECT 'Profesores'   AS Tabla, COUNT(*) AS Total FROM Profesores   UNION ALL
SELECT 'Asignaturas'  AS Tabla, COUNT(*) AS Total FROM Asignaturas   UNION ALL
SELECT 'Estudiantes'  AS Tabla, COUNT(*) AS Total FROM Estudiantes   UNION ALL
SELECT 'Inscripciones'AS Tabla, COUNT(*) AS Total FROM Inscripciones;
GO

-- Vista rápida de inscripciones con nombres
SELECT
    e.Nombre + ' ' + e.Apellido   AS Estudiante,
    a.Nombre                      AS Asignatura,
    p.Nombre                      AS Profesor,
    a.Creditos,
    i.FechaInscripcion
FROM Inscripciones i
INNER JOIN Estudiantes  e ON e.Id = i.IdEstudiante
INNER JOIN Asignaturas  a ON a.Id = i.IdAsignatura
INNER JOIN Profesores   p ON p.Id = a.IdProfesor
ORDER BY e.Apellido, i.FechaInscripcion;
GO
