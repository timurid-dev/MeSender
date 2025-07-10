FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY ["Services/Identity/MeSender.Identity.WebApi/MeSender.Identity.WebApi.csproj", "Services/Identity/MeSender.Identity.WebApi/"]
COPY ["Services/Identity/MeSender.Identity/MeSender.Identity.csproj", "Services/Identity/MeSender.Identity/"]
RUN dotnet restore "Services/Identity/MeSender.Identity.WebApi/MeSender.Identity.WebApi.csproj"
COPY . .
WORKDIR "/src/Services/Identity/MeSender.Identity.WebApi"
RUN dotnet build "MeSender.Identity.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeSender.Identity.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeSender.Identity.WebApi.dll"] 