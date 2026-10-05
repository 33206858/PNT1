INSERT INTO dbo.Categorias (Nombre, Descripcion)
VALUES ('Esencias', 'Esencias disponibles');

INSERT INTO dbo.Productos
    (Codigo, Nombre, Descripcion, CategoriaId, Origen,
     Presentacion, Precio, Stock, Imagen, Activo)
VALUES
    ('MEN-4575', 'Esencia de lavanda', 'Aroma floral',
     1, 0, '100 ml', 12.50, 20, NULL, 1);