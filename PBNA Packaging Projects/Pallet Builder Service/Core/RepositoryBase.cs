using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificLogistics.PalletBuilder
{
	public abstract class RepositoryBase
	{
		static RepositoryBase()
		{
			OracleConfiguration.TnsAdmin = "wallet";
		}

		protected string ConnectionString => "Data Source=palletizer_high;User ID=OMS_O;Password=BackpackingTr1p;Connection Timeout=120;";

		public List<T> Select<T>(string sql, object parameter = null)
		{
			using OracleConnection dbConnection = new OracleConnection(ConnectionString);
			
			dbConnection.Open();

			return dbConnection.Query<T>(sql, parameter).ToList();
		}

	}
}
