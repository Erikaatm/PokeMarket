﻿namespace PokeMarket.Models.Settings
{
	public class EmailSettings
	{
		public string FromName { get; set; } = string.Empty;

		public string FromEmail { get; set; } = string.Empty;

		public string SmtpServer { get; set; } = string.Empty;

		public int Port { get; set; }

		public string Username { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;
	}
}
