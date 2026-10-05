IF NOT EXISTS (
    SELECT 1
    FROM dbo.Categorias
    WHERE Nombre = 'Esencias'
)
BEGIN
    INSERT INTO dbo.Categorias (Nombre, Descripcion)
    VALUES ('Esencias', 'Esencias disponibles');
END;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.Productos
    WHERE Codigo = 'MEN-4575'
)
BEGIN
    DECLARE @CategoriaId int;

    SELECT @CategoriaId = Id
    FROM dbo.Categorias
    WHERE Nombre = 'Esencias';

    INSERT INTO dbo.Productos
        (Codigo, Nombre, Descripcion, CategoriaId, Origen,
         Presentacion, Precio, Stock, Imagen, Activo)
    VALUES
        ('MEN-4575', 'Esencia de lavanda', 'Aroma floral',
         @CategoriaId, 0, '100 ml', 12.50, 20, NULL, 1);
END;