using FlightInformationParser.Dto;

namespace FlightInformationParser.Handler;

public interface IAircraftHandler
{
    Task HandleAircraft(Aircraft aircraft);
}