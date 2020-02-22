using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;

namespace ScientificLogistics.PalletBuilder
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;

		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			string connectionString = "Data Source=palletizer_high;User ID=admin;Password=BackpackingTr1p;Connection Timeout=120;";

			using (OracleConnection objConn = new OracleConnection(connectionString))
			{
				try
				{
					objConn.Open();
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
			return Task.CompletedTask;
		}
	}
}
