using Microsoft.Extensions.Options;
using PokeMarket.Models.Settings;
using System.Net;
using System.Net.Mail;


namespace PokeMarket.Services

{
	public class EmailService
	{
		// Definimos la clase y el constructor
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		// ----------EnviarEmail-----------

		// Método para enviar los emails
		public async Task SendEmailAsync(string toEmail,  string subject, string body)
		{
			// Creamos un nuevo objeto mensaje para construir correos electronicos, viene por defecto con .NET
			var mail = new MailMessage
			{
				// Quien envía el correo
				From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),

				// El asunto del email
				Subject = subject,

				// El cuerpo
				Body = body,

				// Ponemos lo siguiente para poder utilizar etiquetas HTML en el bodu
				IsBodyHtml = true

			};

			// Agregamos el destinatario del correo
			mail.To.Add(toEmail);

			// Configuramos el cliente SMTP, quien se conecta al Gmail y envía el correo
			using (var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
			{
				// Credenciales del correo, es decir usuario y contraseña
				smtp.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

				// Activamos la encriptación para que el email viaje seguro
				smtp.EnableSsl = true;

				// Enviamos el correo
				await smtp.SendMailAsync(mail);
			} 


				
		}


	}
}
