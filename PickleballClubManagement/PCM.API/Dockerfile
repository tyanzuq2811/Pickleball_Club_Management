# Sử dụng image SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy các file csproj và restore dependencies
COPY ["PCM.API/PCM.API.csproj", "PCM.API/"]
COPY ["PCM.Application/PCM.Application.csproj", "PCM.Application/"]
COPY ["PCM.Domain/PCM.Domain.csproj", "PCM.Domain/"]
COPY ["PCM.Infrastructure/PCM.Infrastructure.csproj", "PCM.Infrastructure/"]
RUN dotnet restore "PCM.API/PCM.API.csproj"

# Copy toàn bộ source code và build
COPY . .
WORKDIR "/src/PCM.API"
RUN dotnet build "PCM.API.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "PCM.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Tạo image runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PCM.API.dll"]