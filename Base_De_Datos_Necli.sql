USE necli;

-- Eliminar tablas si existen
DROP TABLE Transacciones;
DROP TABLE Cuentas;
DROP TABLE Usuarios;

-- Tabla Usuarios
CREATE TABLE Usuarios (
    Id VARCHAR(20) PRIMARY KEY,
    NombreUsuario VARCHAR(100) NOT NULL,
    ApellidoUsuario VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Contrasena VARCHAR(300) NOT NULL,
    TipoUsuario int NOT NULL,
    FechaCreacion Datetime2 DEFAULT GETDATE(),
    FechaNacimiento Datetime2 NOT NULL,
	TokenVerificacion varchar(100),
	CorreoVerificado bit,
);

-- Tabla Cuentas
CREATE TABLE Cuentas (
    Numero BIGINT PRIMARY KEY,
    UsuarioId VARCHAR(20) NOT NULL,
    Saldo DECIMAL(18, 2) DEFAULT 0.00,
    FechaCreacion Datetime2 DEFAULT GETDATE(),
);

-- Tabla Transacciones
CREATE TABLE Transacciones (
    NumeroTransaccion INT PRIMARY KEY IDENTITY(1,1),
    FechaTransaccion Datetime2,
    NumeroCuentaOrigen BIGINT NOT NULL,
    NumeroCuentaDestino BIGINT NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL,
    TipoTransaccion int NOT NULL,
	Banco varchar(50)
);

-- Datos de Prueba
USE necli;

-- Insertar usuarios
INSERT INTO Usuarios (Id, NombreUsuario, ApellidoUsuario, Correo, Contrasena, TipoUsuario, FechaNacimiento, TokenVerificacion, CorreoVerificado)
VALUES
('U001', 'Juan', 'Pérez', 'juan.perez@mail.com', 'contrasena123', 1, '1990-05-10', 'tokenjuan001', 1),
('U002', 'María', 'Gómez', 'maria.gomez@mail.com', 'contrasena456', 1, '1985-08-22', 'tokenmaria002', 1),
('U003', 'Carlos', 'Ramírez', 'carlos.ramirez@mail.com', 'contrasena789', 2, '1978-03-15', 'tokencarlos003', 0),
('U004', 'Ana', 'Lopez', 'ana.lopez@mail.com', 'contrasena321', 1, '1995-11-30', 'tokenana004', 1),
('U005', 'Luis', 'Martínez', 'luis.martinez@mail.com', 'contrasena654', 2, '1988-07-07', 'tokenluis005', 0);

-- Insertar cuentas
INSERT INTO Cuentas (Numero, UsuarioId, Saldo, FechaCreacion)
VALUES
(1000000001, 'U001', 1500000.00, GETDATE()),
(1000000002, 'U002', 3000000.00, GETDATE()),
(1000000003, 'U003', 500000.00, GETDATE()),
(1000000004, 'U004', 750000.00, GETDATE()),
(1000000005, 'U005', 1200000.00, GETDATE());


-- Consultas de prueba
SELECT * FROM Usuarios;
SELECT * FROM Cuentas;
SELECT * FROM Transacciones;



