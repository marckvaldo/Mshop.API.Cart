# Etapa de build (compilação com o SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definir diretório de trabalho para a aplicação
WORKDIR /src

# Copiar os arquivos da solução para o container
COPY ../ .

# Copiar arquivos .proto para o local esperado
COPY Mshop.Infra.Consumer/Protos Mshop.Infra.Consumer/Protos
RUN ls -la Mshop.Infra.Consumer/Protos
RUN test -f Mshop.Infra.Consumer/Protos/ProductProto.proto || (echo "Arquivo ProductProto.proto não encontrado!" && exit 1)

# Restaurar as dependências
RUN dotnet restore Mshop.API.Cart/Mshop.API.Cart.csproj
RUN dotnet restore Mshop.GraphQL.Cart/Mshop.API.GraphQL.Cart.csproj


# Publicar o projeto
RUN dotnet publish Mshop.API.Cart/Mshop.API.Cart.csproj -c Release -o /app/APIShopping
RUN dotnet publish Mshop.GraphQL.Cart/Mshop.API.GraphQL.Cart.csproj -c Release -o /app/APIGraphQL

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Criar um diretório para a aplicação
WORKDIR /app

# Instalar Supervisord
RUN apt-get update && apt-get install -y supervisor && rm -rf /var/lib/apt/lists/*

# Criar um usuário não-root e definir permissões
RUN useradd -m -s /bin/bash appuser
RUN chown -R appuser:appuser /app
RUN mkdir -p /var/log && chown -R appuser:appuser /var/log

# Copiar os arquivos publicados do container de build
COPY --from=build /app /app

# Copiar configuração do supervisord
COPY docker/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Mudar para o usuário não-root
USER appuser

CMD ["supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]