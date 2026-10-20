/* AssetStudio stage 2 - Dressing Room (static 3D character dresser).
   Plain JavaScript + WebGL 1, no external library. Works from file://:
   - out/3d/index.js       manifest (window.ASSET3D_INDEX), written by extract3d.py
   - out/3d/m/<name>.js    meshes   A3D.m(name, meta, base64(zlib(float32 pos[n*3] | float32 uv[n*2] | uint16/32 idx)))
   - out/3d/t/<name>.js    textures A3D.t(name, {w,h,f}, base64(zlib(DXT1/DXT3/DXT5 blocks or RGBA)))
   Positions are in C3 space (z = down, the character stands on z = 0); conversion to GL (y up) is done here.
   Stage 3: out/3d/anim.js (window.ASSET3D_ANIM: motion tables), a/<name>.js animations (A3D.a), npcs.js
   (window.ASSET3D_NPCS) + n/<name>.js skinned NPC / monster / mount meshes. Every bone key is the final skinning
   matrix (v * init * M), see anim3d.py; the player motions are multiplied by a per-body correction gi.
*/
(function () {
'use strict';
var IDX = window.ASSET3D_INDEX || null;
var A3D = window.A3D = window.A3D || {};
var pend = {}, cacheP = {};
A3D.m = A3D.t = A3D.a = A3D.p = function (name, meta, b64) { var p = pend[name]; if (p) { delete pend[name]; p.res({ meta: meta, b64: b64 }); } };

function b64bytes(s) { var bin = atob(s), n = bin.length, u = new Uint8Array(n); for (var i = 0; i < n; i++) u[i] = bin.charCodeAt(i); return u; }
function inflate(b64) {
  if (typeof DecompressionStream === 'undefined') return Promise.reject(new Error('this browser has no DecompressionStream (use a current Chrome / Edge / Firefox)'));
  var ds = new DecompressionStream('deflate');
  return new Response(new Blob([b64bytes(b64)]).stream().pipeThrough(ds)).arrayBuffer();
}
function asset(kind, name) {
  var key = kind + '/' + name;
  if (cacheP[key]) return cacheP[key];
  var pr = new Promise(function (res, rej) {
    pend[name] = { res: res };
    var s = document.createElement('script');
    s.src = '3d/' + key + '.js';
    s.onload = function () { s.remove(); if (pend[name]) { delete pend[name]; rej(new Error('bad file ' + key)); } };
    s.onerror = function () { s.remove(); delete pend[name]; rej(new Error('missing 3d/' + key + '.js')); };
    document.head.appendChild(s);
  }).then(function (r) { return inflate(r.b64).then(function (buf) { return { meta: r.meta, buf: buf }; }); });
  cacheP[key] = pr;
  pr.catch(function () { delete cacheP[key]; });
  return pr;
}

/* ---------------------------------------------------------------- DXT decoding */
function decodeTex(meta, buf) {
  var w = meta.w, h = meta.h, f = meta.f, src = new Uint8Array(buf);
  if (f === 'RGBA') return src;
  var out = new Uint8Array(w * h * 4), bw = Math.max(1, (w + 3) >> 2), bh = Math.max(1, (h + 3) >> 2);
  var bs = f === 'DXT1' ? 8 : 16, off = 0, pal = new Uint8Array(16), al = new Uint8Array(16), at = new Uint8Array(8);
  for (var by = 0; by < bh; by++) for (var bx = 0; bx < bw; bx++, off += bs) {
    var co = f === 'DXT1' ? off : off + 8, i;
    if (f === 'DXT3') { for (i = 0; i < 16; i++) { var b = src[off + (i >> 1)]; al[i] = ((i & 1) ? (b >> 4) : (b & 15)) * 17; } }
    else if (f === 'DXT5') {
      var a0 = src[off], a1 = src[off + 1]; at[0] = a0; at[1] = a1;
      if (a0 > a1) for (i = 1; i < 7; i++) at[i + 1] = ((7 - i) * a0 + i * a1) / 7 | 0;
      else { for (i = 1; i < 5; i++) at[i + 1] = ((5 - i) * a0 + i * a1) / 5 | 0; at[6] = 0; at[7] = 255; }
      var lo = src[off + 2] | src[off + 3] << 8 | src[off + 4] << 16, hi = src[off + 5] | src[off + 6] << 8 | src[off + 7] << 16;
      for (i = 0; i < 16; i++) al[i] = at[i < 8 ? (lo >> (3 * i)) & 7 : (hi >> (3 * (i - 8))) & 7];
    } else for (i = 0; i < 16; i++) al[i] = 255;
    var c0 = src[co] | src[co + 1] << 8, c1 = src[co + 2] | src[co + 3] << 8;
    var bits = (src[co + 4] | src[co + 5] << 8 | src[co + 6] << 16 | src[co + 7] << 24) >>> 0;
    pal[0] = (c0 >> 11 & 31) * 255 / 31; pal[1] = (c0 >> 5 & 63) * 255 / 63; pal[2] = (c0 & 31) * 255 / 31; pal[3] = 255;
    pal[4] = (c1 >> 11 & 31) * 255 / 31; pal[5] = (c1 >> 5 & 63) * 255 / 63; pal[6] = (c1 & 31) * 255 / 31; pal[7] = 255;
    if (c0 > c1 || f !== 'DXT1') {
      for (i = 0; i < 3; i++) { pal[8 + i] = (2 * pal[i] + pal[4 + i]) / 3; pal[12 + i] = (pal[i] + 2 * pal[4 + i]) / 3; }
      pal[11] = pal[15] = 255;
    } else {
      for (i = 0; i < 3; i++) { pal[8 + i] = (pal[i] + pal[4 + i]) / 2; pal[12 + i] = 0; }
      pal[11] = 255; pal[15] = 0;
    }
    for (var j = 0; j < 4; j++) {
      var y = by * 4 + j; if (y >= h) break;
      for (i = 0; i < 4; i++) {
        var x = bx * 4 + i; if (x >= w) break;
        var k = j * 4 + i, p = ((bits >>> (2 * k)) & 3) * 4, o = (y * w + x) * 4;
        out[o] = pal[p]; out[o + 1] = pal[p + 1]; out[o + 2] = pal[p + 2];
        out[o + 3] = f === 'DXT1' ? pal[p + 3] : al[k];
      }
    }
  }
  return out;
}

/* ---------------------------------------------------------------- small mat4 (column-major) */
var M4 = {
  id: function () { return new Float32Array([1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]); },
  mul: function (a, b) {
    var o = new Float32Array(16);
    for (var c = 0; c < 4; c++) for (var r = 0; r < 4; r++)
      o[c * 4 + r] = a[r] * b[c * 4] + a[4 + r] * b[c * 4 + 1] + a[8 + r] * b[c * 4 + 2] + a[12 + r] * b[c * 4 + 3];
    return o;
  },
  tr: function (x, y, z) { var m = M4.id(); m[12] = x; m[13] = y; m[14] = z; return m; },
  persp: function (fov, asp, n, f) {
    var t = 1 / Math.tan(fov / 2), m = new Float32Array(16);
    m[0] = t / asp; m[5] = t; m[10] = (f + n) / (n - f); m[11] = -1; m[14] = 2 * f * n / (n - f); return m;
  },
  look: function (e, c, u) {
    var z = norm([e[0] - c[0], e[1] - c[1], e[2] - c[2]]), x = norm(cross(u, z)), y = cross(z, x);
    return new Float32Array([x[0], y[0], z[0], 0, x[1], y[1], z[1], 0, x[2], y[2], z[2], 0,
      -dot(x, e), -dot(y, e), -dot(z, e), 1]);
  },
  pt: function (m, p) { return [m[0] * p[0] + m[4] * p[1] + m[8] * p[2] + m[12], m[1] * p[0] + m[5] * p[1] + m[9] * p[2] + m[13], m[2] * p[0] + m[6] * p[1] + m[10] * p[2] + m[14]]; }
};
function cross(a, b) { return [a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0]]; }
function dot(a, b) { return a[0] * b[0] + a[1] * b[1] + a[2] * b[2]; }
function norm(a) { var l = Math.sqrt(dot(a, a)) || 1; return [a[0] / l, a[1] / l, a[2] / l]; }
// C3 (x, y, z-down, left-handed) -> GL (x, y-up, z): (-x, -z, y)
var CONV = new Float32Array([-1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1]);

/* ---------------------------------------------------------------- WebGL renderer */
var VS = 'attribute vec3 aP;attribute vec2 aT;attribute vec3 aN;uniform mat4 uM,uVP;varying vec2 vT;varying vec3 vN,vW;' +
  'void main(){vec4 w=uM*vec4(aP,1.0);vW=w.xyz;vN=(uM*vec4(aN,0.0)).xyz;vT=aT;gl_Position=uVP*w;}';
var FS = 'precision mediump float;uniform sampler2D uS;uniform float uHas,uPass;uniform vec3 uEye;varying vec2 vT;varying vec3 vN,vW;' +
  'void main(){vec4 c=uHas>0.5?texture2D(uS,vT):vec4(0.72,0.72,0.75,1.0);' +
  'if(uPass>1.5){if(c.a<0.01)discard;gl_FragColor=c;return;}' +
  'if(uPass<0.5){if(c.a<0.5)discard;c.a=1.0;}else{if(c.a<0.02)discard;}' +
  'vec3 n=normalize(vN);vec3 v=normalize(uEye-vW);if(dot(n,v)<0.0)n=-n;' +
  'vec3 L=normalize(v+vec3(0.0,0.9,0.0));float d=max(dot(n,L),0.0);float sky=0.5+0.5*n.y;' +
  'float r=pow(max(dot(reflect(-L,n),v),0.0),24.0)*0.12;' +
  'gl_FragColor=vec4(c.rgb*(0.38+0.22*sky+0.62*d)+r,c.a);}';

function Renderer(canvas) {
  var gl = canvas.getContext('webgl', { antialias: true, preserveDrawingBuffer: true, alpha: false }) ||
           canvas.getContext('experimental-webgl', { preserveDrawingBuffer: true });
  if (!gl) throw new Error('WebGL is not available');
  this.gl = gl; this.canvas = canvas;
  this.u32 = !!gl.getExtension('OES_element_index_uint');
  function sh(t, s) { var o = gl.createShader(t); gl.shaderSource(o, s); gl.compileShader(o); if (!gl.getShaderParameter(o, gl.COMPILE_STATUS)) throw new Error(gl.getShaderInfoLog(o)); return o; }
  var p = gl.createProgram(); gl.attachShader(p, sh(gl.VERTEX_SHADER, VS)); gl.attachShader(p, sh(gl.FRAGMENT_SHADER, FS)); gl.linkProgram(p);
  if (!gl.getProgramParameter(p, gl.LINK_STATUS)) throw new Error(gl.getProgramInfoLog(p));
  this.p = p;
  this.a = { P: gl.getAttribLocation(p, 'aP'), T: gl.getAttribLocation(p, 'aT'), N: gl.getAttribLocation(p, 'aN') };
  this.u = {}; var self = this;
  ['uM', 'uVP', 'uS', 'uHas', 'uPass', 'uEye'].forEach(function (n) { self.u[n] = gl.getUniformLocation(p, n); });
  this.meshes = {}; this.texs = {};
}
// CPU skinning of a body mesh (bind pose + bone indices/weights) with the bone matrices of a pose (row-vector C3 matrices)
function skin(pos, sk, n, bones) {
  var out = new Float32Array(n * 3);
  for (var i = 0; i < n; i++) {
    var x = pos[i * 3], y = pos[i * 3 + 1], z = pos[i * 3 + 2], b0 = sk[i * 3], b1 = sk[i * 3 + 1], w0 = sk[i * 3 + 2] / 255;
    var ws = [b0 !== 255 ? w0 : 0, b1 !== 255 ? 1 - w0 : 0], bs = [b0, b1], ax = 0, ay = 0, az = 0, tw = 0;
    for (var k = 0; k < 2; k++) {
      var w = ws[k], m = bones[bs[k]]; if (!(w > 0)) continue;
      if (!m) { ax += w * x; ay += w * y; az += w * z; tw += w; continue; }
      ax += w * (m[0] * x + m[4] * y + m[8] * z + m[12]); ay += w * (m[1] * x + m[5] * y + m[9] * z + m[13]); az += w * (m[2] * x + m[6] * y + m[10] * z + m[14]); tw += w;
    }
    if (tw > 0) { out[i * 3] = ax / tw; out[i * 3 + 1] = ay / tw; out[i * 3 + 2] = az / tw; } else { out[i * 3] = x; out[i * 3 + 1] = y; out[i * 3 + 2] = z; }
  }
  return out;
}
Renderer.prototype.mesh = function (name, meta, buf, pose) {
  var ck = name + (meta.s && pose ? '|' + pose.key : '');
  if (this.meshes[ck]) return this.meshes[ck];
  var gl = this.gl, n = meta.n, nt = meta.o + meta.a;
  var qa = new Int16Array(buf, 0, n * 5), Q = meta.q, pos = new Float32Array(n * 3), uv = new Float32Array(n * 2), i0 = (n * 10 + 3) & ~3;
  for (var v = 0; v < n; v++) {
    for (var k = 0; k < 3; k++) pos[v * 3 + k] = Q[k] + (qa[v * 3 + k] + 32768) * Q[5 + k];
    for (k = 0; k < 2; k++) uv[v * 2 + k] = Q[3 + k] + (qa[n * 3 + v * 2 + k] + 32768) * Q[8 + k];
  }
  var idx = meta.i === 4 ? new Uint32Array(buf, i0, nt * 3) : new Uint16Array(buf, i0, nt * 3);
  var bb = meta.b;
  if (meta.s && pose && pose.bones) {
    pos = skin(pos, new Uint8Array(buf, i0 + nt * 3 * meta.i, n * 3), n, pose.bones);
    bb = [1e9, 1e9, 1e9, -1e9, -1e9, -1e9];
    for (var q = 0; q < n * 3; q++) { var c3 = q % 3; if (pos[q] < bb[c3]) bb[c3] = pos[q]; if (pos[q] > bb[c3 + 3]) bb[c3 + 3] = pos[q]; }
  }
  var nrm = new Float32Array(n * 3);
  for (var t = 0; t < nt; t++) {
    var a = idx[t * 3] * 3, b = idx[t * 3 + 1] * 3, c = idx[t * 3 + 2] * 3;
    var ux = pos[b] - pos[a], uy = pos[b + 1] - pos[a + 1], uz = pos[b + 2] - pos[a + 2];
    var vx = pos[c] - pos[a], vy = pos[c + 1] - pos[a + 1], vz = pos[c + 2] - pos[a + 2];
    var nx = uy * vz - uz * vy, ny = uz * vx - ux * vz, nz = ux * vy - uy * vx;
    [a, b, c].forEach(function (k) { nrm[k] += nx; nrm[k + 1] += ny; nrm[k + 2] += nz; });
  }
  function vbo(d) { var o = gl.createBuffer(); gl.bindBuffer(gl.ARRAY_BUFFER, o); gl.bufferData(gl.ARRAY_BUFFER, d, gl.STATIC_DRAW); return o; }
  var ib = gl.createBuffer(); gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ib);
  var big = meta.i === 4;
  if (big && !this.u32) { idx = new Uint16Array(idx); big = false; }
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, idx, gl.STATIC_DRAW);
  var m = { p: vbo(pos), t: vbo(uv), n: vbo(nrm), i: ib, it: big ? gl.UNSIGNED_INT : gl.UNSIGNED_SHORT, ib: big ? 4 : 2,
    o: meta.o, al: meta.a, h: (meta.s && pose && pose.h) || meta.h || {}, b: bb, skinned: !!meta.s };
  this.meshes[ck] = m; return m;
};
Renderer.prototype.tex = function (name, meta, buf) {
  if (this.texs[name]) return this.texs[name];
  var gl = this.gl, px = decodeTex(meta, buf), o = gl.createTexture();
  gl.bindTexture(gl.TEXTURE_2D, o);
  gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
  gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, meta.w, meta.h, 0, gl.RGBA, gl.UNSIGNED_BYTE, px);
  var pot = !(meta.w & (meta.w - 1)) && !(meta.h & (meta.h - 1));
  if (pot) { gl.generateMipmap(gl.TEXTURE_2D); gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR_MIPMAP_LINEAR); }
  else gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
  var wr = pot ? gl.REPEAT : gl.CLAMP_TO_EDGE;
  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, wr); gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, wr);
  this.texs[name] = o; return o;
};
Renderer.prototype.draw = function (objs, cam, bg) {
  var fxl = objs.filter(function (o) { return o.fx; });
  objs = objs.filter(function (o) { return !o.fx; });
  var gl = this.gl, c = this.canvas, dpr = window.devicePixelRatio || 1;
  var w = Math.max(1, Math.round(c.clientWidth * dpr)), h = Math.max(1, Math.round(c.clientHeight * dpr));
  if (c.width !== w || c.height !== h) { c.width = w; c.height = h; }
  gl.viewport(0, 0, w, h);
  gl.clearColor(bg[0], bg[1], bg[2], 1); gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
  gl.useProgram(this.p); gl.enable(gl.DEPTH_TEST); gl.disable(gl.CULL_FACE);
  var vp = M4.mul(M4.persp(cam.fov, w / h, cam.near, cam.far), M4.look(cam.eye, cam.target, [0, 1, 0]));
  gl.uniformMatrix4fv(this.u.uVP, false, vp); gl.uniform3fv(this.u.uEye, cam.eye); gl.uniform1i(this.u.uS, 0);
  var self = this, a = this.a;
  function bind(o) {
    var m = o.mesh;
    gl.bindBuffer(gl.ARRAY_BUFFER, m.p); gl.enableVertexAttribArray(a.P); gl.vertexAttribPointer(a.P, 3, gl.FLOAT, false, 0, 0);
    gl.bindBuffer(gl.ARRAY_BUFFER, m.t); gl.enableVertexAttribArray(a.T); gl.vertexAttribPointer(a.T, 2, gl.FLOAT, false, 0, 0);
    if (a.N >= 0) { gl.bindBuffer(gl.ARRAY_BUFFER, m.n); gl.enableVertexAttribArray(a.N); gl.vertexAttribPointer(a.N, 3, gl.FLOAT, false, 0, 0); }
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, m.i);
    gl.uniformMatrix4fv(self.u.uM, false, o.model);
    gl.activeTexture(gl.TEXTURE0); gl.bindTexture(gl.TEXTURE_2D, o.tex || null); gl.uniform1f(self.u.uHas, o.tex ? 1 : 0);
  }
  // pass 1: opaque triangles (alpha-tested), pass 2: alpha triangles back to front, no depth writes
  gl.disable(gl.BLEND); gl.depthMask(true); gl.uniform1f(this.u.uPass, 0);
  objs.forEach(function (o) { if (o.mesh.o) { bind(o); gl.drawElements(gl.TRIANGLES, o.mesh.o * 3, o.mesh.it, 0); } });
  var al = objs.filter(function (o) { return o.mesh.al; }).map(function (o) {
    var cc = M4.pt(o.model, [(o.mesh.b[0] + o.mesh.b[3]) / 2, (o.mesh.b[1] + o.mesh.b[4]) / 2, (o.mesh.b[2] + o.mesh.b[5]) / 2]);
    var d = [cc[0] - cam.eye[0], cc[1] - cam.eye[1], cc[2] - cam.eye[2]];
    return { o: o, d: dot(d, d) };
  }).sort(function (x, y) { return y.d - x.d; });
  if (al.length) {
    gl.enable(gl.BLEND); gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA); gl.uniform1f(this.u.uPass, 1);
    // alpha geometry first writes depth where it is nearly opaque (keeps hair / capes from showing what is behind)
    al.forEach(function (x) { var o = x.o; bind(o); gl.depthMask(false); gl.drawElements(gl.TRIANGLES, o.mesh.al * 3, o.mesh.it, o.mesh.o * 3 * o.mesh.ib); });
    gl.depthMask(true); gl.disable(gl.BLEND);
  }
  this.drawFx(fxl);
};


/* ---------------------------------------------------------------- stage 3: animations (out/3d/a, anim.js) */
var ANIM = window.ASSET3D_ANIM || null, NPCS = window.ASSET3D_NPCS || null;
var ANIMS = {};
function loadAnimByName(name) {
  if (!name) return Promise.resolve(null);
  if (ANIMS[name]) return ANIMS[name];
  var pr = asset('a', name).then(function (r) { return decodeAnim(r.meta, r.buf); });
  ANIMS[name] = pr; pr.catch(function () { delete ANIMS[name]; });
  return pr;
}
function loadAnim(i) { return loadAnimByName(ANIM && i != null && i >= 0 ? ANIM.anims[i] : null); }
// meta.p[i] = [type, bones, keys, tmin x3, tstep x3, rotScale]; data per part: uint16 keyFrames (padded to even),
// then int16 tracks bone-major, component-major, delta-coded along the keys (see anim3d.py)
function decodeAnim(meta, buf) {
  var dv = new DataView(buf), off = 0, qs = meta.qs || 32767, parts = [];
  meta.p.forEach(function (p) {
    var t = p[0], nb = p[1], nk = p[2], k, j, c;
    if (t === '-' || !nb || !nk) { parts.push(null); return; }
    var fr = new Float32Array(nk);
    for (k = 0; k < nk; k++) fr[k] = dv.getUint16(off + 2 * k, true);
    off += (nk + (nk & 1)) * 2;
    var nc = t === 'q' ? 7 : t === 's' ? 8 : 12, v = new Float32Array(nk * nb * nc);
    var t0 = p[3], t1 = p[4], t2 = p[5], s0 = p[6], s1 = p[7], s2 = p[8], rs = p[9];
    for (j = 0; j < nb; j++) for (c = 0; c < nc; c++) {
      var acc = 0, x;
      for (k = 0; k < nk; k++) {
        acc = ((acc + dv.getInt16(off, true) + 32768) & 0xFFFF) - 32768; off += 2;
        if (t === 'm') x = c < 9 ? acc / qs * rs : (c === 9 ? t0 + (acc + 32768) * s0 : c === 10 ? t1 + (acc + 32768) * s1 : t2 + (acc + 32768) * s2);
        else x = c < 4 ? acc / qs : c === 4 ? t0 + (acc + 32768) * s0 : c === 5 ? t1 + (acc + 32768) * s1 : c === 6 ? t2 + (acc + 32768) * s2 : acc / 1000;
        v[(k * nb + j) * nc + c] = x;
      }
    }
    parts.push({ t: t, nb: nb, nk: nk, nc: nc, fr: fr, v: v });
  });
  return { frames: Math.max(1, meta.f), parts: parts };
}
// bone matrices (row-vector 4x4, row-major, 16 per bone) of one motion part at frame f; keys are interpolated
// (quaternions: normalised lerp, matrices: linear), the last key is held to the end of the motion
function samplePart(P, f, out) {
  var nk = P.nk, fr = P.fr, k0 = 0, k1, a = 0;
  while (k0 < nk - 1 && fr[k0 + 1] <= f) k0++;
  k1 = Math.min(k0 + 1, nk - 1);
  if (k1 > k0) a = Math.max(0, Math.min(1, (f - fr[k0]) / Math.max(1e-6, fr[k1] - fr[k0])));
  var nb = P.nb, nc = P.nc, v = P.v, b0 = k0 * nb * nc, b1 = k1 * nb * nc;
  out = out && out.length >= nb * 16 ? out : new Float32Array(nb * 16);
  for (var j = 0; j < nb; j++) {
    var o = j * 16, i0 = b0 + j * nc, i1 = b1 + j * nc;
    if (P.t === 'm') {
      var r = [];
      for (var c = 0; c < 12; c++) r.push(v[i0 + c] + (v[i1 + c] - v[i0 + c]) * a);
      out[o] = r[0]; out[o + 1] = r[1]; out[o + 2] = r[2]; out[o + 3] = 0;
      out[o + 4] = r[3]; out[o + 5] = r[4]; out[o + 6] = r[5]; out[o + 7] = 0;
      out[o + 8] = r[6]; out[o + 9] = r[7]; out[o + 10] = r[8]; out[o + 11] = 0;
      out[o + 12] = r[9]; out[o + 13] = r[10]; out[o + 14] = r[11]; out[o + 15] = 1;
      continue;
    }
    var qx = v[i0], qy = v[i0 + 1], qz = v[i0 + 2], qw = v[i0 + 3], px = v[i1], py = v[i1 + 1], pz = v[i1 + 2], pw = v[i1 + 3];
    if (qx * px + qy * py + qz * pz + qw * pw < 0) { px = -px; py = -py; pz = -pz; pw = -pw; }
    var x = qx + (px - qx) * a, y = qy + (py - qy) * a, z = qz + (pz - qz) * a, w = qw + (pw - qw) * a;
    var l = Math.sqrt(x * x + y * y + z * z + w * w) || 1; x /= l; y /= l; z /= l; w /= l;
    var s = nc === 8 ? v[i0 + 7] + (v[i1 + 7] - v[i0 + 7]) * a : 1;
    // D3DXMatrixRotationQuaternion (row-vector convention), times the uniform scale
    out[o] = (1 - 2 * (y * y + z * z)) * s; out[o + 1] = 2 * (x * y + z * w) * s; out[o + 2] = 2 * (x * z - y * w) * s; out[o + 3] = 0;
    out[o + 4] = 2 * (x * y - z * w) * s; out[o + 5] = (1 - 2 * (x * x + z * z)) * s; out[o + 6] = 2 * (y * z + x * w) * s; out[o + 7] = 0;
    out[o + 8] = 2 * (x * z + y * w) * s; out[o + 9] = 2 * (y * z - x * w) * s; out[o + 10] = (1 - 2 * (x * x + y * y)) * s; out[o + 11] = 0;
    out[o + 12] = v[i0 + 4] + (v[i1 + 4] - v[i0 + 4]) * a; out[o + 13] = v[i0 + 5] + (v[i1 + 5] - v[i0 + 5]) * a;
    out[o + 14] = v[i0 + 6] + (v[i1 + 6] - v[i0 + 6]) * a; out[o + 15] = 1;
  }
  return out;
}
// row-major product of row-vector matrices: out = a * b  (apply a, then b)
function rmul(a, ao, b, bo, out, oo) {
  for (var r = 0; r < 4; r++) for (var c = 0; c < 4; c++)
    out[oo + r * 4 + c] = a[ao + r * 4] * b[bo + c] + a[ao + r * 4 + 1] * b[bo + 4 + c] + a[ao + r * 4 + 2] * b[bo + 8 + c] + a[ao + r * 4 + 3] * b[bo + 12 + c];
  return out;
}
function rmulAll(m, nb, g, out) { out = out && out.length >= nb * 16 ? out : new Float32Array(nb * 16); for (var j = 0; j < nb; j++) rmul(m, j * 16, g, 0, out, j * 16); return out; }
var IDENT = new Float32Array([1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]);

/* dynamic (re-skinned every frame) meshes: m/ bodies (meta.s = 1, uint8 bones) and n/ models (meta.s = 2, uint16) */
Renderer.prototype.dyn = function (name, meta, buf) {
  var ck = 'dyn|' + name;
  if (this.meshes[ck]) return this.meshes[ck];
  var gl = this.gl, n = meta.n, nt = meta.o + meta.a, k, v;
  var qa = new Int16Array(buf, 0, n * 5), Q = meta.q, pos = new Float32Array(n * 3), uv = new Float32Array(n * 2), i0 = (n * 10 + 3) & ~3;
  for (v = 0; v < n; v++) {
    for (k = 0; k < 3; k++) pos[v * 3 + k] = Q[k] + (qa[v * 3 + k] + 32768) * Q[5 + k];
    for (k = 0; k < 2; k++) uv[v * 2 + k] = Q[3 + k] + (qa[n * 3 + v * 2 + k] + 32768) * Q[8 + k];
  }
  var idx = meta.i === 4 ? new Uint32Array(buf, i0, nt * 3) : new Uint16Array(buf, i0, nt * 3);
  var B0 = new Int32Array(n), B1 = new Int32Array(n), W = new Float32Array(n), so = i0 + nt * 3 * meta.i;
  if (meta.s === 2) {
    var sk = new Uint16Array(buf, (so + 3) & ~3, n * 3);
    for (v = 0; v < n; v++) { B0[v] = sk[v * 3] === 65535 ? -1 : sk[v * 3]; B1[v] = sk[v * 3 + 1] === 65535 ? -1 : sk[v * 3 + 1]; W[v] = sk[v * 3 + 2] / 65535; }
  } else if (meta.s) {
    var s8 = new Uint8Array(buf, so, n * 3);
    for (v = 0; v < n; v++) { B0[v] = s8[v * 3] === 255 ? -1 : s8[v * 3]; B1[v] = s8[v * 3 + 1] === 255 ? -1 : s8[v * 3 + 1]; W[v] = s8[v * 3 + 2] / 255; }
  } else { for (v = 0; v < n; v++) { B0[v] = B1[v] = -1; W[v] = 1; } }
  function vbo(d, dyn) { var o = gl.createBuffer(); gl.bindBuffer(gl.ARRAY_BUFFER, o); gl.bufferData(gl.ARRAY_BUFFER, d, dyn ? gl.DYNAMIC_DRAW : gl.STATIC_DRAW); return o; }
  var ib = gl.createBuffer(); gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, ib);
  var big = meta.i === 4;
  if (big && !this.u32) { idx = new Uint16Array(idx); big = false; }
  gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, idx, gl.STATIC_DRAW);
  var m = { p: vbo(pos, 1), t: vbo(uv), n: vbo(new Float32Array(n * 3), 1), i: ib, it: big ? gl.UNSIGNED_INT : gl.UNSIGNED_SHORT, ib: big ? 4 : 2,
    o: meta.o, al: meta.a, h: meta.h || {}, b: meta.b.slice(), dyn: true, base: pos, B0: B0, B1: B1, W: W, idx: idx, nt: nt, N: n,
    cur: new Float32Array(n * 3), nrm: new Float32Array(n * 3), meta: meta };
  this.meshes[ck] = m;
  this.pose(m, null);
  return m;
};
Renderer.prototype.pose = function (m, mats) {
  var gl = this.gl, n = m.N, P = m.base, out = m.cur, B0 = m.B0, B1 = m.B1, W = m.W, nm = mats ? mats.length / 16 : 0;
  var mn0 = 1e9, mn1 = 1e9, mn2 = 1e9, mx0 = -1e9, mx1 = -1e9, mx2 = -1e9, i, o;
  for (i = 0; i < n; i++) {
    var x = P[i * 3], y = P[i * 3 + 1], z = P[i * 3 + 2], b0 = B0[i], b1 = B1[i], w0 = W[i], w1 = 1 - w0, ax = 0, ay = 0, az = 0, tw = 0;
    if (mats && b0 >= 0 && b0 < nm && w0 > 0) { o = b0 * 16; ax += w0 * (x * mats[o] + y * mats[o + 4] + z * mats[o + 8] + mats[o + 12]); ay += w0 * (x * mats[o + 1] + y * mats[o + 5] + z * mats[o + 9] + mats[o + 13]); az += w0 * (x * mats[o + 2] + y * mats[o + 6] + z * mats[o + 10] + mats[o + 14]); tw += w0; }
    if (mats && b1 >= 0 && b1 < nm && w1 > 0) { o = b1 * 16; ax += w1 * (x * mats[o] + y * mats[o + 4] + z * mats[o + 8] + mats[o + 12]); ay += w1 * (x * mats[o + 1] + y * mats[o + 5] + z * mats[o + 9] + mats[o + 13]); az += w1 * (x * mats[o + 2] + y * mats[o + 6] + z * mats[o + 10] + mats[o + 14]); tw += w1; }
    if (tw > 0) { x = ax / tw; y = ay / tw; z = az / tw; }
    out[i * 3] = x; out[i * 3 + 1] = y; out[i * 3 + 2] = z;
    if (x < mn0) mn0 = x; if (y < mn1) mn1 = y; if (z < mn2) mn2 = z; if (x > mx0) mx0 = x; if (y > mx1) mx1 = y; if (z > mx2) mx2 = z;
  }
  var nr = m.nrm, idx = m.idx; nr.fill(0);
  for (var t = 0; t < m.nt; t++) {
    var a = idx[t * 3] * 3, b = idx[t * 3 + 1] * 3, c = idx[t * 3 + 2] * 3;
    var ux = out[b] - out[a], uy = out[b + 1] - out[a + 1], uz = out[b + 2] - out[a + 2];
    var vx = out[c] - out[a], vy = out[c + 1] - out[a + 1], vz = out[c + 2] - out[a + 2];
    var nx = uy * vz - uz * vy, ny = uz * vx - ux * vz, nz = ux * vy - uy * vx;
    nr[a] += nx; nr[a + 1] += ny; nr[a + 2] += nz; nr[b] += nx; nr[b + 1] += ny; nr[b + 2] += nz; nr[c] += nx; nr[c + 1] += ny; nr[c + 2] += nz;
  }
  gl.bindBuffer(gl.ARRAY_BUFFER, m.p); gl.bufferSubData(gl.ARRAY_BUFFER, 0, out);
  gl.bindBuffer(gl.ARRAY_BUFFER, m.n); gl.bufferSubData(gl.ARRAY_BUFFER, 0, nr);
  m.b = [mn0, mn1, mn2, mx0, mx1, mx2];
};

/* rig = a set of n/ meshes animated together: motion part i belongs to the mesh part named refNames[i]
   (the engine pairs MOTI and PHY chunks by index in the reference mesh, the other meshes of the row - saddles,
   extra parts - have their own part order, so they are matched by name) */
function rigMats(R, objs, A, f, gi, cache) {
  var refNames = objs.length && objs[0].mesh.meta && objs[0].mesh.meta.pn || [];
  cache.parts = cache.parts || {};
  objs.forEach(function (o) {
    var m = o.mesh, meta = m.meta;
    if (!m.dyn || !meta || !meta.pb) return;
    var mats = o.mats && o.mats.length >= meta.nb * 16 ? o.mats : (o.mats = new Float32Array(Math.max(1, meta.nb) * 16));
    for (var i = 0; i < meta.pn.length; i++) {
      var mi = refNames.indexOf(meta.pn[i]); if (mi < 0) mi = i;
      var P = A && A.parts[mi], nb = (i + 1 < meta.pb.length ? meta.pb[i + 1] : meta.nb) - meta.pb[i];
      if (!P || nb <= 0) continue;
      var key = mi, s = cache.parts[key] || (cache.parts[key] = samplePart(P, f, null));
      if (s.f !== f) { samplePart(P, f, s); s.f = f; }
      for (var j = 0; j < Math.min(nb, P.nb); j++) rmul(s, j * 16, gi || IDENT, 0, mats, (meta.pb[i] + j) * 16);
    }
    R.pose(m, mats);
  });
}
// attachment helper of an n/ mesh (meta.h[name] = [part, init]) at frame f: init * motion(part)[0] * gi
function rigHelper(objs, A, f, gi, name) {
  for (var k = 0; k < objs.length; k++) {
    var meta = objs[k].mesh.meta, h = meta && meta.h && meta.h[name];
    if (!h || !h.length || typeof h[0] !== 'number') continue;
    var refNames = objs[0].mesh.meta.pn || [], mi = refNames.indexOf(meta.pn[h[0]]);
    if (mi < 0) mi = h[0];
    var P = A && A.parts[mi], z = P ? samplePart(P, f, null) : IDENT, out = new Float32Array(16), t = new Float32Array(16);
    rmul(new Float32Array(h[1]), 0, z, 0, t, 0);
    return rmul(t, 0, gi || IDENT, 0, out, 0);
  }
  return null;
}

/* ---------------------------------------------------------------- NPC / monster model viewer (npcs.js, n/, a/) */
var MV = null;   // { el, r, cam, objs, model, A, t, playing, speed, act, gen }
function mvStop() { if (MV) { MV.dead = true; MV = null; } }
function mvCamera(c) {
  var cp = Math.cos(c.pitch);
  return { eye: [c.target[0] + c.dist * Math.sin(c.yaw) * cp, c.target[1] + c.dist * Math.sin(c.pitch), c.target[2] - c.dist * Math.cos(c.yaw) * cp],
    target: c.target, fov: c.fov, near: c.near, far: Math.max(c.far, c.dist * 4) };
}
function mvFit(V) {
  var mn = [1e9, 1e9, 1e9], mx = [-1e9, -1e9, -1e9];
  V.objs.forEach(function (o) {
    var b = o.mesh.b;
    for (var i = 0; i < 8; i++) {
      var p = M4.pt(o.model, [i & 1 ? b[3] : b[0], i & 2 ? b[4] : b[1], i & 4 ? b[5] : b[2]]);
      for (var k = 0; k < 3; k++) { mn[k] = Math.min(mn[k], p[k]); mx[k] = Math.max(mx[k], p[k]); }
    }
  });
  if (mn[0] > mx[0]) return;
  var r = Math.max(mx[0] - mn[0], mx[1] - mn[1], mx[2] - mn[2]) / 2 || 50;
  V.cam.target = [(mn[0] + mx[0]) / 2, (mn[1] + mx[1]) / 2, (mn[2] + mx[2]) / 2];
  V.cam.dist = r / Math.tan(V.cam.fov / 2) * 1.15 + r * 0.3; V.cam.near = Math.max(1, r * 0.02); V.cam.far = r * 40 + V.cam.dist * 4;
}
function mvLoadAct(V) {
  var m = V.model, ai = m.a[String(V.act)];
  if (ai == null) { var ks = Object.keys(m.a); ai = ks.length ? m.a[ks.indexOf('100') >= 0 ? '100' : ks[0]] : null; }
  var gen = ++V.gen;
  return loadAnim(ai).catch(function () { return null; }).then(function (A) { if (gen === V.gen && !V.dead) { V.A = A; V.t = V.t0 || 0; V.t0 = 0; mvStep(V, 0); } });
}
function mvStep(V, dt) {
  if (!V.objs.length) return;
  if (V.A) {
    if (V.playing && dt) V.t = (V.t + dt * 0.03 * V.speed) % V.A.frames;
    rigMats(V.r, V.objs, V.A, V.t, V.gi, V.cache);
  }
  V.r.draw(V.objs, mvCamera(V.cam), BGS[themeBg()]);
}
function viewModel(el, key, opt) {
  mvStop();
  opt = opt || {};
  if (!NPCS || !NPCS.models[key]) { el.innerHTML = '<div class="empty">' + esc(tt('noModel')) + '</div>'; return; }
  var model = NPCS.models[key];
  if (!document.getElementById('dressCss')) { var st = document.createElement('style'); st.id = 'dressCss'; st.textContent = CSS; document.head.appendChild(st); }
  var acts = Object.keys(model.a).map(Number).sort(function (a, b) { return a - b; });
  el.innerHTML = '<div class="mv"><canvas class="mvc"></canvas><div class="mvbar">' +
    (acts.length ? '<select class="mvact">' + acts.map(function (a) { return '<option value="' + a + '"' + (a === 100 ? ' selected' : '') + '>' + esc(actionName(a)) + '</option>'; }).join('') + '</select>' +
      '<button class="drb mvplay">⏸</button>' : '<span class="mvnote">' + esc(tt('noAnim')) + '</span>') +
    '<button class="drb mvbg" title="PNG">⤓</button></div></div>';
  var cv = el.querySelector('canvas'), r;
  try { r = new Renderer(cv); } catch (e) { el.innerHTML = '<div class="empty">' + esc(tt('noGl')) + '</div>'; return; }
  var V = MV = { el: el, r: r, cam: { yaw: 0.5, pitch: 0.15, dist: 300, target: [0, 80, 0], fov: 0.7, near: 2, far: 5000 },
    objs: [], model: model, A: null, t: 0, playing: true, speed: 1, act: acts.indexOf(100) >= 0 ? 100 : acts[0], gen: 0, cache: {},
    gi: model.gi ? new Float32Array(model.gi) : null };
  var drag = null;
  cv.addEventListener('pointerdown', function (e) { drag = { x: e.clientX, y: e.clientY }; cv.setPointerCapture(e.pointerId); });
  cv.addEventListener('pointermove', function (e) { if (!drag) return; V.cam.yaw -= (e.clientX - drag.x) * 0.01; V.cam.pitch = Math.max(-1.45, Math.min(1.45, V.cam.pitch + (e.clientY - drag.y) * 0.01)); drag = { x: e.clientX, y: e.clientY }; });
  cv.addEventListener('pointerup', function () { drag = null; });
  cv.addEventListener('wheel', function (e) { e.preventDefault(); V.cam.dist = Math.max(10, Math.min(5000, V.cam.dist * Math.pow(1.0015, e.deltaY))); }, { passive: false });
  cv.addEventListener('dblclick', function () { mvFit(V); });
  var sel = el.querySelector('.mvact'), pl = el.querySelector('.mvplay');
  if (sel) sel.addEventListener('change', function () { V.act = +sel.value; mvLoadAct(V); });
  if (pl) pl.addEventListener('click', function () { V.playing = !V.playing; pl.textContent = V.playing ? '⏸' : '▶'; });
  el.querySelector('.mvbg').addEventListener('click', function () {
    mvStep(V, 0); var a = document.createElement('a'); a.href = cv.toDataURL('image/png'); a.download = 'model_' + key + '.png'; document.body.appendChild(a); a.click(); a.remove();
  });
  Promise.all(model.p.map(function (p) {
    var mn = NPCS.meshes[p[0]], tn = p[1] >= 0 ? NPCS.textures[p[1]] : null;
    return Promise.all([asset('n', mn), tn ? asset('t', tn).catch(function () { return null; }) : null]).then(function (x) {
      return { mesh: r.dyn(mn, x[0].meta, x[0].buf), tex: x[1] ? r.tex(tn, x[1].meta, x[1].buf) : null, model: CONV };
    });
  })).then(function (objs) {
    if (V.dead) return;
    V.objs = objs;
    // first frame of the first (stand) motion, then fit the camera on it
    return mvLoadAct(V).then(function () { mvStep(V, 0); mvFit(V); if (opt.yaw != null) V.cam.yaw = opt.yaw; mvStep(V, 0); });
  }).catch(function (e) { if (!V.dead) el.querySelector('.mvbar').insertAdjacentHTML('beforeend', '<span class="mvnote">' + esc(e.message) + '</span>'); });
  if (opt.frame != null) { V.playing = false; if (pl) pl.textContent = '▶'; V.t0 = opt.frame; }
  var last = performance.now();
  (function tick(t) {
    if (V.dead || !document.body.contains(cv)) { if (MV === V) MV = null; return; }
    var dt = Math.min(100, t - last); last = t;
    if (V.objs.length) mvStep(V, dt);
    requestAnimationFrame(tick);
  })(last);
}


/* ---------------------------------------------------------------- stage 3: effects (fx.js, p/, wings, item glows) */
var FX = window.ASSET3D_FX || null;
var PCACHE = {};
function loadParticles(i) {
  var name = FX && FX.particles[i]; if (!name) return Promise.resolve(null);
  if (PCACHE[name]) return PCACHE[name];
  var pr = asset('p', name).then(function (r) { return decodeParticles(r.meta, r.buf); });
  PCACHE[name] = pr; pr.catch(function () { delete PCACHE[name]; });
  return pr;
}
function decodeParticles(meta, buf) {
  var off = 0, out = [];
  meta.s.forEach(function (s) {
    var counts = s[1], nf = counts.length, tot = 0, fo = [];
    counts.forEach(function (c) { fo.push(tot); tot += c * 5; });
    var mats = new Float32Array(buf.slice(off, off + nf * 64)); off += nf * 64;
    var vals = new Int16Array(buf.slice(off, off + tot * 2)); off += tot * 2 + (tot % 2 ? 2 : 0);
    out.push({ row: s[0], counts: counts, pmin: [s[2], s[3], s[4]], st: [s[5], s[6], s[7]], smax: s[8], mats: mats, vals: vals, fo: fo });
  });
  return out;
}
// effect name -> Promise of an instance template { def, parts: [{ mesh | sys, tex, add, s }] }
var FXC = {};
function loadFx(name, R) {
  var key = name + '|' + (R === D.r ? 'd' : 'v');
  if (FXC[key]) return FXC[key];
  var def = FX && FX.defs[name];
  if (!def || !NPCS) return Promise.resolve(null);
  var pr = Promise.all(def.p.map(function (p) {
    var tn = p.t >= 0 ? NPCS.textures[p.t] : null;
    var tp = tn ? asset('t', tn).then(function (r) { return R.tex(tn, r.meta, r.buf); }, function () { return null; }) : Promise.resolve(null);
    var add = p.b;   // D3DBLEND source / destination factors
    if (p.m != null) {
      var mn = NPCS.meshes[p.m];
      return Promise.all([asset('n', mn), tp, p.a >= 0 ? loadAnim(p.a).catch(function () { return null; }) : null]).then(function (x) {
        return { mesh: R.dyn(mn, x[0].meta, x[0].buf), tex: x[1], A: x[2], add: add, s: p.s || 1, cache: {} };
      }, function () { return null; });
    }
    return Promise.all([loadParticles(p.p), tp]).then(function (x) { return x[0] ? { sys: x[0], tex: x[1], add: add, s: p.s || 1 } : null; }, function () { return null; });
  })).then(function (parts) { return { name: name, def: def, parts: parts.filter(Boolean) }; });
  FXC[key] = pr;
  return pr;
}
// effects of the current outfit: [{ name, sc, at: 'root' | 'rw' | 'lw' | 'mount' }]
function fxWanted() {
  var s = D.state, out = [];
  if (!FX || !s.fx) return out;
  var b = s.b, M = FX.maps;
  function look(map, ids, at) {
    for (var i = 0; i < ids.length; i++) {
      var id = ids[i]; if (!id) continue;
      var e = map[b + '|' + id] || map['*|' + id] || map[b + '|' + (id - id % 10)] || map['*|' + (id - id % 10)];
      if (e) { e.forEach(function (x) { out.push({ name: x[0], sc: x[1], at: at }); }); return; }
    }
  }
  look(M.body, [s.garment, s.armor], 'root');
  look(M.weapon, [s.rw], 'rw');
  look(M.weapon, [s.lw], 'lw');
  var mt = s.mount && rowsFor('mount', s.mount, b);
  if (mt) look(M.mount, [+mt.key, +mt.key - (+mt.key % 10000)], 'mount');
  if (s.wing && FX.wings[s.wing]) {
    var W = FX.wings[s.wing], lv = W['0'] || W[Object.keys(W)[0]], nm = lv && (lv.on || lv.up);
    if (nm) out.push({ name: nm, sc: [1, 1, 1], at: 'back' });
  }
  return out;
}
// draw list entries for the effects at time t (ms); att = { root, rw, lw, mount } GL model matrices
function fxObjects(R, insts, t, att, cam) {
  var out = [];
  insts.forEach(function (I) {
    var base = att[I.at]; if (!base || !I.fx) return;
    var def = I.fx.def, fi = Math.max(1, def.fi || 33), o = def.o || [0, 0, 0];
    I.fx.parts.forEach(function (p) {
      var sc = p.s, m = M4.mul(base, M4.mul(M4.tr(o[0], o[1], o[2]), new Float32Array([I.sc[0] * sc, 0, 0, 0, 0, I.sc[1] * sc, 0, 0, 0, 0, I.sc[2] * sc, 0, 0, 0, 0, 1])));
      if (p.mesh) {
        if (p.A) rigMats(R, [{ mesh: p.mesh }], p.A, (t / fi) % p.A.frames, null, p.cache);
        out.push({ mesh: p.mesh, tex: p.tex, model: m, fx: 1, blend: p.add });
      } else if (p.sys) {
        p.sys.forEach(function (S, si) {
          var nf = S.counts.length, f = Math.floor(t / fi) % nf, n = S.counts[f];
          if (!n) return;
          out.push({ quads: particleQuads(S, f, m, cam, p.cacheQ || (p.cacheQ = {}), si), tex: p.tex, fx: 1, blend: p.add });
        });
      }
    });
  });
  return out;
}
function particleQuads(S, f, model, cam, cache, si) {
  var n = S.counts[f], v = S.vals, o = S.fo[f], mo = f * 16, M = S.mats, row = Math.max(1, S.row), cells = row * row;
  var key = si, q = cache[key];
  if (!q || q.cap < n) q = cache[key] = { cap: n, pos: new Float32Array(n * 18), uv: new Float32Array(n * 12) };
  var dx = cam.target[0] - cam.eye[0], dy = cam.target[1] - cam.eye[1], dz = cam.target[2] - cam.eye[2];
  var d = norm([dx, dy, dz]), rt = norm(cross(d, [0, 1, 0])), up = cross(rt, d);
  // the size is in C3 units; scale it like the model does (length of the model's x axis)
  var ms = Math.sqrt(model[0] * model[0] + model[1] * model[1] + model[2] * model[2]);
  for (var i = 0; i < n; i++) {
    var px = S.pmin[0] + (v[o + 3 * i] + 32768) * S.st[0], py = S.pmin[1] + (v[o + 3 * i + 1] + 32768) * S.st[1], pz = S.pmin[2] + (v[o + 3 * i + 2] + 32768) * S.st[2];
    var age = v[o + 3 * n + i] / 30000, size = v[o + 4 * n + i] / 32767 * S.smax * ms * 0.5;
    var x = px * M[mo] + py * M[mo + 4] + pz * M[mo + 8] + M[mo + 12], y = px * M[mo + 1] + py * M[mo + 5] + pz * M[mo + 9] + M[mo + 13], z = px * M[mo + 2] + py * M[mo + 6] + pz * M[mo + 10] + M[mo + 14];
    var c = M4.pt(model, [x, y, z]);
    var cell = Math.min(cells - 1, Math.max(0, Math.floor(age * cells))), cu = (cell % row) / row, cv = Math.floor(cell / row) / row, cw = 1 / row;
    var corners = [[-1, -1, cu, cv + cw], [1, -1, cu + cw, cv + cw], [1, 1, cu + cw, cv], [-1, -1, cu, cv + cw], [1, 1, cu + cw, cv], [-1, 1, cu, cv]];
    for (var k = 0; k < 6; k++) {
      var a = corners[k], b = (i * 6 + k);
      q.pos[b * 3] = c[0] + (rt[0] * a[0] + up[0] * a[1]) * size; q.pos[b * 3 + 1] = c[1] + (rt[1] * a[0] + up[1] * a[1]) * size; q.pos[b * 3 + 2] = c[2] + (rt[2] * a[0] + up[2] * a[1]) * size;
      q.uv[b * 2] = a[2]; q.uv[b * 2 + 1] = a[3];
    }
  }
  q.n = n;
  return q;
}
Renderer.prototype.drawFx = function (list, vp) {
  if (!list || !list.length) return;
  var gl = this.gl, a = this.a, u = this.u, self = this;
  gl.enable(gl.BLEND); gl.depthMask(false); gl.uniform1f(u.uPass, 2);
  if (!this.qb) { this.qb = gl.createBuffer(); this.qt = gl.createBuffer(); }
  list.forEach(function (o) {
    // D3DBLEND -> GL (the canvas has no alpha channel: destination alpha is 1)
    var B = { 1: gl.ZERO, 2: gl.ONE, 3: gl.SRC_COLOR, 4: gl.ONE_MINUS_SRC_COLOR, 5: gl.SRC_ALPHA, 6: gl.ONE_MINUS_SRC_ALPHA, 7: gl.ONE, 8: gl.ZERO, 9: gl.DST_COLOR, 10: gl.ONE_MINUS_DST_COLOR };
    var bl = o.blend || [5, 6];
    gl.blendFunc(B[bl[0]] || gl.SRC_ALPHA, B[bl[1]] || gl.ONE_MINUS_SRC_ALPHA);
    gl.activeTexture(gl.TEXTURE0); gl.bindTexture(gl.TEXTURE_2D, o.tex || null); gl.uniform1f(u.uHas, o.tex ? 1 : 0);
    if (o.quads) {
      var q = o.quads;
      gl.uniformMatrix4fv(u.uM, false, IDENT);
      gl.bindBuffer(gl.ARRAY_BUFFER, self.qb); gl.bufferData(gl.ARRAY_BUFFER, q.pos.subarray(0, q.n * 18), gl.DYNAMIC_DRAW);
      gl.enableVertexAttribArray(a.P); gl.vertexAttribPointer(a.P, 3, gl.FLOAT, false, 0, 0);
      gl.bindBuffer(gl.ARRAY_BUFFER, self.qt); gl.bufferData(gl.ARRAY_BUFFER, q.uv.subarray(0, q.n * 12), gl.DYNAMIC_DRAW);
      gl.enableVertexAttribArray(a.T); gl.vertexAttribPointer(a.T, 2, gl.FLOAT, false, 0, 0);
      if (a.N >= 0) { gl.disableVertexAttribArray(a.N); gl.vertexAttrib3f(a.N, 0, 1, 0); }
      gl.drawArrays(gl.TRIANGLES, 0, q.n * 6);
      if (a.N >= 0) gl.enableVertexAttribArray(a.N);
      return;
    }
    var m = o.mesh;
    gl.bindBuffer(gl.ARRAY_BUFFER, m.p); gl.enableVertexAttribArray(a.P); gl.vertexAttribPointer(a.P, 3, gl.FLOAT, false, 0, 0);
    gl.bindBuffer(gl.ARRAY_BUFFER, m.t); gl.enableVertexAttribArray(a.T); gl.vertexAttribPointer(a.T, 2, gl.FLOAT, false, 0, 0);
    if (a.N >= 0) { gl.bindBuffer(gl.ARRAY_BUFFER, m.n); gl.enableVertexAttribArray(a.N); gl.vertexAttribPointer(a.N, 3, gl.FLOAT, false, 0, 0); }
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, m.i);
    gl.uniformMatrix4fv(u.uM, false, o.model);
    gl.drawElements(gl.TRIANGLES, (m.o + m.al) * 3, m.it, 0);
  });
  gl.depthMask(true); gl.disable(gl.BLEND); gl.uniform1f(u.uPass, 0);
};

/* ---------------------------------------------------------------- dresser */
var TX = {
  ar: { title: 'غرفة التجهيز', body: 'الجسم', fs: 'بنت صغيرة', fl: 'بنت كبيرة', ms: 'ولد صغير', ml: 'ولد كبير', hair: 'الشعر',
    garment: 'جارمنت', armor: 'درع', head: 'خوذة / غطا راس', rw: 'سلاح يمين', lw: 'سلاح شمال', mount: 'خيل',
    none: '— ولا حاجة —', pick: 'اختار…', auto: 'لف تلقائي', bg: 'الخلفية', shot: 'صورة PNG', copy: 'انسخ الأرقام', reset: 'رجّع الكاميرا',
    search: 'دوّر بالاسم أو الـ ID…', unique: 'شكل واحد بس من كل موديل', close: 'قفل', loading: 'بيحمّل…', missing: 'مش لاقي: ',
    noIndex: 'مفيش داتا 3D — شغّل run.bat update (أو python extract3d.py) الأول.', noGl: 'المتصفح ده مفيهوش WebGL.',
    hint: 'اسحب بالماوس عشان تلف • العجلة للزووم • دبل كليك يرجّع الكاميرا', more: 'كمّل الكتابة عشان تشوف الباقي', models: 'موديل',
    rider: 'الراكب على الخيل', notFor: 'مالوش موديل للجسم ده', items: 'أيتم', pose: 'الوقفة', stand: 'واقف', poseNote: 'الوقفة ثابتة (من غير أنيميشن).', noStand: 'الجسم ده ملوش وقفة "واقف" في ملفاته، فبيتعرض T-pose.',
    animP: 'متحرك', act: 'الحركة', stance: 'وقفة السلاح', stAuto: 'تلقائي (من السلاح)', bare: 'إيد فاضية', play: 'تشغيل', pause: 'إيقاف', speed: 'السرعة', frame: 'الفريم',
    gBasic: 'أساسي', gEmote: 'تعبيرات ورقص', gFight: 'قتال', gSkill: 'مهارات وتعاويذ', animNote: 'الأنيميشن من ملفات الكلاينت نفسها (c3/<جسم>/<سلاح>/<حركة>.c3). وقفة الرجلين في المشي والجري زي ما هي في الملفات.',
    dye: 'الصبغة (درع / غطا راس)', dyeDef: 'اللون الأصلي',
    wing: 'جناح', fxOn: 'المؤثرات (أجنحة / لمعان / هالات)', wingType: 'نوع ', fxNote: 'المؤثرات تقريبية: الموديلات المتحركة والجزيئات من ملفات الكلاينت، الشرائط (SHAP) مش مترسومة.',
    noModel: 'مفيش موديل 3D للشكل ده.', hairHead: 'النوع ده من غطا الراس الكلاينت مبيرسملوش موديل، بيفضل الشعر هو اللي باين (زي اللعبة بالظبط).', noAnim: 'مفيش أنيميشن', riding: 'وقفة الركوب' },
  en: { title: 'Dressing Room', body: 'Body', fs: 'Female small', fl: 'Female large', ms: 'Male small', ml: 'Male large', hair: 'Hair',
    garment: 'Garment', armor: 'Armor', head: 'Headgear', rw: 'Right weapon', lw: 'Left weapon', mount: 'Mount',
    none: '— none —', pick: 'Pick…', auto: 'Auto-rotate', bg: 'Background', shot: 'Screenshot PNG', copy: 'Copy ids', reset: 'Reset view',
    search: 'Search name or id…', unique: 'One entry per model', close: 'Close', loading: 'Loading…', missing: 'Missing: ',
    noIndex: 'No 3D data - run run.bat update (or python extract3d.py) first.', noGl: 'This browser has no WebGL.',
    hint: 'Drag to rotate • wheel to zoom • double-click resets the camera', more: 'type more to narrow the list', models: 'models',
    rider: 'Rider on the mount', notFor: 'has no model for this body', items: 'items', pose: 'Pose', stand: 'Standing', poseNote: 'Static pose (no animation).', noStand: 'No standing frame was found for this body, it is shown in T-pose.',
    animP: 'Animated', act: 'Action', stance: 'Weapon stance', stAuto: 'Auto (from the weapon)', bare: 'Bare hands', play: 'Play', pause: 'Pause', speed: 'Speed', frame: 'Frame',
    gBasic: 'Basic', gEmote: 'Emotes & dances', gFight: 'Combat', gSkill: 'Skills & casting', animNote: 'Animations come from the client files (c3/<body>/<weapon>/<action>.c3). Walk/run keep the leg stance stored in those files.',
    dye: 'Dye (armor / headgear)', dyeDef: 'Original colour',
    wing: 'Wings', fxOn: 'Effects (wings / glow / auras)', wingType: 'type ', fxNote: 'Effects are approximate: animated meshes and particles come from the client files, ribbon shapes (SHAP) are not drawn.',
    noModel: 'No 3D model for this look.', hairHead: 'The client draws no model for this headgear type: the hair stays visible (same as in game).', noAnim: 'no animation', riding: 'riding pose' }
};
var BODYKEYS = { 1: 'fs', 2: 'fl', 3: 'ms', 4: 'ml' };
var BGS = [[0.09, 0.10, 0.13], [0.55, 0.58, 0.63], [0.93, 0.94, 0.96], [0.0, 0.69, 0.25], [0.82, 0.85, 0.89]];
function themeBg() { return document.documentElement.getAttribute('data-theme') === 'dark' ? 0 : 2; }
var SLOTS = ['garment', 'armor', 'head', 'rw', 'lw', 'mount'];
var CSS = '.dress{display:grid;grid-template-columns:300px minmax(0,1fr);min-height:0;min-width:0;height:100%}' +
  '.drp{background:var(--bg2);border-inline-end:1px solid var(--line);overflow:auto;padding:12px 12px 20px}' +
  '.drp h4{margin:14px 2px 6px;font-size:11px;letter-spacing:.8px;text-transform:uppercase;color:var(--muted);font-weight:600}' +
  '.bods{display:grid;grid-template-columns:1fr 1fr;gap:6px}.bods button,.drb{background:var(--panel);border:1px solid var(--line);border-radius:8px;padding:7px 8px;cursor:pointer;color:var(--text)}' +
  '.bods button.on{border-color:var(--accent);color:var(--accent2);box-shadow:inset 0 0 0 1px var(--accent)}.drb:hover,.bods button:hover{border-color:var(--hov,#3a4256)}' +
  '.slot{display:flex;align-items:center;gap:8px;background:var(--panel);border:1px solid var(--line);border-radius:9px;padding:6px;margin:6px 0;cursor:pointer}' +
  '.slot:hover{border-color:var(--hov,#3a4256)}.slot .si{width:40px;height:40px;flex:none;border-radius:7px;background:radial-gradient(circle at 50% 40%,var(--icb1,#283044),var(--icb2,#171b24) 70%);display:grid;place-items:center;overflow:hidden}' +
  '.slot .si img{max-width:40px;max-height:40px}.slot .sn{flex:1;min-width:0}.slot .sn b{display:block;font-size:11px;color:var(--muted);font-weight:600}' +
  '.slot .sn span{display:block;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}.slot .sn small{color:var(--muted);font-family:Consolas,monospace}' +
  '.slot .sx{background:none;border:0;color:var(--muted);font-size:18px;cursor:pointer;padding:0 6px}.slot .sx:hover{color:var(--bad)}.slot.err{border-color:var(--bad)}' +
  '.drtools{display:grid;grid-template-columns:1fr 1fr;gap:6px;margin-top:10px}.drtools .on{border-color:var(--accent);color:var(--accent2)}' +
  '.drp select{width:100%;background:var(--panel);border:1px solid var(--line);border-radius:8px;padding:6px}' +
  '.drstat{margin-top:10px;font-size:12px;color:var(--muted);min-height:18px;word-break:break-word}' +
  '.drc{position:relative;min-width:0;min-height:0}.drc canvas{width:100%;height:100%;display:block;cursor:grab;touch-action:none}.drc canvas:active{cursor:grabbing}' +
  '.drhint{position:absolute;bottom:8px;inset-inline-start:10px;font-size:12px;color:var(--muted);pointer-events:none;background:var(--panel);opacity:.85;padding:2px 8px;border-radius:6px}' +
  '.drmsg{position:absolute;inset:0;display:grid;place-items:center;color:var(--muted);text-align:center;padding:30px;pointer-events:none}' +
  '.drpick{position:absolute;inset:10px;max-width:520px;background:var(--panel);border:1px solid var(--line);border-radius:12px;box-shadow:var(--shadow);display:flex;flex-direction:column;z-index:5}' +
  '.drpick .ph{display:flex;gap:8px;align-items:center;padding:10px;border-bottom:1px solid var(--line)}.drpick .ph input[type=text]{flex:1;background:var(--bg2);border:1px solid var(--line);border-radius:8px;padding:7px 9px;outline:0;min-width:0}' +
  '.drpick .pl{overflow:auto;flex:1;padding:6px}.drpick .po{display:flex;align-items:center;gap:8px;padding:5px 8px;border-radius:8px;cursor:pointer}' +
  '.drpick .po:hover,.drpick .po.on{background:var(--panel2)}.drpick .po img,.drpick .po .ni{width:36px;height:36px;object-fit:contain;flex:none}.drpick .po .ni{display:grid;place-items:center;color:var(--icnone,#4b5468)}' +
  '.drpick .po span{flex:1;min-width:0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap}.drpick .po small{color:var(--muted);font-family:Consolas,monospace}' +
  '.drpick .pf{padding:6px 10px;border-top:1px solid var(--line);font-size:12px;color:var(--muted);display:flex;gap:10px;align-items:center}' +
  '.anirow{display:flex;gap:6px;align-items:center;margin-top:6px}.anirow input[type=range]{flex:1;min-width:0}.anirow select{width:auto;flex:1}' +
  '.mv{position:relative;height:360px;border:1px solid var(--line);border-radius:10px;overflow:hidden;background:var(--bg2)}.mvc{width:100%;height:100%;display:block;cursor:grab;touch-action:none}' +
  '.mvbar{position:absolute;left:8px;right:8px;bottom:8px;display:flex;gap:6px;align-items:center}.mvbar select{flex:1;min-width:0;background:var(--panel);border:1px solid var(--line);border-radius:8px;padding:5px}' +
  '.mvnote{font-size:12px;color:var(--muted);background:var(--panel);padding:2px 8px;border-radius:6px}' +
  '@media (max-width:820px){.dress{grid-template-columns:minmax(0,1fr);grid-template-rows:auto minmax(320px,1fr)}.drp{max-height:46vh}}';

var D = {
  ready: false, lang: 'ar', items: null, byId: {}, state: { b: 3, hair: 0, garment: 0, armor: 0, head: 0, rw: 0, lw: 0, mount: 0, rider: 1, p: ANIM ? 2 : 1, act: 100, wt: '', wing: '', fx: 1, dA: 0, dH: 0 },
  playing: true, speed: 1, anim: null, anim0: null,
  cam: { yaw: 0, pitch: 0.12, dist: 300, target: [0, 90, 0], fov: 0.75, near: 2, far: 5000 }, auto: false, bg: themeBg(),
  objs: [], gen: 0, onChange: null, root: null
};
function tt(k) { return (TX[D.lang] || TX.en)[k] || k; }
function esc(s) { return String(s == null ? '' : s).replace(/[&<>"]/g, function (c) { return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]; }); }

/* ---- item -> 3D rows */
function rowsFor(slot, id, b) {
  if (!IDX || !id) return null;
  var base = id - id % 10, k;
  if (slot === 'garment' || slot === 'armor') { k = String(b * 1000000 + base); return IDX.armor[k] ? { key: k, parts: IDX.armor[k] } : null; }
  if (slot === 'head') {
    // client code (Conquer.exe, armet look switch): these headgear families never load a mesh, the head slot keeps
    // the character's hair (body*1000000 + 119000 + hair); every other family uses body*1000000 + id - id%10
    if (HAIR_HEAD[Math.floor(id / 1000) % 1000]) { var hr = rowsFor('hair', D.state.hair, b); return hr ? { key: hr.key, parts: hr.parts, hairOnly: 1 } : null; }
    k = String(b * 1000000 + base); return IDX.armet[k] ? { key: k, parts: IDX.armet[k] } : null;
  }
  if (slot === 'hair') { k = String(b * 1000000 + 119000 + id); return IDX.armet[k] ? { key: k, parts: IDX.armet[k] } : null; }
  if (slot === 'rw' || slot === 'lw') {
    if (IDX.weapon[id]) return { key: String(id), parts: IDX.weapon[id] };
    if (IDX.weapon[base]) return { key: String(base), parts: IDX.weapon[base] };
    return null;
  }
  if (slot === 'mount') { var m = MOUNTS[id]; return m && IDX.mount[m[2]] ? { key: String(m[2]), parts: IDX.mount[m[2]], m: m } : null; }
  return null;
}
// dye colours: the texture's hundreds digit 3..9 (manifest IDX.dye: texture index -> indices for the colours 3..9)
var DYES = { 3: ['#e08a2e', 'Orange', 'برتقالي'], 4: ['#7cc4e8', 'Light blue', 'لبني'], 5: ['#d23c3c', 'Red', 'أحمر'], 6: ['#3a64d0', 'Blue', 'أزرق'],
  7: ['#e8cf3a', 'Yellow', 'أصفر'], 8: ['#8a4cc8', 'Purple', 'بنفسجي'], 9: ['#f2f2f2', 'White', 'أبيض'] };
function dyeRow(row, c) {
  if (!row || !c || !IDX.dye) return row;
  return { key: row.key, hairOnly: row.hairOnly, parts: row.parts.map(function (p) { var d = IDX.dye[p[1]]; return d && d[c - 3] >= 0 ? [p[0], d[c - 3]] : p; }) };
}
var HAIR_HEAD = { 117: 1, 118: 1, 123: 1, 141: 1, 142: 1, 143: 1, 145: 1, 148: 1, 177: 1, 178: 1 };
var MOUNTS = {};   // pseudo id -> mounttype row [item, variant, dbcKey, title, offset]; pseudo id = item for variant 0, item*100+variant otherwise
function mountId(m) { return m[1] ? m[0] * 100 + m[1] : m[0]; }
function slotOf(x) {
  if (!x) return null;
  if (x.cat === 'garment') return 'garment';
  if (x.cat === 'armor') return 'armor';
  if (x.cat === 'headgear') return 'head';
  if (x.cat === 'weapon') return x.sub === 'Arrow' ? null : (x.sub === 'Shield' ? 'lw' : 'rw');
  if (x.cat === 'mount') return 'mount';
  return null;
}
function canTry(x) {
  var s = slotOf(x); if (!s || !IDX) return false;
  if (s === 'mount') return !!rowsFor('mount', x.id, 1);
  for (var b = 1; b <= 4; b++) if (rowsFor(s, x.id, b)) return true;
  return false;
}

/* ---- candidates for a slot (stage-1 items that resolve for the body) */
function candidates(slot) {
  var b = D.state.b, out = [];
  if (slot === 'hair') {
    (IDX.bodies[b].hair || []).forEach(function (h) { out.push({ id: h, name: tt('hair') + ' ' + h, icon: null, look: JSON.stringify(rowsFor('hair', h, b).parts) }); });
    return out;
  }
  if (slot === 'mount') {
    IDX.mounts.forEach(function (m) {
      var it = D.byId[m[0]], pid = mountId(m), r = rowsFor('mount', pid, b);
      if (!r) return;
      out.push({ id: pid, item: m[0], name: (m[3] || (it && it.name) || ('#' + m[0])) + (m[1] ? ' (' + m[1] + ')' : ''), icon: it && it.icon, look: JSON.stringify(r.parts) });
    });
    return out;
  }
  var cat = { garment: 'garment', armor: 'armor', head: 'headgear', rw: 'weapon', lw: 'weapon' }[slot];
  D.items.forEach(function (x) {
    if (x.cat !== cat || (cat === 'weapon' && x.sub === 'Arrow')) return;
    var r = rowsFor(slot, x.id, b); if (!r) return;
    out.push({ id: x.id, name: x.name, icon: x.icon, sub: x.sub, look: JSON.stringify(r.parts) });
  });
  return out;
}

/* ---- UI */
function build(root) {
  if (!document.getElementById('dressCss')) { var st = document.createElement('style'); st.id = 'dressCss'; st.textContent = CSS; document.head.appendChild(st); }
  root.innerHTML = '<div class="dress"><div class="drp" id="drp"></div><div class="drc" id="drc"><canvas id="drcv"></canvas>' +
    '<div class="drhint" id="drhint"></div><div class="drmsg" id="drmsg"></div></div></div>';
  var cv = document.getElementById('drcv');
  try { D.r = new Renderer(cv); } catch (e) { D.r = null; D.glErr = e.message; }
  var drag = null;
  cv.addEventListener('pointerdown', function (e) { drag = { x: e.clientX, y: e.clientY }; cv.setPointerCapture(e.pointerId); });
  cv.addEventListener('pointermove', function (e) {
    if (!drag) return;
    D.cam.yaw -= (e.clientX - drag.x) * 0.01; D.cam.pitch = Math.max(-1.45, Math.min(1.45, D.cam.pitch + (e.clientY - drag.y) * 0.01));
    drag = { x: e.clientX, y: e.clientY }; redraw();
  });
  cv.addEventListener('pointerup', function () { drag = null; });
  cv.addEventListener('wheel', function (e) { e.preventDefault(); D.cam.dist = Math.max(20, Math.min(3000, D.cam.dist * Math.pow(1.0015, e.deltaY))); redraw(); }, { passive: false });
  cv.addEventListener('dblclick', function () { fit(true); redraw(); });
  root.addEventListener('click', onClick);
  root.addEventListener('input', onInput);
  root.addEventListener('change', onInput);
  window.addEventListener('resize', function () { if (D.visible) redraw(); });
}
function panel() {
  var s = D.state, h = '';
  h += '<h4>' + tt('body') + '</h4><div class="bods">' + [1, 2, 3, 4].map(function (b) {
    return '<button data-body="' + b + '" class="' + (s.b === b ? 'on' : '') + '">' + (b < 3 ? '♀ ' : '♂ ') + tt(BODYKEYS[b]) + ' <small style="color:var(--muted)">' + IDX.bodies[b].look + '</small></button>';
  }).join('') + '</div>';
  var hairs = IDX.bodies[s.b].hair || [];
  h += '<h4>' + tt('hair') + '</h4><select id="drhair">' + hairs.map(function (x) { return '<option value="' + x + '"' + (x === s.hair ? ' selected' : '') + '>' + x + '</option>'; }).join('') + '</select>';
  SLOTS.forEach(function (sl) {
    var id = s[sl], it = null, name = tt('none'), icon = null, err = false;
    if (id) {
      if (sl === 'mount') { var m = MOUNTS[id]; it = m && D.byId[m[0]]; name = m ? (m[3] || (it && it.name)) + (m[1] ? ' (' + m[1] + ')' : '') : '#' + id; }
      else { it = D.byId[id]; name = it ? it.name : '#' + id; }
      icon = it && it.icon; err = !rowsFor(sl, id, s.b);
    }
    h += '<h4>' + tt(sl) + '</h4><div class="slot' + (err ? ' err' : '') + '" data-slot="' + sl + '"><div class="si">' + (icon ? '<img src="' + esc(icon) + '" alt="">' : '<span style="color:var(--icnone,#4b5468)">' + (id ? '?' : '+') + '</span>') + '</div>' +
      '<div class="sn"><span>' + esc(name) + '</span>' + (id ? '<small>#' + (sl === 'mount' && MOUNTS[id] ? MOUNTS[id][0] : id) + (err ? ' · ' + tt('notFor') : '') + '</small>' : '<small>' + tt('pick') + '</small>') + '</div>' +
      (id ? '<button class="sx" data-clear="' + sl + '" title="×">×</button>' : '') + '</div>';
    if (sl === 'mount' && id) h += '<label class="chk" style="font-size:12.5px"><input type="checkbox" id="drrider"' + (s.rider ? ' checked' : '') + '> ' + tt('rider') + '</label>';
  });
  if (IDX.dye && (s.armor || s.head)) {
    var dsel = function (id, cur) {
      return '<select id="' + id + '"><option value="0">' + esc(tt('dyeDef')) + '</option>' + [3, 4, 5, 6, 7, 8, 9].map(function (c) {
        return '<option value="' + c + '"' + (cur === c ? ' selected' : '') + ' style="background:' + DYES[c][0] + ';color:#111">■ ' + c + ' · ' + esc(D.lang === 'ar' ? DYES[c][2] : DYES[c][1]) + '</option>';
      }).join('') + '</select>';
    };
    h += '<h4>' + tt('dye') + '</h4><div class="anirow">' + (s.armor && !s.garment ? dsel('drdA', s.dA) : '') + (s.head ? dsel('drdH', s.dH) : '') + '</div>';
  }
  if (FX && Object.keys(FX.wings).length) {
    h += '<h4>' + tt('wing') + '</h4><select id="drwing"><option value="">' + esc(tt('none')) + '</option>' + Object.keys(FX.wings).sort().map(function (f) {
      var it = D.items && D.items.filter(function (x) { return Math.floor(x.id / 1000) === +f; })[0];
      return '<option value="' + f + '"' + (s.wing === f ? ' selected' : '') + '>' + esc((it ? it.name + ' · ' : '') + tt('wingType') + f) + '</option>';
    }).join('') + '</select>';
  }
  if (FX) h += '<label class="chk" style="font-size:12.5px;margin-top:8px;display:flex"><input type="checkbox" id="drfx"' + (s.fx ? ' checked' : '') + '> ' + tt('fxOn') + '</label>';
  h += '<div class="drtools"><button class="drb' + (D.auto ? ' on' : '') + '" data-act="auto">⟳ ' + tt('auto') + '</button><button class="drb" data-act="bg">◐ ' + tt('bg') + '</button>' +
    '<button class="drb" data-act="shot">⤓ ' + tt('shot') + '</button><button class="drb" data-act="copy">⧉ ' + tt('copy') + '</button>' +
    '<button class="drb" data-act="reset" style="grid-column:1/-1">⌖ ' + tt('reset') + '</button></div>';
  var canAnim = !!(ANIM && ANIM.bodies[s.b] && ANIM.bodies[s.b].gi), anim = canAnim && s.p === 2;
  h += '<h4>' + tt('pose') + '</h4><div class="bods" style="grid-template-columns:1fr 1fr 1fr">' +
    (canAnim ? '<button data-pose="2" class="' + (anim ? 'on' : '') + '">▶ ' + tt('animP') + '</button>' : '') +
    '<button data-pose="1" class="' + (s.p && !anim && hasStand(s.b) ? 'on' : '') + '"' + (hasStand(s.b) ? '' : ' disabled style="opacity:.45"') + '>' + tt('stand') + '</button>' +
    '<button data-pose="0" class="' + (!s.p || (!anim && !hasStand(s.b)) ? 'on' : '') + '">T-pose</button></div>';
  if (anim) {
    var tb = bodyTable() || {}, w = stanceOf(tb);
    h += '<div class="anirow"><select id="dract" title="' + esc(tt('act')) + '">' + actionOptions(tb, w, s.act) + '</select></div>';
    h += '<div class="anirow"><select id="drwt" title="' + esc(tt('stance')) + '"><option value="">' + esc(tt('stance') + ': ' + tt('stAuto') + ' → ' + (w === '0' ? tt('bare') : w)) + '</option>' +
      Object.keys(tb).sort(function (a, b) { return a - b; }).map(function (k) { return '<option value="' + k + '"' + (s.wt === k ? ' selected' : '') + '>' + esc(tt('stance') + ': ' + (k === '0' ? tt('bare') + ' (000)' : k)) + '</option>'; }).join('') + '</select></div>';
    h += '<div class="anirow"><button class="drb" data-act="play" id="drplay">' + (D.playing ? '⏸ ' + tt('pause') : '▶ ' + tt('play')) + '</button>' +
      '<select id="drspd" title="' + esc(tt('speed')) + '">' + [0.1, 0.25, 0.5, 1, 1.5, 2].map(function (v) { return '<option value="' + v + '"' + (v === D.speed ? ' selected' : '') + '>' + v + '×</option>'; }).join('') + '</select></div>';
    h += '<div class="anirow"><input type="range" id="drfr" min="0" max="1" value="0" step="1"><small id="drfl" dir="ltr" style="color:var(--muted);font-family:Consolas,monospace;min-width:52px;text-align:end"></small></div>';
    if (s.mount && s.rider && ANIM.bodies[s.b].r && Object.keys(ANIM.bodies[s.b].r).length) h += '<div class="drstat">' + esc(tt('riding')) + '</div>';
  }
  h += '<div class="drstat" id="drstat"></div><div class="drstat">' + tt(anim ? 'animNote' : hasStand(s.b) ? 'poseNote' : 'noStand') + '</div>';
  document.getElementById('drp').innerHTML = h;
  document.getElementById('drhint').textContent = tt('hint');
}
function status(msg) { var e = document.getElementById('drstat'); if (e) e.textContent = msg || ''; }

var PK = null;
function openPicker(slot) {
  closePicker();
  var list = candidates(slot), groups = {}, uniq = [];
  list.forEach(function (c) { var g = groups[c.look]; if (!g) { groups[c.look] = g = { first: c, n: 0 }; uniq.push(g); } g.n++; });
  PK = { slot: slot, list: list, uniq: uniq, q: '', u: slot !== 'hair' };
  var el = document.createElement('div'); el.className = 'drpick'; el.id = 'drpick';
  el.innerHTML = '<div class="ph"><b>' + tt(slot) + '</b><input type="text" id="drq" placeholder="' + esc(tt('search')) + '" autocomplete="off"><button class="drb" data-act="pclose">' + tt('close') + '</button></div>' +
    '<div class="pl" id="drpl"></div><div class="pf"><label class="chk"><input type="checkbox" id="dru"' + (PK.u ? ' checked' : '') + '> ' + tt('unique') + '</label><span id="drpc" style="margin-inline-start:auto"></span></div>';
  document.getElementById('drc').appendChild(el);
  fillPicker();
  setTimeout(function () { var q = document.getElementById('drq'); if (q) q.focus(); }, 0);
}
function closePicker() { var e = document.getElementById('drpick'); if (e) e.remove(); PK = null; }
function fillPicker() {
  if (!PK) return;
  var q = PK.q.trim().toLowerCase(), terms = q ? q.split(/\s+/) : [], cur = D.state[PK.slot];
  var res = PK.list.filter(function (c) { var s = (c.id + ' ' + (c.item || '') + ' ' + c.name + ' ' + (c.sub || '')).toLowerCase(); return terms.every(function (t) { return s.indexOf(t) >= 0; }); })
    .map(function (c) { return { c: c, n: 0 }; });
  if (PK.u) {   // one entry per model (mesh + texture), the first matching item stands for the others
    var seen = {}, u = [];
    res.forEach(function (x) { var g = seen[x.c.look]; if (g) g.n++; else { seen[x.c.look] = x; x.n = 1; u.push(x); } });
    res = u;
  }
  var LIM = 400, h = '<div class="po' + (!cur ? ' on' : '') + '" data-pid="0"><div class="ni">∅</div><span>' + tt('none') + '</span></div>';
  res.slice(0, LIM).forEach(function (x) {
    var c = x.c;
    h += '<div class="po' + (c.id === cur ? ' on' : '') + '" data-pid="' + c.id + '">' + (c.icon ? '<img loading="lazy" src="' + esc(c.icon) + '" alt="">' : '<div class="ni">?</div>') +
      '<span>' + esc(c.name) + (x.n > 1 ? ' <small>×' + x.n + '</small>' : '') + '</span><small>#' + (c.item || c.id) + '</small></div>';
  });
  if (res.length > LIM) h += '<div class="po" style="cursor:default;color:var(--muted)">… ' + (res.length - LIM) + ' — ' + tt('more') + '</div>';
  document.getElementById('drpl').innerHTML = h;
  document.getElementById('drpc').textContent = res.length + ' / ' + PK.list.length + ' ' + tt('items') + ' · ' + PK.uniq.length + ' ' + tt('models');
}

function onClick(e) {
  var el = e.target.closest('[data-body],[data-slot],[data-clear],[data-act],[data-pid],[data-pose]');
  if (!el) return;
  if (el.hasAttribute('data-pose')) { D.state.p = +el.getAttribute('data-pose'); changed(); return; }
  if (el.hasAttribute('data-clear')) { e.stopPropagation(); D.state[el.getAttribute('data-clear')] = 0; changed(); return; }
  if (el.hasAttribute('data-body')) { setBody(+el.getAttribute('data-body')); changed(); return; }
  if (el.hasAttribute('data-slot')) { openPicker(el.getAttribute('data-slot')); return; }
  if (el.hasAttribute('data-pid')) { if (!PK) return; D.state[PK.slot] = +el.getAttribute('data-pid'); closePicker(); changed(); return; }
  var a = el.getAttribute('data-act');
  if (a === 'pclose') closePicker();
  else if (a === 'auto') { D.auto = !D.auto; el.classList.toggle('on', D.auto); loop(); }
  else if (a === 'bg') { D.bg = (D.bg + 1) % BGS.length; redraw(); }
  else if (a === 'reset') { fit(true); redraw(); }
  else if (a === 'shot') screenshot();
  else if (a === 'copy') { if (D.copy) D.copy(idsText()); }
  else if (a === 'play') { D.playing = !D.playing; el.innerHTML = D.playing ? '⏸ ' + tt('pause') : '▶ ' + tt('play'); loop(); }
}
function onInput(e) {
  var id = e.target.id;
  if (id === 'drq' && PK) { PK.q = e.target.value; fillPicker(); }
  else if (id === 'dru' && PK) { PK.u = e.target.checked; fillPicker(); }
  else if (id === 'drhair' && e.type === 'change') { D.state.hair = +e.target.value; changed(); }
  else if (id === 'drrider' && e.type === 'change') { D.state.rider = e.target.checked ? 1 : 0; changed(); }
  else if ((id === 'drdA' || id === 'drdH') && e.type === 'change') { D.state[id.slice(2)] = +e.target.value; changed(); }
  else if (id === 'drwing' && e.type === 'change') { D.state.wing = e.target.value; changed(); }
  else if (id === 'drfx' && e.type === 'change') { D.state.fx = e.target.checked ? 1 : 0; changed(); }
  else if (id === 'dract' && e.type === 'change') { D.state.act = +e.target.value; D.fitted = true; changed(); }
  else if (id === 'drwt' && e.type === 'change') { D.state.wt = e.target.value; changed(); }
  else if (id === 'drspd' && e.type === 'change') { D.speed = +e.target.value; }
  else if (id === 'drfr' && D.anim) { D.playing = false; var pb = document.getElementById('drplay'); if (pb) pb.innerHTML = '▶ ' + tt('play'); D.anim.t = +e.target.value; animFrame(0); redraw(); }
}
function setBody(b) {
  D.state.b = b;
  var hs = IDX.bodies[b].hair || [];
  if (hs.indexOf(D.state.hair) < 0) D.state.hair = hs.filter(function (h) { return h >= 300; })[0] || hs[0] || 0;
}
function changed() { panel(); rebuild(); if (D.onChange) D.onChange(getState()); }

function idsText() {
  var s = D.state, b = IDX.bodies[s.b], parts = ['Body ' + b.look + ' (' + tt(BODYKEYS[s.b]) + ')'];
  if (!s.head && s.hair) parts.push('Hair ' + s.hair);
  SLOTS.forEach(function (sl) {
    var id = s[sl]; if (!id) return;
    if (sl === 'mount') { var m = MOUNTS[id]; parts.push(tt(sl) + ' ' + (m ? m[0] + (m[1] ? ' variant ' + m[1] : '') : id)); }
    else { var it = D.byId[id]; parts.push(tt(sl) + ' ' + id + (it ? ' ' + it.name : '')); }
  });
  return parts.join(' | ');
}
function screenshot() {
  if (!D.r) return;
  redraw(true);
  var url = D.r.canvas.toDataURL('image/png'), a = document.createElement('a');
  var s = D.state; a.href = url; a.download = 'dress_' + IDX.bodies[s.b].look + '_' + [s.garment || s.armor, s.head || ('h' + s.hair), s.rw, s.lw, s.mount].join('_') + '.png';
  document.body.appendChild(a); a.click(); a.remove();
}

/* ---- scene */
function curPose() {
  var P = IDX.poses && IDX.poses[D.state.b];
  if (!P) return null;
  if (D.state.p && P.stand) return { key: 'stand' + D.state.b, bones: P.stand.bones, h: P.stand.h };   // p 2 (animated) falls back to it
  return { key: 'T' + D.state.b, bones: null, h: P.T && P.T.h };
}
function hasStand(b) { return !!(IDX.poses && IDX.poses[b] && IDX.poses[b].stand); }
function loadPart(p, dyn) {
  var mname = IDX.meshes[p[0]], tname = p[1] >= 0 ? IDX.textures[p[1]] : null, pose = curPose();
  return Promise.all([asset('m', mname), tname ? asset('t', tname).catch(function (e) { return null; }) : null]).then(function (r) {
    var mesh = dyn && r[0].meta.s ? D.r.dyn(mname, r[0].meta, r[0].buf) : D.r.mesh(mname, r[0].meta, r[0].buf, pose);
    return { mesh: mesh, tex: r[1] ? D.r.tex(tname, r[1].meta, r[1].buf) : null, name: mname };
  });
}
function loadRow(row, dyn) { return row ? Promise.all(row.parts.map(function (p) { return loadPart(p, dyn); })) : Promise.resolve([]); }
// mounts are animated with their n/ (skinned) copy; when it is missing the static m/ mesh is used
function loadMountRow(row, dyn) {
  if (!row) return Promise.resolve([]);
  return Promise.all(row.parts.map(function (p) {
    var mname = IDX.meshes[p[0]], tname = p[1] >= 0 ? IDX.textures[p[1]] : null, nname = 'n_' + mname.slice(2);
    var mp = dyn ? asset('n', nname).then(function (r) { return { mesh: D.r.dyn(nname, r.meta, r.buf) }; }, function () { return null; }) : Promise.resolve(null);
    return Promise.all([mp, tname ? asset('t', tname).catch(function () { return null; }) : null]).then(function (r) {
      var tex = r[1] ? D.r.tex(tname, r[1].meta, r[1].buf) : null;
      if (r[0]) return { mesh: r[0].mesh, tex: tex, name: nname };
      return loadPart(p, false);
    });
  }));
}
function animated() { return !!(ANIM && D.state.p === 2 && ANIM.bodies[D.state.b] && ANIM.bodies[D.state.b].gi); }
// weapon stance of the motion table: explicit choice, else the right weapon's type, else the left one, else bare hands
function stanceOf(tb) {
  var s = D.state;
  if (s.wt && tb[s.wt]) return s.wt;
  var ids = [s.rw, s.lw];
  for (var i = 0; i < ids.length; i++) {
    if (!ids[i]) continue;
    var t = String(Math.floor(ids[i] / 1000));
    if (tb[t]) return t;
    if (t.charAt(0) === '4' && tb['410']) return '410';
    if (t.charAt(0) === '5' && tb['511']) return '511';
    if (t.charAt(0) === '6' && tb['601']) return '601';
  }
  return '0';
}
function bodyTable() {
  var B = ANIM && ANIM.bodies[D.state.b]; if (!B) return null;
  var riding = D.state.mount && D.state.rider && B.r && Object.keys(B.r).length;
  return riding ? B.r : B.m;
}
function bodyAnimIndex() {
  var tb = bodyTable(); if (!tb) return -1;
  var w = stanceOf(tb), a = String(D.state.act || 100);
  if (tb[w] && tb[w][a] != null) return tb[w][a];
  if (tb['0'] && tb['0'][a] != null) return tb['0'][a];
  if (tb[w] && tb[w]['100'] != null) return tb[w]['100'];
  return tb['0'] && tb['0']['100'] != null ? tb['0']['100'] : -1;
}
function actionName(a) {
  var n = ANIM && ANIM.names[a];
  return n ? (D.lang === 'ar' ? n[1] : n[0]) + ' (' + a + ')' : (D.lang === 'ar' ? 'حركة ' : 'Action ') + a;
}
function actionOptions(tb, w, cur) {
  var set = {};
  [w, '0'].forEach(function (k) { if (tb && tb[k]) Object.keys(tb[k]).forEach(function (a) { set[a] = 1; }); });
  var acts = Object.keys(set).map(Number).sort(function (a, b) { return a - b; });
  var groups = [[tt('gBasic'), function (a) { return a >= 100 && a < 140 || a === 250; }], [tt('gEmote'), function (a) { return a < 100 || a >= 140 && a < 300 && a !== 250; }],
    [tt('gFight'), function (a) { return a >= 300 && a < 500; }], [tt('gSkill'), function (a) { return a >= 500; }]];
  return groups.map(function (g) {
    var list = acts.filter(g[1]); if (!list.length) return '';
    return '<optgroup label="' + esc(g[0]) + '">' + list.map(function (a) { return '<option value="' + a + '"' + (a === cur ? ' selected' : '') + '>' + esc(actionName(a)) + '</option>'; }).join('') + '</optgroup>';
  }).join('');
}

function rebuild() {
  if (!D.r || !IDX) return;
  var s = D.state, b = s.b, gen = ++D.gen, anim = animated();
  var body = rowsFor('garment', s.garment, b) || rowsFor('armor', s.armor, b) || (IDX.bodies[b].naked ? { key: IDX.bodies[b].naked, parts: IDX.armor[IDX.bodies[b].naked] } : null);
  var head = rowsFor('head', s.head, b) || rowsFor('hair', s.hair, b);
  var rw = rowsFor('rw', s.rw, b), lw = rowsFor('lw', s.lw, b), mt = rowsFor('mount', s.mount, b);
  if (s.armor && !s.garment) body = dyeRow(body, s.dA);
  if (s.head && head && !head.hairOnly) head = dyeRow(head, s.dH);
  var errs = [];
  SLOTS.forEach(function (sl) { if (s[sl] && !rowsFor(sl, s[sl], b)) errs.push(tt(sl) + ' #' + s[sl] + ' ' + tt('notFor')); });
  if (head && head.hairOnly) errs.push(tt('hairHead'));
  status(tt('loading'));
  var ld = function (pr, label) { return pr.catch(function (e) { errs.push(label + ': ' + e.message); return []; }); };
  var mk = mt && mt.key, mAnims = anim && mk && ANIM.mounts && (ANIM.mounts[mk] || ANIM.mounts[String(+mk - (+mk % 10000))]);
  var ma = mAnims ? (mAnims[String(D.state.act)] != null ? mAnims[String(D.state.act)] : mAnims[String(D.state.act - D.state.act % 10)] != null ? mAnims[String(D.state.act - D.state.act % 10)] : mAnims['100'] != null ? mAnims['100'] : mAnims['101']) : null;
  Promise.all([ld(loadRow(body, anim), tt('armor')), ld(loadRow(head), tt('head')), ld(loadRow(rw), tt('rw')), ld(loadRow(lw), tt('lw')),
    ld(loadMountRow(mt, anim && ma != null), tt('mount')),
    anim ? loadAnim(bodyAnimIndex()).catch(function (e) { errs.push(e.message); return null; }) : null,
    anim && ma != null ? loadAnim(ma).catch(function () { return null; }) : null]).then(function (L) {
    if (gen !== D.gen) return;
    D.anim = null;
    if (anim && L[5]) {
      var B = ANIM.bodies[b], A = L[5], info = B.info, bi = 0;
      A.parts.forEach(function (p, i) { if (p && p.nb > (A.parts[bi] ? A.parts[bi].nb : 0)) bi = i; });
      var mg = mk && ANIM.mountGi && (ANIM.mountGi[mk] || ANIM.mountGi[String(+mk - (+mk % 10000))]);
      // riding motions (c3/7100x) keep the pelvis at the origin = the saddle point and need no body correction
      var riding = !!(s.mount && s.rider && B.r && Object.keys(B.r).length && L[4].length);
      D.anim = { L: L, A: A, bi: bi, info: info, gi: riding ? IDENT : new Float32Array(B.gi), riding: riding, M: L[6], mgi: mg ? new Float32Array(mg) : null,
        t: D.anim0 != null ? D.anim0 : 0, cache: {}, mcache: {}, tmp: null, mats: null };
      D.anim0 = null;
      animFrame(0);
      fxLoad(gen);
      status(errs.join(' · '));
      if (!D.fitted) fit(true); else fit(false);
      redraw(); loop();
      return;
    }
    var objs = [], bodyH = {}, bodyB = null;
    L[0].forEach(function (o) { Object.keys(o.mesh.h).forEach(function (k) { bodyH[k.toLowerCase()] = o.mesh.h[k]; }); if (!bodyB) bodyB = o.mesh.b; });
    var root = M4.id();
    if (L[4].length) {
      var mh = null; L[4].forEach(function (o) { if (o.mesh.h.v_mount) mh = o.mesh.h.v_mount; });
      if (mh && mh.length === 2) mh = null;   // n/ helper format: only used by the animated path
      L[4].forEach(function (o) { objs.push({ mesh: o.mesh, tex: o.tex, model: CONV }); });
      if (s.rider && mh) {
        var hip = bodyB ? -bodyB[2] * 0.5 : 90;   // T-pose rider: put the hips on the saddle point, legs hang inside the mount body
        root = M4.tr(mh[12], mh[13], mh[14] + hip);
      } else if (!s.rider) root = null;
    }
    if (root) {
      var C = M4.mul(CONV, root);
      L[0].forEach(function (o) { objs.push({ mesh: o.mesh, tex: o.tex, model: C }); });
      var att = function (list, hname) { var hm = bodyH[hname]; if (!hm) return; var m = M4.mul(C, new Float32Array(hm)); list.forEach(function (o) { objs.push({ mesh: o.mesh, tex: o.tex, model: m }); }); };
      att(L[1], 'v_armet'); att(L[2], 'v_r_weapon'); att(L[3], 'v_l_weapon');
      D.att = { root: C, back: M4.mul(C, M4.tr(0, 6, bodyB ? bodyB[2] * 0.66 : -115)), rw: bodyH.v_r_weapon ? M4.mul(C, new Float32Array(bodyH.v_r_weapon)) : null, lw: bodyH.v_l_weapon ? M4.mul(C, new Float32Array(bodyH.v_l_weapon)) : null, mount: CONV };
    } else D.att = { mount: CONV };
    D.objs = objs;
    fxLoad(gen);
    status(errs.join(' · '));
    if (!D.fitted) fit(true); else fit(false);
    redraw();
  });
}
// one animation step: skin the body / mount for the current time and rebuild the draw list
function animFrame(dt) {
  var X = D.anim; if (!X) return;
  var A = X.A, fr = A.frames;
  if (D.playing && dt) X.t = (X.t + dt * 0.03 * D.speed) % fr;   // 30 frames per second at speed 1
  var f = X.t, L = X.L, objs = [], s = D.state;
  X.att = {};
  // body
  var P = A.parts[X.bi];
  X.tmp = samplePart(P, f, X.tmp);
  X.mats = rmulAll(X.tmp, P.nb, X.gi, X.mats);
  L[0].forEach(function (o) { if (o.mesh.dyn) D.r.pose(o.mesh, X.mats); });
  // root: the saddle point of the (animated) mount
  var C = CONV, mountObjs = L[4];
  if (mountObjs.length) {
    var M = X.M, mf = M ? f % M.frames : 0;
    if (M && mountObjs[0].mesh.dyn) rigMats(D.r, mountObjs, M, mf, X.mgi, X.mcache);
    mountObjs.forEach(function (o) { objs.push({ mesh: o.mesh, tex: o.tex, model: CONV }); });
    var vm = mountObjs[0].mesh.dyn ? rigHelper(mountObjs, M, mf, X.mgi, 'v_mount') : null;
    if (!vm) { var mh = mountObjs[0].mesh.h && mountObjs[0].mesh.h.v_mount; if (mh && mh.length === 16) vm = M4.tr(mh[12], mh[13], mh[14]); }
    if (!s.rider) C = null;
    else if (vm) {
      // the riding motions stand the rider on z = 0 with the legs straight: like the client, the hips go on the
      // saddle point (v_mount, translation only) and the legs hang inside the mount's body
      if (X.hip == null) { var bz = 0; L[0].forEach(function (o) { bz = Math.min(bz, o.mesh.b[2]); }); X.hip = -bz * 0.56; }
      C = M4.mul(CONV, M4.tr(vm[12], vm[13], vm[14] + (X.riding ? X.hip : 0)));
    }
  }
  if (C) {
    L[0].forEach(function (o) { objs.push({ mesh: o.mesh, tex: o.tex, model: C }); });
    var info = X.info, hs = info && info.helpers || {};
    [[L[1], 'v_armet'], [L[2], 'v_r_weapon'], [L[3], 'v_l_weapon']].forEach(function (x) {
      var pi = info ? info.parts.indexOf(x[1]) : -1, HP = pi >= 0 ? A.parts[pi] : null;
      if (!HP || !x[0].length) return;
      var z = samplePart(HP, f, X.cache[x[1]]); X.cache[x[1]] = z;
      var t = new Float32Array(16), h = new Float32Array(16);
      rmul(new Float32Array(hs[x[1]] || IDENT), 0, z, 0, t, 0); rmul(t, 0, X.gi, 0, h, 0);
      var m = M4.mul(C, h);
      X.att[x[1] === 'v_r_weapon' ? 'rw' : x[1] === 'v_l_weapon' ? 'lw' : 'head'] = m;
      x[0].forEach(function (o) { objs.push({ mesh: o.mesh, tex: o.tex, model: m }); });
    });
  }
  X.att.root = C; X.att.mount = CONV;
  if (C) {
    // wings hang at the upper back: 2/3 of the body height (measured once, on the first frame), a little behind
    if (X.backZ == null) { var mz = 0; L[0].forEach(function (o) { mz = Math.min(mz, o.mesh.b[2]); }); X.backZ = mz * 0.66; }
    X.att.back = M4.mul(C, M4.tr(0, 6, X.backZ));
  }
  D.att = X.att;
  D.objs = objs;
  var sl = document.getElementById('drfr');
  if (sl && !D.dragFr) { sl.max = String(Math.max(1, fr - 1)); sl.value = String(Math.floor(f)); var fl = document.getElementById('drfl'); if (fl) fl.textContent = Math.floor(f) + ' / ' + fr; }
}
function sceneBox() {
  var mn = [1e9, 1e9, 1e9], mx = [-1e9, -1e9, -1e9];
  D.objs.forEach(function (o) {
    var b = o.mesh.b;
    for (var i = 0; i < 8; i++) {
      var p = M4.pt(o.model, [i & 1 ? b[3] : b[0], i & 2 ? b[4] : b[1], i & 4 ? b[5] : b[2]]);
      for (var k = 0; k < 3; k++) { mn[k] = Math.min(mn[k], p[k]); mx[k] = Math.max(mx[k], p[k]); }
    }
  });
  return mn[0] > mx[0] ? null : [mn, mx];
}
function fit(resetAngles) {
  var bx = sceneBox(); if (!bx) return;
  var c = [(bx[0][0] + bx[1][0]) / 2, (bx[0][1] + bx[1][1]) / 2, (bx[0][2] + bx[1][2]) / 2];
  var r = Math.max(bx[1][0] - bx[0][0], bx[1][1] - bx[0][1], bx[1][2] - bx[0][2]) / 2;
  D.cam.target = c;
  if (resetAngles || !D.fitted) { D.cam.dist = r / Math.tan(D.cam.fov / 2) * 1.2 + r * 0.3; D.cam.yaw = D.viewYaw || 0; D.cam.pitch = 0.12; D.viewYaw = 0; }
  D.cam.near = Math.max(1, r * 0.02); D.cam.far = r * 40 + D.cam.dist * 4;
  D.fitted = true;
}
function camera() {
  var c = D.cam, cp = Math.cos(c.pitch);
  // the characters face -Z after the C3 -> GL conversion: yaw 0 looks at the front
  var e = [c.target[0] + c.dist * Math.sin(c.yaw) * cp, c.target[1] + c.dist * Math.sin(c.pitch), c.target[2] - c.dist * Math.cos(c.yaw) * cp];
  return { eye: e, target: c.target, fov: c.fov, near: c.near, far: Math.max(c.far, c.dist * 4) };
}
var rq = 0;
// effects of the outfit: loaded after the scene (they do not block it), drawn in the last pass
function fxLoad(gen) {
  var want = fxWanted();
  D.fxInst = want;
  if (!want.length) return;
  Promise.all(want.map(function (w) { return loadFx(w.name, D.r).then(function (f) { w.fx = f; }, function () {}); })).then(function () {
    if (gen !== D.gen) return;
    redraw(); loop();
  });
}
function hasFx() { return !!(D.fxInst && D.fxInst.some(function (i) { return i.fx && i.fx.parts.length; })); }
function sceneObjs() {
  if (!hasFx() || !D.att) return D.objs;
  return D.objs.concat(fxObjects(D.r, D.fxInst, D.fxT || 0, D.att, camera()));
}
function redraw(now) {
  if (!D.r || !D.visible) return;
  if (now === true) { D.r.draw(sceneObjs(), camera(), BGS[D.bg]); return; }
  if (rq) return;
  rq = requestAnimationFrame(function () { rq = 0; D.r.draw(sceneObjs(), camera(), BGS[D.bg]); });
}
var lastT = 0, looping = false;
function loop() {
  if (looping) return;
  looping = true; lastT = performance.now();
  (function step(t) {
    var playing = D.anim && D.playing, fx = hasFx();
    if ((!D.auto && !playing && !fx) || !D.visible) { looping = false; return; }
    var dt = Math.min(100, t - lastT); lastT = t;
    if (D.auto) D.cam.yaw += dt * 0.0006;
    if (playing) animFrame(dt);
    if (fx && (D.playing || !D.anim)) D.fxT = (D.fxT || 0) + dt * D.speed;
    D.r && D.r.draw(sceneObjs(), camera(), BGS[D.bg]);
    requestAnimationFrame(step);
  })(lastT);
}

/* ---- state string: b3.h310.g181305.a0.e0.r0.l0.m0.d1 */
function getState() {
  var s = D.state;
  return 'b' + s.b + '.h' + s.hair + '.g' + s.garment + '.a' + s.armor + '.e' + s.head + '.r' + s.rw + '.l' + s.lw + '.m' + s.mount + '.d' + s.rider + '.p' + s.p +
    (s.act !== 100 ? '.x' + s.act : '') + (s.wt ? '.w' + s.wt : '') + (s.wing ? '.n' + s.wing : '') + (s.fx ? '' : '.z0') + (s.dA ? '.k' + s.dA : '') + (s.dH ? '.j' + s.dH : '');
}
function setState(str) {
  if (!str) return;
  var map = { b: 'b', h: 'hair', g: 'garment', a: 'armor', e: 'head', r: 'rw', l: 'lw', m: 'mount', d: 'rider', p: 'p', x: 'act' };
  String(str).split('.').forEach(function (p) {
    var k = map[p[0]]; if (k && /^\d+$/.test(p.slice(1))) D.state[k] = +p.slice(1);
    if (p[0] === 'w' && /^\d+$/.test(p.slice(1))) D.state.wt = p.slice(1);
    if (p[0] === 'n' && /^\d+$/.test(p.slice(1))) D.state.wing = p.slice(1);
    if (p[0] === 'k' && /^\d$/.test(p.slice(1))) D.state.dA = +p.slice(1);
    if (p[0] === 'j' && /^\d$/.test(p.slice(1))) D.state.dH = +p.slice(1);
    if (p[0] === 't' && /^\d+$/.test(p.slice(1))) D.fxT = +p.slice(1);   // effect time (ms), for screenshots
    if (p[0] === 'z' && /^\d$/.test(p.slice(1))) D.state.fx = +p.slice(1);
    if (p[0] === 'f' && /^\d+(\.\d+)?$/.test(p.slice(1))) { D.anim0 = +p.slice(1); D.playing = false; }   // start paused at this frame
    if (p[0] === 'v' && /^-?\d+$/.test(p.slice(1))) D.viewYaw = +p.slice(1) * Math.PI / 180;   // optional start angle (degrees)
  });
  if (!(D.state.b >= 1 && D.state.b <= 4)) D.state.b = 3;
}

window.Dresser = {
  available: function () { return !!IDX; },
  canTry: canTry,
  init: function (opt) {
    D.items = opt.items || []; D.lang = opt.lang || 'ar'; D.copy = opt.copy; D.onChange = opt.onChange;
    D.items.forEach(function (x) { D.byId[x.id] = x; });
    if (IDX) IDX.mounts.forEach(function (m) { MOUNTS[mountId(m)] = m; });
  },
  show: function (container, stateStr) {
    D.visible = true;
    if (!IDX) { container.innerHTML = '<div class="empty">' + esc(tt('noIndex')) + '</div>'; return; }
    if (stateStr) setState(stateStr);
    setBody(D.state.b);   // keeps the hair when this body has it, otherwise takes its first hair
    if (D.root !== container || !container.querySelector('.dress')) { D.root = container; build(container); D.fitted = false; }
    if (!D.r) { document.getElementById('drmsg').textContent = tt('noGl') + ' ' + (D.glErr || ''); }
    panel(); rebuild(); if (D.auto) loop();
  },
  hide: function () { D.visible = false; closePicker(); },
  setLang: function (l) { D.lang = l; if (D.visible && IDX && D.root) { panel(); if (PK) openPicker(PK.slot); } },
  tryOn: function (x, opt) {
    var s = slotOf(x); if (!s) return;
    if (s !== 'mount') {
      var b = D.state.b;
      if (!rowsFor(s, x.id, b)) {   // switch to a body that has this item (female garments etc.)
        var pref = x.sex === 2 ? [1, 2, 3, 4] : x.sex === 1 ? [3, 4, 1, 2] : [b, 3, 1, 4, 2];
        for (var i = 0; i < pref.length; i++) if (rowsFor(s, x.id, pref[i])) { setBody(pref[i]); break; }
      }
      if (s === 'garment') D.state.garment = x.id;
      else if (s === 'armor') { D.state.armor = x.id; D.state.garment = 0; }
      else D.state[s] = x.id;
    } else D.state.mount = x.id;
    if (D.onChange) D.onChange(getState());
  },
  getState: getState,
  setState: setState,
  idsText: idsText,
  setTheme: function () { D.bg = themeBg(); redraw(); },
  hasModels: function () { return !!NPCS; },
  npcData: function () { return NPCS; },
  viewModel: viewModel,
  closeModel: mvStop,
  actionName: function (a) { return actionName(a); },
  renderNow: function () { redraw(true); }
};
})();
