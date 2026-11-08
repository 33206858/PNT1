-- Datos compartidos de la base local. Se pueden ejecutar varias veces.
SET NOCOUNT ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Esencias')
BEGIN
    INSERT INTO dbo.Categorias (Nombre, Descripcion)
    VALUES (N'Esencias', N'Esencias disponibles');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Codigo = N'MEN-4575')
BEGIN
    INSERT INTO dbo.Productos
        (Codigo, Nombre, Descripcion, CategoriaId,
         Presentacion, Precio, Stock, Imagen, Activo)
    VALUES
        (N'MEN-4575', N'Esencia de lavanda', N'Aroma floral',
         (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Esencias'),
         N'100 ml', 15.75, 20, NULL, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Codigo = N'ARO-1002')
BEGIN
    INSERT INTO dbo.Productos
        (Codigo, Nombre, Descripcion, CategoriaId,
         Presentacion, Precio, Stock, Imagen, Activo)
    VALUES
        (N'ARO-1002', N'Esencia de vainilla', N'Aroma dulce y cálido',
         (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Esencias'),
         N'100 ml', 10.9, 15, NULL, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Codigo = N'ARO-1003')
BEGIN
    INSERT INTO dbo.Productos
        (Codigo, Nombre, Descripcion, CategoriaId,
         Presentacion, Precio, Stock, Imagen, Activo)
    VALUES
        (N'ARO-1003', N'Aceite esencial de eucalipto', N'Aroma fresco y herbal',
         (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Esencias'),
         N'30 ml', 18.75, 12, NULL, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Codigo = N'ARO-1004')
BEGIN
    INSERT INTO dbo.Productos
        (Codigo, Nombre, Descripcion, CategoriaId,
         Presentacion, Precio, Stock, Imagen, Activo)
    VALUES
        (N'ARO-1004', N'Esencia de limón', N'Aroma cítrico y vivaz',
         (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Esencias'),
         N'100 ml', 11.25, 18, NULL, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE Codigo = N'ARO-1005')
BEGIN
    INSERT INTO dbo.Productos
        (Codigo, Nombre, Descripcion, CategoriaId,
         Presentacion, Precio, Stock, Imagen, Activo)
    VALUES
        (N'ARO-1005', N'Esencia de rosas', N'Aroma floral suave',
         (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Esencias'),
         N'100 ml', 14, 9, NULL, 1);
END;
