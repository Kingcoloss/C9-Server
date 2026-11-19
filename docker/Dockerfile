# syntax=docker/dockerfile:1
# C9 platform image: game server (agent mode) + Admin Portal. See docs/SERVER_PLATFORM_ARCHITECTURE_TH.md.
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:10.0
ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:10.0

FROM ${DOTNET_SDK} AS build
WORKDIR /src
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
COPY NuGet.Config global.json Directory.Build.props ./
COPY re/evidence/static/windows_map_manifest.json re/evidence/static/
COPY c9server/data/twisters.json c9server/data/
# .dockerignore drops the Windows-only projects (launchers, desktop admin); optional portal modules
# (C9.Portal.Accounts / C9.Portal.World and C9.GameData) are built in automatically when present.
COPY src/ src/
# Optional: behind a TLS-inspecting proxy pass its CA bundle as a build secret
#   docker build --secret id=build_ca,src=C:/path/ca-bundle.crt ...   (never stored in the image)
RUN --mount=type=secret,id=build_ca,required=false \
    if [ -s /run/secrets/build_ca ]; then export SSL_CERT_FILE=/run/secrets/build_ca; fi \
 && dotnet publish src/C9.Server/C9.Server.csproj -c Release -o /out/server \
 && dotnet publish src/C9.Portal/C9.Portal.csproj -c Release -o /out/portal \
 && dotnet publish src/C9.AuthService/C9.AuthService.csproj -c Release -o /out/auth

FROM ${DOTNET_RUNTIME} AS runtime
LABEL org.opencontainers.image.title="c9-platform" \
      org.opencontainers.image.description="C9 local game services (agent mode) and Admin Portal"
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_gcServer=0 \
    ASPNETCORE_ENVIRONMENT=Production \
    TZ=Asia/Bangkok
WORKDIR /c9/app
COPY --from=build /out/server/ /c9/app/server/
COPY --from=build /out/portal/ /c9/app/portal/
COPY --from=build /out/auth/ /c9/app/auth/
COPY --chmod=0644 config/lab-official-login-fixture.json config/lab-official-character-lobby.json /c9/app/config/
COPY --chmod=0755 docker/init-state.sh /c9/app/docker/init-state.sh
# sed: tolerate a Windows checkout that converted the script to CRLF.
RUN sed -i 's/\r$//' /c9/app/docker/init-state.sh \
 && chmod 0755 /c9/app/config /c9/app/docker && mkdir -p /c9/state && chown 1654:1654 /c9/state
# 'app' (uid 1654) is the non-root user that ships with the .NET runtime images.
USER 1654:1654
VOLUME ["/c9/state"]
ENTRYPOINT ["dotnet", "/c9/app/server/C9.Server.dll"]
