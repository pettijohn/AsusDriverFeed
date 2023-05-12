# https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container
dotnet publish --os linux --arch arm64 /t:PublishContainer -c Release
docker image tag asus-driver-feed:1.0.0 ghcr.io/pettijohn/asus-driver-feed:latest
docker image push ghcr.io/pettijohn/asus-driver-feed:latest