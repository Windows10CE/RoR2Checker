FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

ENV COMPlus_EnableDiagnostics=0
WORKDIR /Build
COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/runtime:5.0

ENV COMPlus_EnableDiagnostics=0
WORKDIR /App
COPY --from=build /Build/bin/Release/net5.0/publish .
COPY ReferenceAssemblies ReferenceAssemblies
ENTRYPOINT ["dotnet", "RoR2Checker.dll"]
