# https://docs.docker.com/build/building/multi-stage/
FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
WORKDIR /Code
COPY * .
RUN dotnet publish -c Release -o Out


FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /App
COPY --from=builder /Code/Out .

EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "AsusDriverFeed.dll"]