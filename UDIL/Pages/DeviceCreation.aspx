<%@ Page Title="UDIL Tester - Device Creation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="DeviceCreation.aspx.cs" Inherits="UDIL.Pages.DeviceCreation" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="device-creation">
                <h2 class="section-header"><i class="bi bi-plus-circle"></i> Device Creation</h2>
                <div class="card">
                   
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcTransactionId" runat="server" AssociatedControlID="dcTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="dcTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcPrivateKey" runat="server" AssociatedControlID="dcPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="dcPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcDsn" runat="server" AssociatedControlID="dcDsn" CssClass="form-label" Text="DSN"></asp:Label>
                                <asp:TextBox ID="dcDsn" runat="server" CssClass="form-control" placeholder="Device Serial Number" Text="2998999997" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDcGlobalDeviceId" runat="server" AssociatedControlID="dcGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="dcGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m98999997" />
                            </div>
                        </div>
                        <div class="row">
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
                                     <asp:ListItem Value="2">2 - Mode-II / Keep-alive</asp:ListItem>
                                    <asp:ListItem Value="1">1 - Mode-I / Non-Keepalive</asp:ListItem>
                                   
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
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="DeviceCreation.aspx" class="btn btn-secondary ms-2">Reset</a>
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
                                          <small class="text-muted">Transaction:
                                              <asp:Label ID="lblTrackerTransactionId" runat="server" />
                                          </small>
                                      </div>
                                      <asp:Label ID="lblStage" runat="server" CssClass="badge bg-info px-3 py-2" />
                                  </div>
                              </div>

                              <div class="card-body">

                                  <!-- Progress Bar -->
                                  <div class="progress mb-4" style="height: 6px;">
                                      <asp:Panel ID="progressBar" runat="server"
                                          CssClass="progress-bar bg-primary"
                                          Style="width: 0%">
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
                                      <asp:Label ID="step6" runat="server" CssClass="tracker-step">Device Communication History Log</asp:Label>
                                      <asp:Label ID="step7" runat="server" CssClass="tracker-step">Meter Visual Data Validated</asp:Label>

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
