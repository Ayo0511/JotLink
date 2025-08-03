# Use official .NET SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ../JotLink.sln ./
COPY . ./JotLink.BackEnd/
COPY ../JotLink.Shared/ ./JotLink.Shared/

# Restore
RUN dotnet restore ./JotLink.BackEnd/JotLink.BackEnd.csproj

# Build and publish
RUN dotnet publish ./JotLink.BackEnd/JotLink.BackEnd.csproj -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Use Render’s dynamic port
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 10000

ENTRYPOINT ["dotnet", "JotLink.BackEnd.dll"]