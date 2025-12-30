# syntax=docker/dockerfile:1.7
FROM node:20-bullseye AS base
WORKDIR /workspace

ENV NX_DOTNET_NO_RESTORE=1 \
    NX_SKIP_RUN_MIGRATIONS=1 \
    NX_CACHE_DIRECTORY=/workspace/.nx/cache \
    CI=true \
    YARN_IGNORE_SCRIPTS=1 \
    NODE_OPTIONS=--max-old-space-size=4096

RUN corepack enable && corepack prepare yarn@1.22.22 --activate

FROM base AS deps
COPY package.json yarn.lock* .yarnrc .yarnrc.yml ./
COPY .yarn .yarn

RUN --mount=type=cache,target=/tmp/yarn-cache \
    yarn install --frozen-lockfile --ignore-scripts --ignore-optional --cache-folder /tmp/yarn-cache

FROM base AS build
COPY --from=deps /workspace/node_modules ./node_modules
COPY --from=deps /workspace/.yarn ./.yarn
COPY --from=deps /workspace/.yarnrc* ./
COPY package.json yarn.lock* ./
COPY nx.json project.json tsconfig.base.json jest.config.ts jest.preset.js ./
COPY apps ./apps
COPY libs ./libs

RUN --mount=type=cache,target=/workspace/.nx/cache \
    yarn nx build todo --configuration=production --skip-nx-cache=false

FROM nginx:1.27-alpine AS runtime
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*
COPY apps/todo/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /workspace/dist/apps/todo/browser .

RUN apk add --no-cache curl
EXPOSE 80

HEALTHCHECK --interval=15s --timeout=5s --retries=5 CMD curl -f http://localhost/ || exit 1
