FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2016
ARG source
WORKDIR /PublishFiles
COPY ${source:-bin/app.publish} .