FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["RealTime.Web/RealTime.Web.csproj", "RealTime.Web/"]
RUN dotnet restore "RealTime.Web/RealTime.Web.csproj"
COPY . .
WORKDIR "/src/RealTime.Web"
RUN dotnet build "RealTime.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealTime.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealTime.Web.dll"]