FROM mcr.microsoft.com/dotnet/sdk:6.0 AS BUILDER
LABEL maintainer="Preston Lee"

WORKDIR /app
COPY . .
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=BUILDER /app/out bin
ENTRYPOINT [ "dotnet", "bin/fhir-codegen-cli.dll"]
