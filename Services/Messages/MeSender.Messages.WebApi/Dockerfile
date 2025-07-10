FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY ["Services/Messages/MeSender.Messages.WebApi/MeSender.Messages.WebApi.csproj", "Services/Messages/MeSender.Messages.WebApi/"]
COPY ["Services/Messages/MeSender.Messages/MeSender.Messages.csproj", "Services/Messages/MeSender.Messages/"]
RUN dotnet restore "Services/Messages/MeSender.Messages.WebApi/MeSender.Messages.WebApi.csproj"
COPY . .
WORKDIR "/src/Services/Messages/MeSender.Messages.WebApi"
RUN dotnet build "MeSender.Messages.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeSender.Messages.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeSender.Messages.WebApi.dll"] 