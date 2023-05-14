# https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container

dotnet publish --os linux --arch arm64 /t:PublishContainer -c Release
docker image tag asus-driver-feed:1.0.0 ghcr.io/pettijohn/asus-driver-feed:latest-arm64
docker image push ghcr.io/pettijohn/asus-driver-feed:latest-arm64

dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
docker image tag asus-driver-feed:1.0.0 ghcr.io/pettijohn/asus-driver-feed:latest-amd64
docker image push ghcr.io/pettijohn/asus-driver-feed:latest-amd64

# Merge both into a multi-platform image
docker buildx imagetools create --tag ghcr.io/pettijohn/asus-driver-feed:latest \
    ghcr.io/pettijohn/asus-driver-feed:latest-arm64 \
    ghcr.io/pettijohn/asus-driver-feed:latest-amd64
