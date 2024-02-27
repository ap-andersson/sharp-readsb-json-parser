# sharp-readsb-json-parser
A C# application that reads from readsb JSON port and deserializes the data and... DOES NOTHING FOR NOW

## Introduction

The end goal is to create a full database of all the aircraft/flight/hex thats easily searchable. To be used to find your ICAO hex codes easier to use with tar1090 etc.

## Variables

You can run the program with a appsettings.json to set the variables or as a Docker container and supply the settings as environment variables.

| Variable | Required | Default value | Description
| --- | ----------- | --- | ---
| FEEDER_JSON_PARSER_URL | No | ultrafeeder | URL used to connect to readsb JSON output
| FEEDER_JSON_PARSER_PORT | No | 30047 | Port used to connect to readsb JSON  output 
| FEEDER_JSON_PARSER_TRACE | No | false | Whether to print trace logs or not. Trace logs include every aircraft info parsed. Will spam.

## Docker container
There is a container that is built every time code is pushed and automatically published. The path to the image is ghcr.io/ap-andersson/sharp-readsb-json-parser:main. Below is a docker-compose template:

``` 
services:
  sharp-parser:
    image: ghcr.io/ap-andersson/sharp-readsb-json-parser:main
    container_name: sharp-parser
    environment:
      - FEEDER_JSON_PARSER_URL=ultrafeeder
      - FEEDER_JSON_PARSER_PORT=30047
      - FEEDER_JSON_PARSER_TRACE=false
    restart: unless-stopped
```
