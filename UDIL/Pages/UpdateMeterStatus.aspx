<%@ Page Title="UDIL Tester - Update Meter Status" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="UpdateMeterStatus.aspx.cs" Inherits="UDIL.Pages.UpdateMeterStatus" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="ip-port-update">
                <h2 class="section-header"><i class="bi bi-activity"></i> Update Meter Status</h2>
                <div class="card">
                 
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMsTransactionId" runat="server" AssociatedControlID="msTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="msTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMsPrivateKey" runat="server" AssociatedControlID="msPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="msPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMsGlobalDeviceId" runat="server" AssociatedControlID="msGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="msGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m97999996" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMsRequestDateTime" runat="server" AssociatedControlID="msRequestDateTime" CssClass="form-label" Text="Request DateTime"></asp:Label>
                                <asp:TextBox ID="msRequestDateTime" runat="server" Text="2025-12-03 17:56:00" CssClass="form-control" placeholder="2025-12-03 17:56:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMsMeterActivationStatus" runat="server" AssociatedControlID="msMeterActivationStatus" CssClass="form-label" Text="Meter Activation Status"></asp:Label>
                                <asp:DropDownList ID="msMeterActivationStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="1" Selected="True">1 - Active</asp:ListItem>
                                    <asp:ListItem Value="0">0 - In-Active</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnMeterStatus"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Update Meter Status"
                                OnClick="btnMeterStatus_Click"
                                OnClientClick="showMeterStatusLoading(this); return true;" />

                            <span id="meterStatusSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="UpdateMeterStatus.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblMeterStatusMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                  <asp:UpdatePanel ID="updTracker" runat="server">
                      <ContentTemplate>

                          <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

                              <div class="card-header border-0">
                                  <div class="d-flex justify-content-between align-items-center">
                                      <div>
                                          <h5 class="mb-0">Update Meter Status Tracker</h5>
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
                        <asp:AsyncPostBackTrigger ControlID="btnMeterStatus" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showMeterStatusLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('meterStatusSpinner');

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
        function disableMeterStatusButton() {
            const button = document.getElementById('<%= btnMeterStatus.ClientID %>');
            const spinner = document.getElementById('meterStatusSpinner');


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
        function resetMeterStatusLoading() {
            const button = document.getElementById('<%= btnMeterStatus.ClientID %>');
            const spinner = document.getElementById('meterStatusSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Update Meter Status';
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



