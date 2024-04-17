import { cpSync, rmSync } from 'fs';

const srcDir = './dist';
const outputDir = '../Bergmania.OpenStreetMap.StaticAssets/wwwroot/App_Plugins/Bergmania.OpenStreetMap';

rmSync(outputDir, { recursive: true, force: true });
cpSync(srcDir, outputDir, { recursive: true });

console.log('--- Copied build output to static assets successfully. ---');
