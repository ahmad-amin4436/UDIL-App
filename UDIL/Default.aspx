<%@ Page Title="UDIL Tester - Configuration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="Default.aspx.cs" Inherits="UDIL._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   <style>
.tracker-step {
    flex: 1;
    position: relative;
    font-size: 12px;
    color: #6c757d;
    font-weight: 500;
}

.tracker-step::before {
    content: "";
    display: block;
    margin: 0 auto 8px;
    width: 14px;
    height: 14px;
    border-radius: 50%;
    background: #dee2e6;
}

.tracker-step.active {
    color: #00dd2c;
}

.tracker-step.active::before {
    background: #00dd2c;
    transform: scale(1.2);
    box-shadow: 0 0 8px rgba(108,117,125,0.3);
}

.tracker-step.completed {
    color: #dc3545;
}

.tracker-step.completed::before {
    background: #dc3545;
    box-shadow: 0 0 6px rgba(220,53,69,0.3);
}
</style>
    <div class="main-content">
        <div class="container-fluid">
            <section id="dashboard">
                <h2 class="section-header">Dashboard</h2>
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

            <section id="authorization">
                <h2 class="section-header">Authorization</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-shield-lock"></i> API Authorization
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <label for="txtUsername" class="form-label">Username</label>
                                <asp:TextBox ID="txtUsername" runat="server" Text="accurate" CssClass="form-control" placeholder="Enter username"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtPassword" class="form-label">Password</label>
                                <asp:TextBox ID="txtPassword" runat="server" Text="Hello@123" TextMode="Password" CssClass="form-control" placeholder="Enter password"></asp:TextBox>
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

            <section id="device-creation">
                <h2 class="section-header">Device Creation</h2>
                <div class="card">
                    <div class="card-header">
                        <i class="bi bi-plus-circle"></i> Device Creation
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcTransactionId" runat="server" AssociatedControlID="dcTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="dcTransactionId" runat="server" CssClass="form-control" placeholder="createdevice1959" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcPrivateKey" runat="server" AssociatedControlID="dcPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="dcPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                            <div class="col-md-12 mb-3">
                                <asp:Label ID="lblDcDeviceIdentity" runat="server" AssociatedControlID="dcDeviceIdentity" CssClass="form-label" Text="Device Identity JSON"></asp:Label>
                                <asp:TextBox ID="dcDeviceIdentity" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3">[{"dsn":"2998999997","global_device_id":"m98999997"}]</asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcRequestDateTime" runat="server" AssociatedControlID="dcRequestDateTime" CssClass="form-label" Text="Request DateTime"></asp:Label>
                                <asp:TextBox ID="dcRequestDateTime" runat="server" Text="2024-06-27 11:29:00" CssClass="form-control" placeholder="2024-06-27 11:29:00" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcDeviceType" runat="server" AssociatedControlID="dcDeviceType" CssClass="form-label" Text="Device Type"></asp:Label>
                                <asp:DropDownList ID="dcDeviceType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - Meter</asp:ListItem>
                                    <asp:ListItem Value="2">2 - DCU</asp:ListItem>
                                    <asp:ListItem Value="3">3 - APMS</asp:ListItem>
                                    <asp:ListItem Value="4">4 - Grid meter</asp:ListItem>
                                    <asp:ListItem Value="5">5 - Others</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDcMdiResetDate" runat="server" AssociatedControlID="dcMdiResetDate" CssClass="form-label" Text="MDI Reset Date"></asp:Label>
                                <asp:TextBox ID="dcMdiResetDate" runat="server" CssClass="form-control" TextMode="Number" placeholder="19" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDcMdiResetTime" runat="server" AssociatedControlID="dcMdiResetTime" CssClass="form-label" Text="MDI Reset Time"></asp:Label>
                                <asp:TextBox ID="dcMdiResetTime" runat="server" CssClass="form-control" TextMode="Time"  />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDcBidirectionalDevice" runat="server" AssociatedControlID="dcBidirectionalDevice" CssClass="form-label" Text="Bidirectional Device"></asp:Label>
                                <asp:DropDownList ID="dcBidirectionalDevice" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0">0 - False</asp:ListItem>
                                    <asp:ListItem Value="1">1 - True</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcSimNumber" runat="server" AssociatedControlID="dcSimNumber" CssClass="form-label" Text="SIM Number"></asp:Label>
                                <asp:TextBox ID="dcSimNumber" runat="server" CssClass="form-control" Text="03464436525"  placeholder="03464436525" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcSimId" runat="server" AssociatedControlID="dcSimId" CssClass="form-label" Text="SIM ID"></asp:Label>
                                <asp:TextBox ID="dcSimId" runat="server" CssClass="form-control" Text="899204390623445447" placeholder="899204390623445447" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDcPhase" runat="server" AssociatedControlID="dcPhase" CssClass="form-label" Text="Phase"></asp:Label>
                                <asp:DropDownList ID="dcPhase" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - Single</asp:ListItem>
                                    <asp:ListItem Value="2">2 - Other</asp:ListItem>
                                    <asp:ListItem Value="3">3 - Three-phase</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDcMeterType" runat="server" AssociatedControlID="dcMeterType" CssClass="form-label" Text="Meter Type"></asp:Label>
                                <asp:DropDownList ID="dcMeterType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - Normal</asp:ListItem>
                                    <asp:ListItem Value="2">2 - Whole Current</asp:ListItem>
                                    <asp:ListItem Value="3">3 - CTO</asp:ListItem>
                                    <asp:ListItem Value="4">4 - CTPT</asp:ListItem>
                                    <asp:ListItem Value="5">5 - Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDcCommunicationMode" runat="server" AssociatedControlID="dcCommunicationMode" CssClass="form-label" Text="Communication Mode"></asp:Label>
                                <asp:DropDownList ID="dcCommunicationMode" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - GPRS/3G/4G</asp:ListItem>
                                    <asp:ListItem Value="2">2 - RF</asp:ListItem>
                                    <asp:ListItem Value="3">3 - PLC</asp:ListItem>
                                    <asp:ListItem Value="4">4 - Ethernet</asp:ListItem>
                                    <asp:ListItem Value="5">5 - Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcCommunicationType" runat="server" AssociatedControlID="dcCommunicationType" CssClass="form-label" Text="Communication Type"></asp:Label>
                                <asp:DropDownList ID="dcCommunicationType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - Mode-I / Non-Keepalive</asp:ListItem>
                                    <asp:ListItem Value="2">2 - Mode-II / Keep-alive</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcCommunicationInterval" runat="server" AssociatedControlID="dcCommunicationInterval" CssClass="form-label" Text="Communication Interval"></asp:Label>
                                <asp:TextBox ID="dcCommunicationInterval" runat="server" CssClass="form-control" TextMode="Number" placeholder="30" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcInitialCommunicationTime" runat="server" AssociatedControlID="dcInitialCommunicationTime" CssClass="form-label" Text="Initial Communication Time"></asp:Label>
                                <asp:TextBox ID="dcInitialCommunicationTime" runat="server" CssClass="form-control" TextMode="Time" Text="00:00:00" />
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
    <asp:Button 
        ID="btnDeviceCreation" 
        runat="server" 
        CssClass="btn btn-primary" 
        Text="Send Device Creation" 
        OnClick="btnDeviceCreation_Click"
        OnClientClick="showDeviceCreationLoading(this); return true;" />
        
    <span id="deviceCreationSpinner" 
          class="position-absolute top-50 start-50 translate-middle" 
          style="display:none;">
        <span class="spinner-border spinner-border-sm"></span>
    </span>
</div>
                        <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        <asp:Label ID="lblDeviceCreationMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                </div>
                
              <asp:UpdatePanel ID="updTracker" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

            <div class="card-header border-0">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h5 class="mb-0">Device Creation Tracker</h5>   
                        <small class="text-muted">
                            Transaction: <asp:Label ID="lblTrackerTransactionId" runat="server" />
                        </small>
                    </div>
                    <asp:Label ID="lblStage" runat="server" CssClass="badge bg-info px-3 py-2" />
                </div>
            </div>

            <div class="card-body">

                <!-- Progress Bar -->
                <div class="progress mb-4" style="height:6px;">
                    <asp:Panel ID="progressBar" runat="server"
                        CssClass="progress-bar bg-primary"
                        Style="width:0%">
                    </asp:Panel>
                </div>

                <!-- Steps -->
                <div class="d-flex justify-content-between text-center">

                    <asp:Label ID="step0" runat="server" CssClass="tracker-step">Waiting</asp:Label>
                    <asp:Label ID="step1" runat="server" CssClass="tracker-step">Start</asp:Label>
                    <asp:Label ID="step2" runat="server" CssClass="tracker-step">Request Sent</asp:Label>
                    <asp:Label ID="step3" runat="server" CssClass="tracker-step">Connected</asp:Label>
                    <asp:Label ID="step4" runat="server" CssClass="tracker-step">Command Sent</asp:Label>
                    <asp:Label ID="step5" runat="server" CssClass="tracker-step">Completed</asp:Label>

                </div>

                <!-- Description -->
                <div class="mt-4 text-center">
                    <asp:Label ID="lblStageDescription" runat="server" CssClass="text-muted" />
                </div>

            </div>
        </asp:Panel>

        <asp:Timer ID="timerTracker" runat="server" Interval="2000"
            OnTick="timerTracker_Tick" Enabled="false" />

    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnDeviceCreation" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
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

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
function showDeviceCreationLoading(button) {
    if (!button) return;

    const spinner = document.getElementById('deviceCreationSpinner');

    // Show spinner and change text immediately
    if (spinner) {
        spinner.style.display = 'inline-block';
    }

    // Change text
    const originalText = button.value;
    button.value = 'Sending...';

    // store original text
    button.setAttribute('data-original-text', originalText);

    // Don't disable button immediately - let server handle it
    // This ensures the server-side event fires properly
    return true;
}

// Function to disable button after successful processing
function disableDeviceCreationButton() {
    const button = document.getElementById('<%= btnDeviceCreation.ClientID %>');
    const spinner = document.getElementById('deviceCreationSpinner');
            

    if (button) {
        button.disabled = true;
        button.style.opacity = '0.7';
        button.style.cursor = 'not-allowed';
        
        // Keep "Sending..." text to show completion
        button.value = 'Processing...';
    }
    
    if (spinner) {
        spinner.style.display = 'inline-block';
    }
}

// Optional reset function
function resetDeviceCreationLoading() {
    const button = document.getElementById('<%= btnDeviceCreation.ClientID %>');
    const spinner = document.getElementById('deviceCreationSpinner');

    if (button) {
        button.disabled = false;
        button.style.opacity = '1';
        button.style.cursor = 'pointer';

        const originalText = 'Send Device Creation';
        if (originalText) {
            button.value = originalText;
        }
    }

    if (spinner) {
        spinner.style.display = 'none';
    }
}
</script>
</asp:Content>