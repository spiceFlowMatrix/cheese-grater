/**
 * Custom TypeScript module declaration for 'keycloak-js'.
 *
 * ❗ WHY THIS IS NEEDED:
 * The `keycloak-js` package (v26+) is ESM-only and uses the "exports" field in its package.json,
 * which hides internal paths from TypeScript under the default "moduleResolution": "node".
 *
 * Even though the actual types live at:
 *    node_modules/keycloak-js/lib/keycloak.d.ts
 * TypeScript cannot find them unless "moduleResolution" is set to "bundler" or "nodenext".
 *
 * However, changing "moduleResolution" globally may break other parts of an Nx monorepo.
 * So instead, we use this workaround to tell TypeScript how to resolve 'keycloak-js'.
 *
 * ✅ WHAT THIS DOES:
 * This file tells TypeScript: "If someone imports 'keycloak-js', redirect to its real internal file".
 *
 * That way, type checking and IntelliSense work — without modifying tsconfig.moduleResolution.
 */
declare module 'keycloak-js' {
  import Keycloak from 'keycloak-js/lib/keycloak';
  export default Keycloak;
}
