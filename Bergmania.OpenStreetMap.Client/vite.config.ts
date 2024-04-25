import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            entry: "src/bergmania-openstreetmap-property-editor.ts", // your web component source file
            formats: ["es"],
        },
        outDir: "../Bergmania.OpenStreetMap.StaticAssets/wwwroot/App_Plugins/Bergmania.OpenStreetMap", // your web component will be saved in this location
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    },
});