using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AirQualityCollector
{
	class StationDA
	{
		public async Task SaveStationAsync(StationDto data, CancellationToken cancellationToken)
		{
			using var cnn = new SqlConnection(Settings.SqlConnectionString);
			cnn.Open();

			using var cmd = cnn.CreateCommand();
			cmd.CommandText = @"
merge into dbo.Stations as t
using (
	select  @StationId		as StationId 
		  , @Name			as Name
		  , @Lat 			as Lat 
		  , @Lon 			as Lon 
		  , @AddressStr 	as AddressStr 
		  , @CurrentStatus 	as CurrentStatus
		  , sysutcdatetime() as ModifiedUtc
) as s on s.StationId = t.StationId
when matched then 
	update 
	set Name			= s.Name 
	  , Lat 			= s.Lat 
	  , Lon 			= s.Lon 
	  , AddressStr 		= s.AddressStr 
	  , CurrentStatus	= s.CurrentStatus
	  , ModifiedUtc		= s.ModifiedUtc
when not matched then 
	insert (StationId, Name, Lat, Lon, AddressStr, CurrentStatus, CreatedUtc, ModifiedUtc)
	values (StationId, Name, Lat, Lon, AddressStr, CurrentStatus, ModifiedUtc, ModifiedUtc);
";
			cmd.Parameters.AddWithValue("StationId", data.StationId);
			cmd.Parameters.AddWithValue("Name", data.Name);
			cmd.Parameters.AddWithValue("Lat", data.Lat);
			cmd.Parameters.AddWithValue("Lon", data.Lon);
			cmd.Parameters.AddWithValue("AddressStr", data.AddressStr);
			cmd.Parameters.AddWithValue("CurrentStatus", data.CurrentStatus);

			await cmd.ExecuteNonQueryAsync(cancellationToken);
		}
	}
}
