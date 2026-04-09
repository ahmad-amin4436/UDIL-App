'use strict';
/* ═══════════════════════════════════════════════════════
   UDIL COMPLIANCE TESTER  |  PITC/UDIL/3.3-25029017
   ═══════════════════════════════════════════════════════ */

// Global namespace
const U = window.U = {};

// ─── CONSTANTS ──────────────────────────────────────────
U.VER = 'PITC/UDIL/3.3-25029017';
U.TITLES = { config: 'Configuration', read: 'Read Tests', write: 'Write Tests', ondemand: 'On-Demand Tests', report: 'Test Report' };
U.EVENT_CODES = new Set([101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332]);
U.OD_PARAM_TYPES = ['AUXR', 'DVTM', 'SANC', 'LSCH', 'TIOU', 'IPPO', 'MDSM', 'OPPO', 'WSIM', 'MSIM', 'MTST', 'DMDT', 'MDI', 'OVFC', 'UVFC', 'OCFC', 'OLFC', 'VUFC', 'PFFC', 'CUFC', 'HAPF', 'PMAL'];

// ─── TABLE SCHEMAS ──────────────────────────────────────
const F = (n, t, r, o = {}) => ({ n, t, r, ...o });
U.SCHEMAS = {
  INSTANTANEOUS_DATA: {
    label: 'Instantaneous Data', desc: 'Real-time meter snapshot', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true), F('mdc_read_datetime', 'datetime', true),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }),
      F('voltage_R', 'decimal', true, { dp: 3 }), F('voltage_Y', 'decimal', false, { dp: 3, p3: true }), F('voltage_B', 'decimal', false, { dp: 3, p3: true }),
      F('current_R', 'decimal', true, { dp: 3 }), F('current_Y', 'decimal', false, { dp: 3, p3: true }), F('current_B', 'decimal', false, { dp: 3, p3: true }),
      F('power_factor_R', 'decimal', true, { dp: 3 }), F('power_factor_Y', 'decimal', false, { dp: 3, p3: true }), F('power_factor_B', 'decimal', false, { dp: 3, p3: true }),
      F('frequency', 'decimal', true, { dp: 3 }), F('aggregate_active_pwr_pos', 'decimal', true, { dp: 3 }), F('aggregate_active_pwr_neg', 'decimal', true, { dp: 3 }),
      F('aggregate_reactive_pwr_pos', 'decimal', true, { dp: 3 }), F('aggregate_reactive_pwr_neg', 'decimal', true, { dp: 3 }),
      F('cumulative_energy_wh_pos', 'decimal', true, { dp: 3 }), F('cumulative_energy_wh_neg', 'decimal', true, { dp: 3 }), F('max_demand_kw', 'decimal', true, { dp: 3 }),
    ]
  },
  BILLING_DATA: {
    label: 'Billing Data', desc: 'On-demand billing snapshot per meter', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true, { pk: true }), F('mdc_read_datetime', 'datetime', true),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }), F('billing_date', 'datetime', true),
      F('cumulative_energy_wh_pos', 'decimal', true, { dp: 3 }), F('cumulative_energy_wh_neg', 'decimal', true, { dp: 3 }),
      F('cumulative_energy_varh_pos', 'decimal', true, { dp: 3 }), F('cumulative_energy_varh_neg', 'decimal', true, { dp: 3 }),
      F('max_demand_kw', 'decimal', true, { dp: 3 }), F('max_demand_kw_datetime', 'datetime', true),
    ]
  },
  MONTHLY_BILLING_DATA: {
    label: 'Monthly Billing Data', desc: 'End-of-month billing aggregates', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true, { pk: true }), F('mdc_read_datetime', 'datetime', true),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }), F('billing_month', 'varchar', true, { max: 7 }),
      F('cumulative_energy_wh_pos', 'decimal', true, { dp: 3 }), F('cumulative_energy_wh_neg', 'decimal', true, { dp: 3 }),
      F('max_demand_kw', 'decimal', true, { dp: 3 }), F('max_demand_kw_datetime', 'datetime', true),
    ]
  },
  LOAD_PROFILE_DATA: {
    label: 'Load Profile Data', desc: 'Interval-based energy consumption', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true, { pk: true }), F('mdc_read_datetime', 'datetime', true),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }), F('interval_minutes', 'int', true),
      F('cumulative_energy_wh_pos', 'decimal', true, { dp: 3 }), F('cumulative_energy_wh_neg', 'decimal', true, { dp: 3 }),
      F('cumulative_energy_varh_pos', 'decimal', true, { dp: 3 }), F('cumulative_energy_varh_neg', 'decimal', true, { dp: 3 }),
    ]
  },
  EVENTS: {
    label: 'Events', desc: 'Meter event log (codes 101–332)', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true, { pk: true }), F('mdc_read_datetime', 'datetime', true),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }),
      F('event_code', 'int', true, { eventCode: true }), F('event_description', 'varchar', true, { max: 255 }),
    ]
  },
  METER_VISUALS: {
    label: 'Meter Visuals', desc: 'Parameter & visual data store', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true), F('mdc_read_datetime', 'datetime', true),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }),
      F('aggregate_active_pwr_pos', 'decimal', true, { dp: 3 }), F('aggregate_active_pwr_neg', 'decimal', true, { dp: 3 }),
      F('relay_status', 'bit', true), F('meter_status', 'bit', true), F('signal_strength', 'int', false),
      F('sim_number', 'varchar', false, { max: 15 }), F('load_limit', 'decimal', false, { dp: 3 }),
    ]
  },
  DEVICE_COMMUNICATION_HISTORY: {
    label: 'Device Comm. History', desc: 'MDC↔meter session logs', fields: [
      F('global_device_id', 'bigint', true, { pk: true }), F('mdc_datetime', 'datetime', true, { pk: true }),
      F('db_datetime', 'datetime', true), F('is_synced', 'bit', true, { mustBe: 0 }),
      F('message_log', 'varchar', true, { msgLog: true }), F('signal_strength', 'int', false), F('communication_status', 'bit', true),
    ]
  },
};

// ─── WRITE COMMAND DEFINITIONS ──────────────────────────
U.COMMANDS = {
  AUX_RELAY_OPERATIONS: {
    group: 'A', label: 'Aux Relay Operations', fields: [
      { id: 'relay_operate', label: 'Relay Operate', type: 'select', opts: ['0 — Open Relay', '1 — Close Relay'] },
    ]
  },
  SANCTIONED_LOAD_CONTROL: {
    group: 'A', label: 'Sanctioned Load Control', fields: [
      { id: 'load_limit', label: 'Load Limit (kW)', type: 'number', ph: 'e.g. 5.000' }, { id: 'maximum_retries', label: 'Max Retries', type: 'number', ph: 'e.g. 3' },
      { id: 'retry_interval', label: 'Retry Interval (min)', type: 'number', ph: 'e.g. 15' }, { id: 'threshold_duration', label: 'Threshold Duration (min)', type: 'number', ph: 'e.g. 5' },
      { id: 'retry_clear_interval', label: 'Retry Clear Interval (min)', type: 'number', ph: 'e.g. 60' },
    ]
  },
  LOAD_SHEDDING_SCHEDULING: {
    group: 'A', label: 'Load Shedding Scheduling', fields: [
      { id: 'start_datetime', label: 'Start Datetime', type: 'datetime' }, { id: 'end_datetime', label: 'End Datetime', type: 'datetime' },
      { id: 'load_shedding_slabs', label: 'Shedding Slabs (JSON)', type: 'textarea', ph: '[{"action_time":"08:00:00","relay_operate":0},...]' },
    ]
  },
  TIME_SYNCHRONIZATION: {
    group: 'B', label: 'Time Synchronization', fields: [
      { id: 'request_datetime', label: 'Request Datetime', type: 'datetime', auto: true },
    ]
  },
  UPDATE_TIME_OF_USE: {
    group: 'B', label: 'Update Time of Use', fields: [
      { id: 'activation_datetime', label: 'Activation Datetime', type: 'datetime' },
      { id: 'day_profile', label: 'Day Profile (JSON)', type: 'textarea', ph: '[{"period":1,"start":"00:00","tariff":1}]' },
      { id: 'week_profile', label: 'Week Profile (JSON)', type: 'textarea', ph: '{"mon":1,"tue":1,"wed":1,"thu":1,"fri":1,"sat":2,"sun":2}' },
    ]
  },
  METER_DATA_SAMPLING: {
    group: 'B', label: 'Meter Data Sampling', fields: [
      { id: 'data_type', label: 'Data Type', type: 'select', opts: ['INST', 'BILL', 'LPRO'] },
      { id: 'sampling_interval', label: 'Sampling Interval (min)', type: 'number', ph: '1–59' },
      { id: 'sampling_initial_time', label: 'Initial Time (HH:mm:ss)', type: 'text', ph: '00:00:00' },
    ]
  },
  ACTIVATE_METER_OPTICAL_PORT: {
    group: 'B', label: 'Activate Optical Port', fields: [
      { id: 'optical_port_on_datetime', label: 'Port ON Datetime', type: 'datetime' }, { id: 'optical_port_off_datetime', label: 'Port OFF Datetime', type: 'datetime' },
    ]
  },
  UPDATE_WAKE_UP_SIM_NUMBER: {
    group: 'B', label: 'Update Wake-Up SIM', fields: [
      { id: 'sim_1', label: 'SIM 1 (03XXXXXXXXX)', type: 'text', ph: '03001234567' }, { id: 'sim_2', label: 'SIM 2', type: 'text', ph: '03001234568' },
      { id: 'sim_3', label: 'SIM 3', type: 'text', ph: '03001234569' },
    ]
  },
  UPDATE_METER_STATUS: {
    group: 'B', label: 'Update Meter Status', fields: [
      { id: 'meter_status', label: 'Meter Status', type: 'select', opts: ['0 — Inactive', '1 — Active'] },
    ]
  },
  UPDATE_MDI_RESET_DATE: {
    group: 'B', label: 'Update MDI Reset Date', fields: [
      { id: 'mdi_reset_date', label: 'Reset Date (1–28)', type: 'number', ph: '28' }, { id: 'mdi_reset_time', label: 'Reset Time (HH:mm:ss)', type: 'text', ph: '00:00:00' },
    ]
  },
  DEVICE_CREATION: {
    group: 'C', label: 'Device Creation', fields: [
      { id: 'dsn', label: 'DSN (Device Serial No.)', type: 'text', ph: 'L1234567890' }, { id: 'global_device_id', label: 'Global Device ID', type: 'number', ph: '1001234567' },
      { id: 'device_type', label: 'Device Type', type: 'select', opts: ['1 — Single Phase', '2 — Three Phase LT', '3 — Three Phase HT', '4 — CT Meter', '5 — Prepaid'] },
      { id: 'communication_mode', label: 'Comm Mode', type: 'select', opts: ['1 — GPRS', '2 — RF', '3 — PLC', '4 — Optical', '5 — Other'] },
      { id: 'phase', label: 'Phase', type: 'select', opts: ['1 — Single', '2 — Two', '3 — Three'] },
      { id: 'sim_number', label: 'SIM Number', type: 'text', ph: '03001234567' }, { id: 'mdi_reset_date', label: 'MDI Reset Date', type: 'number', ph: '28' },
      { id: 'communication_interval', label: 'Comm Interval (min)', type: 'number', ph: '30' },
    ]
  },
  UPDATE_DEVICE_METADATA: {
    group: 'C', label: 'Update Device Metadata', fields: [
      { id: 'device_type', label: 'Device Type', type: 'select', opts: ['1 — Single Phase', '2 — Three Phase LT', '3 — Three Phase HT', '4 — CT Meter', '5 — Prepaid'] },
      { id: 'communication_mode', label: 'Comm Mode', type: 'select', opts: ['1 — GPRS', '2 — RF', '3 — PLC', '4 — Optical', '5 — Other'] },
      { id: 'sim_number', label: 'SIM Number', type: 'text', ph: '03001234567' }, { id: 'mdi_reset_date', label: 'MDI Reset Date', type: 'number', ph: '28' },
    ]
  },
  UPDATE_IP_PORT: {
    group: 'C', label: 'Update IP / Port', fields: [
      { id: 'primary_ip_address', label: 'Primary IP', type: 'text', ph: '192.168.1.100' }, { id: 'secondary_ip_address', label: 'Secondary IP', type: 'text', ph: '192.168.1.101' },
      { id: 'primary_port', label: 'Primary Port', type: 'number', ph: '4059' }, { id: 'secondary_port', label: 'Secondary Port', type: 'number', ph: '4060' },
    ]
  },
  APMS_TRIPPING_EVENTS: {
    group: 'D', label: 'APMS Tripping Events', fields: [
      { id: 'type', label: 'Event Type', type: 'select', opts: ['OVFC', 'UVFC', 'OCFC', 'OLFC', 'VUFC', 'PFFC', 'CUFC', 'HAPF'] },
      { id: 'critical_event_threshold_limit', label: 'Critical Threshold', type: 'number', ph: 'e.g. 260' }, { id: 'critical_event_log_time', label: 'Critical Log Time (s)', type: 'number', ph: '5' },
      { id: 'tripping_event_threshold_limit', label: 'Tripping Threshold', type: 'number', ph: 'e.g. 270' }, { id: 'tripping_event_log_time', label: 'Tripping Log Time (s)', type: 'number', ph: '3' },
      { id: 'enable_tripping', label: 'Enable Tripping', type: 'select', opts: ['0 — Disabled', '1 — Enabled'] },
    ]
  },
  PARAMETERIZATION_CANCELLATION: {
    group: 'D', label: 'Parameterization Cancel', fields: [
      { id: 'type', label: 'Cancel Type', type: 'select', opts: ['SANC', 'LSCH', 'TIOU', 'OVFC', 'UVFC', 'OCFC', 'OLFC', 'VUFC', 'PFFC', 'CUFC', 'HAPF'] },
    ]
  },
  UPDATE_MAJOR_ALARMS: {
    group: 'D', label: 'Update Major Alarms', fields: [
      { id: 'event_codes', label: 'Event Codes (max 10)', type: 'multicheck', opts: [101, 102, 103, 201, 202, 203, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310] },
    ]
  },
};

// ─── STATE ──────────────────────────────────────────────
U.State = {
  _d: {},
  init() {
    try { this._d = JSON.parse(localStorage.getItem('udil_s') || '{}'); } catch (e) { this._d = {}; }
    this._d.cfg = this._d.cfg || { url: '', user: '', pass: '', code: '', ids: '1001234567,1001234568', mode: 'webservice', mock: true };
    this._d.auth = this._d.auth || { key: null, at: null };
    this._d.res = this._d.res || { read: {}, write: {}, od: {} };
  },
  save() { try { localStorage.setItem('udil_s', JSON.stringify(this._d)); } catch (e) { } },
  cfg(k, v) { if (v !== undefined) { this._d.cfg[k] = v; this.save(); } return this._d.cfg[k]; },
  key(v) { if (v !== undefined) { this._d.auth.key = v; this._d.auth.at = Date.now(); this.save(); } return this._d.auth.key; },
  result(sec, k, v) { if (v !== undefined) { this._d.res[sec][k] = v; this.save(); } return this._d.res[sec][k]; },
  allResults() { return this._d.res; },
  reset() { localStorage.removeItem('udil_s'); this.init(); },
};

// ─── MOCK DATA ──────────────────────────────────────────
U.Mock = {
  devices: [
    { global_device_id: 1001234567, phase: 1, dsn: 'L1234567890', device_type: 1 },
    { global_device_id: 1001234568, phase: 3, dsn: 'T9876543210', device_type: 2 },
    { global_device_id: 1001234569, phase: 1, dsn: 'L5551112233', device_type: 1 },
  ],
  dt(offset = 0) {
    const d = new Date(Date.now() - offset * 1000);
    return d.toISOString().replace('T', ' ').slice(0, 19);
  },
  auth() { return { status: 1, message: 'Auth successful', privatekey: 'udil_pk_3f8a2b9c4d1e5f7a_' + Date.now() }; },
  table(name) {
    const rows = [];
    this.devices.forEach(dev => {
      const base = {
        global_device_id: dev.global_device_id, mdc_datetime: this.dt(10),
        mdc_read_datetime: this.dt(5), db_datetime: this.dt(3), is_synced: 0,
      };
      if (name === 'INSTANTANEOUS_DATA') {
        const is3 = (dev.phase === 3);
        rows.push({
          ...base, voltage_R: 230.456, voltage_Y: is3 ? 229.321 : null, voltage_B: is3 ? 231.123 : null,
          current_R: 5.234, current_Y: is3 ? 4.891 : null, current_B: is3 ? 5.012 : null,
          power_factor_R: 0.952, power_factor_Y: is3 ? 0.938 : null, power_factor_B: is3 ? 0.945 : null,
          frequency: 50.023, aggregate_active_pwr_pos: 1.205, aggregate_active_pwr_neg: 0.000,
          aggregate_reactive_pwr_pos: 0.421, aggregate_reactive_pwr_neg: 0.000,
          cumulative_energy_wh_pos: 4821.456, cumulative_energy_wh_neg: 0.000, max_demand_kw: 2.150
        });
      } else if (name === 'BILLING_DATA') {
        rows.push({
          ...base, billing_date: this.dt(86400), cumulative_energy_wh_pos: 4700.000,
          cumulative_energy_wh_neg: 0.000, cumulative_energy_varh_pos: 1640.000, cumulative_energy_varh_neg: 0.000,
          max_demand_kw: 2.100, max_demand_kw_datetime: this.dt(3600)
        });
      } else if (name === 'MONTHLY_BILLING_DATA') {
        rows.push({
          ...base, billing_month: '2026-03', cumulative_energy_wh_pos: 120450.000,
          cumulative_energy_wh_neg: 0.000, max_demand_kw: 5.200, max_demand_kw_datetime: this.dt(172800)
        });
      } else if (name === 'LOAD_PROFILE_DATA') {
        for (let i = 0; i < 3; i++) {
          rows.push({
            ...base, mdc_datetime: this.dt(i * 1800), interval_minutes: 30,
            cumulative_energy_wh_pos: 4800.000 + i * 15, cumulative_energy_wh_neg: 0.000,
            cumulative_energy_varh_pos: 1680.000 + i * 5, cumulative_energy_varh_neg: 0.000
          });
        }
      } else if (name === 'EVENTS') {
        rows.push({ ...base, event_code: 101, event_description: 'Power On' });
        rows.push({ ...base, mdc_datetime: this.dt(600), event_code: 201, event_description: 'Cover Tamper Detected' });
      } else if (name === 'METER_VISUALS') {
        rows.push({
          ...base, aggregate_active_pwr_pos: 1.205, aggregate_active_pwr_neg: 0.000,
          relay_status: 1, meter_status: 1, signal_strength: 18, sim_number: '03001234567', load_limit: 10.000
        });
      } else if (name === 'DEVICE_COMMUNICATION_HISTORY') {
        rows.push({
          ...base, message_log: `INST? ${this.dt(10)}\nBILL? ${this.dt(300)}\nLPRO? ${this.dt(600)}\nEVNT? ${this.dt(900)}\nDC ${this.dt(5)}`,
          signal_strength: 21, communication_status: 1
        });
      }
    });
    return rows;
  },
  writeResp(cmd) {
    return { status: 1, message: 'Command accepted and queued', transactionid: U.uuid(), timestamp: this.dt() };
  },
  txnStatus(txnId, step = 2) {
    return {
      transactionid: txnId, transaction_status: step, status_description: ['Waiting', 'Processing', 'Sent', 'Connected', 'Command Sent', 'Executed'][step] || 'Unknown',
      indv_status: this.devices.slice(0, 2).map(d => ({ global_device_id: d.global_device_id, status: step >= 5 ? 1 : 0, message: step >= 5 ? 'Complete' : 'In Progress' }))
    };
  },
  odData(type) { return { status: 1, data: this.table(type === 'INST' ? 'INSTANTANEOUS_DATA' : type === 'BILL' ? 'BILLING_DATA' : type === 'MBIL' ? 'MONTHLY_BILLING_DATA' : type === 'LPRO' ? 'LOAD_PROFILE_DATA' : 'EVENTS') }; },
  paramRead(type) { return { status: 1, type, data: { global_device_id: 1001234567, parameter_type: type, value: 'Active', updated_at: this.dt() } }; },
};

// ─── UTILITIES ──────────────────────────────────────────
U.uuid = () => 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => { const r = Math.random() * 16 | 0; return (c === 'x' ? r : r & 0x3 | 0x8).toString(16); });
U.now = () => { const d = new Date(); return d.toISOString().replace('T', ' ').slice(0, 19); };
U.fmt = v => v === null || v === undefined ? 'NULL' : String(v);
U.dp = v => { const s = String(v); const i = s.indexOf('.'); return i < 0 ? 0 : s.length - i - 1; };

// ─── VALIDATOR ──────────────────────────────────────────
U.validate = (tableName, rows) => {
  const schema = U.SCHEMAS[tableName]; if (!schema) return { checks: [], pass: 0, fail: 0, warn: 0 };
  const checks = []; let pass = 0, fail = 0, warn = 0;
  // Duplicate mdc_datetime check
  const dtVals = rows.map(r => r.mdc_datetime);
  if (dtVals.length > 1 && new Set(dtVals).size === 1) { checks.push({ rule: 'Unique mdc_datetime', status: 'warn', msg: 'All rows have identical mdc_datetime — possible fake/computed timestamp' }); warn++; }
  rows.forEach((row, ri) => {
    const is3 = row.phase === 3 || (rows.find(r => r.global_device_id === row.global_device_id) || {}).phase === 3;
    schema.fields.forEach(f => {
      const val = row[f.n]; const label = `Row ${ri + 1} › ${f.n}`;
      // Required check
      if (f.r && (val === null || val === undefined || val === '')) {
        if (f.p3 && !is3) { checks.push({ rule: label, status: 'pass', msg: 'NULL (single-phase, 3-phase field)' }); pass++; }
        else { checks.push({ rule: label, status: 'fail', msg: 'REQUIRED field is NULL/missing' }); fail++; }
        return;
      }
      if (val === null || val === undefined) { checks.push({ rule: label, status: 'pass', msg: 'NULL (optional)' }); pass++; return; }
      // is_synced
      if (f.mustBe !== undefined && val !== f.mustBe) { checks.push({ rule: label, status: 'fail', msg: `Expected ${f.mustBe}, got ${val}. MDC must insert 0.` }); fail++; return; }
      // Decimal places
      if (f.dp !== undefined && U.dp(val) > f.dp) { checks.push({ rule: label, status: 'fail', msg: `Exceeds ${f.dp} decimal places (got ${U.dp(val)})` }); fail++; return; }
      // Varchar max
      if (f.max && String(val).length > f.max) { checks.push({ rule: label, status: 'fail', msg: `Varchar exceeds max ${f.max} chars (got ${String(val).length})` }); fail++; return; }
      // Event code
      if (f.eventCode && !U.EVENT_CODES.has(Number(val))) { checks.push({ rule: label, status: 'fail', msg: `Event code ${val} not in known range 101–332` }); fail++; return; }
      // db_datetime lag
      if (f.n === 'db_datetime' && row.mdc_read_datetime) {
        const lag = (new Date(val) - new Date(row.mdc_read_datetime)) / 60000;
        if (lag > 5) { checks.push({ rule: label, status: 'warn', msg: `db_datetime is ${lag.toFixed(1)} min after mdc_read_datetime (>5 min)` }); warn++; }
        else { checks.push({ rule: label, status: 'pass', msg: `Insert lag ${(lag * 60).toFixed(0)}s ✓` }); pass++; }
        return;
      }
      // Message log
      if (f.msgLog) {
        const log = String(val); const hasCodes = /INST\?|BILL\?|MBIL\?|LPRO\?|EVNT\?|DC/.test(log);
        if (!hasCodes) { checks.push({ rule: label, status: 'fail', msg: 'message_log missing valid UDIL command codes' }); fail++; }
        else { checks.push({ rule: label, status: 'pass', msg: 'message_log contains valid UDIL codes ✓' }); pass++; }
        return;
      }
      checks.push({ rule: label, status: 'pass', msg: `${U.fmt(val)}` }); pass++;
    });
  });
  return { checks, pass, fail, warn };
};

// ─── API CLIENT ──────────────────────────────────────────
U.API = {
  isMock() { return U.State.cfg('mock'); },
  headers() { return { username: U.State.cfg('user'), password: U.State.cfg('pass'), code: U.State.cfg('code'), privatekey: U.State.key() || '', 'Content-Type': 'application/json' }; },
  async call(endpoint, payload) {
    if (this.isMock()) return new Promise(r => setTimeout(() => r({ ok: true, data: U.Mock.writeResp(endpoint) }), 800));
    try {
      const url = `${U.State.cfg('url').replace(/\/+$/, '')}/${endpoint}`;
      const r = await fetch(url, { method: 'POST', headers: this.headers(), body: JSON.stringify(payload) });
      const text = await r.text();
      try { return { ok: r.ok, status: r.status, data: JSON.parse(text) }; }
      catch (e) { return { ok: false, status: r.status, data: { status: 0, message: `API returned non-JSON (HTTP ${r.status}). Check Base URL.` } }; }
    } catch (e) { return { ok: false, status: 0, data: { status: 0, message: e.message } }; }
  },
  async auth(cfg) {
    if (this.isMock()) return new Promise(r => setTimeout(() => r({ ok: true, data: U.Mock.auth() }), 1000));
    try {
      let base = cfg.url.replace(/\/+$/, '');
      if (base.toLowerCase().endsWith('/authorization_service')) base = base.slice(0, -22);
      const r = await fetch(`${base}/AUTHORIZATION_SERVICE`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ username: cfg.user, password: cfg.pass, code: cfg.code }) });
      const text = await r.text();
      try { return { ok: r.ok, data: JSON.parse(text) }; }
      catch (e) { return { ok: false, data: { status: 0, message: `API returned non-JSON (HTTP ${r.status}). Check Base URL.` } }; }
    } catch (e) { return { ok: false, data: { status: 0, message: e.message } }; }
  },
  async fetchTable(name) {
    if (this.isMock()) return new Promise(r => setTimeout(() => r({ ok: true, data: { status: 1, rows: U.Mock.table(name) } }), 600));
    try {
      const ids = U.State.cfg('ids').split(',').map(s => s.trim()).filter(Boolean);
      const url = `${U.State.cfg('url').replace(/\/+$/, '')}/READ_TABLE`;
      const r = await fetch(url, { method: 'POST', headers: this.headers(), body: JSON.stringify({ table: name, device_ids: ids }) });
      const text = await r.text();
      try { return { ok: r.ok, data: JSON.parse(text) }; }
      catch (e) { return { ok: false, data: { status: 0, message: `API returned non-JSON (HTTP ${r.status}). Check Base URL.` } }; }
    } catch (e) { return { ok: false, data: { status: 0, message: e.message } }; }
  },
  async pollTxn(txnId, step = 0) {
    if (this.isMock()) return new Promise(r => setTimeout(() => r({ ok: true, data: U.Mock.txnStatus(txnId, step) }), 500));
    try {
      const url = `${U.State.cfg('url').replace(/\/+$/, '')}/TRANSACTION_STATUS`;
      const r = await fetch(url, { method: 'POST', headers: this.headers(), body: JSON.stringify({ transactionid: txnId }) });
      const text = await r.text();
      try { return { ok: r.ok, data: JSON.parse(text) }; }
      catch (e) { return { ok: false, data: { status: 0, message: `API returned non-JSON (HTTP ${r.status}). Check Base URL.` } }; }
    } catch (e) { return { ok: false, data: { status: 0, message: e.message } }; }
  },
};

/* END OF PART 1 — UI & App logic in Part 2 (udil-app.js) */
