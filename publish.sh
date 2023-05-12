
# docker buildx create --platform linux/arm64,linux/amd64 --use
docker buildx build . --platform linux/arm64
docker buildx build . --platform linux/amd64 
docker buildx build . --platform linux/arm64,linux/amd64 -t ghcr.io/pettijohn/asus-driver-feed:latest --push