FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Coders-Back.Host/Coders-Back.Host.csproj", "Coders-Back.Host/"]
RUN dotnet restore "Coders-Back.Host/Coders-Back.Host.csproj"
COPY . .
WORKDIR "/src/Coders-Back.Host"
RUN dotnet build "Coders-Back.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Coders-Back.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Coders-Back.Host.dll"]
