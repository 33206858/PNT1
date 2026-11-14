-- Datos ficticios para probar la administración. No descuentan stock.
SET NOCOUNT ON;
DECLARE @Lavanda int = (SELECT Id FROM dbo.Productos WHERE Codigo = N'MEN-4575');
DECLARE @Vainilla int = (SELECT Id FROM dbo.Productos WHERE Codigo = N'ARO-1002');
IF @Lavanda IS NULL OR @Vainilla IS NULL
BEGIN
    THROW 50001, 'Primero ejecutá DatosIniciales.sql.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE Email = N'ana.prueba@example.com')
BEGIN
    INSERT INTO dbo.Clientes (Nombre, Apellido, Email, Telefono)
    VALUES (N'Ana', N'Prueba', N'ana.prueba@example.com', N'1112345678');
END;
IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE Email = N'bruno.ejemplo@example.com')
BEGIN
    INSERT INTO dbo.Clientes (Nombre, Apellido, Email, Telefono)
    VALUES (N'Bruno', N'Ejemplo', N'bruno.ejemplo@example.com', N'1198765432');
END;
DECLARE @Ana int = (SELECT Id FROM dbo.Clientes WHERE Email = N'ana.prueba@example.com');
DECLARE @Bruno int = (SELECT Id FROM dbo.Clientes WHERE Email = N'bruno.ejemplo@example.com');
DECLARE @Pedido int;

IF NOT EXISTS (SELECT 1 FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-01')
BEGIN
    INSERT INTO dbo.Pedidos
        (Fecha, ClienteId, Estado, Entrega, Direccion, Localidad, CodigoPostal, Observaciones, FechaPago)
    VALUES
        (DATEADD(day, -4, GETDATE()), @Ana, 0, 0,
         N'Calle de prueba 123', N'Buenos Aires', N'1000', N'EUMA-PRUEBA-01', NULL);
END;
SELECT @Pedido = Id FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-01';
IF NOT EXISTS (SELECT 1 FROM dbo.DetallesPedido WHERE PedidoId = @Pedido AND ProductoId = @Lavanda)
BEGIN
    INSERT INTO dbo.DetallesPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    SELECT @Pedido, Id, 2, Precio FROM dbo.Productos WHERE Id = @Lavanda;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-02')
BEGIN
    INSERT INTO dbo.Pedidos
        (Fecha, ClienteId, Estado, Entrega, Direccion, Localidad, CodigoPostal, Observaciones, FechaPago)
    VALUES
        (DATEADD(day, -3, GETDATE()), @Ana, 1, 0,
         N'Calle de prueba 123', N'Buenos Aires', N'1000', N'EUMA-PRUEBA-02', DATEADD(day, -3, GETDATE()));
END;
SELECT @Pedido = Id FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-02';
IF NOT EXISTS (SELECT 1 FROM dbo.DetallesPedido WHERE PedidoId = @Pedido AND ProductoId = @Lavanda)
BEGIN
    INSERT INTO dbo.DetallesPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    SELECT @Pedido, Id, 1, Precio FROM dbo.Productos WHERE Id = @Lavanda;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.DetallesPedido WHERE PedidoId = @Pedido AND ProductoId = @Vainilla)
BEGIN
    INSERT INTO dbo.DetallesPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    SELECT @Pedido, Id, 2, Precio FROM dbo.Productos WHERE Id = @Vainilla;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-03')
BEGIN
    INSERT INTO dbo.Pedidos
        (Fecha, ClienteId, Estado, Entrega, Direccion, Localidad, CodigoPostal, Observaciones, FechaPago)
    VALUES
        (DATEADD(day, -2, GETDATE()), @Bruno, 1, 1,
         NULL, NULL, NULL, N'EUMA-PRUEBA-03', DATEADD(day, -2, GETDATE()));
END;
SELECT @Pedido = Id FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-03';
IF NOT EXISTS (SELECT 1 FROM dbo.DetallesPedido WHERE PedidoId = @Pedido AND ProductoId = @Lavanda)
BEGIN
    INSERT INTO dbo.DetallesPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    SELECT @Pedido, Id, 1, Precio FROM dbo.Productos WHERE Id = @Lavanda;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-04')
BEGIN
    INSERT INTO dbo.Pedidos
        (Fecha, ClienteId, Estado, Entrega, Direccion, Localidad, CodigoPostal, Observaciones, FechaPago)
    VALUES
        (DATEADD(day, -1, GETDATE()), @Bruno, 3, 0,
         N'Calle de prueba 123', N'Buenos Aires', N'1000', N'EUMA-PRUEBA-04', DATEADD(day, -1, GETDATE()));
END;
SELECT @Pedido = Id FROM dbo.Pedidos WHERE Observaciones = N'EUMA-PRUEBA-04';
IF NOT EXISTS (SELECT 1 FROM dbo.DetallesPedido WHERE PedidoId = @Pedido AND ProductoId = @Lavanda)
BEGIN
    INSERT INTO dbo.DetallesPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    SELECT @Pedido, Id, 1, Precio FROM dbo.Productos WHERE Id = @Lavanda;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.DetallesPedido WHERE PedidoId = @Pedido AND ProductoId = @Vainilla)
BEGIN
    INSERT INTO dbo.DetallesPedido (PedidoId, ProductoId, Cantidad, PrecioUnitario)
    SELECT @Pedido, Id, 2, Precio FROM dbo.Productos WHERE Id = @Vainilla;
END;
