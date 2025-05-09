#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Bookline.Api/Bookline.Api.csproj", "Bookline.Api/"]
COPY ["Bookline.Application/Bookline.Application.csproj", "Bookline.Application/"]
COPY ["Bookline.Domain/Bookline.Domain.csproj", "Bookline.Domain/"]
COPY ["Bookline.Shared/Bookline.Shared.csproj", "Bookline.Shared/"]
COPY ["Bookline.Infrastructure/Bookline.Infrastructure.csproj", "Bookline.Infrastructure/"]
RUN dotnet restore "Bookline.Api/Bookline.Api.csproj"
COPY . .
WORKDIR "/src/Bookline.Api"
RUN dotnet build "Bookline.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bookline.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookline.Api.dll"]