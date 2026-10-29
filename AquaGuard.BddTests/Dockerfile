FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /src
COPY . .
RUN dotnet restore
CMD ["dotnet", "test", "--no-restore", "--logger", "trx;LogFileName=resultado.trx", "--logger", "html;LogFileName=relatorio.html", "--results-directory", "/results"]
