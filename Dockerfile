# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution or project files for restore
COPY ./JotLink.BackEnd/*.csproj ./JotLink.BackEnd/
COPY ./JotLink.Shared/*.csproj ./JotLink.Shared/

# Restore dependencies for both projects
RUN dotnet restore ./JotLink.BackEnd/JotLink.BackEnd.csproj

# Copy everything else
COPY . .

# Publish the backend project
RUN dotnet publish ./JotLink.BackEnd/JotLink.BackEnd.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "JotLink.BackEnd.dll"]