using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Services
{
	public interface IAzureTableConnection
	{
		Task<CloudTable> GetTable();
	}

	public class AzureTableConnection : IAzureTableConnection
	{
		private readonly IConfiguration _configuration;
		private CloudTableClient _TableClient;
		private CloudTable _Table;

		public AzureTableConnection(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<CloudTable> GetTable()
		{
			if (_Table != null)
				return _Table;

			var TableName = _configuration.GetValue<string>("TableName");
			if (string.IsNullOrWhiteSpace(TableName))
			{
				throw new ArgumentException("Configuration must contain TableName");
			}

			var TableClient = GetClient();

			_Table = TableClient.GetTableReference(TableName);
			if (await _Table.CreateIfNotExistsAsync())
			{
				await _Table.SetPermissionsAsync(new TablePermissions { PublicAccess = TablePublicAccessType.Table });
			}
			return _Table;
		}

		private CloudTableClient GetClient()
		{
			if (_TableClient != null)
				return _TableClient;

			var storageConnectionString = _configuration.GetValue<string>("StorageConnectionString");
			if (string.IsNullOrWhiteSpace(storageConnectionString))
			{
				throw new ArgumentException("Configuration must contain StorageConnectionString");
			}

			if (!CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
			{
				throw new Exception("Could not create storage account with StorageConnectionString configuration");
			}

			_TableClient = storageAccount.CreateCloudTableClient();
			return _TableClient;
		}
	}
}
