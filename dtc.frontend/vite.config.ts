import { defineConfig } from "vite";
import plugin from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin(), tailwindcss()],
  server: {
    port: 49622,
    proxy: {
      "/api": {
        target: "https://localhost:7142",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
