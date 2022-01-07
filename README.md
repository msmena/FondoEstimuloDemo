# Demostración de proyecto ASP.Net 5.0

Pequeño proyecto de implementación real en entidad gubernamental para cálculo de concepto salarial de los agentes que trabajan en la entidad.

## Características

* Arquitectura MVC de tres capas y un nivel.
* Creación de base de datos si no existe.
* Conexión a motor PostgreSQL.
* Autorización y autenticación.
* Uso de javascript, css y boostrap v4.6.
* Uso de Identity Core.
* Envio de email.
* Lectura de archivos cvs con libreria CsvHelper.
* Lectura de archivos excel con libreria EPPlus.
* Registro de modificaciones de registros en base de datos, mediante Json.
* Registro de eventos en comando con libreria Serilog.
* Uso de Entity Framework Core.
* Separación de areas para página con permisos de autorización.
* Notificación por email de error en la página.

## Configuración

* Conexión a la base de datos
  * appsettings.json: FondoEstimuloContext: datos de la base de datos de producción
  * appsettings.Development.json: FondoEstimuloContext: datos de la base de datos de prueba
* Clase DbInitializer.cs: Método priado EnsureUserAdmin: configuración del email y usuario del administrador del sitio. Se enviará un email para especificación de la contraseña.
* Clase Email.cs: configuración del servidor de envio de email, usuario, contraseña y copia a jefes de departamento.
* Controlador Error.cshtml.cs: Método público OnGet: especificación de correo electrónico para notificación de error de página.

## Contacto

Puedes comunicarte conmigo mediante mi email:

- atp.gmmena@chaco.gov.ar

## Licencia

[GNU Public License](LICENSE.txt)
