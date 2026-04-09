'use strict';
/* UDIL App UI — Part 2 | PITC/UDIL/3.3-25029017 */

// ─── UI HELPERS ──────────────────────────────────────────
U.toast = (msg, type = 'info') => {
  const rack = document.getElementById('toast-rack'); if (!rack) return;
  const icons = { pass: '✅', fail: '❌', warn: '⚠️', info: 'ℹ️' };
  const el = document.createElement('div'); el.className = `toast ${type}`;
  el.innerHTML = `<span class="toast-icon">${icons[type] || 'ℹ️'}</span><span>${msg}</span>`;
  rack.appendChild(el);
  setTimeout(() => { el.classList.add('toast-out'); setTimeout(() => el.remove(), 300); }, 3500);
};
U.modal = (title, html) => {
  const ov = document.getElementById('modal-overlay');
  if (!ov) return;
  document.getElementById('modal-ttl').textContent = title;
  document.getElementById('modal-body').innerHTML = html;
  ov.style.display = 'flex';
};
U.closeModal = () => {
  const ov = document.getElementById('modal-overlay');
  if (ov) ov.style.display = 'none';
};
U.json = (obj) => {
  const s = JSON.stringify(obj, null, 2);
  return s.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, m => {
    let cls = 'json-num';
    if (/^"/.test(m)) { cls = /:$/.test(m) ? 'json-key' : 'json-str'; }
    else if (/true|false/.test(m)) { cls = 'json-bool'; }
    else if (/null/.test(m)) { cls = 'json-null'; }
    return `<span class="${cls}">${m}</span>`;
  });
};
U.jsonBlock = (label, obj, open = false) => `
<div class="json-block${open ? ' open' : ''}">
  <div class="json-block-hdr" onclick="this.parentElement.classList.toggle('open')">
    <span class="json-block-label">${label}</span>
    <span class="json-block-toggle">▼</span>
  </div>
  <div class="json-block-body"><pre class="json-body">${U.json(obj)}</pre></div>
</div>`;
U.badge = (txt, type) => {
  const classes = {
    pass: 'badge bg-success',
    fail: 'badge bg-danger',
    warn: 'badge bg-warning text-dark',
    info: 'badge bg-info text-dark',
    muted: 'badge bg-secondary',
  };
  return `<span class="${classes[type] || 'badge bg-secondary'}">${txt}</span>`;
};
U.statusIcon = (s) => s === 'pass' ? '<span style="color:var(--teal)">✓</span>' : s === 'fail' ? '<span style="color:var(--red)">✗</span>' : '<span style="color:var(--amber)">⚠</span>';
U.pipeline = (steps, current) => {
  return `<div class="pipeline">${steps.map((s, i) => {
    const st = i < current ? 'done' : i === current ? 'active' : '';
    return `<div class="pipe-step ${st}"><div class="pipe-node">${i < current ? '✓' : i + 1}</div><div class="pipe-label">${s}</div></div>`;
  }).join('')}</div>`;
};
U.circleChart = (pct, size = 120) => {
  const r = 46, c = Math.PI * 2 * r, dash = c * (pct / 100), color = pct >= 80 ? 'var(--teal)' : pct >= 50 ? 'var(--amber)' : 'var(--red)';
  return `<div class="progress-container" style="width:${size}px;height:${size}px">
    <svg class="progress-ring" width="${size}" height="${size}" viewBox="0 0 100 100">
      <circle class="ring-bg" cx="50" cy="50" r="${r}" stroke-width="8"/>
      <circle class="ring-fill" cx="50" cy="50" r="${r}" stroke-width="8" stroke="${color}"
        stroke-dasharray="${dash} ${c}" stroke-dashoffset="0"/>
    </svg>
    <div class="ring-center"><span class="ring-pct" style="color:${color}">${Math.round(pct)}%</span><span class="ring-pct-label">Score</span></div>
  </div>`;
};

// ─── NAV & ROUTING ───────────────────────────────────────
U.navigate = (sec) => {
  document.querySelectorAll('.section').forEach(s => s.classList.add('d-none'));
  document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
  const el = document.getElementById(`section-${sec}`); if (el) el.classList.remove('d-none');
  const nav = document.getElementById(`nav-${sec}`); if (nav) nav.classList.add('active');
  document.getElementById('page-title').textContent = U.TITLES[sec] || sec;
};

// ═══════════════════════════════════════════════════════
// SECTION 1: CONFIGURATION
// ═══════════════════════════════════════════════════════
U.ConfigSection = {
  render() {
    const cfg = U.State.cfg;
    const map = { url: 'cfg-url', user: 'cfg-user', pass: 'cfg-pass', code: 'cfg-code', ids: 'cfg-ids', mode: 'cfg-mode' };
    Object.entries(map).forEach(([key, id]) => {
      const el = document.getElementById(id);
      if (el) el.value = U.State.cfg(key) || '';
    });
    this.refreshAuthUI();
  },
  bindEvents() {
    document.getElementById('btn-connect').addEventListener('click', () => this.connect());
    document.getElementById('btn-save-cfg').addEventListener('click', () => this.saveConfig());
    document.getElementById('btn-reset').addEventListener('click', () => { if (confirm('Reset all session data?')) { U.State.reset(); U.App.init(); } });
  },
  async connect() {
    const btn = document.getElementById('btn-connect');
    btn.classList.add('btn-loading'); btn.disabled = true;
    const cfg = { url: document.getElementById('cfg-url').value.trim(), user: document.getElementById('cfg-user').value.trim(), pass: document.getElementById('cfg-pass').value.trim(), code: document.getElementById('cfg-code').value.trim() };
    const res = await U.API.auth(cfg);
    btn.classList.remove('btn-loading'); btn.disabled = false;
    if (res.ok && res.data.status === 1) {
      ['url', 'user', 'pass', 'code'].forEach(k => U.State.cfg(k, cfg[k] || ''));
      U.State.key(res.data.privatekey);
      document.getElementById('auth-detail').innerHTML = U.jsonBlock('AUTH Response', res.data, true);
      this.refreshAuthUI(true);
      const pill = document.getElementById('auth-pill'); pill.className = 'auth-pill connected';
      document.getElementById('auth-dot').className = 'auth-dot connected';
      document.getElementById('auth-text').textContent = 'Authenticated';
      document.getElementById('nav-dot-config').className = 'nav-dot ok';
      U.toast('Authenticated successfully!', 'pass');
      this.startTimer();
    } else {
      document.getElementById('auth-detail').innerHTML = U.jsonBlock('AUTH Response', res.data, true);
      this.refreshAuthUI(false); U.toast('Authentication failed: ' + (res.data.message || 'Unknown error'), 'fail');
    }
  },
  saveConfig() {
    U.State.cfg('ids', document.getElementById('cfg-ids').value.trim());
    U.State.cfg('mode', document.getElementById('cfg-mode').value);
    U.toast('Settings saved', 'pass');
  },
  refreshAuthUI(ok) {
    const key = U.State.key(); const dot = document.getElementById('auth-big-dot'); const lbl = document.getElementById('auth-status-label'); const kd = document.getElementById('auth-key-display');
    if (ok === true || (ok === undefined && key)) {
      dot.style.background = 'var(--teal)'; dot.style.borderColor = 'var(--teal)'; dot.style.boxShadow = '0 0 16px var(--teal-glow)';
      lbl.style.color = 'var(--teal)'; lbl.textContent = 'Authenticated ✓';
      kd.textContent = key ? `Key: ${key.slice(0, 20)}...` : '';
    } else if (ok === false) {
      dot.style.background = 'var(--red)'; dot.style.borderColor = 'var(--red)'; dot.style.boxShadow = '0 0 12px rgba(239,68,68,0.3)';
      lbl.style.color = 'var(--red)'; lbl.textContent = 'Authentication Failed'; kd.textContent = '';
    }
  },
  _timerInt: null,
  startTimer() {
    if (this._timerInt) clearInterval(this._timerInt);
    const tp = document.getElementById('token-pill'); const tt = document.getElementById('token-time');
    tp.style.display = 'flex'; let secs = 1800;
    this._timerInt = setInterval(() => { secs--; const m = String(Math.floor(secs / 60)).padStart(2, '0'), s = String(secs % 60).padStart(2, '0'); tt.textContent = `${m}:${s}`; if (secs <= 0) { clearInterval(this._timerInt); tp.style.display = 'none'; U.toast('Session token expired. Re-authenticate.', 'warn'); } }, 1000);
  },
};

// ═══════════════════════════════════════════════════════
// SECTION 2: READ TESTS
// ═══════════════════════════════════════════════════════
U.ReadSection = {
  render() {
    const tables = Object.keys(U.SCHEMAS);
    document.getElementById('read-cards').innerHTML = tables.map(t => this.cardHtml(t)).join('');
    tables.forEach(t => {
      const btn = document.getElementById(`run-btn-${t}`);
      if (btn) btn.addEventListener('click', () => this.runTest(t));
      const saved = U.State.result('read', t);
      if (saved) this.renderResult(t, saved.rows, saved.validation);
    });
    const runAll = document.getElementById('btn-run-all-read');
    if (runAll) runAll.addEventListener('click', () => { tables.forEach(t => this.runTest(t)); });
  },
  cardHtml(t) {
    const s = U.SCHEMAS[t]; const fields = s.fields;
    const pkFields = fields.filter(f => f.pk).map(f => `<span class="badge bg-dark me-1 mb-1">🔑 ${f.n}</span>`).join('');
    const typeMap = { bigint: 'bg-secondary', decimal: 'bg-info text-dark', varchar: 'bg-warning text-dark', datetime: 'bg-primary text-white', bit: 'bg-success text-dark', int: 'bg-info text-dark' };
    const fieldTags = fields.slice(0, 6).map(f => `<span class="badge ${typeMap[f.t] || 'bg-secondary'} me-1 mb-1">${f.n}</span>`).join('') + `${fields.length > 6 ? `<span class="badge bg-secondary me-1 mb-1">+${fields.length - 6} more</span>` : ''}`;
    return `<div class="col">
  <div class="card h-100 shadow-sm" id="card-${t}">
    <div class="card-header d-flex justify-content-between align-items-start">
      <div>
        <h5 class="card-title mb-1">${t}</h5>
        <p class="text-muted mb-0">${s.label} — ${s.desc}</p>
      </div>
      <button type="button" class="btn btn-primary btn-sm" id="run-btn-${t}">Run Test</button>
    </div>
    <div class="card-body">
      <div class="mb-3">${pkFields} ${fieldTags}</div>
      <div id="result-${t}" class="bg-light p-3 rounded">
        <div class="text-center py-4">
          <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/></svg>
          <p class="mb-0">Click <strong>Run Test</strong> to fetch and validate table data</p>
        </div>
      </div>
    </div>
  </div>
</div>`;
  },
  async runTest(t) {
    const btn = document.getElementById(`run-btn-${t}`);
    if (btn) { btn.classList.add('btn-loading'); btn.disabled = true; }
    const res = document.getElementById(`result-${t}`);
    res.innerHTML = `<div class="idle-state"><div class="status-dot pulse" style="width:12px;height:12px;background:var(--blue);animation:pulse-dot 1s infinite"></div><p>Fetching ${t}…</p></div>`;
    const r = await U.API.fetchTable(t);
    if (btn) { btn.classList.remove('btn-loading'); btn.disabled = false; }
    if (!r.ok || !r.data.rows) { res.innerHTML = `<div class="alert alert-danger"><span>❌</span><span>Failed to fetch data: ${(r.data || {}).message || 'Network error'}</span></div>`; return; }
    const rows = r.data.rows; const validation = U.validate(t, rows);
    U.State.result('read', t, { rows, validation });
    this.renderResult(t, rows, validation);
    // Update nav badge
    U.ReportSection.updateBadges();
  },
  renderResult(t, rows, v) {
    const res = document.getElementById(`result-${t}`); if (!res) return;
    const total = v.pass + v.fail + v.warn;
    const pct = total ? Math.round((v.pass / total) * 100) : 0;
    const statusBadge = v.fail > 0 ? 'fail' : v.warn > 0 ? 'warn' : 'pass';
    res.innerHTML = `
<div class="result-summary-bar">
  <div class="result-counts">
    ${U.badge(`✓ ${v.pass} Pass`, 'pass')}${U.badge(`✗ ${v.fail} Fail`, 'fail')}${U.badge(`⚠ ${v.warn} Warn`, 'warn')}
  </div>
  <div style="display:flex;align-items:center;gap:10px">
    <span style="font-size:12px;color:var(--text-2)">${rows.length} rows · ${v.checks.length} checks · ${pct}% pass rate</span>
    <button type="button" class="btn btn-outline-secondary btn-sm" onclick="U.ReadSection.showDetail('${t}')">View Details</button>
  </div>
</div>
<div class="table-responsive"><table class="table table-sm table-striped"><thead><tr><th>Validation Rule</th><th>Status</th><th>Details</th></tr></thead>
<tbody>${v.checks.slice(0, 20).map(c => `<tr>
  <td class="text-monospace small">${c.rule}</td>
  <td>${U.badge(c.status.toUpperCase(), c.status === 'pass' ? 'pass' : c.status === 'fail' ? 'fail' : 'warn')}</td>
  <td class="text-muted small">${c.msg}</td>
</tr>`).join('')}
${v.checks.length > 20 ? `<tr><td colspan="3" class="text-center text-muted small">… ${v.checks.length - 20} more checks — click View Details</td></tr>` : ''}
</tbody></table></div>`;
  },
  showDetail(t) {
    const saved = U.State.result('read', t); if (!saved) return;
    const v = saved.validation;
    U.modal(`${t} — Full Validation Results`, `
<div class="mb-3">${U.badge(`✓ ${v.pass}`, 'pass')} ${U.badge(`✗ ${v.fail}`, 'fail')} ${U.badge(`⚠ ${v.warn}`, 'warn')}</div>
<div class="table-responsive"><table class="table table-sm table-striped"><thead><tr><th>Rule</th><th>Status</th><th>Details</th></tr></thead>
<tbody>${v.checks.map(c => `<tr><td class="text-monospace small">${c.rule}</td><td>${U.badge(c.status.toUpperCase(), c.status)}</td><td class="text-muted small">${c.msg}</td></tr>`).join('')}</tbody></table></div>`);
  },
};

// ═══════════════════════════════════════════════════════
// SECTION 3: WRITE TESTS
// ═══════════════════════════════════════════════════════
U.WriteSection = {
  PIPE_STEPS: ['Waiting', 'Processing', 'Sent', 'Connected', 'Cmd Sent', 'Executed'],
  GROUPS: { A: 'Relay & Load Control', B: 'Meter Configuration', C: 'Device Setup', D: 'APMS & Alarms' },
  render() {
    const allCmds = Object.keys(U.COMMANDS);
    let html = '';
    allCmds.forEach(c => { html += `<div class="col">${this.cmdCard(c)}</div>`; });
    const container = document.getElementById('write-cards');
    if (container) container.innerHTML = `<div class="row row-cols-1 row-cols-lg-2 g-4">${html}</div>`;
    allCmds.forEach(c => {
      const btn = document.getElementById(`send-${c}`);
      if (btn) btn.addEventListener('click', () => this.sendCmd(c));
    });
  },
  cmdCard(cmd) {
    const def = U.COMMANDS[cmd];
    const fields = def.fields.map(f => {
      if (f.type === 'select') return `<div class="mb-3"><label class="form-label">${f.label}</label><select class="form-select" id="f-${cmd}-${f.id}">${f.opts.map(o => `<option>${o}</option>`).join('')}</select></div>`;
      if (f.type === 'multicheck') return `<div class="mb-3"><label class="form-label">${f.label}</label><div class="d-flex flex-wrap gap-2">${f.opts.map(o => `<div class="form-check form-check-inline"><input class="form-check-input" type="checkbox" id="f-${cmd}-${f.id}-${o}" value="${o}"><label class="form-check-label" for="f-${cmd}-${f.id}-${o}">${o}</label></div>`).join('')}</div></div>`;
      if (f.type === 'textarea') return `<div class="mb-3"><label class="form-label">${f.label}</label><textarea class="form-control" id="f-${cmd}-${f.id}" rows="2" placeholder="${f.ph || ''}"></textarea></div>`;
      return `<div class="mb-3"><label class="form-label">${f.label}</label><input class="form-control" id="f-${cmd}-${f.id}" type="${f.type || 'text'}" placeholder="${f.ph || ''}" ${f.auto ? `value="${U.now()}"` : ''}></div>`;
    }).join('');

    return `<div class="card shadow-sm" id="card-cmd-${cmd}">
      <div class="card-header d-flex justify-content-between align-items-start">
        <div>
          <div class="fw-bold" style="font-family:var(--font-mono);">${cmd}</div>
          <div class="text-muted" style="font-size:.9rem">${def.desc || ''}</div>
        </div>
        <button type="button" class="btn btn-primary btn-sm" id="send-${cmd}">Send</button>
      </div>
      <div class="card-body">
        <div class="mb-3"><label class="form-label">Device IDs</label><input class="form-control" id="f-${cmd}-device_ids" placeholder="1001234567,1001234568" value="${U.State.cfg('ids') || ''}"></div>
        ${fields}
        <div id="result-cmd-${cmd}" class="mt-3"></div>
      </div>
    </div>`;
  },
  buildPayload(cmd) {
    const def = U.COMMANDS[cmd]; const payload = { transactionid: U.uuid(), request_datetime: U.now(), privatekey: U.State.key() || '' };
    const ids = document.getElementById(`f-${cmd}-device_ids`);
    if (ids) payload.global_device_id_list = ids.value.split(',').map(s => s.trim()).filter(Boolean).map(Number);
    def.fields.forEach(f => {
      if (f.type === 'multicheck') {
        const checked = [...document.querySelectorAll(`#card-cmd-${cmd} .checkbox-item input:checked`)].map(el => Number(el.value));
        payload[f.id] = checked;
      } else {
        const el = document.getElementById(`f-${cmd}-${f.id}`);
        if (el) { let v = el.value; if (el.tagName === 'SELECT') v = v.split(' ')[0]; payload[f.id] = v || ''; }
      }
    });
    return payload;
  },
  async sendCmd(cmd) {
    const btn = document.getElementById(`send-${cmd}`);
    btn.classList.add('btn-loading'); btn.disabled = true;
    const payload = this.buildPayload(cmd);
    const res = document.getElementById(`result-cmd-${cmd}`);
    res.innerHTML = U.jsonBlock('Request Payload', payload, false);
    const r = await U.API.call(cmd, payload);
    btn.classList.remove('btn-loading'); btn.disabled = false;
    const ok = r.ok && r.data.status === 1;
    const txnId = r.data.transactionid || payload.transactionid;
    U.State.result('write', cmd, { payload, response: r.data, ok, txnId });
    res.innerHTML = `
${U.badge(ok ? '✓ PASS' : '✗ FAIL', ok ? 'pass' : 'fail')}
<div style="margin:8px 0">${U.jsonBlock('Request', payload, false)}${U.jsonBlock('Response', r.data, true)}</div>
<div id="pipe-${cmd}">${U.pipeline(this.PIPE_STEPS, ok ? 0 : -1)}</div>`;
    if (ok) { this.pollTxn(cmd, txnId, 1); }
    U.ReportSection.updateBadges();
  },
  async pollTxn(cmd, txnId, step) {
    if (step > 5) return;
    await new Promise(r => setTimeout(r, U.State.cfg('mock') ? 900 : 5000));
    const r = await U.API.pollTxn(txnId, step);
    const pipeEl = document.getElementById(`pipe-${cmd}`);
    if (pipeEl) pipeEl.innerHTML = U.pipeline(this.PIPE_STEPS, step);
    if (step < 5) this.pollTxn(cmd, txnId, step + 1);
  },
};

// ═══════════════════════════════════════════════════════
// SECTION 4: ON-DEMAND
// ═══════════════════════════════════════════════════════
U.OnDemandSection = {
  render() {
    const html = `
<div class="row row-cols-1 row-cols-xl-2 g-4">
  <div class="col">
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-start">
        <div class="fw-bold" style="font-family:var(--font-mono)">ON_DEMAND_DATA_READ</div>
        <button type="button" class="btn btn-primary btn-sm" id="btn-od-data">Run</button>
      </div>
      <div class="card-body">
        <div class="row g-3">
          <div class="col-12"><label class="form-label">Device ID</label><input class="form-control" id="od-device-id" placeholder="1001234567"></div>
          <div class="col-12"><label class="form-label">Data Type</label><select class="form-select" id="od-type"><option>INST</option><option>BILL</option><option>MBIL</option><option>LPRO</option><option>EVNT</option></select></div>
          <div class="col-md-6"><label class="form-label">Start Datetime</label><input class="form-control" id="od-start" type="datetime-local"></div>
          <div class="col-md-6"><label class="form-label">End Datetime</label><input class="form-control" id="od-end" type="datetime-local"></div>
        </div>
        <div id="od-data-result" class="mt-3"></div>
      </div>
    </div>
  </div>
  <div class="col">
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-start">
        <div class="fw-bold" style="font-family:var(--font-mono)">ON_DEMAND_PARAMETER_READ</div>
        <button type="button" class="btn btn-primary btn-sm" id="btn-od-param">Run</button>
      </div>
      <div class="card-body">
        <div class="mb-3"><label class="form-label">Device ID</label><input class="form-control" id="od-param-device" placeholder="1001234567"></div>
        <div class="mb-3"><label class="form-label">Parameter Type</label><select class="form-select" id="od-param-type">${U.OD_PARAM_TYPES.map(t => `<option>${t}</option>`).join('')}</select></div>
        <div id="od-param-result" class="mt-3"></div>
      </div>
    </div>
  </div>
  <div class="col">
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-start">
        <div class="fw-bold" style="font-family:var(--font-mono)">TRANSACTION_STATUS</div>
        <button type="button" class="btn btn-primary btn-sm" id="btn-od-txn">Fetch</button>
      </div>
      <div class="card-body">
        <div class="mb-3"><label class="form-label">Transaction ID(s) — comma-separated</label><textarea class="form-control" id="od-txn-ids" rows="2" placeholder="${U.uuid()}"></textarea></div>
        <div id="od-txn-result" class="mt-3"></div>
      </div>
    </div>
  </div>
  <div class="col">
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-start">
        <div class="fw-bold" style="font-family:var(--font-mono)">TRANSACTION_CANCEL</div>
        <button type="button" class="btn btn-danger btn-sm" id="btn-od-cancel">Cancel</button>
      </div>
      <div class="card-body">
        <div class="mb-3"><label class="form-label">Transaction ID</label><input class="form-control" id="od-cancel-id" placeholder="${U.uuid()}"></div>
        <div id="od-cancel-result" class="mt-3"></div>
      </div>
    </div>
  </div>
</div>`;
    const container = document.getElementById('ondemand-cards');
    if (container) container.innerHTML = html;
    const odType = document.getElementById('od-type');
    if (odType) odType.addEventListener('change', e => { const dis = e.target.value === 'INST'; document.getElementById('od-start').disabled = dis; document.getElementById('od-end').disabled = dis; });
    const btnData = document.getElementById('btn-od-data'); if (btnData) btnData.addEventListener('click', () => this.runDataRead());
    const btnParam = document.getElementById('btn-od-param'); if (btnParam) btnParam.addEventListener('click', () => this.runParamRead());
    const btnTxn = document.getElementById('btn-od-txn'); if (btnTxn) btnTxn.addEventListener('click', () => this.runTxnStatus());
    const btnCancel = document.getElementById('btn-od-cancel'); if (btnCancel) btnCancel.addEventListener('click', () => this.runCancel());
  },
  async runDataRead() {
    const type = document.getElementById('od-type').value;
    const btn = document.getElementById('btn-od-data'); btn.classList.add('btn-loading'); btn.disabled = true;
    const r = await (U.State.cfg('mock') ? new Promise(res => setTimeout(() => res({ ok: true, data: U.Mock.odData(type) }), 700)) : U.API.call('ON_DEMAND_DATA_READ', { global_device_id: document.getElementById('od-device-id').value, type }));
    btn.classList.remove('btn-loading'); btn.disabled = false;
    const el = document.getElementById('od-data-result');
    if (r.ok && r.data.data) {
      const rows = r.data.data; const tName = type === 'INST' ? 'INSTANTANEOUS_DATA' : type === 'BILL' ? 'BILLING_DATA' : type === 'MBIL' ? 'MONTHLY_BILLING_DATA' : type === 'LPRO' ? 'LOAD_PROFILE_DATA' : 'EVENTS';
      const v = U.validate(tName, rows);
      U.State.result('od', 'data_read', { type, rows, validation: v });
      el.innerHTML = `${U.badge('✓ ' + rows.length + ' rows', 'pass')} ${U.badge(v.fail + ' fails', 'fail')}<div style="margin-top:8px">${U.jsonBlock('Response Data', r.data, true)}</div>`;
    } else { el.innerHTML = `<div class="alert alert-danger"><span>❌</span><span>${(r.data || {}).message || 'Error'}</span></div>`; }
    U.ReportSection.updateBadges();
  },
  async runParamRead() {
    const type = document.getElementById('od-param-type').value;
    const btn = document.getElementById('btn-od-param'); btn.classList.add('btn-loading'); btn.disabled = true;
    const r = await (U.State.cfg('mock') ? new Promise(res => setTimeout(() => res({ ok: true, data: U.Mock.paramRead(type) }), 700)) : U.API.call('ON_DEMAND_PARAMETER_READ', { global_device_id: document.getElementById('od-param-device').value, type }));
    btn.classList.remove('btn-loading'); btn.disabled = false;
    U.State.result('od', 'param_read', { type, data: r.data });
    document.getElementById('od-param-result').innerHTML = U.jsonBlock('Parameter Response', r.data, true);
    U.ReportSection.updateBadges();
  },
  async runTxnStatus() {
    const ids = document.getElementById('od-txn-ids').value.split(',').map(s => s.trim()).filter(Boolean);
    if (!ids.length) { U.toast('Enter at least one transaction ID', 'warn'); return; }
    const btn = document.getElementById('btn-od-txn'); btn.classList.add('btn-loading'); btn.disabled = true;
    const results = await Promise.all(ids.map(id => U.State.cfg('mock') ? new Promise(r => setTimeout(() => r({ ok: true, data: U.Mock.txnStatus(id, Math.floor(Math.random() * 6)) }), 500)) : U.API.pollTxn(id)));
    btn.classList.remove('btn-loading'); btn.disabled = false;
    document.getElementById('od-txn-result').innerHTML = results.map((r, i) => U.jsonBlock(`TXN ${ids[i]}`, r.data, i === 0)).join('');
    U.State.result('od', 'txn_status', { ids, results: results.map(r => r.data) });
    U.ReportSection.updateBadges();
  },
  async runCancel() {
    const id = document.getElementById('od-cancel-id').value.trim();
    if (!id) { U.toast('Enter a transaction ID', 'warn'); return; }
    if (!confirm(`Cancel transaction ${id}?`)) return;
    const btn = document.getElementById('btn-od-cancel'); btn.classList.add('btn-loading'); btn.disabled = true;
    const r = await (U.State.cfg('mock') ? new Promise(res => setTimeout(() => res({ ok: true, data: { status: 1, message: 'Transaction cancelled', transactionid: id } }), 600)) : U.API.call('TRANSACTION_CANCEL', { transactionid: id }));
    btn.classList.remove('btn-loading'); btn.disabled = false;
    document.getElementById('od-cancel-result').innerHTML = U.jsonBlock('Cancel Response', r.data, true);
    U.toast(r.data.status === 1 ? 'Transaction cancelled' : 'Cancel failed', r.data.status === 1 ? 'pass' : 'fail');
    U.State.result('od', 'txn_cancel', { id, data: r.data });
    U.ReportSection.updateBadges();
  },
};

// ═══════════════════════════════════════════════════════
// SECTION 5: REPORT
// ═══════════════════════════════════════════════════════
U.ReportSection = {
  updateBadges() {
    const res = U.State.allResults();
    const readKeys = Object.keys(U.SCHEMAS); const readDone = readKeys.filter(k => res.read[k]).length;
    const writeDone = Object.keys(U.COMMANDS).filter(k => res.write[k]).length;
    const odDone = Object.keys(res.od).length;
    const readTotal = readKeys.length;
    const nb = document.getElementById('nbadge-read');
    if (nb) {
      if (readDone === 0) { nb.textContent = `0/${readTotal}`; nb.className = 'nav-badge'; }
      else { nb.textContent = `${readDone}/${readTotal}`; nb.className = `nav-badge ${readDone === readTotal ? 'pass' : 'warn'}`; }
    }
    const writeTotal = Object.keys(U.COMMANDS).length;
    const wb = document.getElementById('nbadge-write');
    if (wb) {
      if (writeDone === 0) { wb.textContent = `0/${writeTotal}`; wb.className = 'nav-badge'; }
      else { wb.textContent = `${writeDone}/${writeTotal}`; wb.className = `nav-badge ${writeDone === writeTotal ? 'pass' : 'warn'}`; }
    }
    const ob = document.getElementById('nbadge-ondemand');
    if (ob) {
      if (odDone === 0) { ob.textContent = '0/4'; ob.className = 'nav-badge'; }
      else { ob.textContent = `${odDone}/4`; ob.className = `nav-badge ${odDone >= 4 ? 'pass' : 'warn'}`; }
    }
    const rb = document.getElementById('nbadge-report');
    if (rb) {
      const s = U.ReportSection.score();
      if (s.total === 0) { rb.textContent = '0%'; rb.className = 'nav-badge'; }
      else { rb.textContent = `${s.pct}%`; rb.className = `nav-badge ${s.pct === 100 ? 'pass' : s.pct >= 50 ? 'warn' : 'fail'}`; }
    }
  },
  score() {
    const res = U.State.allResults(); let total = 0, passed = 0, items = [];
    // Read tests
    Object.keys(U.SCHEMAS).forEach(k => { total++; const r = res.read[k]; if (r) { const ok = r.validation.fail === 0; if (ok) passed++; items.push({ label: k, status: r.validation.fail > 0 ? 'fail' : r.validation.warn > 0 ? 'warn' : 'pass', section: 'Read' }); } else { items.push({ label: k, status: 'muted', section: 'Read' }); } });
    // Write tests
    Object.keys(U.COMMANDS).forEach(k => { total++; const r = res.write[k]; if (r) { if (r.ok) passed++; items.push({ label: k, status: r.ok ? 'pass' : 'fail', section: 'Write' }); } else { items.push({ label: k, status: 'muted', section: 'Write' }); } });
    // OD tests
    ['data_read', 'param_read', 'txn_status', 'txn_cancel'].forEach(k => { total++; const r = res.od[k]; if (r) { passed++; items.push({ label: k, status: 'pass', section: 'On-Demand' }); } else { items.push({ label: k, status: 'muted', section: 'On-Demand' }); } });
    return { pct: total ? Math.round((passed / total) * 100) : 0, total, passed, items };
  },
  render() {
    const { pct, total, passed, items } = this.score();
    const res = U.State.allResults();
    const readDone = Object.keys(U.SCHEMAS).filter(k => res.read[k]).length;
    const writeDone = Object.keys(U.COMMANDS).filter(k => res.write[k]).length;
    const odDone = Object.keys(res.od).length;
    const critFails = items.filter(i => i.status === 'fail');
    const container = document.getElementById('report-content');
    if (!container) return;
    container.innerHTML = `
${critFails.length ? `<div class="alert alert-danger mb-4"><strong>${critFails.length} critical failure(s):</strong> ${critFails.map(f => f.label).join(', ')}</div>` : ''}
<div class="row row-cols-1 row-cols-md-3 g-4 mb-4">
  <div class="col"><div class="card shadow-sm p-3 h-100"><div class="text-muted small mb-2">Read Tables</div><div class="h3 mb-1">${readDone}/7</div><div class="text-muted small">UDIL database tables validated</div></div></div>
  <div class="col"><div class="card shadow-sm p-3 h-100"><div class="text-muted small mb-2">Write Commands</div><div class="h3 mb-1">${writeDone}/16</div><div class="text-muted small">API commands tested</div></div></div>
  <div class="col"><div class="card shadow-sm p-3 h-100"><div class="text-muted small mb-2">On-Demand Tests</div><div class="h3 mb-1">${odDone}/4</div><div class="text-muted small">Live data read tests</div></div></div>
</div>
<div class="card shadow-sm">
  <div class="card-body">
    <div class="table-responsive">
      <table class="table table-striped align-middle mb-0">
        <thead><tr><th>Test Name</th><th>Section</th><th>Status</th></tr></thead>
        <tbody>${items.map(i => `<tr>
          <td class="text-monospace small">${i.label}</td>
          <td><span class="badge bg-info text-dark">${i.section}</span></td>
          <td>${i.status === 'muted' ? U.badge('Not Run', 'muted') : U.badge(i.status === 'pass' ? '✓ PASS' : i.status === 'fail' ? '✗ FAIL' : '⚠ WARN', i.status)}</td>
        </tr>`).join('')}</tbody>
      </table>
    </div>
  </div>
</div>`;
  },
  exportJSON() {
    const data = { version: U.VER, tested_at: new Date().toISOString(), mdc_url: U.State.cfg('url'), score: this.score(), results: U.State.allResults() };
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
    const a = document.createElement('a'); a.href = URL.createObjectURL(blob); a.download = `udil_report_${Date.now()}.json`; a.click();
  },
};

// ═══════════════════════════════════════════════════════
// APP INIT
// ═══════════════════════════════════════════════════════
U.App = {
  init() {
    U.State.init();
    // Mock toggle sync
    const mt = document.getElementById('toggle-mock');
    mt.checked = U.State.cfg('mock');
    mt.addEventListener('change', e => { U.State.cfg('mock', e.target.checked); const p = document.getElementById('mock-pill'); p.className = e.target.checked ? 'mock-pill' : 'mock-pill hidden'; });
    const mp = document.getElementById('mock-pill'); if (mp) mp.className = U.State.cfg('mock') ? 'mock-pill' : 'mock-pill hidden';
    U.ConfigSection.bindEvents();
    // Nav
    document.querySelectorAll('.nav-item').forEach(btn => {
      btn.addEventListener('click', () => {
        const sec = btn.dataset.section; U.navigate(sec);
        if (sec === 'config') U.ConfigSection.render();
        else if (sec === 'read') U.ReadSection.render();
        else if (sec === 'write') U.WriteSection.render();
        else if (sec === 'ondemand') U.OnDemandSection.render();
        else if (sec === 'report') U.ReportSection.render();
      });
    });
    // Sidebar toggle
    const menuBtn = document.getElementById('menu-btn'); const sidebar = document.getElementById('sidebar');
    if (menuBtn) menuBtn.addEventListener('click', () => sidebar.classList.toggle('collapsed'));
    // Report buttons
    const exportBtn = document.getElementById('btn-export-json'); if (exportBtn) exportBtn.addEventListener('click', () => U.ReportSection.exportJSON());
    const printBtn = document.getElementById('btn-print-pdf'); if (printBtn) printBtn.addEventListener('click', () => window.print());
    const refreshBtn = document.getElementById('btn-refresh-report'); if (refreshBtn) refreshBtn.addEventListener('click', () => U.ReportSection.render());
    // Modal close
    const modalClose = document.getElementById('modal-close'); if (modalClose) modalClose.addEventListener('click', U.closeModal);
    const modalOverlay = document.getElementById('modal-overlay'); if (modalOverlay) modalOverlay.addEventListener('click', e => { if (e.target === e.currentTarget) U.closeModal(); });
    // Render initial section (config)
    U.ConfigSection.render();
    U.ReportSection.updateBadges();
    // If previously authed, show status
    if (U.State.key()) {
      const pill = document.getElementById('auth-pill'); if (pill) { pill.className = 'auth-pill connected'; }
      const dot = document.getElementById('auth-dot'); if (dot) dot.className = 'auth-dot connected';
      const at = document.getElementById('auth-text'); if (at) at.textContent = 'Authenticated';
    }
    console.log(`%cUDIL Tester ${U.VER}`, 'color:#00d4aa;font-weight:bold;font-size:14px');
  },
};

document.addEventListener('DOMContentLoaded', () => U.App.init());
