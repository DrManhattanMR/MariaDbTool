# MariaDbTool

Esta herramienta permite realizar respaldos y restauraciones de una base de datos MariaDB/MySQL desde la línea de comandos.

## Uso

### Ejemplos de uso

- Para crear un respaldo (backup):

  ```bash
  dotnet run -- --backup [ruta/opcional]
  ```

  Si no se especifica una ruta, el archivo de respaldo se guardará automáticamente en la carpeta `respaldos` con un nombre basado en la fecha y hora actual.

- Para restaurar desde un archivo SQL:

  ```bash
  dotnet run -- --restore ruta/al/archivo.sql
  ```

  > **Nota:** Es obligatorio especificar la ruta del archivo SQL al restaurar.

### Parámetros de conexión

La cadena de conexión a la base de datos está definida en el código fuente (`Program.cs`). Modifícala según tus credenciales y configuración de base de datos.

---

✅ Paso 3: Ejecutar
Si estás en desarrollo:
```bash
dotnet run -- --backup
```
```bash
dotnet run -- --restore
```
Para compilar como ejecutable:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Cualquier duda o mejora, abre un issue o un pull request.


ejemplo 
dotnet run --project MariaDbTool\MariaDbTool.csproj -- --backup