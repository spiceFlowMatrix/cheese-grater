FROM adorsys/keycloak-config-cli:latest

USER root
RUN apt-get update && apt-get install -y gettext-base curl && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /opt/keycloak-config/templates /opt/keycloak-config/generated \
    && chown -R 1000:1000 /opt/keycloak-config

# 🔥 Override the base image environment that forces /config/*
ENV IMPORT_FILES_LOCATIONS=""
ENV IMPORT_PATH=""
ENV IMPORT_FILE=""    

COPY master-realm.yaml.template /opt/keycloak-config/templates/
COPY entrypoint.sh /opt/keycloak-config/entrypoint.sh
RUN chmod +x /opt/keycloak-config/entrypoint.sh \
    && chown 1000:1000 /opt/keycloak-config/entrypoint.sh

USER 1000
ENTRYPOINT ["/opt/keycloak-config/entrypoint.sh"]
