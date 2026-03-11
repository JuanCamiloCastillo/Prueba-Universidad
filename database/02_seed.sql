-- 02_seed.sql: Datos de prueba - 5 profesores y 8 asignaturas

INSERT INTO Profesores (Nombre, Email) VALUES
('Carlos Mendoza',   'c.mendoza@universidad.edu'),
('Ana Garcia',       'a.garcia@universidad.edu'),
('Roberto Herrera',  'r.herrera@universidad.edu'),
('Laura Sanchez',    'l.sanchez@universidad.edu'),
('Miguel Torres',    'm.torres@universidad.edu');

INSERT INTO Asignaturas (Nombre, Creditos, IdProfesor, CapacidadMaxima) VALUES
('Calculo Diferencial',    4, 1, 30),
('Programacion I',         3, 2, 30),
('Algebra Lineal',         3, 3, 30),
('Bases de Datos',         3, 2, 25),
('Estructuras de Datos',   3, 4, 30),
('Fisica I',               4, 5, 30),
('Ingles Tecnico',         2, 4, 35),
('Estadistica',            3, 1, 30);
