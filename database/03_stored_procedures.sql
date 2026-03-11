-- 03_stored_procedures.sql: Procedimientos almacenados de inscripcion
-- sp_EnrollStudent: usa UPDLOCK+ROWLOCK para control de concurrencia
-- sp_GetClassmates: impone privacidad a nivel BD (solo Nombre + Apellido)
-- Codigos de error (SqlException.Number en C#):
--   50001 = Limite de 3 materias superado
--   50002 = Profesor duplicado
--   50003 = Materia sin cupo
--   50004 = Ya inscrito en esta materia

CREATE OR ALTER PROCEDURE sp_EnrollStudent
    @StudentId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @CupoActual     INT;
        DECLARE @CapacidadMaxima INT;
        DECLARE @IdProfesor      INT;
        DECLARE @TotalMaterias   INT;

        -- Bloqueo pesimista: evita sobrecupo concurrente
        SELECT
            @CupoActual     = COUNT(i.Id),
            @CapacidadMaxima = a.CapacidadMaxima,
            @IdProfesor      = a.IdProfesor
        FROM Asignaturas a WITH (UPDLOCK, ROWLOCK)
        LEFT JOIN Inscripciones i ON i.IdAsignatura = a.Id
        WHERE a.Id = @SubjectId
        GROUP BY a.CapacidadMaxima, a.IdProfesor;

        -- Regla 1: max 3 materias por estudiante
        SELECT @TotalMaterias = COUNT(*)
        FROM Inscripciones WITH (UPDLOCK, ROWLOCK)
        WHERE IdEstudiante = @StudentId;

        IF @TotalMaterias >= 3
            THROW 50001, 'No puedes inscribirte en mas de 3 materias.', 1;

        -- Regla 2: sin profesor duplicado
        IF EXISTS (
            SELECT 1
            FROM Inscripciones i
            JOIN Asignaturas a ON a.Id = i.IdAsignatura
            WHERE i.IdEstudiante = @StudentId
              AND a.IdProfesor = @IdProfesor
        )
            THROW 50002, 'Ya tienes una clase con este profesor.', 1;

        -- Regla 3: verificar cupo
        IF @CupoActual >= @CapacidadMaxima
            THROW 50003, 'Esta materia ya alcanzo su cupo maximo.', 1;

        -- Regla 4: ya inscrito
        IF EXISTS (
            SELECT 1 FROM Inscripciones
            WHERE IdEstudiante = @StudentId AND IdAsignatura = @SubjectId
        )
            THROW 50004, 'Ya estas inscrito en esta materia.', 1;

        INSERT INTO Inscripciones (IdEstudiante, IdAsignatura, FechaInscripcion)
        VALUES (@StudentId, @SubjectId, GETUTCDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Impone privacidad a nivel BD: nunca expone email ni datos sensibles
CREATE OR ALTER PROCEDURE sp_GetClassmates
    @SubjectId           INT,
    @RequestingStudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.Nombre,
        e.Apellido
    FROM Inscripciones i
    JOIN Estudiantes e ON e.Id = i.IdEstudiante
    WHERE i.IdAsignatura = @SubjectId
      AND i.IdEstudiante <> @RequestingStudentId;
END;
GO
