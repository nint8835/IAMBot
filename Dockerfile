FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /build

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build-env /build/out .
ENTRYPOINT ["/app/IAMBot"]
