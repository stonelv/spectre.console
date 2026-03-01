FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY src/Directory.Build.props ./src/
COPY src/Directory.Packages.props ./src/
COPY resources ./resources

COPY src/Spectre.Console ./src/Spectre.Console

WORKDIR /app/src/Spectre.Console
RUN dotnet build -c Release -p:TargetFrameworks=net9.0 -p:LangVersion=13

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app
COPY --from=build /app/src/Spectre.Console/bin/Release/net9.0 /app/lib

CMD ["echo", "Spectre.Console build successful! Library files in /app/lib"]
