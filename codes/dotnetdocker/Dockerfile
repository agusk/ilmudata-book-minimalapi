FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY dotnetdocker.csproj .
RUN dotnet restore "dotnetdocker.csproj"
COPY . .
RUN dotnet build "dotnetdocker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dotnetdocker.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnetdocker.dll"]