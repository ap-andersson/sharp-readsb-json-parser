namespace FeederJsonParser.Dto;

public class AppSettings
{
    public string FEEDER_JSON_PARSER_URL { get; set; } = "ultrafeeder";
    public int FEEDER_JSON_PARSER_PORT { get; set; } = 30047;
    public bool FEEDER_JSON_PARSER_TRACE { get; set; } = false;
}