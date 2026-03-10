-- 01_schema.sql: Create tables for StudentEnrollment database

CREATE TABLE Professors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Email NVARCHAR(200) NOT NULL
);

CREATE TABLE Subjects (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Credits INT NOT NULL DEFAULT 3,
    ProfessorId INT NOT NULL,
    MaxCapacity INT NOT NULL DEFAULT 30,
    CONSTRAINT FK_Subjects_Professors FOREIGN KEY (ProfessorId) REFERENCES Professors(Id)
);

CREATE TABLE Students (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    RowVersion ROWVERSION NOT NULL
);

CREATE TABLE Enrollments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    SubjectId INT NOT NULL,
    EnrolledAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Enrollments_Students FOREIGN KEY (StudentId) REFERENCES Students(Id),
    CONSTRAINT FK_Enrollments_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(Id),
    CONSTRAINT UQ_Enrollment_Student_Subject UNIQUE (StudentId, SubjectId)
);

-- Indexes
CREATE UNIQUE INDEX IX_Students_Email ON Students(Email);
CREATE INDEX IX_Enrollments_StudentId ON Enrollments(StudentId);
CREATE INDEX IX_Enrollments_SubjectId ON Enrollments(SubjectId);
CREATE INDEX IX_Subjects_ProfessorId ON Subjects(ProfessorId);
