-- Stored procedure: sp_EnrollStudent
-- Impone las reglas de negocio de inscripcion de forma atomica.
-- Usa UPDLOCK + ROWLOCK para evitar condiciones de carrera en inscripciones simultaneas.
-- Codigos de error (SqlException.Number en C#):
--   50001 = estudiante ya inscrito en 3 materias
--   50002 = estudiante ya tiene clase con ese profesor
--   50003 = materia sin cupo disponible
--   50004 = estudiante ya inscrito en esa materia

IF OBJECT_ID('dbo.sp_EnrollStudent', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_EnrollStudent;
GO

CREATE PROCEDURE dbo.sp_EnrollStudent
    @StudentId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @CupoActual     INT;
        DECLARE @CapacidadMaxima INT;
        DECLARE @IdProfesor      INT;
        DECLARE @TotalMaterias   INT;

        -- Bloqueo pesimista sobre la asignatura para evitar sobrecupo concurrente
        SELECT
            @CupoActual      = COUNT(i.Id),
            @CapacidadMaxima = a.CapacidadMaxima,
            @IdProfesor      = a.IdProfesor
        FROM Asignaturas a WITH (UPDLOCK, ROWLOCK)
        LEFT JOIN Inscripciones i ON i.IdAsignatura = a.Id
        WHERE a.Id = @SubjectId
        GROUP BY a.CapacidadMaxima, a.IdProfesor;

        -- Bloqueo sobre las inscripciones del estudiante
        SELECT @TotalMaterias = COUNT(*)
        FROM Inscripciones WITH (UPDLOCK, ROWLOCK)
        WHERE IdEstudiante = @StudentId;

        -- Regla 1: max 3 materias por estudiante
        IF @TotalMaterias >= 3
            THROW 50001, 'No puedes inscribirte en mas de 3 materias.', 1;

        -- Regla 2: sin profesor duplicado
        IF EXISTS (
            SELECT 1
            FROM Inscripciones i
            INNER JOIN Asignaturas a ON a.Id = i.IdAsignatura
            WHERE i.IdEstudiante = @StudentId
              AND a.IdProfesor = @IdProfesor
        )
            THROW 50002, 'Ya tienes una clase con este profesor.', 1;

        -- Regla 3: verificar cupo disponible
        IF @CupoActual >= @CapacidadMaxima
            THROW 50003, 'Esta materia ya alcanzo su cupo maximo.', 1;

        -- Regla 4: ya inscrito en esta materia
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
