FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
RUN dotnet build

# Set environment variable for testing
ENV SecretThree="MyVerySecretActualSecretKeyIsThisSecretKeyNothingMore"
RUN cd /App \ && dotnet test
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "Banking.API.dll"]

#RUN dotnet test #--configuration Release