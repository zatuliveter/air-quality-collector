using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AirQualityCollector
{
	class StationDataDA
	{
		public async Task SaveStationAsync(StationDataDto data, CancellationToken cancellationToken)
		{
			using var cnn = new SqlConnection(Settings.SqlConnectionString);
			cnn.Open();

			using var cmd = cnn.CreateCommand();
			cmd.CommandText = @"
merge into dbo.StationData as t
using (
	select  @StationId			as StationId 
		  , @DataType			as DataType
		  , @DateTimeUtc		as DateTimeUtc
		  , @MeasurementCount	as MeasurementCount
		  , @MaxValue			as MaxValue
		  , @MinValue			as MinValue
		  , @MeanValue			as MeanValue
		  , @MedianValue		as MedianValue
		  , @StddevValue		as StddevValue
		  , sysutcdatetime()	as ModifiedUtc
) as s on s.StationId = t.StationId and s.DataType = t.DataType and s.DateTimeUtc = t.DateTimeUtc
when matched then 
	update 
	set MeasurementCount = s.MeasurementCount
	  , MaxValue		 = s.MaxValue
	  , MinValue		 = s.MinValue
	  , MeanValue		 = s.MeanValue
	  , MedianValue		 = s.MedianValue
	  , StddevValue		 = s.StddevValue
	  , ModifiedUtc		 = s.ModifiedUtc

when not matched then 
	insert (StationId, DataType, DateTimeUtc, MeasurementCount, MaxValue, MinValue, MeanValue, MedianValue, StddevValue, CreatedUtc, ModifiedUtc)
	values (StationId, DataType, DateTimeUtc, MeasurementCount, MaxValue, MinValue, MeanValue, MedianValue, StddevValue, ModifiedUtc, ModifiedUtc);
";
			cmd.Parameters.AddWithValue("StationId", data.StationId);
			cmd.Parameters.AddWithValue("DataType", (byte) data.DataType);
			cmd.Parameters.AddWithValue("DateTimeUtc", data.DateTimeUtc);
			cmd.Parameters.AddWithValue("MeasurementCount", data.MeasurementCount);
			cmd.Parameters.AddWithValue("MaxValue", data.MaxValue);
			cmd.Parameters.AddWithValue("MinValue", data.MinValue);
			cmd.Parameters.AddWithValue("MeanValue", data.MeanValue);
			cmd.Parameters.AddWithValue("MedianValue", data.MedianValue);
			cmd.Parameters.AddWithValue("StddevValue", data.StddevValue);

			await cmd.ExecuteNonQueryAsync(cancellationToken);
		}
	}
}
