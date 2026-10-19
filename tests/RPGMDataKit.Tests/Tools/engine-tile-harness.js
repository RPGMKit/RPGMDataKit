// Draws a dumped scenario with the code of the engine of MV (Tilemap of rpg_core.js) and prints the hash of the picture.
// usage: node engine-tile-harness.js <rpg_core.js of an MV project> <scenario folder>   (writes engine.rgba there)
// The scenario folder is written by MapRendererTests when RPGMDATAKIT_TILE_SCENARIO_DUMP is set (see TileScenario.cs); see .claude-docs/imaging.md.
const fs = require('fs');
const crypto = require('crypto');
const [coreFile, folder] = process.argv.slice(2);

// ---- the picture the engine draws on: the same whole-number laying over as PixelCanvas.Over ----
class Bitmap {
  constructor(width, height, data) { this.width = width; this.height = height; this.data = data || new Uint8Array(width * height * 4); }
  static over(d, di, sr, sg, sb, sa) {
    if (sa === 0) return;
    const da = d[di + 3];
    if (sa === 255 || da === 0) { d[di] = sr; d[di + 1] = sg; d[di + 2] = sb; d[di + 3] = sa; return; }
    const total = sa * 255 + da * (255 - sa);
    const half = Math.floor(total / 2);
    d[di] = Math.floor((sr * sa * 255 + d[di] * da * (255 - sa) + half) / total);
    d[di + 1] = Math.floor((sg * sa * 255 + d[di + 1] * da * (255 - sa) + half) / total);
    d[di + 2] = Math.floor((sb * sa * 255 + d[di + 2] * da * (255 - sa) + half) / total);
    d[di + 3] = Math.floor((total + 127) / 255);
  }
  bltImage(source, sx, sy, sw, sh, dx, dy, dw, dh) {
    // the engine never scales here
    if (sw !== dw || sh !== dh) throw new Error('scaled blt');
    sx = Math.round(sx); sy = Math.round(sy); sw = Math.round(sw); sh = Math.round(sh); dx = Math.round(dx); dy = Math.round(dy);
    for (let y = 0; y < sh; y++) {
      const fy = sy + y, ty = dy + y;
      if (fy < 0 || fy >= source.height || ty < 0 || ty >= this.height) continue;
      for (let x = 0; x < sw; x++) {
        const fx = sx + x, tx = dx + x;
        if (fx < 0 || fx >= source.width || tx < 0 || tx >= this.width) continue;
        const si = (fy * source.width + fx) * 4;
        Bitmap.over(this.data, (ty * this.width + tx) * 4, source.data[si], source.data[si + 1], source.data[si + 2], source.data[si + 3]);
      }
    }
  }
  fillRect(x, y, w, h, color) {
    const m = /rgba\((\d+),\s*(\d+),\s*(\d+),\s*([\d.]+)\)/.exec(color);
    const r = +m[1], g = +m[2], b = +m[3], a = Math.round(+m[4] * 255);
    for (let yy = 0; yy < h; yy++) for (let xx = 0; xx < w; xx++) {
      const tx = x + xx, ty = y + yy;
      if (tx < 0 || tx >= this.width || ty < 0 || ty >= this.height) continue;
      Bitmap.over(this.data, (ty * this.width + tx) * 4, r, g, b, a);
    }
  }
  clearRect(x, y, w, h) {
    for (let yy = 0; yy < h; yy++) for (let xx = 0; xx < w; xx++) {
      const tx = x + xx, ty = y + yy;
      if (tx < 0 || tx >= this.width || ty < 0 || ty >= this.height) continue;
      const i = (ty * this.width + tx) * 4; this.data[i] = this.data[i + 1] = this.data[i + 2] = this.data[i + 3] = 0;
    }
  }
}

// ---- the code of the engine, cut out of rpg_core.js ----
const lines = fs.readFileSync(coreFile, 'utf8').split('\n');
const index = (re) => { const i = lines.findIndex((l) => re.test(l)); if (i < 0) throw new Error('not found: ' + re); return i; };
const methods = lines.slice(index(/^Tilemap\.prototype\._paintTiles = /), index(/^Tilemap\.prototype\._sortChildren = /)).join('\n');
const staticsStart = index(/^Tilemap\.TILE_ID_B /);
const staticsEnd = lines.findIndex((l, i) => i > staticsStart && /^Tilemap\.WATERFALL_AUTOTILE_TABLE = /.test(l));
const staticsClose = lines.findIndex((l, i) => i > staticsEnd && /^\];/.test(l));
const statics = lines.slice(staticsStart, staticsClose + 1).join('\n');
Number.prototype.mod = function (n) { return ((this % n) + n) % n; };
Array.prototype.equals = function (a) { return a.length === this.length && this.every((v, i) => v === a[i]); };
const Tilemap = new Function('function Tilemap() {}\n' + statics + '\n' + methods + '\nreturn Tilemap;')();

// ---- the scenario ----
const sc = JSON.parse(fs.readFileSync(folder + '/scenario.json', 'utf8'));
const T = sc.tileSize, W = sc.width, H = sc.height;
const bitmaps = [];
for (const s of sc.sheets) bitmaps[s.index] = new Bitmap(s.width, s.height, new Uint8Array(fs.readFileSync(`${folder}/sheet${s.index}.rgba`)));
const mapData = [].concat(sc.layers[0], sc.layers[1], sc.layers[2], sc.layers[3], sc.shadows, new Array(W * H).fill(0));

const tm = Object.create(Tilemap.prototype);
Object.assign(tm, {
  _tileWidth: T, _tileHeight: T, _layerWidth: W * T, _layerHeight: H * T,
  _mapWidth: W, _mapHeight: H, _mapData: mapData, horizontalWrap: false, verticalWrap: false,
  flags: sc.flags, bitmaps, animationFrame: sc.animationFrame || 0, _lastTiles: [], _frameUpdated: false,
  _lowerBitmap: new Bitmap(W * T, H * T), _upperBitmap: new Bitmap(W * T, H * T),
});
for (let y = 0; y < H; y++) for (let x = 0; x < W; x++) tm._paintTiles(0, 0, x, y);

// the upper layer over the lower one, as they are on the screen
const out = tm._lowerBitmap;
const up = tm._upperBitmap;
for (let i = 0; i < W * H * T * T; i++) Bitmap.over(out.data, i * 4, up.data[i * 4], up.data[i * 4 + 1], up.data[i * 4 + 2], up.data[i * 4 + 3]);
fs.writeFileSync(folder + '/engine.rgba', Buffer.from(out.data));
console.log(crypto.createHash('sha256').update(Buffer.from(out.data)).digest('hex'));
