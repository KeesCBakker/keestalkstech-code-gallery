#################################
# Configuration settings (v2.1.7)
#################################

ARG \
   ASPNET_VERSION="8.0" \
   # options: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
   # minimal keeps your pipeline readable while informing you what's going on
   VERBOSITY="minimal" \
   SONAR_PROJECT_KEY=keestalkstech-code-gallery  \
   SONAR_HOST_URL \
   SONAR_TOKEN

#########################################################################
# Build image, uses VERBOSITY, EXECUTE_TESTS, PROJECTS_TO_PUBLISH, APP_DIR
#########################################################################

FROM mcr.microsoft.com/dotnet/sdk:$ASPNET_VERSION AS build

ARG VERBOSITY

ENV \
   TZ=Europe/Amsterdam \
   DOTNET_CLI_TELEMETRY_OPTOUT=true \
   DOTNET_NOLOGO=true

WORKDIR /build

# Install Sonar Scanner, Coverlet and Java (required for Sonar Scanner)
RUN apt-get update && apt-get install -y openjdk-17-jdk
RUN dotnet tool install --global dotnet-sonarscanner
RUN dotnet tool install --global coverlet.console
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . .

RUN \
    chmod +x build.sh && \
    ./build.sh
