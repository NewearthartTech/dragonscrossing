/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  env: {
    API: process.env.NEXT_PUBLIC_API || ".",
    ALCHEMY_KEY: process.env.NEXT_PUBLIC_ALCHEMY_KEY,
  },
};

module.exports = nextConfig;
