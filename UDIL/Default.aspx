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

            <section id="session">
                <h2 class="section-header"><i class="bi bi-clock-history"></i> Session Management</h2>
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="txtSessionName" class="form-label">Session Name</label>
                                <asp:TextBox ID="txtSessionName" runat="server" CssClass="form-control" placeholder="Enter session name"></asp:TextBox>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtSessionDescription" class="form-label">Description</label>
                                <asp:TextBox ID="txtSessionDescription" runat="server" CssClass="form-control" placeholder="Enter session description"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label for="ddlSavedSessions" class="form-label">Saved Sessions</label>
                                <asp:DropDownList ID="ddlSavedSessions" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSavedSessions_SelectedIndexChanged">
                                    <asp:ListItem Text="-- Select Session --" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label for="txtSessionDate" class="form-label">Session Date</label>
                                <asp:TextBox ID="txtSessionDate" runat="server" CssClass="form-control" placeholder="Auto-generated on create"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12 mb-3">
                                <h6 class="text-primary">Session Information</h6>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <label for="txtTestEnvironment" class="form-label">Test Environment</label>
                                <asp:TextBox ID="txtTestEnvironment" runat="server" Text="Production" CssClass="form-control" placeholder="Test environment"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtDeviceCount" class="form-label">Device Count</label>
                                <asp:TextBox ID="txtDeviceCount" runat="server" Text="0" CssClass="form-control" placeholder="Number of devices"></asp:TextBox>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label for="txtTestType" class="form-label">Test Type</label>
                                <asp:TextBox ID="txtTestType" runat="server" Text="Compliance" CssClass="form-control" placeholder="Type of testing"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:Button ID="btnCreateSession" runat="server" Text="Create Session" CssClass="btn btn-success me-2" OnClick="btnCreateSession_Click" />
                                <asp:Button ID="btnLoadSession" runat="server" Text="Load Session" CssClass="btn btn-info me-2" OnClick="btnLoadSession_Click" />
                                <asp:Button ID="btnDeleteSession" runat="server" Text="Delete Session" CssClass="btn btn-danger me-2" OnClick="btnDeleteSession_Click" />
                                <asp:Button ID="btnExportSession" runat="server" Text="Export Failed Tests" CssClass="btn btn-warning me-2" OnClick="btnExportSession_Click" UseSubmitBehavior="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <div class="card">
                                    <div class="card-header">
                                        <i class="bi bi-info-circle"></i> Current Session Details
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <p><strong>Session ID:</strong> <asp:Label ID="lblSessionId" runat="server" Text="N/A"></asp:Label></p>
                                                <p><strong>Status:</strong> <asp:Label ID="lblSessionStatus" runat="server" CssClass="badge bg-secondary" Text="Not Started"></asp:Label></p>
                                                <p><strong>Created:</strong> <asp:Label ID="lblSessionCreated" runat="server" Text="N/A"></asp:Label></p>
                                            </div>
                                            <div class="col-md-6">
                                                <p><strong>Last Modified:</strong> <asp:Label ID="lblSessionModified" runat="server" Text="N/A"></asp:Label></p>
                                                <p><strong>Tests Completed:</strong> <asp:Label ID="lblTestsCompleted" runat="server" Text="0"></asp:Label></p>
                                                <p><strong>Total Tests:</strong> <asp:Label ID="lblTotalTests" runat="server" Text="0"></asp:Label></p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="mt-3">
                            <asp:Label ID="lblSessionMessage" runat="server" CssClass="text-info"></asp:Label>
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
                                <asp:Button ID="btnApplyConfig" runat="server" Text="Apply Configuration" CssClass="btn btn-dark-primary" OnClick="btnApplyConfig_Click" />
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
                        <asp:Button ID="btnAuthorize" runat="server" Text="Authorize" CssClass="btn btn-dark-primary" OnClick="btnAuthorize_Click" />
                        <button type="reset" class="btn btn-secondary ms-2">Reset</button>
                        <div id="auth-detail" class="mt-3">
                            <asp:Label ID="lblAuthMessage" runat="server" CssClass="text-info"></asp:Label>
                        </div>
                    </div>
                </div>
            </section>

           

           
        </div>
    </div>
    
</asp:Content>

