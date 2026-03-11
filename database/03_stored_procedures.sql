-- sp_EnrollStudent + sp_GetClassmates
-- UPDLOCK + ROWLOCK para control de concurrencia en inscripciones simultaneas.
-- Codigos de error (SqlException.Number en C#):
--   50001 = max 3 materias superado
--   50002 = profesor duplicado
--   50003 = materia sin cupo
--   50004 = ya inscrito en esta materia

CREATE OR ALTER PROCEDURE dbo.sp_EnrollStudent
    @StudentId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY

        DECLARE @CapacidadMaxima INT;
        DECLARE @IdProfesor      INT;
        DECLARE @CupoActual      INT;
        DECLARE @TotalMaterias   INT;

        -- Bloqueo sobre la asignatura para evitar sobrecupo concurrente
        SELECT @CapacidadMaxima = CapacidadMaxima,
               @IdProfesor      = IdProfesor
        FROM   Asignaturas WITH (UPDLOCK, ROWLOCK)
        WHERE  Id = @SubjectId;

        -- Cupo actual de la asignatura
        SELECT @CupoActual = COUNT(*)
        FROM   Inscripciones WITH (ROWLOCK)
        WHERE  IdAsignatura = @SubjectId;

        -- Total de materias del estudiante
        SELECT @TotalMaterias = COUNT(*)
        FROM   Inscripciones WITH (UPDLOCK, ROWLOCK)
        WHERE  IdEstudiante = @StudentId;

        -- Regla 1: max 3 materias
        IF @TotalMaterias >= 3
        BEGIN
            THROW 50001, 'No puedes inscribirte en mas de 3 materias.', 1;
        END;

        -- Regla 2: sin profesor duplicado
        IF EXISTS (
            SELECT 1
            FROM   Inscripciones AS ins
            INNER JOIN Asignaturas AS asig ON asig.Id = ins.IdAsignatura
            WHERE  ins.IdEstudiante = @StudentId
              AND  asig.IdProfesor  = @IdProfesor
        )
        BEGIN
            THROW 50002, 'Ya tienes una clase con este profesor.', 1;
        END;

        -- Regla 3: cupo disponible
        IF @CupoActual >= @CapacidadMaxima
        BEGIN
            THROW 50003, 'Esta materia ya alcanzo su cupo maximo.', 1;
        END;

        -- Regla 4: ya inscrito
        IF EXISTS (
            SELECT 1
            FROM   Inscripciones
            WHERE  IdEstudiante = @StudentId
              AND  IdAsignatura = @SubjectId
        )
        BEGIN
            THROW 50004, 'Ya estas inscrito en esta materia.', 1;
        END;

        INSERT INTO Inscripciones (IdEstudiante, IdAsignatura, FechaInscripcion)
        VALUES (@StudentId, @SubjectId, GETUTCDATE());

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- Impone privacidad a nivel BD: nunca expone email ni datos sensibles
CREATE OR ALTER PROCEDURE dbo.sp_GetClassmates
    @SubjectId           INT,
    @RequestingStudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT est.Nombre,
           est.Apellido
    FROM   Inscripciones  AS ins
    INNER JOIN Estudiantes AS est ON est.Id = ins.IdEstudiante
    WHERE  ins.IdAsignatura  = @SubjectId
      AND  ins.IdEstudiante <> @RequestingStudentId;
END;
GO
