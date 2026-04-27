<%@ Page Title="UDIL Tester - Bill Data Sampling" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="MeterDataSampling.aspx.cs" Inherits="UDIL.Pages.MeterDataSampling" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="ip-port-update">
                <h2 class="section-header"><i class="bi bi-lightning"></i> Meter Data Sampling Bill</h2>
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
                                <asp:Label ID="lblTsActivationDateTime" runat="server" AssociatedControlID="tsActivationDateTime" CssClass="form-label" Text="Activation DateTime"></asp:Label>
                                <asp:TextBox ID="tsActivationDateTime" runat="server" CssClass="form-control" placeholder="2026-04-21 15:22:00" Text="2026-04-21 15:22:00" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsDataType" runat="server" AssociatedControlID="tsDataType" CssClass="form-label" Text="Data Type"></asp:Label>
                                <asp:DropDownList ID="tsDataType" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="BILL" Value="BILL" Selected="True" />
                                    <asp:ListItem Text="LPRO" Value="LPRO" />
                                    <asp:ListItem Text="INST" Value="INST" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsSamplingInterval" runat="server" AssociatedControlID="tsSamplingInterval" CssClass="form-label" Text="Sampling Interval"></asp:Label>
                                <asp:TextBox ID="tsSamplingInterval" runat="server" CssClass="form-control" placeholder="Sampling Interval" Text="30" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsSamplingInitialTime" runat="server" AssociatedControlID="tsSamplingInitialTime" CssClass="form-label" Text="Sampling Initial Time"></asp:Label>
                                <asp:TextBox ID="tsSamplingInitialTime" runat="server" CssClass="form-control" placeholder="Sampling Initial Time" Text="00" />
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnMeterDataSampling"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Send Meter Data Sampling Bill"
                                OnClick="btnMeterDataSampling_Click"
                                OnClientClick="showMeterDataSamplingLoading(this); return true;" />

                            <span id="meterDataSamplingSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="MeterDataSampling.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblMeterDataSamplingMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                  <asp:UpdatePanel ID="updTracker" runat="server">
                      <ContentTemplate>

                          <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

                              <div class="card-header border-0">
                                  <div class="d-flex justify-content-between align-items-center">
                                      <div>
                                          <h5 class="mb-0">Meter Data Sampling Bill Tracker</h5>
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
                        <asp:AsyncPostBackTrigger ControlID="btnMeterDataSampling" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showMeterDataSamplingLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('meterDataSamplingSpinner');

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
        function disableMeterDataSamplingButton() {
            const button = document.getElementById('<%= btnMeterDataSampling.ClientID %>');
            const spinner = document.getElementById('meterDataSamplingSpinner');


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
        function resetMeterDataSamplingLoading() {
            const button = document.getElementById('<%= btnMeterDataSampling.ClientID %>');
            const spinner = document.getElementById('meterDataSamplingSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Send Meter Data Sampling Bill';
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



