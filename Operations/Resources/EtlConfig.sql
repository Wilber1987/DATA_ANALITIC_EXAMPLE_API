
CREATE TABLE etl_config (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BeginDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Update_At DATETIME DEFAULT GETDATE(),
    
    -- Restricciones para garantizar integridad
    CONSTRAINT CHK_Fechas_Validas CHECK (BeginDate <= EndDate),
    CONSTRAINT UQ_Rango_Fechas UNIQUE (BeginDate, EndDate)
);
GO