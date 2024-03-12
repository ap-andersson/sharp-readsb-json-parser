namespace FlightInformationApi;

public class AppSettings
{
	public const string ConfigAreaName = "FlightInformationApiSettings";

	public string MysqlConnectionString { get; set; } = "";

    public List<Sender> Senders { get; set; }
}

public class Sender
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}