#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-sac2016 AS build
WORKDIR /src
COPY ["src/Count4U.Service.WebAPI.Authentication/Count4U.Service.WebAPI.Authentication.csproj", "src/Count4U.Service.WebAPI.Authentication/"]
COPY ["src/Count4U.Service.Common/Count4U.Service.Common.csproj", "src/Count4U.Service.Common/"]
COPY ["src/Count4U.Service.Model/Count4U.Service.Model.csproj", "src/Count4U.Service.Model/"]
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