/* =====================================================================
   03_BaseDatos.sql  -  UniCampus (Registro de Actividades Académicas)
   Script reproducible: crea la estructura y carga datos de prueba.
   - Se puede ejecutar varias veces sin duplicar nada.
   - Si el equipo ya tiene una base/tablas equivalentes, comparar con el
     diagrama entidad-relación ANTES de ejecutar y ajustar nombres.
   ===================================================================== */

IF DB_ID(N'UniCampusDB') IS NULL
    CREATE DATABASE UniCampusDB;
GO

USE UniCampusDB;
GO

/* ---------- 1. TipoActividad ---------- */
IF OBJECT_ID(N'dbo.TipoActividad', N'U') IS NULL
CREATE TABLE dbo.TipoActividad (
    TipoId      INT IDENTITY(1,1) CONSTRAINT PK_TipoActividad PRIMARY KEY,
    Nombre      NVARCHAR(50)  NOT NULL CONSTRAINT UQ_TipoActividad_Nombre UNIQUE,
    Descripcion NVARCHAR(200) NULL
);
GO

/* ---------- 2. ActividadAcademica ---------- */
IF OBJECT_ID(N'dbo.ActividadAcademica', N'U') IS NULL
CREATE TABLE dbo.ActividadAcademica (
    ActividadId   INT IDENTITY(1,1) CONSTRAINT PK_ActividadAcademica PRIMARY KEY,
    Titulo        NVARCHAR(120)  NOT NULL,
    Descripcion   NVARCHAR(1000) NULL,
    TipoId        INT            NOT NULL,
    FechaInicio   DATE           NOT NULL,
    FechaFin      DATE           NOT NULL,
    HoraInicio    TIME(0)        NOT NULL,
    HoraFin       TIME(0)        NOT NULL,
    Modalidad     NVARCHAR(20)   NOT NULL,
    Lugar         NVARCHAR(150)  NOT NULL,
    Cupos         INT            NOT NULL,
    Estado        NVARCHAR(20)   NOT NULL CONSTRAINT DF_Actividad_Estado DEFAULT (N'Activa'),
    FechaCreacion DATETIME2      NOT NULL CONSTRAINT DF_Actividad_FechaCreacion DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Actividad_Tipo      FOREIGN KEY (TipoId) REFERENCES dbo.TipoActividad(TipoId),
    CONSTRAINT CK_Actividad_Cupos     CHECK (Cupos > 0),
    CONSTRAINT CK_Actividad_Fechas    CHECK (FechaFin >= FechaInicio),
    CONSTRAINT CK_Actividad_Horas     CHECK (HoraFin > HoraInicio),
    CONSTRAINT CK_Actividad_Modalidad CHECK (Modalidad IN (N'Presencial', N'Virtual', N'Híbrida')),
    CONSTRAINT CK_Actividad_Estado    CHECK (Estado IN (N'Activa', N'Próxima', N'Finalizada', N'Inactiva'))
);
GO

/* ---------- 3. Usuario ---------- */
IF OBJECT_ID(N'dbo.Usuario', N'U') IS NULL
CREATE TABLE dbo.Usuario (
    UsuarioId INT IDENTITY(1,1) CONSTRAINT PK_Usuario PRIMARY KEY,
    Nombre    NVARCHAR(100) NOT NULL,
    Correo    NVARCHAR(150) NOT NULL CONSTRAINT UQ_Usuario_Correo UNIQUE,
    Programa  NVARCHAR(100) NULL,
    Rol       NVARCHAR(30)  NOT NULL,
    Estado    NVARCHAR(20)  NOT NULL CONSTRAINT DF_Usuario_Estado DEFAULT (N'Activo'),
    CONSTRAINT CK_Usuario_Rol CHECK (Rol IN (N'Estudiante', N'Administrador', N'Organizador'))
);
GO

/* ---------- 4. Inscripcion ---------- */
IF OBJECT_ID(N'dbo.Inscripcion', N'U') IS NULL
CREATE TABLE dbo.Inscripcion (
    InscripcionId    INT IDENTITY(1,1) CONSTRAINT PK_Inscripcion PRIMARY KEY,
    ActividadId      INT          NOT NULL,
    UsuarioId        INT          NOT NULL,
    FechaInscripcion DATETIME2    NOT NULL CONSTRAINT DF_Inscripcion_Fecha DEFAULT (SYSUTCDATETIME()),
    Estado           NVARCHAR(20) NOT NULL CONSTRAINT DF_Inscripcion_Estado DEFAULT (N'Activa'),
    CONSTRAINT FK_Inscripcion_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.ActividadAcademica(ActividadId),
    CONSTRAINT FK_Inscripcion_Usuario   FOREIGN KEY (UsuarioId)   REFERENCES dbo.Usuario(UsuarioId),
    CONSTRAINT UQ_Inscripcion_Actividad_Usuario UNIQUE (ActividadId, UsuarioId),
    CONSTRAINT CK_Inscripcion_Estado CHECK (Estado IN (N'Activa', N'Cancelada'))
);
GO

/* ---------- 5. Resena ---------- */
IF OBJECT_ID(N'dbo.Resena', N'U') IS NULL
CREATE TABLE dbo.Resena (
    ResenaId      INT IDENTITY(1,1) CONSTRAINT PK_Resena PRIMARY KEY,
    ActividadId   INT           NOT NULL,
    UsuarioId     INT           NOT NULL,
    Calificacion  TINYINT       NOT NULL,
    Comentario    NVARCHAR(1000) NULL,
    FechaCreacion DATETIME2     NOT NULL CONSTRAINT DF_Resena_Fecha DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT CK_Resena_Calificacion CHECK (Calificacion BETWEEN 1 AND 5),
    CONSTRAINT FK_Resena_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.ActividadAcademica(ActividadId),
    CONSTRAINT FK_Resena_Usuario   FOREIGN KEY (UsuarioId)   REFERENCES dbo.Usuario(UsuarioId),
    CONSTRAINT UQ_Resena_Actividad_Usuario UNIQUE (ActividadId, UsuarioId)
);
GO

/* ---------- Índices de apoyo ---------- */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Actividad_Estado_Fecha')
    CREATE INDEX IX_Actividad_Estado_Fecha ON dbo.ActividadAcademica (Estado, FechaInicio);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Inscripcion_Actividad')
    CREATE INDEX IX_Inscripcion_Actividad ON dbo.Inscripcion (ActividadId);
GO

/* =====================================================================
   DATOS DE PRUEBA (ficticios). Solo se insertan si la tabla está vacía.
   ===================================================================== */
IF NOT EXISTS (SELECT 1 FROM dbo.TipoActividad)
INSERT INTO dbo.TipoActividad (Nombre, Descripcion) VALUES
(N'Taller',      N'Actividad práctica guiada'),
(N'Conferencia', N'Charla magistral'),
(N'Seminario',   N'Sesión de estudio especializada'),
(N'Curso',       N'Formación por módulos'),
(N'Encuentro',   N'Reunión académica o de investigación');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario)
INSERT INTO dbo.Usuario (Nombre, Correo, Programa, Rol) VALUES
(N'María López',       N'maria.lopez@ejemplo.edu',  N'Ingeniería de Sistemas', N'Estudiante'),
(N'Ana Torres',        N'ana.torres@ejemplo.edu',   N'Ingeniería de Sistemas', N'Estudiante'),
(N'Carlos Pérez',      N'carlos.perez@ejemplo.edu', N'Diseño Gráfico',         N'Estudiante'),
(N'Laura Gómez',       N'laura.gomez@ejemplo.edu',  N'Administración',         N'Estudiante'),
(N'Admin UniCampus',   N'admin@ejemplo.edu',        NULL,                      N'Administrador');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ActividadAcademica)
INSERT INTO dbo.ActividadAcademica
    (Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupos, Estado)
SELECT v.Titulo, v.Descripcion, t.TipoId, v.FechaInicio, v.FechaFin, v.HoraInicio, v.HoraFin,
       v.Modalidad, v.Lugar, v.Cupos, v.Estado
FROM (VALUES
 (N'Taller de Programación Web',
  N'Espacio práctico para reforzar conceptos de programación web con ASP.NET.',
  N'Taller',      '20261112', '20261113', '08:00', '12:00', N'Presencial', N'Laboratorio 3',      30,  N'Activa'),
 (N'Conferencia de IA',
  N'Panorama actual de la inteligencia artificial y sus aplicaciones.',
  N'Conferencia', '20261120', '20261120', '14:00', '16:00', N'Híbrida',    N'Auditorio principal', 100, N'Próxima'),
 (N'Seminario de Bases de Datos',
  N'Modelado, consultas y buenas prácticas en SQL Server.',
  N'Seminario',   '20261205', '20261205', '09:00', '12:00', N'Virtual',    N'Plataforma Teams',   50,  N'Próxima'),
 (N'Curso de Diseño UX/UI',
  N'Fundamentos de experiencia e interfaz de usuario.',
  N'Curso',       '20261210', '20261212', '14:00', '18:00', N'Presencial', N'Laboratorio 2',      40,  N'Próxima'),
 (N'Encuentro de Investigación',
  N'Presentación de avances de semilleros de investigación.',
  N'Encuentro',   '20261015', '20261015', '08:00', '13:00', N'Presencial', N'Sala de conferencias', 80, N'Finalizada')
) AS v(Titulo, Descripcion, Tipo, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupos, Estado)
INNER JOIN dbo.TipoActividad t ON t.Nombre = v.Tipo;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Inscripcion)
INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId)
SELECT a.ActividadId, u.UsuarioId
FROM dbo.ActividadAcademica a
CROSS JOIN dbo.Usuario u
WHERE a.Titulo = N'Taller de Programación Web'
  AND u.Correo IN (N'ana.torres@ejemplo.edu', N'carlos.perez@ejemplo.edu', N'laura.gomez@ejemplo.edu');
GO

/* ---------- Verificación rápida ---------- */
SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.Cupos, a.Estado,
       (SELECT COUNT(*) FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos
FROM dbo.ActividadAcademica a
INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId
ORDER BY a.FechaInicio;
GO
