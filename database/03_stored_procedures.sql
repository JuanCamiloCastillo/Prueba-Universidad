-- 03_stored_procedures.sql: Stored procedures for enrollment operations

-- sp_EnrollStudent: Enrolls a student in a subject with concurrency control
-- Uses UPDLOCK + ROWLOCK to prevent race conditions when checking capacity
-- Error codes:
--   50001 = Max 3 subjects exceeded
--   50002 = Duplicate professor (student already has class with this professor)
--   50003 = Subject at max capacity
--   50004 = Already enrolled in this subject
CREATE OR ALTER PROCEDURE sp_EnrollStudent
    @StudentId INT,
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Acquire row-level lock on the subject to prevent concurrent over-enrollment
        DECLARE @CurrentEnrollment INT;
        DECLARE @MaxCapacity       INT;
        DECLARE @ProfessorId       INT;
        DECLARE @StudentCount      INT;

        SELECT
            @CurrentEnrollment = COUNT(*),
            @MaxCapacity       = s.MaxCapacity,
            @ProfessorId       = s.ProfessorId
        FROM Subjects s WITH (UPDLOCK, ROWLOCK)
        LEFT JOIN Enrollments e ON e.SubjectId = s.Id
        WHERE s.Id = @SubjectId
        GROUP BY s.MaxCapacity, s.ProfessorId;

        -- Rule 1: Max 3 subjects per student
        SELECT @StudentCount = COUNT(*)
        FROM Enrollments WITH (UPDLOCK, ROWLOCK)
        WHERE StudentId = @StudentId;

        IF @StudentCount >= 3
        BEGIN
            RAISERROR('Student has reached the maximum of 3 subjects.', 16, 1) WITH SETERROR;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Rule 2: No duplicate professor
        IF EXISTS (
            SELECT 1
            FROM Enrollments e
            JOIN Subjects s ON s.Id = e.SubjectId
            WHERE e.StudentId = @StudentId
              AND s.ProfessorId = @ProfessorId
        )
        BEGIN
            RAISERROR('Student already has a class with this professor.', 16, 2) WITH SETERROR;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Rule 3: Subject capacity
        IF @CurrentEnrollment >= @MaxCapacity
        BEGIN
            RAISERROR('Subject has reached its maximum capacity.', 16, 3) WITH SETERROR;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Rule 4: Already enrolled
        IF EXISTS (
            SELECT 1 FROM Enrollments
            WHERE StudentId = @StudentId AND SubjectId = @SubjectId
        )
        BEGIN
            RAISERROR('Student is already enrolled in this subject.', 16, 4) WITH SETERROR;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- Insert enrollment
        INSERT INTO Enrollments (StudentId, SubjectId, EnrolledAt)
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

-- sp_GetClassmates: Returns only FirstName + LastName of classmates (never email or sensitive data)
CREATE OR ALTER PROCEDURE sp_GetClassmates
    @SubjectId        INT,
    @RequestingStudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.FirstName,
        s.LastName
    FROM Enrollments e
    JOIN Students s ON s.Id = e.StudentId
    WHERE e.SubjectId = @SubjectId
      AND e.StudentId <> @RequestingStudentId;
END;
GO
