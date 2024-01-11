# Install dependencies only when needed
FROM node:16-alpine AS deps
RUN apk add --no-cache libc6-compat
WORKDIR /app
COPY package.json yarn.lock .npmrc ./
RUN yarn install --frozen-lockfile && npm list @dcx/dcx-backend |grep dcx-backend|grep -o 'dcx-backend@.*' |cut -f2- -d@ > /dcx-backend-version 

# Rebuild the source code only when needed
FROM node:16-alpine AS builder

WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY --from=deps /dcx-backend-version   /dcx-backend-version 

COPY . .

RUN yarn build

EXPOSE 3000
ENV PORT 3000
ENV NODE_ENV=production

CMD ["node", "server.js"]

