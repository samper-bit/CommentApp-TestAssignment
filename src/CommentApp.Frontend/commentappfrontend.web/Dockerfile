FROM node:20 AS build

WORKDIR /usr/app
COPY ./ /usr/app

RUN npm install

RUN npm install @microsoft/signalr

RUN cd node_modules/npm-run-all2 && \
    echo "module.exports = require('@npmcli/package-json');" > read-package-json.js

RUN npm run build

FROM nginx:alpine

COPY --from=build /usr/app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]