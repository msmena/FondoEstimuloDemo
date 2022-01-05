using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FondoEstimulo
{
    public class Email
    {
        #region Public Constructors

        public Email()
        {
            StringBuilder texto = new();
            texto.AppendLine("<html xmlns='http://www.w3.org/1999/xhtml'>");
            texto.AppendLine("<head>");
            texto.AppendLine("<title>Sistema interno</title>");
            texto.AppendLine("<body>");
            texto.AppendLine("<h2>ATP - Modulo Fondo de Estimulo</h2>");
            texto.AppendLine("<h3>Especificación contraseña</h3>");
            texto.AppendLine("<p>Se ha establecido usuario nuevo para el sistema interno de Fondo de Estimulo.<p>");
            texto.AppendLine("<p><label>Datos:</label><br />");
            texto.AppendLine("<label>Usuario: {0}</label><br />");
            texto.AppendLine("<label>Email: {1}</label><br />");
            texto.AppendLine("<label>Especificación de contraseña: {2}</label><br /></p>");
            texto.AppendLine("<hr />");
            //cuerpo.AppendLine("<img src='https://localhost:5001/images/atp-logo.jpg' height='50'>";
            texto.AppendLine("<p><label>Empresa - Provincia del Chaco</label><br />");
            texto.AppendLine("<label>Resistencia, Chaco. Argentina</label></p>");
            CuerpoEstablecerContrasena = texto.ToString();

            texto.Clear();
            texto.AppendLine("<html xmlns='http://www.w3.org/1999/xhtml'>");
            texto.AppendLine("<head>");
            texto.AppendLine("<title>Sistema interno</title>");
            texto.AppendLine("<body>");
            texto.AppendLine("<h2>ATP - Modulo Fondo de Estimulo</h2>");
            texto.AppendLine("<h3>Notificación de Error</h3>");
            texto.AppendLine("<p>Se ha producido un error fatal en el sistema interno de Fondo de Estimulo.<p>");
            texto.AppendLine("<p><label>Información:</label><br />");
            texto.AppendLine("<label>Usuario: {0}</label><br />");
            texto.AppendLine("<label>Error: {1}</label><br /></p>");
            texto.AppendLine("<label>URL: {2}</label><br /></p>");
            texto.AppendLine("<hr />");
            //texto.AppendLine("<img src='https://localhost:5001/images/atp-logo.jpg' height='50'>";
            texto.AppendLine("<p><label>Empresa - Provincia del Chaco</label><br />");
            texto.AppendLine("<label>Resistencia, Chaco. Argentina</label></p>");
            CuerpoError = texto.ToString();
        }

        #endregion Public Constructors

        #region Public Properties

        public string CuerpoEstablecerContrasena { get; set; }
        public string CuerpoError { get; set; }

        #endregion Public Properties

        #region Public Methods

        public async Task Notificar(string to, string subject, string text, bool release = true, bool copiaJefes = false)
        {
            try
            {
                string from = "envio.email@servidor.com";
                MailMessage mailMessage = new(from, to, subject, text);
                mailMessage.IsBodyHtml = true;

                if (copiaJefes)
                {
                    mailMessage.Bcc.Add("jefe1@servidor.com");
                    mailMessage.Bcc.Add("jefe2@servidor.com");
                }

                SmtpClient client = new("servidor.");
                client.Port = 587;
                client.Credentials = new NetworkCredential("usuario", "pass");

                // Configuracion del envio de email local para usarlo con programa Papercut Smpt
                if (!release)
                {
                    client = new("localhost");
                    client.Port = 25;
                }

                await client.SendMailAsync(mailMessage);
            }
            catch
            {
                throw;
            }
        }

        #endregion Public Methods
    }
}