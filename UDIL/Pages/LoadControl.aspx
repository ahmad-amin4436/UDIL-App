<%@ Page Title="UDIL Tester - Sanc Load Control" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="LoadControl.aspx.cs" Inherits="UDIL.Pages.LoadControl" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="ip-port-update">
                <h2 class="section-header"><i class="bi bi-lightning"></i> Sanc Load Control</h2>
                <div class="card">
                 
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsTransactionId" runat="server" AssociatedControlID="tsTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="tsTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsPrivateKey" runat="server" AssociatedControlID="tsPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="tsPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsGlobalDeviceId" runat="server" AssociatedControlID="tsGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="tsGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m97999996" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsRequestDateTime" runat="server" AssociatedControlID="tsRequestDateTime" CssClass="form-label" Text="Request DateTime"></asp:Label>
                                <asp:TextBox ID="tsRequestDateTime" runat="server" Text="2025-12-27 15:37:00" CssClass="form-control" placeholder="2025-12-27 15:37:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsLoadLimit" runat="server" AssociatedControlID="tsLoadLimit" CssClass="form-label" Text="Load Limit"></asp:Label>
                                <asp:TextBox ID="tsLoadLimit" runat="server" CssClass="form-control" placeholder="Load Limit" Text="5" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsMaximumRetries" runat="server" AssociatedControlID="tsMaximumRetries" CssClass="form-label" Text="Maximum Retries"></asp:Label>
                                <asp:TextBox ID="tsMaximumRetries" runat="server" CssClass="form-control" placeholder="Maximum Retries" Text="20" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsRetryInterval" runat="server" AssociatedControlID="tsRetryInterval" CssClass="form-label" Text="Retry Interval"></asp:Label>
                                <asp:TextBox ID="tsRetryInterval" runat="server" CssClass="form-control" placeholder="Retry Interval" Text="30" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsThresholdDuration" runat="server" AssociatedControlID="tsThresholdDuration" CssClass="form-label" Text="Threshold Duration"></asp:Label>
                                <asp:TextBox ID="tsThresholdDuration" runat="server" CssClass="form-control" placeholder="Threshold Duration" Text="20" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsRetryClearInterval" runat="server" AssociatedControlID="tsRetryClearInterval" CssClass="form-label" Text="Retry Clear Interval"></asp:Label>
                                <asp:TextBox ID="tsRetryClearInterval" runat="server" CssClass="form-control" placeholder="Retry Clear Interval" Text="200" />
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnSancLoadControl"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Send Sanc Load Control"
                                OnClick="btnSancLoadControl_Click"
                                OnClientClick="showSancLoadControlLoading(this); return true;" />

                            <span id="sancLoadControlSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="Update_IP_Port.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblSancLoadControlMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                  <asp:UpdatePanel ID="updTracker" runat="server">
                      <ContentTemplate>

                          <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

                              <div class="card-header border-0">
                                  <div class="d-flex justify-content-between align-items-center">
                                      <div>
                                          <h5 class="mb-0">Sanc Load Control Tracker</h5>
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
                        <asp:AsyncPostBackTrigger ControlID="btnSancLoadControl" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showSancLoadControlLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('sancLoadControlSpinner');

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
        function disableSancLoadControlButton() {
            const button = document.getElementById('<%= btnSancLoadControl.ClientID %>');
            const spinner = document.getElementById('sancLoadControlSpinner');


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
        function resetSancLoadControlLoading() {
            const button = document.getElementById('<%= btnSancLoadControl.ClientID %>');
            const spinner = document.getElementById('sancLoadControlSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Send Sanc Load Control';
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


