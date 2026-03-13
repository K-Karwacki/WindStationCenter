# Build stage
FROM node:24-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

ENV VITE_API_HOST=""

COPY . .
RUN npm run build

# Serve stage
FROM nginx:alpine

# install envsubst for template rendering
RUN apk add --no-cache gettext

COPY --from=build /app/dist /usr/share/nginx/html
# copy nginx configuration as a template to be rendered at container start
COPY nginx.conf /etc/nginx/templates/default.conf.template

EXPOSE 80
