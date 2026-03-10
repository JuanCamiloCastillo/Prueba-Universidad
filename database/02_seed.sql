-- 02_seed.sql: Seed data for 5 professors and 10 subjects

INSERT INTO Professors (Name, Email) VALUES
('Dr. Ana Garcia',    'ana.garcia@university.edu'),
('Dr. Carlos Lopez',  'carlos.lopez@university.edu'),
('Dr. Maria Martinez','maria.martinez@university.edu'),
('Dr. Juan Rodriguez','juan.rodriguez@university.edu'),
('Dr. Laura Sanchez', 'laura.sanchez@university.edu');

-- 10 subjects, 2 per professor, all 3 credits, MaxCapacity = 30
INSERT INTO Subjects (Name, Credits, ProfessorId, MaxCapacity) VALUES
('Calculus I',              3, 1, 30),
('Calculus II',             3, 1, 30),
('Data Structures',         3, 2, 30),
('Algorithms',              3, 2, 30),
('Database Systems',        3, 3, 30),
('Software Engineering',    3, 3, 30),
('Operating Systems',       3, 4, 30),
('Computer Networks',       3, 4, 30),
('Artificial Intelligence', 3, 5, 30),
('Machine Learning',        3, 5, 30);
