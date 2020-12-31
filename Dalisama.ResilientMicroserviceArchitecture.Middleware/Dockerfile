#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Dalisama.ResilientMicroserviceArchitecture.Middleware/Dalisama.ResilientMicroserviceArchitecture.Middleware.csproj", "Dalisama.ResilientMicroserviceArchitecture.Middleware/"]
RUN dotnet restore "Dalisama.ResilientMicroserviceArchitecture.Middleware/Dalisama.ResilientMicroserviceArchitecture.Middleware.csproj"
COPY . .
WORKDIR "/src/Dalisama.ResilientMicroserviceArchitecture.Middleware"
RUN dotnet build "Dalisama.ResilientMicroserviceArchitecture.Middleware.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dalisama.ResilientMicroserviceArchitecture.Middleware.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dalisama.ResilientMicroserviceArchitecture.Middleware.dll"]