FROM adorsys/keycloak-config-cli:latest

USER root
RUN apt-get update \
    && apt-get install -y gettext-base curl bash \
    && rm -rf /var/lib/apt/lists/*

RUN mkdir -p /opt/keycloak-config/templates /opt/keycloak-config/generated \
    && chown -R 1000:1000 /opt/keycloak-config

ENV IMPORT_FILES_LOCATIONS="" \
    IMPORT_PATH="" \
    IMPORT_FILE=""

COPY master-realm.yaml.template /opt/keycloak-config/templates/
COPY entrypoint.sh /opt/keycloak-config/entrypoint.sh

# 🔥 Always convert CRLF to LF, even if dos2unix is missing
RUN sed -i 's/\r$//' /opt/keycloak-config/entrypoint.sh \
    && chmod +x /opt/keycloak-config/entrypoint.sh \
    && chown 1000:1000 /opt/keycloak-config/entrypoint.sh

USER 1000

ENTRYPOINT ["bash", "/opt/keycloak-config/entrypoint.sh"]
