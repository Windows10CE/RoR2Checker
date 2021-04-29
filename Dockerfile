FROM mcr.microsoft.com/dotnet/runtime:5.0

ENV COMPlus_EnableDiagnostics=0
COPY bin/Release/net5.0/publish /App
COPY ReferenceAssemblies /App/ReferenceAssemblies
WORKDIR /App
ENTRYPOINT ["dotnet", "RoR2Checker.dll"]
