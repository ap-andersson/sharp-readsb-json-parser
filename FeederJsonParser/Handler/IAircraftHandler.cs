using FeederJsonParser.Dto;

public interface IAircraftHandler
{
	Task HandleAircraft(Aircraft aircraft);
}