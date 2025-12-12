//! Licensed to the .NET Foundation under one or more agreements.
//! The .NET Foundation licenses this file to you under the MIT license.

var e=!1;const t=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,4,1,96,0,0,3,2,1,0,10,8,1,6,0,6,64,25,11,11])),o=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,15,1,13,0,65,1,253,15,65,2,253,15,253,128,2,11])),n=async()=>WebAssembly.validate(new Uint8Array([0,97,115,109,1,0,0,0,1,5,1,96,0,1,123,3,2,1,0,10,10,1,8,0,65,0,253,15,253,98,11])),r=Symbol.for("wasm promise_control");function i(e,t){let o=null;const n=new Promise((function(n,r){o={isDone:!1,promise:null,resolve:t=>{o.isDone||(o.isDone=!0,n(t),e&&e())},reject:e=>{o.isDone||(o.isDone=!0,r(e),t&&t())}}}));o.promise=n;const i=n;return i[r]=o,{promise:i,promise_control:o}}function s(e){return e[r]}function a(e){e&&function(e){return void 0!==e[r]}(e)||Be(!1,"Promise is not controllable")}const l="__mono_message__",c=["debug","log","trace","warn","info","error"],d="MONO_WASM: ";let u,f,m,g,p,h;function w(e){g=e}function b(e){if(Pe.diagnosticTracing){const t="function"==typeof e?e():e;console.debug(d+t)}}function y(e,...t){console.info(d+e,...t)}function v(e,...t){console.info(e,...t)}function E(e,...t){console.warn(d+e,...t)}function _(e,...t){if(t&&t.length>0&&t[0]&&"object"==typeof t[0]){if(t[0].silent)return;if(t[0].toString)return void console.error(d+e,t[0].toString())}console.error(d+e,...t)}function x(e,t,o){return function(...n){try{let r=n[0];if(void 0===r)r="undefined";else if(null===r)r="null";else if("function"==typeof r)r=r.toString();else if("string"!=typeof r)try{r=JSON.stringify(r)}catch(e){r=r.toString()}t(o?JSON.stringify({method:e,payload:r,arguments:n.slice(1)}):[e+r,...n.slice(1)])}catch(e){m.error(`proxyConsole failed: ${e}`)}}}function j(e,t,o){f=t,g=e,m={...t};const n=`${o}/console`.replace("https://","wss://").replace("http://","ws://");u=new WebSocket(n),u.addEventListener("error",A),u.addEventListener("close",S),function(){for(const e of c)f[e]=x(`console.${e}`,T,!0)}()}function R(e){let t=30;const o=()=>{u?0==u.bufferedAmount||0==t?(e&&v(e),function(){for(const e of c)f[e]=x(`console.${e}`,m.log,!1)}(),u.removeEventListener("error",A),u.removeEventListener("close",S),u.close(1e3,e),u=void 0):(t--,globalThis.setTimeout(o,100)):e&&m&&m.log(e)};o()}function T(e){u&&u.readyState===WebSocket.OPEN?u.send(e):m.log(e)}function A(e){m.error(`[${g}] proxy console websocket error: ${e}`,e)}function S(e){m.debug(`[${g}] proxy console websocket closed: ${e}`,e)}function D(){Pe.preferredIcuAsset=O(Pe.config);let e="invariant"==Pe.config.globalizationMode;if(!e)if(Pe.preferredIcuAsset)Pe.diagnosticTracing&&b("ICU data archive(s) available, disabling invariant mode");else{if("custom"===Pe.config.globalizationMode||"all"===Pe.config.globalizationMode||"sharded"===Pe.config.globalizationMode){const e="invariant globalization mode is inactive and no ICU data archives are available";throw _(`ERROR: ${e}`),new Error(e)}Pe.diagnosticTracing&&b("ICU data archive(s) not available, using invariant globalization mode"),e=!0,Pe.preferredIcuAsset=null}const t="DOTNET_SYSTEM_GLOBALIZATION_INVARIANT",o=Pe.config.environmentVariables;if(void 0===o[t]&&e&&(o[t]="1"),void 0===o.TZ)try{const e=Intl.DateTimeFormat().resolvedOptions().timeZone||null;e&&(o.TZ=e)}catch(e){y("failed to detect timezone, will fallback to UTC")}}function O(e){var t;if((null===(t=e.resources)||void 0===t?void 0:t.icu)&&"invariant"!=e.globalizationMode){const t=e.applicationCulture||(ke?globalThis.navigator&&globalThis.navigator.languages&&globalThis.navigator.languages[0]:Intl.DateTimeFormat().resolvedOptions().locale),o=e.resources.icu;let n=null;if("custom"===e.globalizationMode){if(o.length>=1)return o[0].name}else t&&"all"!==e.globalizationMode?"sharded"===e.globalizationMode&&(n=function(e){const t=e.split("-")[0];return"en"===t||["fr","fr-FR","it","it-IT","de","de-DE","es","es-ES"].includes(e)?"icudt_EFIGS.dat":["zh","ko","ja"].includes(t)?"icudt_CJK.dat":"icudt_no_CJK.dat"}(t)):n="icudt.dat";if(n)for(let e=0;e<o.length;e++){const t=o[e];if(t.virtualPath===n)return t.name}}return e.globalizationMode="invariant",null}(new Date).valueOf();const C=class{constructor(e){this.url=e}toString(){return this.url}};async function k(e,t){try{const o="function"==typeof globalThis.fetch;if(Se){const n=e.startsWith("file://");if(!n&&o)return globalThis.fetch(e,t||{credentials:"same-origin"});p||(h=Ne.require("url"),p=Ne.require("fs")),n&&(e=h.fileURLToPath(e));const r=await p.promises.readFile(e);return{ok:!0,headers:{length:0,get:()=>null},url:e,arrayBuffer:()=>r,json:()=>JSON.parse(r),text:()=>{throw new Error("NotImplementedException")}}}if(o)return globalThis.fetch(e,t||{credentials:"same-origin"});if("function"==typeof read)return{ok:!0,url:e,headers:{length:0,get:()=>null},arrayBuffer:()=>new Uint8Array(read(e,"binary")),json:()=>JSON.parse(read(e,"utf8")),text:()=>read(e,"utf8")}}catch(t){return{ok:!1,url:e,status:500,headers:{length:0,get:()=>null},statusText:"ERR28: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t},text:()=>{throw t}}}throw new Error("No fetch implementation available")}function I(e){return"string"!=typeof e&&Be(!1,"url must be a string"),!M(e)&&0!==e.indexOf("./")&&0!==e.indexOf("../")&&globalThis.URL&&globalThis.document&&globalThis.document.baseURI&&(e=new URL(e,globalThis.document.baseURI).toString()),e}const U=/^[a-zA-Z][a-zA-Z\d+\-.]*?:\/\//,P=/[a-zA-Z]:[\\/]/;function M(e){return Se||Ie?e.startsWith("/")||e.startsWith("\\")||-1!==e.indexOf("///")||P.test(e):U.test(e)}let L,N=0;const $=[],z=[],W=new Map,F={"js-module-threads":!0,"js-module-runtime":!0,"js-module-dotnet":!0,"js-module-native":!0,"js-module-diagnostics":!0},B={...F,"js-module-library-initializer":!0},V={...F,dotnetwasm:!0,heap:!0,manifest:!0},q={...B,manifest:!0},H={...B,dotnetwasm:!0},J={dotnetwasm:!0,symbols:!0},Z={...B,dotnetwasm:!0,symbols:!0},Q={symbols:!0};function G(e){return!("icu"==e.behavior&&e.name!=Pe.preferredIcuAsset)}function K(e,t,o){null!=t||(t=[]),Be(1==t.length,`Expect to have one ${o} asset in resources`);const n=t[0];return n.behavior=o,X(n),e.push(n),n}function X(e){V[e.behavior]&&W.set(e.behavior,e)}function Y(e){Be(V[e],`Unknown single asset behavior ${e}`);const t=W.get(e);if(t&&!t.resolvedUrl)if(t.resolvedUrl=Pe.locateFile(t.name),F[t.behavior]){const e=ge(t);e?("string"!=typeof e&&Be(!1,"loadBootResource response for 'dotnetjs' type should be a URL string"),t.resolvedUrl=e):t.resolvedUrl=ce(t.resolvedUrl,t.behavior)}else if("dotnetwasm"!==t.behavior)throw new Error(`Unknown single asset behavior ${e}`);return t}function ee(e){const t=Y(e);return Be(t,`Single asset for ${e} not found`),t}let te=!1;async function oe(){if(!te){te=!0,Pe.diagnosticTracing&&b("mono_download_assets");try{const e=[],t=[],o=(e,t)=>{!Z[e.behavior]&&G(e)&&Pe.expected_instantiated_assets_count++,!H[e.behavior]&&G(e)&&(Pe.expected_downloaded_assets_count++,t.push(se(e)))};for(const t of $)o(t,e);for(const e of z)o(e,t);Pe.allDownloadsQueued.promise_control.resolve(),Promise.all([...e,...t]).then((()=>{Pe.allDownloadsFinished.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),await Pe.runtimeModuleLoaded.promise;const n=async e=>{const t=await e;if(t.buffer){if(!Z[t.behavior]){t.buffer&&"object"==typeof t.buffer||Be(!1,"asset buffer must be array-like or buffer-like or promise of these"),"string"!=typeof t.resolvedUrl&&Be(!1,"resolvedUrl must be string");const e=t.resolvedUrl,o=await t.buffer,n=new Uint8Array(o);pe(t),await Ue.beforeOnRuntimeInitialized.promise,Ue.instantiate_asset(t,e,n)}}else J[t.behavior]?("symbols"===t.behavior&&(await Ue.instantiate_symbols_asset(t),pe(t)),J[t.behavior]&&++Pe.actual_downloaded_assets_count):(t.isOptional||Be(!1,"Expected asset to have the downloaded buffer"),!H[t.behavior]&&G(t)&&Pe.expected_downloaded_assets_count--,!Z[t.behavior]&&G(t)&&Pe.expected_instantiated_assets_count--)},r=[],i=[];for(const t of e)r.push(n(t));for(const e of t)i.push(n(e));Promise.all(r).then((()=>{Ce||Ue.coreAssetsInMemory.promise_control.resolve()})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e})),Promise.all(i).then((async()=>{Ce||(await Ue.coreAssetsInMemory.promise,Ue.allAssetsInMemory.promise_control.resolve())})).catch((e=>{throw Pe.err("Error in mono_download_assets: "+e),Xe(1,e),e}))}catch(e){throw Pe.err("Error in mono_download_assets: "+e),e}}}let ne=!1;function re(){if(ne)return;ne=!0;const e=Pe.config,t=[];if(e.assets)for(const t of e.assets)"object"!=typeof t&&Be(!1,`asset must be object, it was ${typeof t} : ${t}`),"string"!=typeof t.behavior&&Be(!1,"asset behavior must be known string"),"string"!=typeof t.name&&Be(!1,"asset name must be string"),t.resolvedUrl&&"string"!=typeof t.resolvedUrl&&Be(!1,"asset resolvedUrl could be string"),t.hash&&"string"!=typeof t.hash&&Be(!1,"asset resolvedUrl could be string"),t.pendingDownload&&"object"!=typeof t.pendingDownload&&Be(!1,"asset pendingDownload could be object"),t.isCore?$.push(t):z.push(t),X(t);else if(e.resources){const o=e.resources;o.wasmNative||Be(!1,"resources.wasmNative must be defined"),o.jsModuleNative||Be(!1,"resources.jsModuleNative must be defined"),o.jsModuleRuntime||Be(!1,"resources.jsModuleRuntime must be defined"),K(z,o.wasmNative,"dotnetwasm"),K(t,o.jsModuleNative,"js-module-native"),K(t,o.jsModuleRuntime,"js-module-runtime"),o.jsModuleDiagnostics&&K(t,o.jsModuleDiagnostics,"js-module-diagnostics");const n=(e,t,o)=>{const n=e;n.behavior=t,o?(n.isCore=!0,$.push(n)):z.push(n)};if(o.coreAssembly)for(let e=0;e<o.coreAssembly.length;e++)n(o.coreAssembly[e],"assembly",!0);if(o.assembly)for(let e=0;e<o.assembly.length;e++)n(o.assembly[e],"assembly",!o.coreAssembly);if(0!=e.debugLevel&&Pe.isDebuggingSupported()){if(o.corePdb)for(let e=0;e<o.corePdb.length;e++)n(o.corePdb[e],"pdb",!0);if(o.pdb)for(let e=0;e<o.pdb.length;e++)n(o.pdb[e],"pdb",!o.corePdb)}if(e.loadAllSatelliteResources&&o.satelliteResources)for(const e in o.satelliteResources)for(let t=0;t<o.satelliteResources[e].length;t++){const r=o.satelliteResources[e][t];r.culture=e,n(r,"resource",!o.coreAssembly)}if(o.coreVfs)for(let e=0;e<o.coreVfs.length;e++)n(o.coreVfs[e],"vfs",!0);if(o.vfs)for(let e=0;e<o.vfs.length;e++)n(o.vfs[e],"vfs",!o.coreVfs);const r=O(e);if(r&&o.icu)for(let e=0;e<o.icu.length;e++){const t=o.icu[e];t.name===r&&n(t,"icu",!1)}if(o.wasmSymbols)for(let e=0;e<o.wasmSymbols.length;e++)n(o.wasmSymbols[e],"symbols",!1)}if(e.appsettings)for(let t=0;t<e.appsettings.length;t++){const o=e.appsettings[t],n=he(o);"appsettings.json"!==n&&n!==`appsettings.${e.applicationEnvironment}.json`||z.push({name:o,behavior:"vfs",noCache:!0,useCredentials:!0})}e.assets=[...$,...z,...t]}async function ie(e){const t=await se(e);return await t.pendingDownloadInternal.response,t.buffer}async function se(e){try{return await ae(e)}catch(t){if(!Pe.enableDownloadRetry)throw t;if(Ie||Se)throw t;if(e.pendingDownload&&e.pendingDownloadInternal==e.pendingDownload)throw t;if(e.resolvedUrl&&-1!=e.resolvedUrl.indexOf("file://"))throw t;if(t&&404==t.status)throw t;e.pendingDownloadInternal=void 0,await Pe.allDownloadsQueued.promise;try{return Pe.diagnosticTracing&&b(`Retrying download '${e.name}'`),await ae(e)}catch(t){return e.pendingDownloadInternal=void 0,await new Promise((e=>globalThis.setTimeout(e,100))),Pe.diagnosticTracing&&b(`Retrying download (2) '${e.name}' after delay`),await ae(e)}}}async function ae(e){for(;L;)await L.promise;try{++N,N==Pe.maxParallelDownloads&&(Pe.diagnosticTracing&&b("Throttling further parallel downloads"),L=i());const t=await async function(e){if(e.pendingDownload&&(e.pendingDownloadInternal=e.pendingDownload),e.pendingDownloadInternal&&e.pendingDownloadInternal.response)return e.pendingDownloadInternal.response;if(e.buffer){const t=await e.buffer;return e.resolvedUrl||(e.resolvedUrl="undefined://"+e.name),e.pendingDownloadInternal={url:e.resolvedUrl,name:e.name,response:Promise.resolve({ok:!0,arrayBuffer:()=>t,json:()=>JSON.parse(new TextDecoder("utf-8").decode(t)),text:()=>{throw new Error("NotImplementedException")},headers:{get:()=>{}}})},e.pendingDownloadInternal.response}const t=e.loadRemote&&Pe.config.remoteSources?Pe.config.remoteSources:[""];let o;for(let n of t){n=n.trim(),"./"===n&&(n="");const t=le(e,n);e.name===t?Pe.diagnosticTracing&&b(`Attempting to download '${t}'`):Pe.diagnosticTracing&&b(`Attempting to download '${t}' for ${e.name}`);try{e.resolvedUrl=t;const n=fe(e);if(e.pendingDownloadInternal=n,o=await n.response,!o||!o.ok)continue;return o}catch(e){o||(o={ok:!1,url:t,status:0,statusText:""+e});continue}}const n=e.isOptional||e.name.match(/\.pdb$/)&&Pe.config.ignorePdbLoadErrors;if(o||Be(!1,`Response undefined ${e.name}`),!n){const t=new Error(`download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`);throw t.status=o.status,t}y(`optional download '${o.url}' for ${e.name} failed ${o.status} ${o.statusText}`)}(e);return t?(J[e.behavior]||(e.buffer=await t.arrayBuffer(),++Pe.actual_downloaded_assets_count),e):e}finally{if(--N,L&&N==Pe.maxParallelDownloads-1){Pe.diagnosticTracing&&b("Resuming more parallel downloads");const e=L;L=void 0,e.promise_control.resolve()}}}function le(e,t){let o;return null==t&&Be(!1,`sourcePrefix must be provided for ${e.name}`),e.resolvedUrl?o=e.resolvedUrl:(o=""===t?"assembly"===e.behavior||"pdb"===e.behavior?e.name:"resource"===e.behavior&&e.culture&&""!==e.culture?`${e.culture}/${e.name}`:e.name:t+e.name,o=ce(Pe.locateFile(o),e.behavior)),o&&"string"==typeof o||Be(!1,"attemptUrl need to be path or url string"),o}function ce(e,t){return Pe.modulesUniqueQuery&&q[t]&&(e+=Pe.modulesUniqueQuery),e}let de=0;const ue=new Set;function fe(e){try{e.resolvedUrl||Be(!1,"Request's resolvedUrl must be set");const t=function(e){let t=e.resolvedUrl;if(Pe.loadBootResource){const o=ge(e);if(o instanceof Promise)return o;"string"==typeof o&&(t=o)}const o={};return Pe.config.disableNoCacheFetch||(o.cache="no-cache"),e.useCredentials?o.credentials="include":!Pe.config.disableIntegrityCheck&&e.hash&&(o.integrity=e.hash),Pe.fetch_like(t,o)}(e),o={name:e.name,url:e.resolvedUrl,response:t};return ue.add(e.name),o.response.then((()=>{"assembly"==e.behavior&&Pe.loadedAssemblies.push(e.name),de++,Pe.onDownloadResourceProgress&&Pe.onDownloadResourceProgress(de,ue.size)})),o}catch(t){const o={ok:!1,url:e.resolvedUrl,status:500,statusText:"ERR29: "+t,arrayBuffer:()=>{throw t},json:()=>{throw t}};return{name:e.name,url:e.resolvedUrl,response:Promise.resolve(o)}}}const me={resource:"assembly",assembly:"assembly",pdb:"pdb",icu:"globalization",vfs:"configuration",manifest:"manifest",dotnetwasm:"dotnetwasm","js-module-dotnet":"dotnetjs","js-module-native":"dotnetjs","js-module-runtime":"dotnetjs","js-module-threads":"dotnetjs"};function ge(e){var t;if(Pe.loadBootResource){const o=null!==(t=e.hash)&&void 0!==t?t:"",n=e.resolvedUrl,r=me[e.behavior];if(r){const t=Pe.loadBootResource(r,e.name,n,o,e.behavior);return"string"==typeof t?I(t):t}}}function pe(e){e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null}function he(e){let t=e.lastIndexOf("/");return t>=0&&t++,e.substring(t)}async function we(e){e&&await Promise.all((null!=e?e:[]).map((e=>async function(e){try{const t=e.name;if(!e.moduleExports){const o=ce(Pe.locateFile(t),"js-module-library-initializer");Pe.diagnosticTracing&&b(`Attempting to import '${o}' for ${e}`),e.moduleExports=await import(/*! webpackIgnore: true */o)}Pe.libraryInitializers.push({scriptName:t,exports:e.moduleExports})}catch(t){E(`Failed to import library initializer '${e}': ${t}`)}}(e))))}async function be(e,t){if(!Pe.libraryInitializers)return;const o=[];for(let n=0;n<Pe.libraryInitializers.length;n++){const r=Pe.libraryInitializers[n];r.exports[e]&&o.push(ye(r.scriptName,e,(()=>r.exports[e](...t))))}await Promise.all(o)}async function ye(e,t,o){try{await o()}catch(o){throw E(`Failed to invoke '${t}' on library initializer '${e}': ${o}`),Xe(1,o),o}}function ve(e,t){if(e===t)return e;const o={...t};return void 0!==o.assets&&o.assets!==e.assets&&(o.assets=[...e.assets||[],...o.assets||[]]),void 0!==o.resources&&(o.resources=_e(e.resources||{assembly:[],jsModuleNative:[],jsModuleRuntime:[],wasmNative:[]},o.resources)),void 0!==o.environmentVariables&&(o.environmentVariables={...e.environmentVariables||{},...o.environmentVariables||{}}),void 0!==o.runtimeOptions&&o.runtimeOptions!==e.runtimeOptions&&(o.runtimeOptions=[...e.runtimeOptions||[],...o.runtimeOptions||[]]),Object.assign(e,o)}function Ee(e,t){if(e===t)return e;const o={...t};return o.config&&(e.config||(e.config={}),o.config=ve(e.config,o.config)),Object.assign(e,o)}function _e(e,t){if(e===t)return e;const o={...t};return void 0!==o.coreAssembly&&(o.coreAssembly=[...e.coreAssembly||[],...o.coreAssembly||[]]),void 0!==o.assembly&&(o.assembly=[...e.assembly||[],...o.assembly||[]]),void 0!==o.lazyAssembly&&(o.lazyAssembly=[...e.lazyAssembly||[],...o.lazyAssembly||[]]),void 0!==o.corePdb&&(o.corePdb=[...e.corePdb||[],...o.corePdb||[]]),void 0!==o.pdb&&(o.pdb=[...e.pdb||[],...o.pdb||[]]),void 0!==o.jsModuleWorker&&(o.jsModuleWorker=[...e.jsModuleWorker||[],...o.jsModuleWorker||[]]),void 0!==o.jsModuleNative&&(o.jsModuleNative=[...e.jsModuleNative||[],...o.jsModuleNative||[]]),void 0!==o.jsModuleDiagnostics&&(o.jsModuleDiagnostics=[...e.jsModuleDiagnostics||[],...o.jsModuleDiagnostics||[]]),void 0!==o.jsModuleRuntime&&(o.jsModuleRuntime=[...e.jsModuleRuntime||[],...o.jsModuleRuntime||[]]),void 0!==o.wasmSymbols&&(o.wasmSymbols=[...e.wasmSymbols||[],...o.wasmSymbols||[]]),void 0!==o.wasmNative&&(o.wasmNative=[...e.wasmNative||[],...o.wasmNative||[]]),void 0!==o.icu&&(o.icu=[...e.icu||[],...o.icu||[]]),void 0!==o.satelliteResources&&(o.satelliteResources=function(e,t){if(e===t)return e;for(const o in t)e[o]=[...e[o]||[],...t[o]||[]];return e}(e.satelliteResources||{},o.satelliteResources||{})),void 0!==o.modulesAfterConfigLoaded&&(o.modulesAfterConfigLoaded=[...e.modulesAfterConfigLoaded||[],...o.modulesAfterConfigLoaded||[]]),void 0!==o.modulesAfterRuntimeReady&&(o.modulesAfterRuntimeReady=[...e.modulesAfterRuntimeReady||[],...o.modulesAfterRuntimeReady||[]]),void 0!==o.extensions&&(o.extensions={...e.extensions||{},...o.extensions||{}}),void 0!==o.vfs&&(o.vfs=[...e.vfs||[],...o.vfs||[]]),Object.assign(e,o)}function xe(){const e=Pe.config;if(e.environmentVariables=e.environmentVariables||{},e.runtimeOptions=e.runtimeOptions||[],e.resources=e.resources||{assembly:[],jsModuleNative:[],jsModuleWorker:[],jsModuleRuntime:[],wasmNative:[],vfs:[],satelliteResources:{}},e.assets){Pe.diagnosticTracing&&b("config.assets is deprecated, use config.resources instead");for(const t of e.assets){const o={};switch(t.behavior){case"assembly":o.assembly=[t];break;case"pdb":o.pdb=[t];break;case"resource":o.satelliteResources={},o.satelliteResources[t.culture]=[t];break;case"icu":o.icu=[t];break;case"symbols":o.wasmSymbols=[t];break;case"vfs":o.vfs=[t];break;case"dotnetwasm":o.wasmNative=[t];break;case"js-module-threads":o.jsModuleWorker=[t];break;case"js-module-runtime":o.jsModuleRuntime=[t];break;case"js-module-native":o.jsModuleNative=[t];break;case"js-module-diagnostics":o.jsModuleDiagnostics=[t];break;case"js-module-dotnet":break;default:throw new Error(`Unexpected behavior ${t.behavior} of asset ${t.name}`)}_e(e.resources,o)}}e.debugLevel,e.applicationEnvironment||(e.applicationEnvironment="Production"),e.applicationCulture&&(e.environmentVariables.LANG=`${e.applicationCulture}.UTF-8`),Ue.diagnosticTracing=Pe.diagnosticTracing=!!e.diagnosticTracing,Ue.waitForDebugger=e.waitForDebugger,Pe.maxParallelDownloads=e.maxParallelDownloads||Pe.maxParallelDownloads,Pe.enableDownloadRetry=void 0!==e.enableDownloadRetry?e.enableDownloadRetry:Pe.enableDownloadRetry}let je=!1;async function Re(e){var t;if(je)return void await Pe.afterConfigLoaded.promise;let o;try{if(e.configSrc||Pe.config&&0!==Object.keys(Pe.config).length&&(Pe.config.assets||Pe.config.resources)||(e.configSrc="dotnet.boot.js"),o=e.configSrc,je=!0,o&&(Pe.diagnosticTracing&&b("mono_wasm_load_config"),await async function(e){const t=e.configSrc,o=Pe.locateFile(t);let n=null;void 0!==Pe.loadBootResource&&(n=Pe.loadBootResource("manifest",t,o,"","manifest"));let r,i=null;if(n)if("string"==typeof n)n.includes(".json")?(i=await s(I(n)),r=await Ae(i)):r=(await import(I(n))).config;else{const e=await n;"function"==typeof e.json?(i=e,r=await Ae(i)):r=e.config}else o.includes(".json")?(i=await s(ce(o,"manifest")),r=await Ae(i)):r=(await import(ce(o,"manifest"))).config;function s(e){return Pe.fetch_like(e,{method:"GET",credentials:"include",cache:"no-cache"})}Pe.config.applicationEnvironment&&(r.applicationEnvironment=Pe.config.applicationEnvironment),ve(Pe.config,r)}(e)),xe(),await we(null===(t=Pe.config.resources)||void 0===t?void 0:t.modulesAfterConfigLoaded),await be("onRuntimeConfigLoaded",[Pe.config]),e.onConfigLoaded)try{await e.onConfigLoaded(Pe.config,Le),xe()}catch(e){throw _("onConfigLoaded() failed",e),e}xe(),Pe.afterConfigLoaded.promise_control.resolve(Pe.config)}catch(t){const n=`Failed to load config file ${o} ${t} ${null==t?void 0:t.stack}`;throw Pe.config=e.config=Object.assign(Pe.config,{message:n,error:t,isError:!0}),Xe(1,new Error(n)),t}}function Te(){return!!globalThis.navigator&&(Pe.isChromium||Pe.isFirefox)}async function Ae(e){const t=Pe.config,o=await e.json();t.applicationEnvironment||o.applicationEnvironment||(o.applicationEnvironment=e.headers.get("Blazor-Environment")||e.headers.get("DotNet-Environment")||void 0),o.environmentVariables||(o.environmentVariables={});const n=e.headers.get("DOTNET-MODIFIABLE-ASSEMBLIES");n&&(o.environmentVariables.DOTNET_MODIFIABLE_ASSEMBLIES=n);const r=e.headers.get("ASPNETCORE-BROWSER-TOOLS");return r&&(o.environmentVariables.__ASPNETCORE_BROWSER_TOOLS=r),o}"function"!=typeof importScripts||globalThis.onmessage||(globalThis.dotnetSidecar=!0);const Se="object"==typeof process&&"object"==typeof process.versions&&"string"==typeof process.versions.node,De="function"==typeof importScripts,Oe=De&&"undefined"!=typeof dotnetSidecar,Ce=De&&!Oe,ke="object"==typeof window||De&&!Se,Ie=!ke&&!Se;let Ue={},Pe={},Me={},Le={},Ne={},$e=!1;const ze={},We={config:ze},Fe={mono:{},binding:{},internal:Ne,module:We,loaderHelpers:Pe,runtimeHelpers:Ue,diagnosticHelpers:Me,api:Le};function Be(e,t){if(e)return;const o="Assert failed: "+("function"==typeof t?t():t),n=new Error(o);_(o,n),Ue.nativeAbort(n)}function Ve(){return void 0!==Pe.exitCode}function qe(){return Ue.runtimeReady&&!Ve()}function He(){Ve()&&Be(!1,`.NET runtime already exited with ${Pe.exitCode} ${Pe.exitReason}. You can use runtime.runMain() which doesn't exit the runtime.`),Ue.runtimeReady||Be(!1,".NET runtime didn't start yet. Please call dotnet.create() first.")}function Je(){ke&&(globalThis.addEventListener("unhandledrejection",et),globalThis.addEventListener("error",tt))}let Ze,Qe;function Ge(e){Qe&&Qe(e),Xe(e,Pe.exitReason)}function Ke(e){Ze&&Ze(e||Pe.exitReason),Xe(1,e||Pe.exitReason)}function Xe(t,o){var n,r;const i=o&&"object"==typeof o;t=i&&"number"==typeof o.status?o.status:void 0===t?-1:t;const s=i&&"string"==typeof o.message?o.message:""+o;(o=i?o:Ue.ExitStatus?function(e,t){const o=new Ue.ExitStatus(e);return o.message=t,o.toString=()=>t,o}(t,s):new Error("Exit with code "+t+" "+s)).status=t,o.message||(o.message=s);const a=""+(o.stack||(new Error).stack);try{Object.defineProperty(o,"stack",{get:()=>a})}catch(e){}const l=!!o.silent;if(o.silent=!0,Ve())Pe.diagnosticTracing&&b("mono_exit called after exit");else{try{We.onAbort==Ke&&(We.onAbort=Ze),We.onExit==Ge&&(We.onExit=Qe),ke&&(globalThis.removeEventListener("unhandledrejection",et),globalThis.removeEventListener("error",tt)),Ue.runtimeReady?(Ue.jiterpreter_dump_stats&&Ue.jiterpreter_dump_stats(!1),0===t&&(null===(n=Pe.config)||void 0===n?void 0:n.interopCleanupOnExit)&&Ue.forceDisposeProxies(!0,!0),e&&0!==t&&(null===(r=Pe.config)||void 0===r||r.dumpThreadsOnNonZeroExit)):(Pe.diagnosticTracing&&b(`abort_startup, reason: ${o}`),function(e){Pe.allDownloadsQueued.promise_control.reject(e),Pe.allDownloadsFinished.promise_control.reject(e),Pe.afterConfigLoaded.promise_control.reject(e),Pe.wasmCompilePromise.promise_control.reject(e),Pe.runtimeModuleLoaded.promise_control.reject(e),Ue.dotnetReady&&(Ue.dotnetReady.promise_control.reject(e),Ue.afterInstantiateWasm.promise_control.reject(e),Ue.beforePreInit.promise_control.reject(e),Ue.afterPreInit.promise_control.reject(e),Ue.afterPreRun.promise_control.reject(e),Ue.beforeOnRuntimeInitialized.promise_control.reject(e),Ue.afterOnRuntimeInitialized.promise_control.reject(e),Ue.afterPostRun.promise_control.reject(e))}(o))}catch(e){E("mono_exit A failed",e)}try{l||(function(e,t){if(0!==e&&t){const e=Ue.ExitStatus&&t instanceof Ue.ExitStatus?b:_;"string"==typeof t?e(t):(void 0===t.stack&&(t.stack=(new Error).stack+""),t.message?e(Ue.stringify_as_error_with_stack?Ue.stringify_as_error_with_stack(t.message+"\n"+t.stack):t.message+"\n"+t.stack):e(JSON.stringify(t)))}!Ce&&Pe.config&&(Pe.config.logExitCode?Pe.config.forwardConsoleLogsToWS?R("WASM EXIT "+e):v("WASM EXIT "+e):Pe.config.forwardConsoleLogsToWS&&R())}(t,o),function(e){if(ke&&!Ce&&Pe.config&&Pe.config.appendElementOnExit&&document){const t=document.createElement("label");t.id="tests_done",0!==e&&(t.style.background="red"),t.innerHTML=""+e,document.body.appendChild(t)}}(t))}catch(e){E("mono_exit B failed",e)}Pe.exitCode=t,Pe.exitReason||(Pe.exitReason=o),!Ce&&Ue.runtimeReady&&We.runtimeKeepalivePop()}if(Pe.config&&Pe.config.asyncFlushOnExit&&0===t)throw(async()=>{try{await async function(){try{const e=await import(/*! webpackIgnore: true */"process"),t=e=>new Promise(((t,o)=>{e.on("error",o),e.end("","utf8",t)})),o=t(e.stderr),n=t(e.stdout);let r;const i=new Promise((e=>{r=setTimeout((()=>e("timeout")),1e3)}));await Promise.race([Promise.all([n,o]),i]),clearTimeout(r)}catch(e){_(`flushing std* streams failed: ${e}`)}}()}finally{Ye(t,o)}})(),o;Ye(t,o)}function Ye(e,t){if(Ue.runtimeReady&&Ue.nativeExit)try{Ue.nativeExit(e)}catch(e){!Ue.ExitStatus||e instanceof Ue.ExitStatus||E("set_exit_code_and_quit_now failed: "+e.toString())}if(0!==e||!ke)throw Se&&Ne.process?Ne.process.exit(e):Ue.quit&&Ue.quit(e,t),t}function et(e){ot(e,e.reason,"rejection")}function tt(e){ot(e,e.error,"error")}function ot(e,t,o){e.preventDefault();try{t||(t=new Error("Unhandled "+o)),void 0===t.stack&&(t.stack=(new Error).stack),t.stack=t.stack+"",t.silent||(_("Unhandled error:",t),Xe(1,t))}catch(e){}}!function(e){if($e)throw new Error("Loader module already loaded");$e=!0,Ue=e.runtimeHelpers,Pe=e.loaderHelpers,Me=e.diagnosticHelpers,Le=e.api,Ne=e.internal,Object.assign(Le,{INTERNAL:Ne,invokeLibraryInitializers:be}),Object.assign(e.module,{config:ve(ze,{environmentVariables:{}})});const r={mono_wasm_bindings_is_ready:!1,config:e.module.config,diagnosticTracing:!1,nativeAbort:e=>{throw e||new Error("abort")},nativeExit:e=>{throw new Error("exit:"+e)}},l={gitHash:"fad253f51b461736dfd3cd9c15977bb7493becef",config:e.module.config,diagnosticTracing:!1,maxParallelDownloads:16,enableDownloadRetry:!0,_loaded_files:[],loadedFiles:[],loadedAssemblies:[],libraryInitializers:[],workerNextNumber:1,actual_downloaded_assets_count:0,actual_instantiated_assets_count:0,expected_downloaded_assets_count:0,expected_instantiated_assets_count:0,afterConfigLoaded:i(),allDownloadsQueued:i(),allDownloadsFinished:i(),wasmCompilePromise:i(),runtimeModuleLoaded:i(),loadingWorkers:i(),is_exited:Ve,is_runtime_running:qe,assert_runtime_running:He,mono_exit:Xe,createPromiseController:i,getPromiseController:s,assertIsControllablePromise:a,mono_download_assets:oe,resolve_single_asset_path:ee,setup_proxy_console:j,set_thread_prefix:w,installUnhandledErrorHandler:Je,retrieve_asset_download:ie,invokeLibraryInitializers:be,isDebuggingSupported:Te,exceptions:t,simd:n,relaxedSimd:o};Object.assign(Ue,r),Object.assign(Pe,l)}(Fe);let nt,rt,it,st=!1,at=!1;async function lt(e){if(!at){if(at=!0,ke&&Pe.config.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&j("main",globalThis.console,globalThis.location.origin),We||Be(!1,"Null moduleConfig"),Pe.config||Be(!1,"Null moduleConfig.config"),"function"==typeof e){const t=e(Fe.api);if(t.ready)throw new Error("Module.ready couldn't be redefined.");Object.assign(We,t),Ee(We,t)}else{if("object"!=typeof e)throw new Error("Can't use moduleFactory callback of createDotnetRuntime function.");Ee(We,e)}await async function(e){if(Se){const e=await import(/*! webpackIgnore: true */"process"),t=14;if(e.versions.node.split(".")[0]<t)throw new Error(`NodeJS at '${e.execPath}' has too low version '${e.versions.node}', please use at least ${t}. See also https://aka.ms/dotnet-wasm-features`)}const t=/*! webpackIgnore: true */import.meta.url,o=t.indexOf("?");var n;if(o>0&&(Pe.modulesUniqueQuery=t.substring(o)),Pe.scriptUrl=t.replace(/\\/g,"/").replace(/[?#].*/,""),Pe.scriptDirectory=(n=Pe.scriptUrl).slice(0,n.lastIndexOf("/"))+"/",Pe.locateFile=e=>"URL"in globalThis&&globalThis.URL!==C?new URL(e,Pe.scriptDirectory).toString():M(e)?e:Pe.scriptDirectory+e,Pe.fetch_like=k,Pe.out=console.log,Pe.err=console.error,Pe.onDownloadResourceProgress=e.onDownloadResourceProgress,ke&&globalThis.navigator){const e=globalThis.navigator,t=e.userAgentData&&e.userAgentData.brands;t&&t.length>0?Pe.isChromium=t.some((e=>"Google Chrome"===e.brand||"Microsoft Edge"===e.brand||"Chromium"===e.brand)):e.userAgent&&(Pe.isChromium=e.userAgent.includes("Chrome"),Pe.isFirefox=e.userAgent.includes("Firefox"))}Ne.require=Se?await import(/*! webpackIgnore: true */"module").then((e=>e.createRequire(/*! webpackIgnore: true */import.meta.url))):Promise.resolve((()=>{throw new Error("require not supported")})),void 0===globalThis.URL&&(globalThis.URL=C)}(We)}}async function ct(e){return await lt(e),Ze=We.onAbort,Qe=We.onExit,We.onAbort=Ke,We.onExit=Ge,We.ENVIRONMENT_IS_PTHREAD?async function(){(function(){const e=new MessageChannel,t=e.port1,o=e.port2;t.addEventListener("message",(e=>{var n,r;n=JSON.parse(e.data.config),r=JSON.parse(e.data.monoThreadInfo),st?Pe.diagnosticTracing&&b("mono config already received"):(ve(Pe.config,n),Ue.monoThreadInfo=r,xe(),Pe.diagnosticTracing&&b("mono config received"),st=!0,Pe.afterConfigLoaded.promise_control.resolve(Pe.config),ke&&n.forwardConsoleLogsToWS&&void 0!==globalThis.WebSocket&&Pe.setup_proxy_console("worker-idle",console,globalThis.location.origin)),t.close(),o.close()}),{once:!0}),t.start(),self.postMessage({[l]:{monoCmd:"preload",port:o}},[o])})(),await Pe.afterConfigLoaded.promise,function(){const e=Pe.config;e.assets||Be(!1,"config.assets must be defined");for(const t of e.assets)X(t),Q[t.behavior]&&z.push(t)}(),setTimeout((async()=>{try{await oe()}catch(e){Xe(1,e)}}),0);const e=dt(),t=await Promise.all(e);return await ut(t),We}():async function(){var e;await Re(We),re();const t=dt();(async function(){try{const e=ee("dotnetwasm");await se(e),e&&e.pendingDownloadInternal&&e.pendingDownloadInternal.response||Be(!1,"Can't load dotnet.native.wasm");const t=await e.pendingDownloadInternal.response,o=t.headers&&t.headers.get?t.headers.get("Content-Type"):void 0;let n;if("function"==typeof WebAssembly.compileStreaming&&"application/wasm"===o)n=await WebAssembly.compileStreaming(t);else{ke&&"application/wasm"!==o&&E('WebAssembly resource does not have the expected content type "application/wasm", so falling back to slower ArrayBuffer instantiation.');const e=await t.arrayBuffer();Pe.diagnosticTracing&&b("instantiate_wasm_module buffered"),n=Ie?await Promise.resolve(new WebAssembly.Module(e)):await WebAssembly.compile(e)}e.pendingDownloadInternal=null,e.pendingDownload=null,e.buffer=null,e.moduleExports=null,Pe.wasmCompilePromise.promise_control.resolve(n)}catch(e){Pe.wasmCompilePromise.promise_control.reject(e)}})(),setTimeout((async()=>{try{D(),await oe()}catch(e){Xe(1,e)}}),0);const o=await Promise.all(t);return await ut(o),await Ue.dotnetReady.promise,await we(null===(e=Pe.config.resources)||void 0===e?void 0:e.modulesAfterRuntimeReady),await be("onRuntimeReady",[Fe.api]),Le}()}function dt(){const e=ee("js-module-runtime"),t=ee("js-module-native");if(nt&&rt)return[nt,rt,it];"object"==typeof e.moduleExports?nt=e.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${e.resolvedUrl}' for ${e.name}`),nt=import(/*! webpackIgnore: true */e.resolvedUrl)),"object"==typeof t.moduleExports?rt=t.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${t.resolvedUrl}' for ${t.name}`),rt=import(/*! webpackIgnore: true */t.resolvedUrl));const o=Y("js-module-diagnostics");return o&&("object"==typeof o.moduleExports?it=o.moduleExports:(Pe.diagnosticTracing&&b(`Attempting to import '${o.resolvedUrl}' for ${o.name}`),it=import(/*! webpackIgnore: true */o.resolvedUrl))),[nt,rt,it]}async function ut(e){const{initializeExports:t,initializeReplacements:o,configureRuntimeStartup:n,configureEmscriptenStartup:r,configureWorkerStartup:i,setRuntimeGlobals:s,passEmscriptenInternals:a}=e[0],{default:l}=e[1],c=e[2];s(Fe),t(Fe),c&&c.setRuntimeGlobals(Fe),await n(We),Pe.runtimeModuleLoaded.promise_control.resolve(),l((e=>(Object.assign(We,{ready:e.ready,__dotnet_runtime:{initializeReplacements:o,configureEmscriptenStartup:r,configureWorkerStartup:i,passEmscriptenInternals:a}}),We))).catch((e=>{if(e.message&&e.message.toLowerCase().includes("out of memory"))throw new Error(".NET runtime has failed to start, because too much memory was requested. Please decrease the memory by adjusting EmccMaximumHeapSize. See also https://aka.ms/dotnet-wasm-features");throw e}))}const ft=new class{withModuleConfig(e){try{return Ee(We,e),this}catch(e){throw Xe(1,e),e}}withOnConfigLoaded(e){try{return Ee(We,{onConfigLoaded:e}),this}catch(e){throw Xe(1,e),e}}withConsoleForwarding(){try{return ve(ze,{forwardConsoleLogsToWS:!0}),this}catch(e){throw Xe(1,e),e}}withExitOnUnhandledError(){try{return ve(ze,{exitOnUnhandledError:!0}),Je(),this}catch(e){throw Xe(1,e),e}}withAsyncFlushOnExit(){try{return ve(ze,{asyncFlushOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withExitCodeLogging(){try{return ve(ze,{logExitCode:!0}),this}catch(e){throw Xe(1,e),e}}withElementOnExit(){try{return ve(ze,{appendElementOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withInteropCleanupOnExit(){try{return ve(ze,{interopCleanupOnExit:!0}),this}catch(e){throw Xe(1,e),e}}withDumpThreadsOnNonZeroExit(){try{return ve(ze,{dumpThreadsOnNonZeroExit:!0}),this}catch(e){throw Xe(1,e),e}}withWaitingForDebugger(e){try{return ve(ze,{waitForDebugger:e}),this}catch(e){throw Xe(1,e),e}}withInterpreterPgo(e,t){try{return ve(ze,{interpreterPgo:e,interpreterPgoSaveDelay:t}),ze.runtimeOptions?ze.runtimeOptions.push("--interp-pgo-recording"):ze.runtimeOptions=["--interp-pgo-recording"],this}catch(e){throw Xe(1,e),e}}withConfig(e){try{return ve(ze,e),this}catch(e){throw Xe(1,e),e}}withConfigSrc(e){try{return e&&"string"==typeof e||Be(!1,"must be file path or URL"),Ee(We,{configSrc:e}),this}catch(e){throw Xe(1,e),e}}withVirtualWorkingDirectory(e){try{return e&&"string"==typeof e||Be(!1,"must be directory path"),ve(ze,{virtualWorkingDirectory:e}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariable(e,t){try{const o={};return o[e]=t,ve(ze,{environmentVariables:o}),this}catch(e){throw Xe(1,e),e}}withEnvironmentVariables(e){try{return e&&"object"==typeof e||Be(!1,"must be dictionary object"),ve(ze,{environmentVariables:e}),this}catch(e){throw Xe(1,e),e}}withDiagnosticTracing(e){try{return"boolean"!=typeof e&&Be(!1,"must be boolean"),ve(ze,{diagnosticTracing:e}),this}catch(e){throw Xe(1,e),e}}withDebugging(e){try{return null!=e&&"number"==typeof e||Be(!1,"must be number"),ve(ze,{debugLevel:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArguments(...e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ve(ze,{applicationArguments:e}),this}catch(e){throw Xe(1,e),e}}withRuntimeOptions(e){try{return e&&Array.isArray(e)||Be(!1,"must be array of strings"),ze.runtimeOptions?ze.runtimeOptions.push(...e):ze.runtimeOptions=e,this}catch(e){throw Xe(1,e),e}}withMainAssembly(e){try{return ve(ze,{mainAssemblyName:e}),this}catch(e){throw Xe(1,e),e}}withApplicationArgumentsFromQuery(){try{if(!globalThis.window)throw new Error("Missing window to the query parameters from");if(void 0===globalThis.URLSearchParams)throw new Error("URLSearchParams is supported");const e=new URLSearchParams(globalThis.window.location.search).getAll("arg");return this.withApplicationArguments(...e)}catch(e){throw Xe(1,e),e}}withApplicationEnvironment(e){try{return ve(ze,{applicationEnvironment:e}),this}catch(e){throw Xe(1,e),e}}withApplicationCulture(e){try{return ve(ze,{applicationCulture:e}),this}catch(e){throw Xe(1,e),e}}withResourceLoader(e){try{return Pe.loadBootResource=e,this}catch(e){throw Xe(1,e),e}}async download(){try{await async function(){lt(We),await Re(We),re(),D(),oe(),await Pe.allDownloadsFinished.promise}()}catch(e){throw Xe(1,e),e}}async create(){try{return this.instance||(this.instance=await async function(){return await ct(We),Fe.api}()),this.instance}catch(e){throw Xe(1,e),e}}async run(){try{return We.config||Be(!1,"Null moduleConfig.config"),this.instance||await this.create(),this.instance.runMainAndExit()}catch(e){throw Xe(1,e),e}}},mt=Xe,gt=ct;Ie||"function"==typeof globalThis.URL||Be(!1,"This browser/engine doesn't support URL API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),"function"!=typeof globalThis.BigInt64Array&&Be(!1,"This browser/engine doesn't support BigInt64Array API. Please use a modern version. See also https://aka.ms/dotnet-wasm-features"),ft.withConfig(/*json-start*/{
  "mainAssemblyName": "SwashbucklerDiary.WebAssembly",
  "resources": {
    "hash": "sha256-G41mx5GqYDF4Zq86/mjjTB8ScnzUx3R0ZUbvlgKuuas=",
    "jsModuleNative": [
      {
        "name": "dotnet.native.rgtfw4k8i2.js"
      }
    ],
    "jsModuleRuntime": [
      {
        "name": "dotnet.runtime.0j6ezsi0n0.js"
      }
    ],
    "wasmNative": [
      {
        "name": "dotnet.native.nna2e5v2j9.wasm",
        "integrity": "sha256-jesbyNtpSGK6cbr385Q07TTdb1owlS3HmXbwqEeos28="
      }
    ],
    "icu": [
      {
        "virtualPath": "icudt_CJK.dat",
        "name": "icudt_CJK.tjcz0u77k5.dat",
        "integrity": "sha256-SZLtQnRc0JkwqHab0VUVP7T3uBPSeYzxzDnpxPpUnHk="
      },
      {
        "virtualPath": "icudt_EFIGS.dat",
        "name": "icudt_EFIGS.tptq2av103.dat",
        "integrity": "sha256-8fItetYY8kQ0ww6oxwTLiT3oXlBwHKumbeP2pRF4yTc="
      },
      {
        "virtualPath": "icudt_no_CJK.dat",
        "name": "icudt_no_CJK.lfu7j35m59.dat",
        "integrity": "sha256-L7sV7NEYP37/Qr2FPCePo5cJqRgTXRwGHuwF5Q+0Nfs="
      }
    ],
    "coreAssembly": [
      {
        "virtualPath": "System.Runtime.InteropServices.JavaScript.wasm",
        "name": "System.Runtime.InteropServices.JavaScript.50ez5m42lb.wasm",
        "integrity": "sha256-W9ESzMNrB0Cjt1YZL39Yb4daAPwKatQUst9AmVjw0M8="
      },
      {
        "virtualPath": "System.Private.CoreLib.wasm",
        "name": "System.Private.CoreLib.72cnwkv4tn.wasm",
        "integrity": "sha256-MYOxTaGSzueje/gq0EZpQdKctqP77oS+OvSwzimZI+M="
      }
    ],
    "assembly": [
      {
        "virtualPath": "Azure.Core.wasm",
        "name": "Azure.Core.92q6rqqhly.wasm",
        "integrity": "sha256-cUx0yT4TBboCr8yulFrimAvC9ecu4FtrhfEZ7T8ragU="
      },
      {
        "virtualPath": "Azure.Identity.wasm",
        "name": "Azure.Identity.b6ekoqlu3w.wasm",
        "integrity": "sha256-wxR1E0zJLWYxnckYUOXZsbZoUjCYCwgg3aYbYbCLaGo="
      },
      {
        "virtualPath": "BemIt.wasm",
        "name": "BemIt.9ci0d0am18.wasm",
        "integrity": "sha256-2ake0gd2dtojp4ggyggAonW/vaG9/g9H17nIL+b8YJI="
      },
      {
        "virtualPath": "Blazored.LocalStorage.wasm",
        "name": "Blazored.LocalStorage.12n6dz54qr.wasm",
        "integrity": "sha256-OaMAAd5n7ORfyur5e3QIyEVKJ76MKIvwbg7/icnnYcU="
      },
      {
        "virtualPath": "ClosedXML.wasm",
        "name": "ClosedXML.6d8ybop3kc.wasm",
        "integrity": "sha256-Jga3CUuNfwvpwnsHZxpWURvivrlrEqnUo9XR5EOLYGc="
      },
      {
        "virtualPath": "ClosedXML.Parser.wasm",
        "name": "ClosedXML.Parser.kcd8ka7nog.wasm",
        "integrity": "sha256-bN619zkNwVyqGT2tFn5NMXPDIo3OUVaRYMG/tXRtn+A="
      },
      {
        "virtualPath": "DeepCloner.Core.wasm",
        "name": "DeepCloner.Core.f4ud32ue6m.wasm",
        "integrity": "sha256-QZsqX6WYGkTxasYbmcBcF4qwdIu5pTgzFYNKfY1cVow="
      },
      {
        "virtualPath": "DocumentFormat.OpenXml.wasm",
        "name": "DocumentFormat.OpenXml.0gr15g35pw.wasm",
        "integrity": "sha256-MBbZzfjPOSIquvoCx+NxtLYAI63vk0mstqecqcKULfc="
      },
      {
        "virtualPath": "DocumentFormat.OpenXml.Framework.wasm",
        "name": "DocumentFormat.OpenXml.Framework.2u15sjes4c.wasm",
        "integrity": "sha256-dr/uXeOB5jiI36oQZ8p+FPEz9c/Fl4Fp2tI2TdaUd6s="
      },
      {
        "virtualPath": "ExcelNumberFormat.wasm",
        "name": "ExcelNumberFormat.zkc9yronjy.wasm",
        "integrity": "sha256-DWrWJf+NGbjj12iqpO+Ufl4YS0bkW1yz6JwjocU15wY="
      },
      {
        "virtualPath": "FluentValidation.wasm",
        "name": "FluentValidation.psv1xceu9w.wasm",
        "integrity": "sha256-OSimv5A1VplcdJeNacGUy2pP8QpgEYH0ErqRudW+CBw="
      },
      {
        "virtualPath": "FluentValidation.DependencyInjectionExtensions.wasm",
        "name": "FluentValidation.DependencyInjectionExtensions.41c0wmz8h2.wasm",
        "integrity": "sha256-6KQYww6sKCsgyXz87RALQHnxIvSGV6U0vjdo9k2UY5w="
      },
      {
        "virtualPath": "Masa.Blazor.wasm",
        "name": "Masa.Blazor.2v9f6bvt72.wasm",
        "integrity": "sha256-Uh7Hpt5QB8ul0ejehLWQpIIBmpq940qin0tt9dF7fJ8="
      },
      {
        "virtualPath": "Masa.Blazor.MobileComponents.wasm",
        "name": "Masa.Blazor.MobileComponents.uvelbp8ikw.wasm",
        "integrity": "sha256-ZtLf7lkP12nf9WsGe1EOLHGE0mD5sSGXDFwqtoy74m8="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.wasm",
        "name": "Microsoft.AspNetCore.Components.bjvzm1a8kn.wasm",
        "integrity": "sha256-Z6Qw+4JyOoI2myIDmFQMFAq7m/f7Q7p1GF/8vu5dgWk="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.Authorization.wasm",
        "name": "Microsoft.AspNetCore.Components.Authorization.uelsft7h9p.wasm",
        "integrity": "sha256-Zg7CdaHDLsYmLyB2d5bzHtXiy5Ug2D++r3UfAob8fBo="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.Forms.wasm",
        "name": "Microsoft.AspNetCore.Components.Forms.9ulxulen7z.wasm",
        "integrity": "sha256-0d4ViT6slIgDaE3Xns4Lvn2oZ0RIoaV0s5JthKWmaG0="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.Web.wasm",
        "name": "Microsoft.AspNetCore.Components.Web.y79bdpdhs8.wasm",
        "integrity": "sha256-bkThXulBynAvhp9RF2QCBZnQ27L6+NTHJOvHJWNERBc="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Components.WebAssembly.wasm",
        "name": "Microsoft.AspNetCore.Components.WebAssembly.ai7dlrl9c0.wasm",
        "integrity": "sha256-Qng74yDv3I0x0XRfqAP/AwhjzeR97rfUi8RyJwC8jew="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Http.Abstractions.wasm",
        "name": "Microsoft.AspNetCore.Http.Abstractions.4pucu9jwqe.wasm",
        "integrity": "sha256-aPjR8YQYZRQOCbPYFnx/tOBYEqSXaAF/F/Cd883tTH4="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Http.Extensions.wasm",
        "name": "Microsoft.AspNetCore.Http.Extensions.efvdr0uq6r.wasm",
        "integrity": "sha256-WjtmQB8VQ3hRd/VWugfjN2UeetmLH9kht2ejKAdQrDY="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Http.Features.wasm",
        "name": "Microsoft.AspNetCore.Http.Features.4kfzemgx1u.wasm",
        "integrity": "sha256-hF86nD892lo0u26YKc0I78W2vbzPNsSJRdXv/fpfy60="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Routing.wasm",
        "name": "Microsoft.AspNetCore.Routing.zhz1xlbqxn.wasm",
        "integrity": "sha256-985D/gRGqetVIABQ+6K4ZWRfZ8j7sMkhjv0BCWrH3Tg="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.Routing.Abstractions.wasm",
        "name": "Microsoft.AspNetCore.Routing.Abstractions.wdbene573a.wasm",
        "integrity": "sha256-JLA2a68UlQExL1dYl5UT71qQywUwA2fm/xweayPu4qg="
      },
      {
        "virtualPath": "Microsoft.AspNetCore.WebUtilities.wasm",
        "name": "Microsoft.AspNetCore.WebUtilities.kly0i8sxkj.wasm",
        "integrity": "sha256-2N5wv1CJzMrbtjiOxmvyw/klQNx4ThuEonPU2ach4DM="
      },
      {
        "virtualPath": "Microsoft.Bcl.AsyncInterfaces.wasm",
        "name": "Microsoft.Bcl.AsyncInterfaces.9wftx6xvg7.wasm",
        "integrity": "sha256-uV6JPE8pFJZ3vgxDKUeimvMSk+wGuYyOKWg5xKqP7Qo="
      },
      {
        "virtualPath": "Microsoft.Data.SqlClient.wasm",
        "name": "Microsoft.Data.SqlClient.6s8bz9pxib.wasm",
        "integrity": "sha256-ymf6tjWo0WasN1cagVmIRHYBrkcqL+cS1obnTstugrE="
      },
      {
        "virtualPath": "Microsoft.Data.Sqlite.wasm",
        "name": "Microsoft.Data.Sqlite.gkk35qz4nk.wasm",
        "integrity": "sha256-IHiZbt4s76fR45F+gpWh7In+KdhFZTcCRWyAhLHGtHU="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.wasm",
        "name": "Microsoft.Extensions.Configuration.rg826fsozh.wasm",
        "integrity": "sha256-W7mtMnscYp8Lm6bOFiNQIzi2jmhPwTSt+p55NOguXJk="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Abstractions.wasm",
        "name": "Microsoft.Extensions.Configuration.Abstractions.gdtptxu0m1.wasm",
        "integrity": "sha256-DF5ZQPnsimHgu/OhXI14ihBGcEBOY9jXNikZbJl7ouk="
      },
      {
        "virtualPath": "Microsoft.Extensions.Configuration.Json.wasm",
        "name": "Microsoft.Extensions.Configuration.Json.3ped9ou2lc.wasm",
        "integrity": "sha256-WQurBbuDtkPNdWdC7Lbk07OTfLLaEGN9l68/4BxEU7c="
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.wasm",
        "name": "Microsoft.Extensions.DependencyInjection.kvgi2w7dn6.wasm",
        "integrity": "sha256-EGgp7FwAfHqwe4uGUaKVwoLPAz4hN5jflAlEr+jfJzY="
      },
      {
        "virtualPath": "Microsoft.Extensions.DependencyInjection.Abstractions.wasm",
        "name": "Microsoft.Extensions.DependencyInjection.Abstractions.5486g1glq0.wasm",
        "integrity": "sha256-lWLPTSMHjNkytQkPyGAEKo4XMr+M1VfdIzWFTVKwnA4="
      },
      {
        "virtualPath": "Microsoft.Extensions.FileProviders.Abstractions.wasm",
        "name": "Microsoft.Extensions.FileProviders.Abstractions.gtbfjuvfa2.wasm",
        "integrity": "sha256-lMTbBXdfYBgjwOSWgNjJuHHDnCUCiNj8gkL0YQJ0f8o="
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.wasm",
        "name": "Microsoft.Extensions.Logging.h9thiay8tq.wasm",
        "integrity": "sha256-m3dkXDtvT8CpRp4jyzyTgs/n7oZDz3K+SgaC0ebRyi4="
      },
      {
        "virtualPath": "Microsoft.Extensions.Logging.Abstractions.wasm",
        "name": "Microsoft.Extensions.Logging.Abstractions.l3mim8lipi.wasm",
        "integrity": "sha256-n/lLdGSkdyqBkL9n9qrvzWgm2me6+rB4cziTO4S8twY="
      },
      {
        "virtualPath": "Microsoft.Extensions.ObjectPool.wasm",
        "name": "Microsoft.Extensions.ObjectPool.snz1taphvf.wasm",
        "integrity": "sha256-yN/YwcJojmFmoiBEEYT4EioSwZ0GQpHJnZJ+qVozBHs="
      },
      {
        "virtualPath": "Microsoft.Extensions.Options.wasm",
        "name": "Microsoft.Extensions.Options.pw7q4gm2az.wasm",
        "integrity": "sha256-d7HuWvA9vLeZ31zMZ1zSI77tayFgOLfVjTOdy4cOCCg="
      },
      {
        "virtualPath": "Microsoft.Extensions.Primitives.wasm",
        "name": "Microsoft.Extensions.Primitives.k3nnrz8cxo.wasm",
        "integrity": "sha256-sLvdsL8XNOQag4uX8wrGa4ASRnk7KafkUAbBjtLkIYo="
      },
      {
        "virtualPath": "Microsoft.Identity.Client.wasm",
        "name": "Microsoft.Identity.Client.93n8f1kiao.wasm",
        "integrity": "sha256-Gwa3cQwq9yyAVl8B3b7uwIzsHs0StExllkoDerTbBZs="
      },
      {
        "virtualPath": "Microsoft.Identity.Client.Extensions.Msal.wasm",
        "name": "Microsoft.Identity.Client.Extensions.Msal.z7cdsa1cr0.wasm",
        "integrity": "sha256-4WyJjsOVdSuGyNEo32OkpX4Mx1kOu0iPXbftEFdTa9Q="
      },
      {
        "virtualPath": "Microsoft.IdentityModel.Abstractions.wasm",
        "name": "Microsoft.IdentityModel.Abstractions.sqh09yr2m9.wasm",
        "integrity": "sha256-LAXieymb0SOMqcyaLT5+tDo5aZ5bA9OG3Hy7R/xyY+U="
      },
      {
        "virtualPath": "Microsoft.IdentityModel.JsonWebTokens.wasm",
        "name": "Microsoft.IdentityModel.JsonWebTokens.39lcez2l12.wasm",
        "integrity": "sha256-aW/J4yAm7Dnvw37F1GKWotc8DDnHtk+03sCjNtwD5dY="
      },
      {
        "virtualPath": "Microsoft.IdentityModel.Logging.wasm",
        "name": "Microsoft.IdentityModel.Logging.f9avtk6mc2.wasm",
        "integrity": "sha256-szIazOh5cFd4f6aIlwf75r4fwZzhoopbPnbOB3lNU4s="
      },
      {
        "virtualPath": "Microsoft.IdentityModel.Protocols.wasm",
        "name": "Microsoft.IdentityModel.Protocols.sb86v23jm0.wasm",
        "integrity": "sha256-qCZWN08Qq6E9fBApMutJZBSq1N7wWMKLsRqrfOtBd74="
      },
      {
        "virtualPath": "Microsoft.IdentityModel.Protocols.OpenIdConnect.wasm",
        "name": "Microsoft.IdentityModel.Protocols.OpenIdConnect.eu4njaflvc.wasm",
        "integrity": "sha256-RgOe7J9L12oMsSaprXN4sC9zXbwXOeqduQY0fz5J0tA="
      },
      {
        "virtualPath": "Microsoft.IdentityModel.Tokens.wasm",
        "name": "Microsoft.IdentityModel.Tokens.lpd4jqitqf.wasm",
        "integrity": "sha256-aRkBevAl9X4Lsxqf6yFoTtx1W3wtpcifUVrVNu6ju70="
      },
      {
        "virtualPath": "Microsoft.JSInterop.wasm",
        "name": "Microsoft.JSInterop.mudtk5e1pl.wasm",
        "integrity": "sha256-XtNoZU8TVWQn8KpBvNtiRLU/ijr40e0hcU8N4uP9VW4="
      },
      {
        "virtualPath": "Microsoft.JSInterop.WebAssembly.wasm",
        "name": "Microsoft.JSInterop.WebAssembly.0o8pok1eno.wasm",
        "integrity": "sha256-4I+0kHd4SomBH7+AYwAomyTpYchOHG0eb6kHl9ZrDTk="
      },
      {
        "virtualPath": "Microsoft.Net.Http.Headers.wasm",
        "name": "Microsoft.Net.Http.Headers.5ivg4x4ytz.wasm",
        "integrity": "sha256-fWubjFwWLytXfVhK8yeF2Xy8FLUJQVqcuApOe8EeKL0="
      },
      {
        "virtualPath": "Microsoft.SqlServer.Server.wasm",
        "name": "Microsoft.SqlServer.Server.yamodpu5qp.wasm",
        "integrity": "sha256-Fig+5hq00gGQlXAgSnyFlUlWyhlx9f+yPJb4INt3gNc="
      },
      {
        "virtualPath": "MySqlConnector.wasm",
        "name": "MySqlConnector.qnh6qevles.wasm",
        "integrity": "sha256-5iDwdKGG4bYgDV7pZ4nVnWbXlVjcM6uM5lYnLTz68hc="
      },
      {
        "virtualPath": "Newtonsoft.Json.wasm",
        "name": "Newtonsoft.Json.y96379yjhz.wasm",
        "integrity": "sha256-erKuIr8Gjq+X/Ji4X+pGauW71/eyGT7GHVu8k+7ie8I="
      },
      {
        "virtualPath": "Npgsql.wasm",
        "name": "Npgsql.pzdjkkbut8.wasm",
        "integrity": "sha256-lWGRyltmjrVxDU7e9ltydZukHcV+IOLWxGyNH24BNwI="
      },
      {
        "virtualPath": "OneOf.wasm",
        "name": "OneOf.z1vz4pqseg.wasm",
        "integrity": "sha256-CVCRDxxxvdK2h7HnIwm4TpCw1nfJUBr8XNg7msF7aKE="
      },
      {
        "virtualPath": "RBush.wasm",
        "name": "RBush.048ali807d.wasm",
        "integrity": "sha256-cHsLmPL/ZYfNtaqW793aQui9cR4N7JXjHm+xggcoz7w="
      },
      {
        "virtualPath": "Serilog.wasm",
        "name": "Serilog.8hjtlxk5b3.wasm",
        "integrity": "sha256-JB82ZlkjGaPlTTSxUifXWnmrPSI/6CtL6ywznMFd58w="
      },
      {
        "virtualPath": "Serilog.Extensions.Logging.wasm",
        "name": "Serilog.Extensions.Logging.4aarm1ia2h.wasm",
        "integrity": "sha256-NHk6xRS3dIv7nLWuBUZvce3EXz7kvxiBsrjE7KvSMcI="
      },
      {
        "virtualPath": "Serilog.Sinks.Debug.wasm",
        "name": "Serilog.Sinks.Debug.wm4zeothzh.wasm",
        "integrity": "sha256-OEANd7H/LtEUuu/2wVTnwdZYptIQAM/dOxu0sU707Z8="
      },
      {
        "virtualPath": "SixLabors.Fonts.wasm",
        "name": "SixLabors.Fonts.1ntl3xbxha.wasm",
        "integrity": "sha256-TYAwAMga8iYHgcD5+CMUwDAmCjtNUbzJJP6/e+Xtl5w="
      },
      {
        "virtualPath": "SQLite-net.wasm",
        "name": "SQLite-net.koied9adqa.wasm",
        "integrity": "sha256-7Ngmq28t6OPuOudH45NkueQhppIBaUPRYDYwGZ6/djQ="
      },
      {
        "virtualPath": "SQLitePCLRaw.batteries_v2.wasm",
        "name": "SQLitePCLRaw.batteries_v2.trscsr1tgv.wasm",
        "integrity": "sha256-qVRu5wdr7VF0EtgHF93kWVv1rJ+z41C3XQ2hIy793Mc="
      },
      {
        "virtualPath": "SQLitePCLRaw.core.wasm",
        "name": "SQLitePCLRaw.core.xkz8xl2v4t.wasm",
        "integrity": "sha256-34SMrsayL0bhoZ4JGGWjN0gYAqZotemwzF8evivgz8Q="
      },
      {
        "virtualPath": "SQLitePCLRaw.provider.e_sqlite3.wasm",
        "name": "SQLitePCLRaw.provider.e_sqlite3.cidb3ql7by.wasm",
        "integrity": "sha256-3EuT9kGxKjK50Qkss/AuRYuEc5IbUEhy3mC1Zid492w="
      },
      {
        "virtualPath": "SqlSugar.wasm",
        "name": "SqlSugar.3ie1umkl1m.wasm",
        "integrity": "sha256-q3XtbpB7iP9yiDbfJtit3OCFjJ4Tx4rJgzX02PaMr30="
      },
      {
        "virtualPath": "System.ClientModel.wasm",
        "name": "System.ClientModel.8j5eiu0uyl.wasm",
        "integrity": "sha256-hB3YvRqtJh9HC69ryOu2jplvTqYa8opx1wPk+N/txKs="
      },
      {
        "virtualPath": "System.Configuration.ConfigurationManager.wasm",
        "name": "System.Configuration.ConfigurationManager.50p01uimie.wasm",
        "integrity": "sha256-awi0DdRMBQr2w3YJtU9l+nQSH2MH5AsDPHbXPa4qWO8="
      },
      {
        "virtualPath": "System.Diagnostics.EventLog.wasm",
        "name": "System.Diagnostics.EventLog.r8r83x8oi4.wasm",
        "integrity": "sha256-FT09aAHl9Pr4fpKSFTYUltt0EmahXb+zoKoalbSMVG4="
      },
      {
        "virtualPath": "System.IdentityModel.Tokens.Jwt.wasm",
        "name": "System.IdentityModel.Tokens.Jwt.x02onqryz4.wasm",
        "integrity": "sha256-v5COmwdTSk+2jCebXcDk4w3eTs8HT3CFIXlUwrv8GdE="
      },
      {
        "virtualPath": "System.IO.Packaging.wasm",
        "name": "System.IO.Packaging.iwe4qdgpbs.wasm",
        "integrity": "sha256-ju1HTYJ6cGOLfdntmvZzkWEBgjQha8t1GCd9R0xIkKk="
      },
      {
        "virtualPath": "System.Memory.Data.wasm",
        "name": "System.Memory.Data.9qfc8r8gag.wasm",
        "integrity": "sha256-oa47oEq24c04+SqrF0aDKfadFgOh7up7I5vdcwdrLOw="
      },
      {
        "virtualPath": "System.Security.Cryptography.ProtectedData.wasm",
        "name": "System.Security.Cryptography.ProtectedData.2yj0iaqwy3.wasm",
        "integrity": "sha256-BkHqtpEGgmL8T3LHQ3xBXuG30aVtho8kecNryUecI4k="
      },
      {
        "virtualPath": "TagLibSharp.wasm",
        "name": "TagLibSharp.n2haqcvrym.wasm",
        "integrity": "sha256-7y4v+i9aJYkeMFE4PipzBICkVWNrLfzRUJafjUUCPts="
      },
      {
        "virtualPath": "Util.Reflection.wasm",
        "name": "Util.Reflection.3yeepjwilb.wasm",
        "integrity": "sha256-K6hIzztWakRScpqG3NsgXUqxClAnf0xFCquKB76/U9E="
      },
      {
        "virtualPath": "Microsoft.CSharp.wasm",
        "name": "Microsoft.CSharp.zp77lblu32.wasm",
        "integrity": "sha256-1zM3htwjf+Jnb4Q3648lG57/uM7d/RWwc1vDrbCwwEY="
      },
      {
        "virtualPath": "Microsoft.Win32.Primitives.wasm",
        "name": "Microsoft.Win32.Primitives.8m1225bit8.wasm",
        "integrity": "sha256-UOWim5DzjNCTCkxC9yd29dMsDE/bzc6Aejie/iaa1Qs="
      },
      {
        "virtualPath": "System.Buffers.wasm",
        "name": "System.Buffers.0rjii1srtc.wasm",
        "integrity": "sha256-hFUXSZrGMpGzVJiVUXosiKJDEzee8T58DnNTCaMu6+Q="
      },
      {
        "virtualPath": "System.Collections.Concurrent.wasm",
        "name": "System.Collections.Concurrent.2pvys80o2a.wasm",
        "integrity": "sha256-Cm/0vxhEdjCyxGe4VBTFsd2Od6nhQV9z+qvDrAeNIpE="
      },
      {
        "virtualPath": "System.Collections.Immutable.wasm",
        "name": "System.Collections.Immutable.tn4vxkjqd7.wasm",
        "integrity": "sha256-p5XdEPDHmUGCyWovE8QqfFcXfMwObMOETyA/e5uhoac="
      },
      {
        "virtualPath": "System.Collections.NonGeneric.wasm",
        "name": "System.Collections.NonGeneric.5o655c4w2f.wasm",
        "integrity": "sha256-+zmKYaW18eLCaQ4QYEJZpbkdymxryw6JkRIZhO56NJ4="
      },
      {
        "virtualPath": "System.Collections.Specialized.wasm",
        "name": "System.Collections.Specialized.jlsdivr9lp.wasm",
        "integrity": "sha256-+Q9PSGF0WCTcx4+nK6pGY6vwqBM2ATJuNIkGDg6xD/o="
      },
      {
        "virtualPath": "System.Collections.wasm",
        "name": "System.Collections.scy0ruvzjc.wasm",
        "integrity": "sha256-7JDM8wUI6gYf1LJoFwRHnMGjQe1P0kira3K0fpZFiv8="
      },
      {
        "virtualPath": "System.ComponentModel.Annotations.wasm",
        "name": "System.ComponentModel.Annotations.l57uyzum6n.wasm",
        "integrity": "sha256-R1I/JrItuprPK/FNICl6pmxLcrI9wJ69h4cMpDeYA6E="
      },
      {
        "virtualPath": "System.ComponentModel.Primitives.wasm",
        "name": "System.ComponentModel.Primitives.ikrbu4117e.wasm",
        "integrity": "sha256-Uh5QIoU61dh7oLcb5ftz1appahWZy3FRf7Fw7g6QrMo="
      },
      {
        "virtualPath": "System.ComponentModel.TypeConverter.wasm",
        "name": "System.ComponentModel.TypeConverter.frelp2l09r.wasm",
        "integrity": "sha256-BXEJfWtbGbGrcxNeoU6m8k2e/AESzaVSs2jPp05cz70="
      },
      {
        "virtualPath": "System.ComponentModel.wasm",
        "name": "System.ComponentModel.dolejox6e1.wasm",
        "integrity": "sha256-PUjpNqMNH5s4px1VIYViXWP2UNMdPgjQfFbpE3QUuYo="
      },
      {
        "virtualPath": "System.Console.wasm",
        "name": "System.Console.en3u1g2o8r.wasm",
        "integrity": "sha256-+m5hRul52NUDYsjnDhu1VjZaDdeR0kxagy7DSlQcbLU="
      },
      {
        "virtualPath": "System.Data.Common.wasm",
        "name": "System.Data.Common.qb3wk2hnsu.wasm",
        "integrity": "sha256-1oTonNsLYzZkTtwb8RfAVpexoyBfzzHOijXi/RR54HI="
      },
      {
        "virtualPath": "System.Diagnostics.Debug.wasm",
        "name": "System.Diagnostics.Debug.dowtp4gqaz.wasm",
        "integrity": "sha256-rRNq8BwMutsyG712L+0zVFeKIp/PiNHfIKupXLUefZY="
      },
      {
        "virtualPath": "System.Diagnostics.DiagnosticSource.wasm",
        "name": "System.Diagnostics.DiagnosticSource.eny8kfyzag.wasm",
        "integrity": "sha256-anFzSwWKS6oV7g5yfp5CNpi1QpOrB5Vw9Y706VQyWY8="
      },
      {
        "virtualPath": "System.Diagnostics.Process.wasm",
        "name": "System.Diagnostics.Process.uwfyu0mmfo.wasm",
        "integrity": "sha256-8rxS6dKRgcCViyq7KFUC7/CBNkJ3yiewSqRM44KgzDM="
      },
      {
        "virtualPath": "System.Diagnostics.TextWriterTraceListener.wasm",
        "name": "System.Diagnostics.TextWriterTraceListener.8ccdky3fb2.wasm",
        "integrity": "sha256-SDQWvfETO4b8NF7PV/jEgaPF1Z+XJqjQi5e6+eL6924="
      },
      {
        "virtualPath": "System.Diagnostics.Tools.wasm",
        "name": "System.Diagnostics.Tools.zvhw0dg88j.wasm",
        "integrity": "sha256-cYm3uKQPsY2mJqOHk1D9D+y14p8IuBR0Syuy0Ch55pw="
      },
      {
        "virtualPath": "System.Diagnostics.TraceSource.wasm",
        "name": "System.Diagnostics.TraceSource.3it0ubhrsv.wasm",
        "integrity": "sha256-pbDQJGywgbDqVNhW90VLx1q6848AiOda175o2wvFUZc="
      },
      {
        "virtualPath": "System.Diagnostics.Tracing.wasm",
        "name": "System.Diagnostics.Tracing.ptpux7v9g3.wasm",
        "integrity": "sha256-PUudXHUMJH42NwqsBa4LLhWtWV/nH3RqX9bDvjJp6Qk="
      },
      {
        "virtualPath": "System.Drawing.Primitives.wasm",
        "name": "System.Drawing.Primitives.10nm2qwwg3.wasm",
        "integrity": "sha256-8Qfe6ONqKy4+7UUxWb5TJCSjWsz0ZsG7Q8Fl1U0JFYY="
      },
      {
        "virtualPath": "System.Drawing.wasm",
        "name": "System.Drawing.8v1a6ukkfm.wasm",
        "integrity": "sha256-DJoelqUhVeHGNEhPHthja1BgXjgCleJwptglGHOM9OA="
      },
      {
        "virtualPath": "System.Formats.Asn1.wasm",
        "name": "System.Formats.Asn1.um8rfshlz3.wasm",
        "integrity": "sha256-dhOINEsPoLdVEzzTyVnz326l/2pobxhyad3mtWanXqU="
      },
      {
        "virtualPath": "System.IO.Compression.Brotli.wasm",
        "name": "System.IO.Compression.Brotli.dsk3h9bi2z.wasm",
        "integrity": "sha256-/TDqh3TfOKgEN6Qbdan5epqECRLhZkNhrt/6rPQg3WY="
      },
      {
        "virtualPath": "System.IO.Compression.ZipFile.wasm",
        "name": "System.IO.Compression.ZipFile.8y8nf5q7u0.wasm",
        "integrity": "sha256-EJCvEFKVJWBDztUuTWHOz06ai8Mucaia7m1dviPVcko="
      },
      {
        "virtualPath": "System.IO.Compression.wasm",
        "name": "System.IO.Compression.rk51ty9f5s.wasm",
        "integrity": "sha256-eyc08nU/pw9GVm1KlzAD0KUYnZnWnc1fidkXuK51xKA="
      },
      {
        "virtualPath": "System.IO.FileSystem.AccessControl.wasm",
        "name": "System.IO.FileSystem.AccessControl.hopiqu0p82.wasm",
        "integrity": "sha256-gSQrAb0DSRw6nTeGcmHu/W5Ku7W9G76FJQwG9s+XUFA="
      },
      {
        "virtualPath": "System.IO.FileSystem.Watcher.wasm",
        "name": "System.IO.FileSystem.Watcher.oc7fs4vkr8.wasm",
        "integrity": "sha256-+O69nNtgJi0TuZDyOZoVXg2v5wL45kCSH7Xt49obKlU="
      },
      {
        "virtualPath": "System.IO.FileSystem.wasm",
        "name": "System.IO.FileSystem.yxfxk8n583.wasm",
        "integrity": "sha256-Cs0cNNz/VL3Bp5wnIaaSd5JfzNx0zduBWzB/jxNx8Wc="
      },
      {
        "virtualPath": "System.IO.Pipelines.wasm",
        "name": "System.IO.Pipelines.k38bln6ppa.wasm",
        "integrity": "sha256-QHFZl18RxrSUXjF28DJPOafp5QWBQfHDNVjjU/MlDnU="
      },
      {
        "virtualPath": "System.IO.Pipes.wasm",
        "name": "System.IO.Pipes.7spacti0it.wasm",
        "integrity": "sha256-B+F0EBPfv7IuGWfYuuZ2eIc3XRlVi0Sp/PP54+d2cgE="
      },
      {
        "virtualPath": "System.Linq.Expressions.wasm",
        "name": "System.Linq.Expressions.5aj80s0k2f.wasm",
        "integrity": "sha256-Y/y0SkhyMMWsy+I1c6ZvLx5jfkPzeh6N/PpG5DZZvMw="
      },
      {
        "virtualPath": "System.Linq.Parallel.wasm",
        "name": "System.Linq.Parallel.ml0nz9n9i4.wasm",
        "integrity": "sha256-NibSjm1TIwvdKlA2p2QhNPkYPLfExqd4EczXuseOxQ0="
      },
      {
        "virtualPath": "System.Linq.wasm",
        "name": "System.Linq.62ynl6zss4.wasm",
        "integrity": "sha256-0rZKkamuBeqJiy5lpLNFZxQ3izTAp7IHj6IjbTrKwpk="
      },
      {
        "virtualPath": "System.Memory.wasm",
        "name": "System.Memory.z90949i4jl.wasm",
        "integrity": "sha256-p9HL4bhpogAxU/NbTciIkQK8xBiZNqGMLCo/2FvM6BY="
      },
      {
        "virtualPath": "System.Net.Http.Json.wasm",
        "name": "System.Net.Http.Json.aecw4jdajf.wasm",
        "integrity": "sha256-wgZZjzkdl6pTZqL4kNBUUnUaGnOKC9XbsuRPcVJhcjM="
      },
      {
        "virtualPath": "System.Net.Http.wasm",
        "name": "System.Net.Http.kam2dt0s2v.wasm",
        "integrity": "sha256-pGGPy+7czlr8zuIEKQAVLGZNBTZ+nbUbdNGmDNWeiFw="
      },
      {
        "virtualPath": "System.Net.HttpListener.wasm",
        "name": "System.Net.HttpListener.fuu73009a9.wasm",
        "integrity": "sha256-GUVP8wXC4aKOiOqUK7/f0aLqzSDoP0ge3gHbSgTHzZM="
      },
      {
        "virtualPath": "System.Net.NameResolution.wasm",
        "name": "System.Net.NameResolution.suz7azpj8f.wasm",
        "integrity": "sha256-RLcl0Uk/yfQ2zY5CqtTA9Wlq4FuoABV/pG9rAM30ywA="
      },
      {
        "virtualPath": "System.Net.NetworkInformation.wasm",
        "name": "System.Net.NetworkInformation.dgsycjjxeo.wasm",
        "integrity": "sha256-cxPn6/cE6LThdf7qwL7BvW+wBwUTbwRxMQ6o9CoMj3s="
      },
      {
        "virtualPath": "System.Net.Primitives.wasm",
        "name": "System.Net.Primitives.u5dmuq3msr.wasm",
        "integrity": "sha256-IiKUlDRWrY0bRWw3ESvNOksfrtr26ewDx6QYaI+kf1k="
      },
      {
        "virtualPath": "System.Net.Security.wasm",
        "name": "System.Net.Security.wiq8xzf9zg.wasm",
        "integrity": "sha256-uGpC+Rn/lKyNQQ0iVdag/Awj9SrHo8CTdIc9FQ0iiNE="
      },
      {
        "virtualPath": "System.Net.Sockets.wasm",
        "name": "System.Net.Sockets.p3m8da0kx3.wasm",
        "integrity": "sha256-5bZHBFEvdsBkJGCZvzrziRAMilLdDRhRUWTf+pYUT7c="
      },
      {
        "virtualPath": "System.Net.WebClient.wasm",
        "name": "System.Net.WebClient.unvek5d6lr.wasm",
        "integrity": "sha256-NlA1PzwY/R2fg+S1i4Q1FX4NcebIF9BOkdhJL1+PJZA="
      },
      {
        "virtualPath": "System.Net.WebSockets.wasm",
        "name": "System.Net.WebSockets.snml18rn76.wasm",
        "integrity": "sha256-CGmUkAMwJAylNstnyq2NdlGp/pjCwjM7/Aye4n88F3Q="
      },
      {
        "virtualPath": "System.Numerics.Vectors.wasm",
        "name": "System.Numerics.Vectors.6kbacfx87b.wasm",
        "integrity": "sha256-BpHAFX2p3rMGv5djy88todDwX1fMEA/vWHm6Uxw6XsU="
      },
      {
        "virtualPath": "System.ObjectModel.wasm",
        "name": "System.ObjectModel.ak0kp7553x.wasm",
        "integrity": "sha256-JvscaL+R4TrE1119yVgo144Kxd4uDe8JhKhNOOskHXo="
      },
      {
        "virtualPath": "System.Private.Uri.wasm",
        "name": "System.Private.Uri.e1acqxw0vp.wasm",
        "integrity": "sha256-LsQo6Ps2LQXnrOJk+YdXm7zrSorOmTYcDKwPKD4Rd44="
      },
      {
        "virtualPath": "System.Private.Xml.Linq.wasm",
        "name": "System.Private.Xml.Linq.nj1av4pq0a.wasm",
        "integrity": "sha256-U0V/jPi53ucOvBjJo8z2+QYQbvUWfuozvPIjlRdgy1M="
      },
      {
        "virtualPath": "System.Private.Xml.wasm",
        "name": "System.Private.Xml.rg5u0ix1wr.wasm",
        "integrity": "sha256-1bB/bobfVsbV72LG3mwzqo7axL2UPbTUgS8FevWkQv4="
      },
      {
        "virtualPath": "System.Reflection.Emit.ILGeneration.wasm",
        "name": "System.Reflection.Emit.ILGeneration.gdndtxn7ev.wasm",
        "integrity": "sha256-bLcX6bpH+hWhdnDZr58iK7FCNTeYHGZBFFYSoZCUdxs="
      },
      {
        "virtualPath": "System.Reflection.Emit.Lightweight.wasm",
        "name": "System.Reflection.Emit.Lightweight.rllaqc97f8.wasm",
        "integrity": "sha256-IRrkSCHDvxy2L57RTN9g7qudJ/88H9+9yxsEeziOtg8="
      },
      {
        "virtualPath": "System.Reflection.Primitives.wasm",
        "name": "System.Reflection.Primitives.gep9eginc1.wasm",
        "integrity": "sha256-2NaObkwoP7Agp2ayvIKSgzJtWIeWP1z5fp4koewJLPA="
      },
      {
        "virtualPath": "System.Resources.ResourceManager.wasm",
        "name": "System.Resources.ResourceManager.cfbstgu0fj.wasm",
        "integrity": "sha256-2IVkqQKbh3NMXkiEWzm71a71UnViAEiNTrMhJSw66MM="
      },
      {
        "virtualPath": "System.Runtime.CompilerServices.Unsafe.wasm",
        "name": "System.Runtime.CompilerServices.Unsafe.gjlzj0lo0m.wasm",
        "integrity": "sha256-zJo6g1jH/ZNpgUalIIWNspjgOKPQiHKjZ/LMOW95y3A="
      },
      {
        "virtualPath": "System.Runtime.Extensions.wasm",
        "name": "System.Runtime.Extensions.n7dfhybubd.wasm",
        "integrity": "sha256-XWYXP+R/Ud3wOrpRaJ/fKp4dAGSKpXdfEi1K/7s+ZFM="
      },
      {
        "virtualPath": "System.Runtime.InteropServices.RuntimeInformation.wasm",
        "name": "System.Runtime.InteropServices.RuntimeInformation.6k5lxywrzi.wasm",
        "integrity": "sha256-ZliUF7sdCZsAt6Ztvq78KMzYXrDOfm1AnSnaUxnk4E4="
      },
      {
        "virtualPath": "System.Runtime.InteropServices.wasm",
        "name": "System.Runtime.InteropServices.beaujbgr2k.wasm",
        "integrity": "sha256-3bB6seJLvmeeQI6nIP0n8SbSn404G23iElXpcVNTUbo="
      },
      {
        "virtualPath": "System.Runtime.Numerics.wasm",
        "name": "System.Runtime.Numerics.h8zz8qn0r0.wasm",
        "integrity": "sha256-30If9DjmHJumugt5wm+4ziQV3AenAaUMWwLGCS5Pe5M="
      },
      {
        "virtualPath": "System.Runtime.Serialization.Formatters.wasm",
        "name": "System.Runtime.Serialization.Formatters.x4cyzvhp57.wasm",
        "integrity": "sha256-D1/3zmgIADDHPVSvGN0Djm4r6etnkLNikYIwpJBTPRw="
      },
      {
        "virtualPath": "System.Runtime.Serialization.Primitives.wasm",
        "name": "System.Runtime.Serialization.Primitives.df619ahp99.wasm",
        "integrity": "sha256-yTdjv2VTQlcJxfAEBApTZ2aYKPyS1BaUg/B13IO8jb0="
      },
      {
        "virtualPath": "System.Runtime.wasm",
        "name": "System.Runtime.58i3krbx6v.wasm",
        "integrity": "sha256-BU/wnEc3kcBwZFxZ5PGC0zIQepDK6zfGe/e0CDqEsKg="
      },
      {
        "virtualPath": "System.Security.AccessControl.wasm",
        "name": "System.Security.AccessControl.gyty6txvji.wasm",
        "integrity": "sha256-KPZMmkYTCqJSzUBIXaRImlzMuqSNQt/pEt6cQI26bsk="
      },
      {
        "virtualPath": "System.Security.Claims.wasm",
        "name": "System.Security.Claims.qr4orhfi6v.wasm",
        "integrity": "sha256-Jn1uZZWHos1+SMC9wztzt/8Hl54l3PqDog+MgQjrNU0="
      },
      {
        "virtualPath": "System.Security.Cryptography.Algorithms.wasm",
        "name": "System.Security.Cryptography.Algorithms.arvwszo3xb.wasm",
        "integrity": "sha256-Ok+F/1dftlnJ8WVPQFCOo32ZwYxqRc6FaIu26O4gmKY="
      },
      {
        "virtualPath": "System.Security.Cryptography.Cng.wasm",
        "name": "System.Security.Cryptography.Cng.r1y1lp6x9r.wasm",
        "integrity": "sha256-YcSEJkl9t468ftqdE9troVOYjGTfoiRzi2qh4rf4ydc="
      },
      {
        "virtualPath": "System.Security.Cryptography.Csp.wasm",
        "name": "System.Security.Cryptography.Csp.ou0goymy1l.wasm",
        "integrity": "sha256-sNdemXIqHnXa3uhnKHv4c2ouq9JeILDSvRNbAVUBjTI="
      },
      {
        "virtualPath": "System.Security.Cryptography.Encoding.wasm",
        "name": "System.Security.Cryptography.Encoding.b4wticcmf4.wasm",
        "integrity": "sha256-JXbQuITFGlquZ3+yalJc0Hut05iycIi7U7nQx6Ddz5I="
      },
      {
        "virtualPath": "System.Security.Cryptography.Primitives.wasm",
        "name": "System.Security.Cryptography.Primitives.jkhpjbhg9a.wasm",
        "integrity": "sha256-zcv8xT4MYiD3ULPbJqL/YdGTZkNu4K1geSnNHp+FsgY="
      },
      {
        "virtualPath": "System.Security.Cryptography.X509Certificates.wasm",
        "name": "System.Security.Cryptography.X509Certificates.qt7ysc7lm4.wasm",
        "integrity": "sha256-HlgQF54jcfIF15YJfT9qx9QQ4nQDVmrImOHeJbgBDV8="
      },
      {
        "virtualPath": "System.Security.Cryptography.wasm",
        "name": "System.Security.Cryptography.uknd2urwvn.wasm",
        "integrity": "sha256-nuIFyxOxyZw6CY86JuwJ0OXoeVKVLtxseyHRAHGBFd0="
      },
      {
        "virtualPath": "System.Security.Principal.Windows.wasm",
        "name": "System.Security.Principal.Windows.94ygklotae.wasm",
        "integrity": "sha256-udjkkOY3W50aztYR1oL5MxyYILmq2xdTSTcndekupgk="
      },
      {
        "virtualPath": "System.Text.Encoding.Extensions.wasm",
        "name": "System.Text.Encoding.Extensions.x045pxoox8.wasm",
        "integrity": "sha256-oQAEHPquYbMKJ8T/eIffYGTLyztUrt7lGS0RvX1BUxs="
      },
      {
        "virtualPath": "System.Text.Encodings.Web.wasm",
        "name": "System.Text.Encodings.Web.5cww9lv23r.wasm",
        "integrity": "sha256-SwckNpOJ7fat95NP9z7NaSTbTnPYUaa5u5tSKqMALHs="
      },
      {
        "virtualPath": "System.Text.Json.wasm",
        "name": "System.Text.Json.9q18rrj505.wasm",
        "integrity": "sha256-wXFCVuXa8xAkZSE8hmlegoZjPUyWnIar8FomMmKKXsQ="
      },
      {
        "virtualPath": "System.Text.RegularExpressions.wasm",
        "name": "System.Text.RegularExpressions.620om3d65b.wasm",
        "integrity": "sha256-stcg3ZvYU/3cRuJbZ3sb92ZjfWiwiP7qrsFH0wl4t+o="
      },
      {
        "virtualPath": "System.Threading.Channels.wasm",
        "name": "System.Threading.Channels.7p4g7ighez.wasm",
        "integrity": "sha256-y6proUQU+ag9X+nTs722UR+pWad3o+s8Kzj9a3+fEts="
      },
      {
        "virtualPath": "System.Threading.Tasks.Extensions.wasm",
        "name": "System.Threading.Tasks.Extensions.o5w4hig1ay.wasm",
        "integrity": "sha256-zPQ3NQd18VdCA4lpMvPBdPT+brpH7sYopEjSfs0syPg="
      },
      {
        "virtualPath": "System.Threading.Tasks.Parallel.wasm",
        "name": "System.Threading.Tasks.Parallel.7b1lmf3d9x.wasm",
        "integrity": "sha256-HSS6dmsVwhHkXwXlmC+GzBdsqGvLk1jGCNNZ4fE6Wps="
      },
      {
        "virtualPath": "System.Threading.Tasks.wasm",
        "name": "System.Threading.Tasks.w241fx2h0k.wasm",
        "integrity": "sha256-GEJP4YaPc57tJ+bQnZW7WPalgfNVWvyglEwlQPnvqAU="
      },
      {
        "virtualPath": "System.Threading.Thread.wasm",
        "name": "System.Threading.Thread.srs9aepq0v.wasm",
        "integrity": "sha256-6EtDwfOJWOBBahKY9RohQa0DTY++FA8KQ3p+Xfy/ahM="
      },
      {
        "virtualPath": "System.Threading.wasm",
        "name": "System.Threading.kemeboty64.wasm",
        "integrity": "sha256-FPxsGxO/zhryoBi8IJ8yAT/M6Gh2oM/J6bxAI/SEQrE="
      },
      {
        "virtualPath": "System.Transactions.Local.wasm",
        "name": "System.Transactions.Local.yq6a48aebf.wasm",
        "integrity": "sha256-qgR4cdXIjchhLd+Fd3DtSIcAxXF9Ye6MPdwJJj15Zxk="
      },
      {
        "virtualPath": "System.Xml.Linq.wasm",
        "name": "System.Xml.Linq.ezielfqkzx.wasm",
        "integrity": "sha256-msiP+hhmOL4pxMsH0pIsueaLh78LNDe1D7Gz60N0jvk="
      },
      {
        "virtualPath": "System.Xml.ReaderWriter.wasm",
        "name": "System.Xml.ReaderWriter.amrekbaehh.wasm",
        "integrity": "sha256-rry8NSs7a3zG3OX/9EkVw4KldYkbrkjv1QaEKKA0w6E="
      },
      {
        "virtualPath": "System.Xml.XDocument.wasm",
        "name": "System.Xml.XDocument.w3nwgorqlr.wasm",
        "integrity": "sha256-/NxqYmf0+HY7Vd3gSnaylRUo2GJI5OCW8tq7T9L98V8="
      },
      {
        "virtualPath": "System.Xml.XmlSerializer.wasm",
        "name": "System.Xml.XmlSerializer.o53lc57e2u.wasm",
        "integrity": "sha256-PLOSXslhBMAOE9AdL7yHLTvRWrK2ELNsDgTA0taubwo="
      },
      {
        "virtualPath": "System.wasm",
        "name": "System.73436igil7.wasm",
        "integrity": "sha256-cwWOZaOlWiAEvO+KvJwrGbZmVGU/y9yYgansWdR1DsQ="
      },
      {
        "virtualPath": "netstandard.wasm",
        "name": "netstandard.kovdix8hmj.wasm",
        "integrity": "sha256-pBeTqdDH7R52LgmcAmLjfb+8rb+MtbC4K/Q+I4dsp00="
      },
      {
        "virtualPath": "Serilog.Sinks.SQLite.Simplified.wasm",
        "name": "Serilog.Sinks.SQLite.Simplified.vxqea64yka.wasm",
        "integrity": "sha256-HZ5oecbL2NTZRWdMBMYLDDK29/Ki4DVFK8nTnTzhcKI="
      },
      {
        "virtualPath": "SwashbucklerDiary.Rcl.wasm",
        "name": "SwashbucklerDiary.Rcl.3o18x3sdf3.wasm",
        "integrity": "sha256-jZiCFO+yHK6LhU03DmFOr2Y2cpFzMeIlelrv/e/V8hM="
      },
      {
        "virtualPath": "SwashbucklerDiary.Shared.wasm",
        "name": "SwashbucklerDiary.Shared.vj5thnbd79.wasm",
        "integrity": "sha256-peFIhHInBnWiy+zqqOSWCDWNpsUQgKPkzO66Jlee38A="
      },
      {
        "virtualPath": "SwashbucklerDiary.WebAssembly.wasm",
        "name": "SwashbucklerDiary.WebAssembly.5bj3nfj93p.wasm",
        "integrity": "sha256-f/jKIKVcPszKPzB5vZqwjLSERBDewi7OtrCQhaptF8g="
      }
    ],
    "satelliteResources": {
      "de": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.5v26j7q483.wasm",
          "integrity": "sha256-AWOqUW9SMAdg228QayG09+IEkJfxvc7sXfy69Tq+4pI="
        }
      ],
      "es": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.pxqbnwvbua.wasm",
          "integrity": "sha256-4/t/iRGFbF+cCpEJUA9Mc2YNBMIMNbKsMgNKySC3ljs="
        }
      ],
      "fr": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.3gzmw6jiez.wasm",
          "integrity": "sha256-Dfvyw8PqW22oSv8Kbbjacj75zQPu7dEvu7Oh0phxkv4="
        }
      ],
      "it": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.jwruepgobw.wasm",
          "integrity": "sha256-w7pY3ZzRH2R19VN+/1P4d72HAnnwTDCaRIEMbWGtBDQ="
        }
      ],
      "ja": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.n08qpeqa8n.wasm",
          "integrity": "sha256-6YNtTIB/sQ3lB+pP3WUcvNNlUHElcmyoLQVxc+YDv1A="
        }
      ],
      "ko": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.j1q4ut9ukn.wasm",
          "integrity": "sha256-KSHUsM6PHCPLjDUhY2a26SprO1VpeCj9+1+1PHK0KkY="
        }
      ],
      "pt-BR": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.m3voefpqjx.wasm",
          "integrity": "sha256-ci06CXWPhjQHDGNRRPIJTiM6lfyvG3CWYsSoY2J00wY="
        }
      ],
      "ru": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.h6ku26wilp.wasm",
          "integrity": "sha256-qVtETqaIdC4sgMaE0ehfDuvcAyYHRR7HlYpAMwuyuwE="
        }
      ],
      "zh-Hans": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.c709qta22q.wasm",
          "integrity": "sha256-uIgN/luP9hu20nk0/8408pnCohB5oh/VuV3sP8UG5R0="
        }
      ],
      "zh-Hant": [
        {
          "virtualPath": "Microsoft.Data.SqlClient.resources.wasm",
          "name": "Microsoft.Data.SqlClient.resources.4uf1ay23zb.wasm",
          "integrity": "sha256-bPjSVwhkp4RpKUPks76lwWogK09foZDSb9eu21znK+g="
        }
      ]
    }
  },
  "debugLevel": 0,
  "linkerEnabled": true,
  "globalizationMode": "sharded",
  "extensions": {
    "blazor": {}
  },
  "runtimeConfig": {
    "runtimeOptions": {
      "configProperties": {
        "Microsoft.AspNetCore.Components.Routing.RegexConstraintSupport": false,
        "Microsoft.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability": true,
        "System.ComponentModel.DefaultValueAttribute.IsSupported": false,
        "System.ComponentModel.Design.IDesignerHost.IsSupported": false,
        "System.ComponentModel.TypeConverter.EnableUnsafeBinaryFormatterInDesigntimeLicenseContextSerialization": false,
        "System.ComponentModel.TypeDescriptor.IsComObjectDescriptorSupported": false,
        "System.Data.DataSet.XmlSerializationIsSupported": false,
        "System.Diagnostics.Debugger.IsSupported": false,
        "System.Diagnostics.Metrics.Meter.IsSupported": false,
        "System.Diagnostics.Tracing.EventSource.IsSupported": false,
        "System.GC.Server": true,
        "System.Globalization.Invariant": false,
        "System.TimeZoneInfo.Invariant": false,
        "System.Linq.Enumerable.IsSizeOptimized": true,
        "System.Net.Http.EnableActivityPropagation": false,
        "System.Net.Http.WasmEnableStreamingResponse": true,
        "System.Net.SocketsHttpHandler.Http3Support": false,
        "System.Reflection.Metadata.MetadataUpdater.IsSupported": false,
        "System.Reflection.NullabilityInfoContext.IsSupported": true,
        "System.Resources.ResourceManager.AllowCustomResourceTypes": false,
        "System.Resources.UseSystemResourceKeys": true,
        "System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported": true,
        "System.Runtime.InteropServices.BuiltInComInterop.IsSupported": false,
        "System.Runtime.InteropServices.EnableConsumingManagedCodeFromNativeHosting": false,
        "System.Runtime.InteropServices.EnableCppCLIHostActivation": false,
        "System.Runtime.InteropServices.Marshalling.EnableGeneratedComInterfaceComImportInterop": false,
        "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization": false,
        "System.StartupHookProvider.IsSupported": false,
        "System.Text.Encoding.EnableUnsafeUTF7Encoding": false,
        "System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault": true,
        "System.Threading.Thread.EnableAutoreleasePool": false,
        "Microsoft.AspNetCore.Components.Endpoints.NavigationManager.DisableThrowNavigationException": false
      }
    }
  }
}/*json-end*/);export{gt as default,ft as dotnet,mt as exit};
