#################################
# Configuration settings (v2.2.1)
#################################

ARG MAIN_API_NAME="Ktt.Docker.Todo.Api"
ARG \
   MAIN_API_DLL="$MAIN_API_NAME.dll" \
   PROJECTS_TO_PUBLISH="$MAIN_API_NAME" \
   PORT=5000 \
   ASPNET_VERSION="8.0" \
   # options: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
   # minimal keeps your pipeline readable while informing you what's going on
   VERBOSITY="minimal" \
   APP_DIR="/app" \
   TAG="" \
   EXECUTE_TESTS="true"
   

#########################################################################
# Build image, uses VERBOSITY, EXECUTE_TESTS, PROJECTS_TO_PUBLISH, APP_DIR
#########################################################################

FROM mcr.microsoft.com/dotnet/sdk:$ASPNET_VERSION AS build

ARG VERBOSITY EXECUTE_TESTS PROJECTS_TO_PUBLISH APP_DIR

ENV \
   TZ=Europe/Amsterdam \
   DOTNET_CLI_TELEMETRY_OPTOUT=true \
   DOTNET_NOLOGO=true

WORKDIR /build

# Let's restore the solution, nuget and project files and do a restore
# in cacheable layers. This will speed up the build process greatly

# copy global files to restore
COPY *.sln *.*config ./

# copy src files to restore
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# copy test files to restore
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done; \
   echo "" \
   && echo "---------" \
   && echo "RESTORING" \
   && echo "---------" \
   && echo "" \
   && dotnet restore --verbosity "$VERBOSITY" || exit 1

# copy dirs that are only needed for building and testing
COPY src ./src
COPY test ./test

# Note on build: don't use --no-restore, sometimes certain packages cannot be
# restored by the dotnet restore. The build will add them, as it has more context (!?)
# example: Package System.Text.Json, version 6.0.0 was not found

RUN echo "" \
   && echo "--------" \
   && echo "BUILDING" \
   && echo "--------" \
   && echo "" \
   && dotnet build --configuration Release --verbosity "$VERBOSITY" -nowarn:NETSDK1004 || exit 1

# defining the argument here caches the previous layers when the value switches
ARG EXECUTE_TESTS
RUN echo "" \
   && echo "-------" \
   && echo "TESTING" \
   && echo "-------" \
   && echo ""; \
   if [ "$EXECUTE_TESTS" = "true" ]; then \
      dotnet test --filter "Category!=Integration" --configuration Release --logger "console;verbosity=$VERBOSITY" --no-build || exit 1; \
   else \
      echo "Skipping unit tests"; \
   fi;
   
# publish project(s)
RUN echo "" \
   && echo "----------" \
   && echo "PUBLISHING" \
   && echo "----------" \
   && echo ""; \
   for project in $PROJECTS_TO_PUBLISH; do \
      echo "Publishing $project..."; \
      dotnet publish "src/$project/$project.csproj" \
        --configuration Release \
        --output "$APP_DIR/$project" \
        --no-restore -nowarn:NETSDK1004 || exit 1; \
   done


########################
# Integration test image
########################

FROM mcr.microsoft.com/dotnet/sdk:$ASPNET_VERSION AS integration-test

ENV \
   TZ=Europe/Amsterdam \
   DOTNET_CLI_TELEMETRY_OPTOUT=true \
   DOTNET_NOLOGO=true

WORKDIR /build

# install Docker
RUN apt-get update && \
    apt-get install -y --no-install-recommends docker.io procps && \
    rm -rf /var/lib/apt/lists/*

# copy entire build context
COPY --from=build /build /build

# run Docker daemon and tests
CMD bash -euo pipefail -c '\
  trap "echo Shutting down dockerd; pkill dockerd || true" EXIT; \
  echo "Starting Docker daemon..."; \
  dockerd > /var/log/dockerd.log 2>&1 & \
  for i in {1..30}; do \
    docker info > /dev/null 2>&1 && break || echo "Waiting for Docker... ($i/30)"; sleep 1; \
  done; \
  if ! docker info > /dev/null 2>&1; then \
    echo "❌ Docker failed to start. Log output:"; cat /var/log/dockerd.log || echo "(No log found)"; exit 1; \
  fi; \
  echo "✅ Docker is ready. Running integration tests..."; \
  dotnet test --filter "Category=Integration" --nologo --logger "console;verbosity=${VERBOSITY:-minimal}"'
   

#####################################################################
# Runtime image, uses PORT, TAG, MAIN_API_DLL, MAIN_API_NAME, APP_DIR
#####################################################################

FROM mcr.microsoft.com/dotnet/aspnet:$ASPNET_VERSION-alpine as runtime

RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib tzdata

ARG PORT MAIN_API_NAME APP_DIR MAIN_API_DLL TAG

WORKDIR $APP_DIR
COPY --from=build $APP_DIR .

# create a new user and change directory ownership
RUN adduser --disabled-password \
   --home "$APP_DIR" \
   --gecos '' dotnetuser && chown -R dotnetuser:dotnetuser "$APP_DIR"

# impersonate into the new user
USER dotnetuser

ENV \
   ASPNETCORE_URLS=http://*:$PORT \
   ASPNETCORE_ENVIRONMENT=Production \
   TZ=Europe/Amsterdam \
   DOTNET_CLI_TELEMETRY_OPTOUT=true \
   DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE $PORT

WORKDIR "$APP_DIR/$MAIN_API_NAME"

ENV PROGRAM="$MAIN_API_DLL"
ENTRYPOINT dotnet "$PROGRAM"