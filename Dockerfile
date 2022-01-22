#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
#WORKDIR /src
#COPY ["ZHONGJIAN_API.csproj", "."]
#RUN dotnet restore "./ZHONGJIAN_API.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "ZHONGJIAN_API.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "ZHONGJIAN_API.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "ZHONGJIAN_API.dll"]

#¾µÏñÉú³É
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base

COPY . /app
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
ENV LANG C.UTF-8

WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "ZHONGJIAN_API.dll"]