FROM mcr.microsoft.com/dotnet/sdk:6.0
LABEL maintainer="Preston Lee"

WORKDIR /app

COPY . .

RUN dotnet build

ENTRYPOINT [ "dotnet", "run", "--project", "src/fhir-codegen-cli", "--" ]
