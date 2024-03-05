# ------------------------------------------------------------------ build ---

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

ARG APP_NAME=Alpha.Token.Service

ARG GITHUB_USER
ENV GITHUB_USER ${GITHUB_USER}

ARG GITHUB_TOKEN
ENV GITHUB_TOKEN ${GITHUB_TOKEN}

COPY . ./${APP_NAME}

RUN dotnet restore ${APP_NAME}

# ----------------------------------------------------------------- publish ---

FROM build as publish

ARG APP_NAME=Alpha.Token.Service

RUN dotnet publish ./${APP_NAME} -c Release -o /${APP_NAME}/publish/ 

# --------------------------------------------------------------------- app ---

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS app
EXPOSE 8080

ARG APP_NAME=Alpha.Token.Service
ENV APP_NAME ${APP_NAME}

WORKDIR /app

COPY --from=publish /${APP_NAME}/publish/ .


ENTRYPOINT ["dotnet", "Alpha.Token.Service.dll"]
