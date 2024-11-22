FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RabbitMqLearning/RabbitMqLearning.csproj", "RabbitMqLearning/"]
RUN dotnet restore "RabbitMqLearning/RabbitMqLearning.csproj"
COPY . .
WORKDIR "/src/RabbitMqLearning"
RUN dotnet build "RabbitMqLearning.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMqLearning.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMqLearning.dll"]
