using System.Text.Json.Serialization;

namespace FeederJsonParser.Dto;

public class Aircraft
{
    [JsonPropertyName("now")]
    public double? GeneratedTimeEpoch { get; set; }

    [JsonPropertyName("hex")]
    public string Hex { get; set; }

    [JsonPropertyName("flight")]
    public string? Flight { get; set; }

    [JsonPropertyName("r")]
    public string AircraftRegistration { get; set; }

    [JsonPropertyName("t")]
    public string AircraftType { get; set; }

    [JsonPropertyName("type")]
    public string MessageType { get; set; }

    //[JsonPropertyName("alt_baro")]
    //public int? AltBaro { get; set; }

    //[JsonPropertyName("alt_geom")]
    //public int? AltGeom { get; set; }

    //[JsonPropertyName("gs")]
    //public double? Gs { get; set; }

    //[JsonPropertyName("ias")]
    //public int? Ias { get; set; }

    //[JsonPropertyName("tas")]
    //public int? Tas { get; set; }

    //[JsonPropertyName("mach")]
    //public double? Mach { get; set; }

    //[JsonPropertyName("wd")]
    //public int? Wd { get; set; }

    //[JsonPropertyName("ws")]
    //public int? Ws { get; set; }

    //[JsonPropertyName("oat")]
    //public int? Oat { get; set; }

    //[JsonPropertyName("tat")]
    //public int? Tat { get; set; }

    //[JsonPropertyName("track")]
    //public double? Track { get; set; }

    //[JsonPropertyName("roll")]
    //public double? Roll { get; set; }

    //[JsonPropertyName("mag_heading")]
    //public double? MagHeading { get; set; }

    //[JsonPropertyName("true_heading")]
    //public double? TrueHeading { get; set; }

    //[JsonPropertyName("baro_rate")]
    //public int? BaroRate { get; set; }

    //[JsonPropertyName("geom_rate")]
    //public int? GeomRate { get; set; }

    //[JsonPropertyName("squawk")]
    //public string Squawk { get; set; }

    //[JsonPropertyName("emergency")]
    //public string Emergency { get; set; }

    //[JsonPropertyName("category")]
    //public string Category { get; set; }

    //[JsonPropertyName("nav_qnh")]
    //public double? NavQnh { get; set; }

    //[JsonPropertyName("nav_altitude_mcp")]
    //public int? NavAltitudeMcp { get; set; }

    //[JsonPropertyName("lat")]
    //public double? Lat { get; set; }

    //[JsonPropertyName("lon")]
    //public double? Lon { get; set; }

    //[JsonPropertyName("nic")]
    //public int? Nic { get; set; }

    //[JsonPropertyName("rc")]
    //public int? Rc { get; set; }

    //[JsonPropertyName("seen_pos")]
    //public double? SeenPos { get; set; }

    //[JsonPropertyName("r_dst")]
    //public double? RDst { get; set; }

    //[JsonPropertyName("r_dir")]
    //public double? RDir { get; set; }

    //[JsonPropertyName("version")]
    //public int? Version { get; set; }

    //[JsonPropertyName("nic_baro")]
    //public int? NicBaro { get; set; }

    //[JsonPropertyName("nac_p")]
    //public int? NacP { get; set; }

    //[JsonPropertyName("nac_v")]
    //public int? NacV { get; set; }

    //[JsonPropertyName("sil")]
    //public int? Sil { get; set; }

    //[JsonPropertyName("sil_type")]
    //public string SilType { get; set; }

    //[JsonPropertyName("gva")]
    //public int? Gva { get; set; }

    //[JsonPropertyName("sda")]
    //public int? Sda { get; set; }

    //[JsonPropertyName("alert")]
    //public int? Alert { get; set; }

    //[JsonPropertyName("spi")]
    //public int? Spi { get; set; }

    //[JsonPropertyName("mlat")]
    //public List<object> Mlat { get; set; }

    //[JsonPropertyName("tisb")]
    //public List<object> Tisb { get; set; }

    //[JsonPropertyName("messages")]
    //public int? Messages { get; set; }

    //[JsonPropertyName("seen")]
    //public double? Seen { get; set; }

    //[JsonPropertyName("rssi")]
    //public double? Rssi { get; set; }

    public override string ToString()
    {
        return $"Hex: {Hex} | AircraftRegistration: {AircraftRegistration} | Flight: {Flight} | AircraftType: {AircraftType}";
    }
}