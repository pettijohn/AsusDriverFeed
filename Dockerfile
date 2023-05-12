# https://docs.docker.com/build/building/multi-stage/
FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
#FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy-arm64v8 AS builder

ARG TARGETARCH
ARG TARGETOS

RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-$arch > /tmp/rid

WORKDIR /Code
COPY * .
RUN dotnet restore -r $(cat /tmp/rid)
RUN dotnet publish -c Release -o /Out -r $(cat /tmp/rid) --self-contained false --no-restore

# #RUN arch=$(arch | sed s/aarch64/arm64/ | sed s/x86_64/x64/) && \
# Don't know why, but dotnet publish fails with docker buildx, so using build
#RUN dotnet clean && dotnet restore 
#RUN dotnet build -c Release -o Out


FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /App
COPY --from=builder /Code/Out .

EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "AsusDriverFeed.dll"]