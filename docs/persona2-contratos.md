# Persona 2: contratos de Entities y negocio

## Alcance y estado

Implementado en .NET Framework 4.8, manteniendo el namespace real uniCampus.BLL.
No se modificaron Web Forms, Presenters, repositorios concretos, conexión ni SQL.
El servicio puede probarse sin SQL Server mediante IActividadRepository.
Esto no demuestra todavía un flujo Web -> SQL Server.

## DTOs públicos (namespace UniCampus.Entities)

### ActividadDto

| Propiedad | Tipo C# | Significado |
|---|---|---|
| ActividadId | int | Identificador; 0 en el ejemplo de creación. El servicio no lo modifica. |
| Titulo | string | Obligatorio; no nulo, vacío ni solo espacios. |
| Descripcion | string | Obligatoria; no nula, vacía ni solo espacios. |
| TipoId | int | Positivo; verificar existencia corresponde al catálogo/esquema acordado. |
| FechaInicio | DateTime | Obligatoria; negocio utiliza Date y rechaza la fecha de default(DateTime). |
| FechaFin | DateTime | Obligatoria; negocio utiliza Date y rechaza la fecha de default(DateTime). |
| HoraInicio | TimeSpan | Hora del día en [00:00, 24:00). |
| HoraFin | TimeSpan | Hora del día en [00:00, 24:00). |
| Modalidad | string | Valor perteneciente al catálogo configurado. |
| Lugar | string | Puede ser nulo cuando la modalidad no lo exige. |
| Cupos | int | Mayor que cero. |
| Estado | string | Estado administrativo perteneciente al catálogo configurado. |

### TipoActividadDto

| Propiedad | Tipo C# |
|---|---|
| TipoId | int |
| Nombre | string |
| Descripcion | string |

No hay campos adicionales ni repositorio de tipos implementado por Persona 2.

### ResumenDto

| Propiedad | Tipo C# | Semántica pendiente de coordinar |
|---|---|---|
| TotalActividades | int | Universo de actividades e inclusión de canceladas/inactivas. |
| TotalInscritos | int | Estudiantes únicos frente a número de inscripciones; bajas/cancelaciones. |
| ActividadesProximas | int | Regla temporal y estados administrativos elegibles. |
| ActividadesFinalizadas | int | Regla temporal y estados administrativos elegibles. |

Los DTOs no contienen controles, acceso SQL ni referencias a Web/BLL/DAL.

## Contrato requerido para Persona 1

Namespace: UniCampus.Entities.Contracts.

    public interface IActividadRepository
    {
        int Guardar(ActividadDto actividad);
    }

La clase ActividadRepository existente podrá hacerse pública e implementar esta interfaz.
Persona 1 debe agregar la referencia DAL -> Entities. Esta rama no cambia esa clase ni su proyecto.

Obligaciones propuestas:
- Recibir el DTO validado y persistir todos los campos mediante ADO.NET parametrizado.
- Devolver el identificador positivo realmente persistido.
- No simular éxito mediante 0, -1 o un identificador inventado.
- Gestionar recursos y transacciones en DAL; registrar allí el diagnóstico técnico seguro.
- Propagar el fallo al servicio. BLL lo convierte en un mensaje público sin detalles.
- Una excepción no asegura que la transacción no se haya confirmado: el servicio no reintenta.

No se acordaron tablas, tipos SQL, longitudes, catálogo de tipos, restricciones ni semántica de actualización.
Guardar no implementa ni promete operaciones de edición. Debe acordarse con Persona 1 el tratamiento
de ActividadId distinto de cero antes de agregar edición; el servicio actual no altera ese valor.
Los parámetros opcionales deben mapearse a DBNull cuando el esquema lo permita.

## API pública de BLL

    public ActividadService(IActividadRepository repository, ReglasActividad reglas);
    public ResultadoGuardarActividad Guardar(ActividadDto actividad);

ResultadoGuardarActividad expone:
- bool Exitoso.
- int? ActividadId: positivo al guardar; null cuando falla.
- IReadOnlyList<string> Errores: vacía al guardar; mensajes cuando falla.

La colección de errores es de solo lectura. El servicio no asigna ActividadId al DTO de entrada,
no modifica sus cadenas y no transforma fechas ni horas antes de enviarlas al repositorio.

Dependencias nulas y configuración incoherente son errores de programación: los constructores
lanzan ArgumentNullException o ArgumentException. La validación del usuario devuelve un resultado,
no excepciones.

## Configuración de reglas

    public ReglasActividad(
        IEnumerable<string> modalidadesPermitidas,
        IEnumerable<string> modalidadesConLugarObligatorio,
        IEnumerable<string> estadosPermitidos);

No hay catálogos de producción predeterminados. El punto de composición de Web debe proporcionar
los valores acordados por el equipo. Los ejemplos y pruebas usan catálogos ilustrativos:
no constituyen aprobación de esos estados/modalidades.

Los catálogos se copian y comparan sin distinguir mayúsculas, con OrdinalIgnoreCase.
No se eliminan espacios ni se normalizan los valores persistidos. No configurar variantes con
espacios accidentales; Presenter y DAL deben acordar la representación canónica.
Modalidades y estados deben tener al menos un valor; la lista de modalidades con lugar puede
estar vacía. No se permiten elementos nulos o en blanco. Toda modalidad con lugar obligatorio
debe pertenecer al catálogo de modalidades.

## Reglas implementadas

1. DTO no nulo.
2. Título y descripción obligatorios, incluidos nulos y espacios.
3. TipoId y Cupos mayores que cero.
4. Fechas de inicio y fin obligatorias (distintas de la fecha default); FechaFin.Date no anterior a FechaInicio.Date.
5. Horas del día no negativas y menores de 24 horas.
6. FechaFin.Date + HoraFin estrictamente posterior a FechaInicio.Date + HoraInicio.
7. Modalidad y estado presentes en sus catálogos explícitos.
8. Lugar obligatorio únicamente para modalidades configuradas con ese requisito.
9. Acumulación de errores sin invocar persistencia.
10. Entrada válida: una sola llamada a Guardar del repositorio.
11. Id positivo devuelto; excepciones o ids no positivos producen un error seguro.

Se permiten actividades de varios días y cruzar medianoche cuando FechaFin es el día siguiente.
No se impone HoraFin > HoraInicio si las fechas son distintas.
No se restringe el rango de fechas al de SQL datetime sin un esquema acordado: se valida el rango
de DateTime de .NET, salvo la fecha reservada para indicar ausencia; se conserva el máximo representable. Persona 1 debe confirmar date/datetime2 frente
a datetime y la precisión SQL time. La conversión de horas no debe redondear a 24:00.

Se ignora el componente horario de FechaInicio/FechaFin al validar: las horas explícitas son
la fuente del horario. DAL debe guardar esa misma separación, no conservar horas ocultas en fechas.
No hay validación de fecha futura, límites de longitud, unicidad ni transiciones no solicitadas.

## Ejemplo de consumo desde Persona 3

Ejemplo ilustrativo del cuerpo de un Presenter; no se agrega un Presenter en esta rama:

    // Los valores deben ser sustituidos por el catálogo aprobado por el equipo.
    var reglas = new ReglasActividad(
        new[] { "Presencial", "Virtual" },
        new[] { "Presencial" },
        new[] { "Activa" });

    // repository es un IActividadRepository concreto provisto por el punto de composición.
    var servicio = new ActividadService(repository, reglas);
    var resultado = servicio.Guardar(actividad);

    if (resultado.Exitoso)
    {
        int idGuardado = resultado.ActividadId.Value;
        // Comunicar éxito a la vista mediante el contrato que acuerde Persona 3.
    }
    else
    {
        string mensaje = string.Join(Environment.NewLine, resultado.Errores);
        // Comunicar los errores a la vista, codificados según su contexto de salida.
    }

Persona 3 debe convertir entradas con TryParse/TryParseExact y cultura explícita antes de
construir el DTO. Un error de conversión debe mostrarse en la vista y no sustituirse por MinValue.
Agregar Web -> Entities será necesario cuando ese Presenter use los DTOs.
Se conserva Web -> BLL y no se cambian sus referencias actuales a DAL y Tests.

## Estadísticas para Persona 4

Contrato preparado en uniCampus.BLL.Services:

    public interface IResumenActividadService
    {
        ResumenDto ObtenerResumen(DateTime fechaReferencia);
    }

Persona 4 recibirá ResumenDto con los cuatro enteros documentados.
No existe aún implementación: no hay una consulta DAL que permita obtener cifras reales.
No se debe construir un servicio ficticio que devuelva cuatro ceros como sustituto de datos.
Antes de implementarlo, Persona 1/5 debe ofrecer operaciones de lectura/agregación y el equipo
debe cerrar las definiciones de conteo, la zona horaria y los límites temporales.

Propuesta para discutir, NO regla implementada:
- Estado administrativo: decisión del equipo (por ejemplo Activa); no se cambia automáticamente.
- Estado temporal: derivado de inicio/fin y fechaReferencia, separado del Estado del DTO.
- Próxima: referencia < inicio; en curso: inicio <= referencia < fin; finalizada: referencia >= fin.
- Confirmar si estas categorías incluyen actividades canceladas y cómo filtrar por estado.
- No usar un estado administrativo como sustituto del cálculo temporal.
- Acordar si fechaReferencia y las fechas de actividad representan hora local de Colombia u otra zona.

## Verificación reproducible

Restaurar con MSBuild de Visual Studio, no con el MSBuild de dotnet para el proyecto Web clásico:

    MSBuild.exe Unicampus.slnx /t:Restore /p:RestorePackagesConfig=true
    MSBuild.exe Unicampus.slnx /t:Rebuild /p:Configuration=Debug
    vstest.console.exe Tests\UniCampus.Test\bin\Debug\UniCampus.Test.dll /Logger:trx
    MSBuild.exe Unicampus.slnx /t:Rebuild /p:Configuration=Release
    vstest.console.exe Tests\UniCampus.Test\bin\Release\UniCampus.Test.dll /Logger:trx

Se usan MSTest.TestFramework y MSTest.TestAdapter 2.2.10 ya existentes.
No hay paquetes nuevos. ActividadServiceTests usa un repositorio falso y no conecta a SQL Server.
La prueba vacía UnitTest1 de la base se conserva para no mezclar limpiezas ajenas.

## Pendientes para integrar la primera entrega

- Persona 1: esquema, conexión, catálogo de tipos y ActividadRepository público implementando el contrato.
- Persona 3: formulario, parseo, Presenter, composición de dependencias y mensajes de la vista.
- Equipo: modalidades, lugar, estados administrativos y semántica de creación/edición.
- Persona 4/5: definición y origen de estadísticas, conteo de inscritos y contrato temporal.
- Equipo: ejecutar una prueba de integración real Web -> Presenter -> BLL -> DAL -> SQL Server.
## Resultados de implementación inicial (8 de noviembre de 2026)

- Base original: solución compilada y 1 prueba vacía ejecutada correctamente.
- RED: 52 casos ejecutados; 51 nuevos fallaron contra el servicio sin implementar y pasó la prueba vacía original.
- GREEN/final Debug: 52/52 correctos (51 nuevos + 1 original).
- Final Release: 52/52 correctos.
- Rebuild completo con MSBuild de Visual Studio: Debug y Release con 0 errores y 0 advertencias.
- Revisión independiente estática: sin hallazgos accionables.
- Web, DAL, Database y sus referencias existentes: sin cambios.
- Evidencia local de compilación y TRX en TestResults (ignorado por Git); no contiene una prueba de SQL.


## Corrección: fechas obligatorias

Los DTOs conservan DateTime no nullable para evitar romper el contrato compartido.
Por ello se reserva default(DateTime).Date (0001-01-01) como indicador de fecha no proporcionada.
Se compara Date, no el instante completo: una hora interna distinta de cero no debe ocultar
la ausencia, porque HoraInicio/HoraFin son la fuente independiente del horario.

Si falta una fecha se devuelve su error obligatorio; si faltan ambas se devuelven los dos.
No se comparan ni combinan fechas incompletas; no se llama a IActividadRepository.Guardar.
La validación de los demás campos sigue acumulando errores.

La primera fecha admitida es 0001-01-02. Esta exclusión del primer día deriva únicamente del
indicador de ausencia del DTO, no de una política de antigüedad o de fechas futuras.
Se mantienen pruebas del primer día admitido, DateTime.MaxValue y el último tick horario válido.
La prueba anterior que aceptaba DateTime.MinValue fue actualizada porque contradice esta regla.
Si el equipo necesita representar legítimamente 0001-01-01, deberá acordar un contrato nullable
o una representación explícita de ausencia; no se cambia ese contrato en esta corrección.

## Decisiones de integración todavía pendientes

| Tema | Responsabilidad propuesta y decisión pendiente |
|---|---|
| Longitud máxima de Titulo y Descripcion | Persona 1 debe confirmar tipos, tamaños y nulabilidad del futuro esquema SQL; el equipo debe aprobar los límites y después alinearlos en BLL y formulario. Esta rama no fija longitudes, trunca cadenas ni presupone un tamaño SQL. |
| Registro técnico de fallos de persistencia | Propuesta: DAL/infraestructura registra el diagnóstico técnico con el mecanismo de logging que acuerde el equipo, sin credenciales ni datos sensibles. BLL devuelve únicamente el error público seguro. El registro aún no está implementado ni verificado en DAL; se debe acordar responsable, destino y correlación. |
| Creación con ActividadId distinto de cero | Pendiente acordar si se rechaza, se ignora para generar otro id o se trata mediante una operación de edición separada. El ejemplo de creación usa 0. Esta corrección no selecciona una política: BLL conserva y reenvía el valor; el repositorio concreto no debe asumir silenciosamente una semántica sin acuerdo. |
| Configuración desde Web | El punto de composición Web debe obtener los catálogos aprobados y construir ReglasActividad, el repositorio concreto y ActividadService; después inyectar el servicio en el Presenter. No se fija si los catálogos proceden de configuración o de datos. Deben incluirse modalidades, estados y el subconjunto de modalidades con lugar obligatorio. Los valores de ejemplos/pruebas siguen siendo ilustrativos. |

No se modifica código Web, DAL ni SQL para resolver estos acuerdos.

## Verificación de la corrección de fechas (8 de noviembre de 2026)

- RED: 57 casos ejecutados; los 5 casos nuevos fallaron y los 52 anteriores pasaron.
- Rebuild Debug con MSBuild de Visual Studio: 0 errores y 0 advertencias.
- MSTest Debug: 57/57 correctos.
- Rebuild Release con MSBuild de Visual Studio: 0 errores y 0 advertencias.
- MSTest Release: 57/57 correctos.
- Total: 56 casos de negocio y la prueba vacía original conservada.
- Se comprobaron fechas ausentes individualmente y juntas, ausencia con hora interna,
  error obligatorio por campo y cero llamadas al repositorio para estos datos inválidos.
- La prueba de límite inferior ahora usa 0001-01-02 y se conserva el límite superior.
- git diff --check correcto; solo cambian validador, pruebas y este documento.
- Logs/TRX locales en TestResults/fechas-*; no se ejecutó SQL.
