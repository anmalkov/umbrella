FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy-arm64v8 AS base
#FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy-arm64v8 AS build
#FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim-arm64v8
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["Umbrella.Ui/Umbrella.Ui.csproj", "Umbrella.Ui/"]
RUN dotnet restore "Umbrella.Ui/Umbrella.Ui.csproj"
COPY . .
WORKDIR "/src/Umbrella.Ui"
RUN dotnet build "Umbrella.Ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Umbrella.Ui.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Umbrella.Ui.dll"]