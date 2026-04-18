<%@ Page Title="UDIL Tester - Configuration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="Default.aspx.cs" Inherits="UDIL._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content  mt-5">
        <div class="container-fluid">
            <section id="dashboard">
                <h2 class="section-header"><i class="bi bi-house-door"></i> Dashboard</h2>
                <div class="row">
                    <div class="col-lg-4 col-md-6 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title"><i class="bi bi-activity"></i> System Status</h5>
                                <p class="card-text">All systems operational.</p>
                                <span class="badge bg-success">Online</span>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title"><i class="bi bi-play-circle"></i> Active Tests</h5>
                                <p class="card-text">5 tests running.</p>
                                <span class="badge bg-warning">In Progress</span>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title"><i class="bi bi-clock-history"></i> Recent Logs</h5>
                                <p class="card-text">Last update: 2026-04-07</p>
                                <span class="badge bg-info">Updated</span>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section id="configuration">
                <h2 class="section-header"><i class="bi bi-gear"></i>  Configuration</h2>
                <div class="card">
                    
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtBaseUrl" class="form-label">Base URL of APIs</label>
                                <asp:TextBox ID="txtBaseUrl" runat="server" Text="http://116.58.46.245:4050/UIP" CssClass="form-control" placeholder="Enter base URL"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtConfigName" class="form-label">Configuration Name</label>
                                <asp:TextBox ID="txtConfigName" runat="server" CssClass="form-control" placeholder="Enter configuration name"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="ddlSavedConfigs" class="form-label">Saved Configurations</label>
                                <asp:DropDownList ID="ddlSavedConfigs" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSavedConfigs_SelectedIndexChanged">
                                    <asp:ListItem Text="-- Select Configuration --" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtTimeout" class="form-label">Timeout (seconds)</label>
                                <asp:TextBox ID="txtTimeout" runat="server" Text="60" CssClass="form-control" placeholder="Enter timeout in seconds"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12 mb-3">
                                <h6 class="text-primary">Database Configuration</h6>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <label for="txtDbServer" class="form-label">Database Server</label>
                                <asp:TextBox ID="txtDbServer" runat="server" Text="116.58.46.245" CssClass="form-control" placeholder="Database server IP"></asp:TextBox>
                            </div>
                            <div class="col-md-2 mb-3">
                                <label for="txtDbPort" class="form-label">Port</label>
                                <asp:TextBox ID="txtDbPort" runat="server" Text="4000" CssClass="form-control" placeholder="Port"></asp:TextBox>
                            </div>
                            <div class="col-md-3 mb-3">
                                <label for="txtDbName" class="form-label">Database Name</label>
                                <asp:TextBox ID="txtDbName" runat="server" Text="udil33" CssClass="form-control" placeholder="Database name"></asp:TextBox>
                            </div>
                            <div class="col-md-3 mb-3">
                                <label for="txtDbUid" class="form-label">Username</label>
                                <asp:TextBox ID="txtDbUid" runat="server" Text="accurate" CssClass="form-control" placeholder="Database username"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtDbPwd" class="form-label">Password</label>
                                <asp:TextBox ID="txtDbPwd" runat="server" Text="Accurate@123" CssClass="form-control" placeholder="Database password"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtDbProvider" class="form-label">Provider</label>
                                <asp:TextBox ID="txtDbProvider" runat="server" Text="MySql.Data.MySqlClient" CssClass="form-control" placeholder="Database provider"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:Button ID="btnSaveConfig" runat="server" Text="Save Configuration" CssClass="btn btn-success me-2" OnClick="btnSaveConfig_Click" />
                                <asp:Button ID="btnLoadConfig" runat="server" Text="Load Configuration" CssClass="btn btn-info me-2" OnClick="btnLoadConfig_Click" />
                                <asp:Button ID="btnDeleteConfig" runat="server" Text="Delete Configuration" CssClass="btn btn-danger me-2" OnClick="btnDeleteConfig_Click" />
                                <asp:Button ID="btnApplyConfig" runat="server" Text="Apply Configuration" CssClass="btn btn-primary" OnClick="btnApplyConfig_Click" />
                            </div>
                        </div>
                        <div class="mt-3">
                            <asp:Label ID="lblConfigMessage" runat="server" CssClass="text-info"></asp:Label>
                        </div>
                    </div>
                </div>
            </section>

            <section id="authorization">
                <h2 class="section-header"><i class="bi bi-shield-lock"></i>  Authorization</h2>
                <div class="card">
                    
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <label for="txtUsername" class="form-label">Username</label>
                                <asp:TextBox ID="txtUsername" runat="server" Text="accurate" CssClass="form-control" placeholder="Enter username"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtPassword" class="form-label">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" Text="Helloaccurate@987"  CssClass="form-control" placeholder="Enter password"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtCode" class="form-label">Code</label>
                                <asp:TextBox ID="txtCode" runat="server" Text="235" CssClass="form-control" placeholder="Enter code"></asp:TextBox>
                            </div>
                        </div>
                        <asp:Button ID="btnAuthorize" runat="server" Text="Authorize" CssClass="btn btn-primary" OnClick="btnAuthorize_Click" />
                        <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        <div id="auth-detail" class="mt-3">
                            <asp:Label ID="lblAuthMessage" runat="server" CssClass="text-info"></asp:Label>
                        </div>
                    </div>
                </div>
            </section>

           

            <section id="instantaneous-data">
                <h2 class="section-header">Instantaneous Data</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-graph-up"></i> Real-time Meter Data
                    </div>
                    <div class="card-body">
                        <form>
                            <div class="row">
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="voltageA" class="form-label">Voltage Phase A (V)</label>
                                    <input type="number" step="0.01" class="form-control" id="voltageA" placeholder="230.5">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="voltageB" class="form-label">Voltage Phase B (V)</label>
                                    <input type="number" step="0.01" class="form-control" id="voltageB" placeholder="231.2">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="voltageC" class="form-label">Voltage Phase C (V)</label>
                                    <input type="number" step="0.01" class="form-control" id="voltageC" placeholder="229.8">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="currentA" class="form-label">Current Phase A (A)</label>
                                    <input type="number" step="0.01" class="form-control" id="currentA" placeholder="15.3">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="currentB" class="form-label">Current Phase B (A)</label>
                                    <input type="number" step="0.01" class="form-control" id="currentB" placeholder="14.8">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="currentC" class="form-label">Current Phase C (A)</label>
                                    <input type="number" step="0.01" class="form-control" id="currentC" placeholder="16.1">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="frequency" class="form-label">Frequency (Hz)</label>
                                    <input type="number" step="0.01" class="form-control" id="frequency" placeholder="50.0">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="powerFactor" class="form-label">Power Factor</label>
                                    <input type="number" step="0.001" min="0" max="1" class="form-control" id="powerFactor" placeholder="0.95">
                                </div>
                                <div class="col-lg-4 col-md-6 mb-3">
                                    <label for="aggregatePower" class="form-label">Aggregate Power (kW)</label>
                                    <input type="number" step="0.01" class="form-control" id="aggregatePower" placeholder="5.2">
                                </div>
                                <div class="col-lg-6 mb-3">
                                    <label for="timestamp" class="form-label">Timestamp</label>
                                    <input type="datetime-local" class="form-control" id="timestamp">
                                </div>
                            </div>
                            <button type="submit" class="btn btn-primary">Submit Data</button>
                            <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        </form>
                        <hr>
                        <h5>Data Preview</h5>
                        <div class="table-responsive">
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Timestamp</th>
                                        <th>Voltage A (V)</th>
                                        <th>Voltage B (V)</th>
                                        <th>Voltage C (V)</th>
                                        <th>Current A (A)</th>
                                        <th>Current B (A)</th>
                                        <th>Current C (A)</th>
                                        <th>Frequency (Hz)</th>
                                        <th>Power Factor</th>
                                        <th>Aggregate Power (kW)</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>2026-04-07 10:00:00</td>
                                        <td>230.5</td>
                                        <td>231.2</td>
                                        <td>229.8</td>
                                        <td>15.3</td>
                                        <td>14.8</td>
                                        <td>16.1</td>
                                        <td>50.0</td>
                                        <td>0.95</td>
                                        <td>5.2</td>
                                    </tr>
                                    <tr>
                                        <td>2026-04-07 10:01:00</td>
                                        <td>231.0</td>
                                        <td>230.8</td>
                                        <td>230.1</td>
                                        <td>14.9</td>
                                        <td>15.2</td>
                                        <td>15.7</td>
                                        <td>50.0</td>
                                        <td>0.96</td>
                                        <td>5.1</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </section>

            <section id="billing-data">
                <h2 class="section-header">Billing Data</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-receipt"></i> Tariff-Based Energy Data
                    </div>
                    <div class="card-body">
                        <form>
                            <div class="row">
                                <div class="col-md-3 mb-3">
                                    <label for="t1Energy" class="form-label">T1 Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="t1Energy" placeholder="500.25">
                                </div>
                                <div class="col-md-3 mb-3">
                                    <label for="t2Energy" class="form-label">T2 Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="t2Energy" placeholder="300.50">
                                </div>
                                <div class="col-md-3 mb-3">
                                    <label for="t3Energy" class="form-label">T3 Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="t3Energy" placeholder="200.75">
                                </div>
                                <div class="col-md-3 mb-3">
                                    <label for="t4Energy" class="form-label">T4 Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="t4Energy" placeholder="150.00">
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="importEnergy" class="form-label">Import Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="importEnergy" placeholder="1151.50">
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="exportEnergy" class="form-label">Export Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="exportEnergy" placeholder="0.00">
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="absoluteEnergy" class="form-label">Absolute Energy (kWh)</label>
                                    <input type="number" step="0.01" class="form-control" id="absoluteEnergy" placeholder="1151.50">
                                </div>
                            </div>
                            <button type="submit" class="btn btn-primary">Submit Billing Data</button>
                            <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        </form>
                        <hr>
                        <h5>Billing Data Preview</h5>
                        <div class="table-responsive">
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Billing Period</th>
                                        <th>T1 (kWh)</th>
                                        <th>T2 (kWh)</th>
                                        <th>T3 (kWh)</th>
                                        <th>T4 (kWh)</th>
                                        <th>Import (kWh)</th>
                                        <th>Export (kWh)</th>
                                        <th>Absolute (kWh)</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>2026-03</td>
                                        <td>450.25</td>
                                        <td>280.50</td>
                                        <td>180.75</td>
                                        <td>120.00</td>
                                        <td>1031.50</td>
                                        <td>0.00</td>
                                        <td>1031.50</td>
                                        <td><span class="badge bg-success">Billed</span></td>
                                    </tr>
                                    <tr>
                                        <td>2026-04</td>
                                        <td>500.25</td>
                                        <td>300.50</td>
                                        <td>200.75</td>
                                        <td>150.00</td>
                                        <td>1151.50</td>
                                        <td>0.00</td>
                                        <td>1151.50</td>
                                        <td><span class="badge bg-warning">Pending</span></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </section>

            <section id="aux-relay-operations">
                <h2 class="section-header">Aux Relay Operations</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-toggle-on"></i> Relay Control Command
                    </div>
                    <div class="card-body">
                        <form>
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label for="commandType" class="form-label">Command Type</label>
                                    <select class="form-select" id="commandType">
                                        <option selected>Choose command</option>
                                        <option value="turn-on">Turn On</option>
                                        <option value="turn-off">Turn Off</option>
                                        <option value="toggle">Toggle</option>
                                    </select>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label for="relayId" class="form-label">Relay ID</label>
                                    <input type="text" class="form-control" id="relayId" placeholder="Enter relay identifier">
                                </div>
                            </div>
                            <button type="submit" class="btn btn-primary">Execute Command</button>
                            <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        </form>
                    </div>
                </div>
            </section>

            <section id="transaction-status">
                <h2 class="section-header">Transaction Status</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-check-circle"></i> Check Transaction Status
                    </div>
                    <div class="card-body">
                        <form>
                            <div class="mb-3">
                                <label for="transactionId" class="form-label">Transaction ID</label>
                                <input type="text" class="form-control" id="transactionId" placeholder="Enter transaction ID">
                            </div>
                            <button type="submit" class="btn btn-primary">Check Status</button>
                            <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        </form>
                        <hr>
                        <h5>Transaction History</h5>
                        <div class="table-responsive">
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Transaction ID</th>
                                        <th>Command</th>
                                        <th>Status</th>
                                        <th>Timestamp</th>
                                        <th>Message</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>TXN001</td>
                                        <td>Data Read</td>
                                        <td><span class="badge bg-success">Completed</span></td>
                                        <td>2026-04-07 09:00:00</td>
                                        <td>Success</td>
                                    </tr>
                                    <tr>
                                        <td>TXN002</td>
                                        <td>Relay Operation</td>
                                        <td><span class="badge bg-danger">Failed</span></td>
                                        <td>2026-04-07 09:05:00</td>
                                        <td>Timeout</td>
                                    </tr>
                                    <tr>
                                        <td>TXN003</td>
                                        <td>Billing Query</td>
                                        <td><span class="badge bg-warning">Pending</span></td>
                                        <td>2026-04-07 09:10:00</td>
                                        <td>Processing</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </section>

            <section id="test-results-logs">
                <h2 class="section-header">Test Results / Logs</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-journal-text"></i> Testing Logs
                    </div>
                    <div class="card-body">
                        <div class="alert alert-info">
                            <i class="bi bi-info-circle"></i> Test logs are displayed below. Refresh to update.
                        </div>
                        <div class="table-responsive">
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Timestamp</th>
                                        <th>Test Case</th>
                                        <th>Status</th>
                                        <th>Duration (ms)</th>
                                        <th>Message</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>2026-04-07 08:00:00</td>
                                        <td>Authorization Test</td>
                                        <td><span class="badge bg-success">Pass</span></td>
                                        <td>1250</td>
                                        <td>Token validated successfully</td>
                                    </tr>
                                    <tr>
                                        <td>2026-04-07 08:05:00</td>
                                        <td>Instantaneous Data Read</td>
                                        <td><span class="badge bg-success">Pass</span></td>
                                        <td>890</td>
                                        <td>Data retrieved successfully</td>
                                    </tr>
                                    <tr>
                                        <td>2026-04-07 08:10:00</td>
                                        <td>Billing Data Query</td>
                                        <td><span class="badge bg-danger">Fail</span></td>
                                        <td>5000</td>
                                        <td>Connection timeout</td>
                                    </tr>
                                    <tr>
                                        <td>2026-04-07 08:15:00</td>
                                        <td>Relay Operation</td>
                                        <td><span class="badge bg-warning">Pending</span></td>
                                        <td>-</td>
                                        <td>Command queued</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </section>

            <section id="monthly-billing">
                <h2 class="section-header">Monthly Billing</h2>
                <div class="card">
                    <div class="card-body">
                        <p>Monthly billing data forms and tables will be implemented here.</p>
                    </div>
                </div>
            </section>
        </div>
    </div>
    
</asp:Content>

