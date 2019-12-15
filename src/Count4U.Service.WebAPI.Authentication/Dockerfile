#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["src/Count4U.Service.WebAPI.Authentication/Count4U.Service.WebAPI.Authentication.csproj", "src/Count4U.Service.WebAPI.Authentication/"]
RUN dotnet restore "src/Count4U.Service.WebAPI.Authentication/Count4U.Service.WebAPI.Authentication.csproj"
COPY . .
WORKDIR "/src/src/Count4U.Service.WebAPI.Authentication"
RUN dotnet build "Count4U.Service.WebAPI.Authentication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Count4U.Service.WebAPI.Authentication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Count4U.Service.WebAPI.Authentication.dll"]