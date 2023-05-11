# https://docs.docker.com/build/building/multi-stage/
FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
WORKDIR /Code
COPY ** .
RUN dotnet publish -c Release -o Out


FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=builder /Code/Out .
ENTRYPOINT ["dotnet", "AsusDriverFeed.dll"]