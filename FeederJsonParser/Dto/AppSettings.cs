namespace FeederJsonParser.Dto;

public class AppSettings
{
	public const string ConfigAreaName = "FeederJsonParser";

	public string URL { get; set; } = "ultrafeeder";
    public int Port { get; set; } = 30047;
    public string Sqlite_Location { get; set; } = "/feeder-json-parser/";
}