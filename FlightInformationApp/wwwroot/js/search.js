$(function () {
    console.log("ready!");

    $('.datepicker').datepicker({
        format: 'yyyy-mm-dd',
        endDate: '+0d'
    });

    var table = new Tabulator("#search-results", {
        height: "600px",
        layout: "fitColumns",
        placeholder: "No Data Set",
        columns: [
            { title: "Hex", field: "aircraftHex", sorter: "string" },
            { title: "Type", field: "aircraftType", sorter: "string" },
            { title: "Registration", field: "aircraftRegistration", sorter: "string" },
            { title: "Flight number", field: "flightNumber", sorter: "string" },
            {
                title: "Start", field: "startTime", sorter: "datetime", formatter: "datetime", formatterParams: {
                    inputFormat: "iso",
                    outputFormat: "yyyy-MM-dd HH:mm:ss",
                    invalidPlaceholder: "(invalid date)"
                },
                sorterParams: {
                    format: "iso",
                    alignEmptyValues: "top",
                }
            },
            {
                title: "End", field: "endTime", sorter: "datetime", formatter: "datetime", formatterParams: {
                    inputFormat: "iso",
                    outputFormat: "yyyy-MM-dd HH:mm:ss",
                    invalidPlaceholder: "(invalid date)"
                },
                sorterParams: {
                    format: "iso",
                    alignEmptyValues: "top",
                }
            },
        ],
    });

    $("#btn-submit").on('click', () => {
        // console.log("click");

        var jqxhr = $.get("https://flightdata.andynet.se/FlightData/Filter",
            {
                SenderId: $("#SenderId").val(),
                AircraftHex: $("#AircraftHex").val(),
                AircraftType: $("#AircraftType").val(),
                AircraftRegistration: $("#AircraftRegistration").val(),
                FlightNumber: $("#FlightNumber").val(),
                Day: $("#Day").val()
            }
            , function (data) {
                // console.log("success");
                table.setData(data);
            })
            .done(function () {
                // console.log("second success");
            })
            .fail(function () {
                console.log("error");
            })
            .always(function () {
                // console.log("finished");
            });
    });
});