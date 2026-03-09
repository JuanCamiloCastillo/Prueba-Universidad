-- Stored procedure: sp_EnrollStudent
-- Enforces enrollment business rules atomically:
--   50001 = student already enrolled in 3 subjects
--   50002 = student already has a class with this professor
--   50003 = subject has reached its max capacity
--   50004 = student is already enrolled in this subject

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

    -- Rule 1: student cannot enroll in more than 3 subjects
    IF (SELECT COUNT(*) FROM Enrollments WHERE StudentId = @StudentId) >= 3
    BEGIN
        ROLLBACK;
        RAISERROR('Student cannot enroll in more than 3 subjects.', 16, 1) WITH SETERROR;
        RETURN 50001;
    END;

    -- Rule 2: student cannot have two classes with the same professor
    IF EXISTS (
        SELECT 1
        FROM Enrollments e
        INNER JOIN Subjects s ON s.Id = e.SubjectId
        WHERE e.StudentId = @StudentId
          AND s.ProfessorId = (SELECT ProfessorId FROM Subjects WHERE Id = @SubjectId)
    )
    BEGIN
        ROLLBACK;
        RAISERROR('Student already has a class with this professor.', 16, 1) WITH SETERROR;
        RETURN 50002;
    END;

    -- Rule 3: subject must not be at full capacity
    IF (
        SELECT COUNT(*) FROM Enrollments WHERE SubjectId = @SubjectId
    ) >= (
        SELECT MaxCapacity FROM Subjects WHERE Id = @SubjectId
    )
    BEGIN
        ROLLBACK;
        RAISERROR('Subject has reached its maximum capacity.', 16, 1) WITH SETERROR;
        RETURN 50003;
    END;

    -- Rule 4: student must not already be enrolled in this subject
    IF EXISTS (
        SELECT 1 FROM Enrollments
        WHERE StudentId = @StudentId AND SubjectId = @SubjectId
    )
    BEGIN
        ROLLBACK;
        RAISERROR('Student is already enrolled in this subject.', 16, 1) WITH SETERROR;
        RETURN 50004;
    END;

    INSERT INTO Enrollments (StudentId, SubjectId, EnrolledAt)
    VALUES (@StudentId, @SubjectId, GETUTCDATE());

    COMMIT;
END;
GO
