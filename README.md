# UniCampus - Registro de actividades académicas

Corporación Universitaria Americana · Desarrollo de Aplicaciones Web II

## Tecnologías
- C#, ASP.NET Web Forms (.NET Framework 4.8), SQL Server y ADO.NET.
- Visual Studio Community e IIS Express.

## Arquitectura
- **Presentación** (`Presentacion/UniCampus.Web`): Web Forms + MVP (vistas, presenters y contratos).
- **Negocio** (`Business/uniCampus.BLL`): servicios, validaciones y permisos por rol.
- **Datos** (`Data/UniCampus.DAL`): repositorios ADO.NET y consultas parametrizadas.
- **Entidades** (`Entities/UniCampus.Entities`): DTOs y contratos.
- **Pruebas** (`Tests/UniCampus.Test`): pruebas unitarias de servicios.
- Flujo: Web → BLL → DAL → SQL Server. La aplicación Web no referencia directamente la DAL.

## Abrir y ejecutar en Visual Studio Community
1. Extrae el ZIP completo en una carpeta local que no esté dentro de `Program Files`, OneDrive ni una carpeta sincronizada.
2. Abre `UniCampus.sln` (solución clásica compatible con Visual Studio 2019/2022). También se conserva `Unicampus.slnx`.
3. En el Explorador de soluciones, clic derecho en `UniCampus.Web` → **Establecer como proyecto de inicio**.
4. Restaura los paquetes NuGet: clic derecho en la solución → **Restaurar paquetes NuGet**. Los paquetes no se incluyen en el ZIP; se descargan durante la restauración.
5. Verifica que esté instalado el componente **Desarrollo de ASP.NET y web** de Visual Studio y que IIS Express esté disponible.
6. Abre `Presentacion/UniCampus.Web/Web.config` y ajusta `Data Source` para que coincida con la instancia real de SQL Server del equipo. La configuración inicial es `.\SQLEXPRESS`.
7. Comprueba que la base `UniCampusDB` exista y que tenga las tablas/columnas que usan los repositorios. **No es necesario ejecutar el script SQL si la base ya existe.** El proyecto no ejecuta ni elimina la base automáticamente.
8. Ejecuta con F5. El proyecto Web está configurado para usar IIS Express, puerto local 50101 y abrir `Login.aspx`.

Si el puerto 50101 está ocupado, cambia el puerto en las propiedades Web del proyecto y conserva una única URL de IIS Express. No habilites el servidor IIS local para este proyecto.

## Inicio de sesión: limitación funcional importante
La versión recibida autentica consultando únicamente el correo y verificando que el usuario tenga `Estado = 'Activo'`. El formulario y la tabla `Usuario` no contienen contraseña ni hash de contraseña. Por tanto, **no es autenticación con credenciales completas**: cualquier persona que conozca el correo de un usuario activo podría entrar como ese usuario. Se conserva ese comportamiento para no alterar el esquema ni inventar un requisito de autenticación; antes de publicar el sistema debe definirse e implementarse autenticación segura con contraseña o proveedor institucional.

## Registro de errores
Los errores no controlados y los errores capturados durante el login y las operaciones de presentación/negocio se envían a `System.Diagnostics.Trace`. En Visual Studio, revisa **Ver → Salida → Depuración**. El mensaje mostrado al usuario sigue siendo genérico y no revela la cadena de conexión ni detalles internos.

## Base de datos
La cadena de conexión está centralizada bajo el nombre `UniCampusDb` en `Web.config`. La instancia `.\SQLEXPRESS` es un valor inicial y debe coincidir con la instalación real de SQL Server. El ZIP no permite comprobar el estado del servicio SQL Server ni conectarse a la base de datos del equipo.

El archivo `Database/Scripts/03_BaseDatos.sql` se conserva como script de referencia/creación. No lo ejecutes sobre una base existente sin revisar previamente su esquema y contar con respaldo.

## Funciones que aparecen implementadas en el código
- Listar actividades, paginar y buscar.
- Registrar, editar y desactivar actividades según rol.
- Consultar detalle y estudiantes inscritos.
- Consultar y cancelar las propias inscripciones.
- Inscripción con control de cupos y duplicados mediante transacción.
- Servicios y repositorio de reseñas; la pantalla de reseñas figura como pendiente.

## Alcance de verificación de esta entrega
Se revisaron estructura, referencias entre proyectos, archivos incluidos en los `.csproj`, configuraciones XML, rutas de redirección, manejo de sesión, cadena de conexión y consultas frente al script SQL incluido. Se realizaron comprobaciones estáticas sobre los archivos.

No fue posible compilar ni ejecutar la solución en este entorno: no hay Visual Studio, MSBuild/.NET Framework, IIS Express ni SQL Server disponibles. Por eso no se afirma que F5, el login, la conexión SQL o los módulos hayan sido probados en ejecución. Las pruebas unitarias existentes se conservan, pero deben ejecutarse desde el Test Explorer de Visual Studio después de restaurar NuGet.
