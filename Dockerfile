FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /FeederJsonParser

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore -a $TARGETARCH
# Build and publish a release
RUN dotnet publish -a $TARGETARCH --no-restore -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /FeederJsonParser
COPY --from=build /FeederJsonParser/out .
ENTRYPOINT ["dotnet", "FeederJsonParser.dll"]