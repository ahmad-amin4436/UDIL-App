<%@ Page Title="UDIL Tester - Wake SIM Number" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="WakeUpSimNumber.aspx.cs" Inherits="UDIL.Pages.WakeUpSimNumber" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="ip-port-update">
                <h2 class="section-header"><i class="bi bi-phone"></i> Update Wake-up SIM Number</h2>
                <div class="card">
                 
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblWsTransactionId" runat="server" AssociatedControlID="wsTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="wsTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblWsPrivateKey" runat="server" AssociatedControlID="wsPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="wsPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblWsGlobalDeviceId" runat="server" AssociatedControlID="wsGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="wsGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m98999997" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblWsRequestDateTime" runat="server" AssociatedControlID="wsRequestDateTime" CssClass="form-label" Text="Request DateTime"></asp:Label>
                                <asp:TextBox ID="wsRequestDateTime" runat="server" Text="2022-03-30 13:56:00" CssClass="form-control" placeholder="2022-03-30 13:56:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblWsWakeupNumber1" runat="server" AssociatedControlID="wsWakeupNumber1" CssClass="form-label" Text="Wake Up Number 1"></asp:Label>
                                <asp:TextBox ID="wsWakeupNumber1" runat="server" CssClass="form-control" placeholder="Wake Up Number 1" Text="03464394600" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblWsWakeupNumber2" runat="server" AssociatedControlID="wsWakeupNumber2" CssClass="form-label" Text="Wake Up Number 2"></asp:Label>
                                <asp:TextBox ID="wsWakeupNumber2" runat="server" CssClass="form-control" placeholder="Wake Up Number 2" Text="03464436525" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblWsWakeupNumber3" runat="server" AssociatedControlID="wsWakeupNumber3" CssClass="form-label" Text="Wake Up Number 3"></asp:Label>
                                <asp:TextBox ID="wsWakeupNumber3" runat="server" CssClass="form-control" placeholder="Wake Up Number 3" Text="03114883765" />
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnWakeSimNumber"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Wake SIM Number"
                                OnClick="btnWakeSimNumber_Click"
                                OnClientClick="showWakeSimNumberLoading(this); return true;" />

                            <span id="wakeSimNumberSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="WakeUpSimNumber.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblWakeSimNumberMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                  <asp:UpdatePanel ID="updTracker" runat="server">
                      <ContentTemplate>

                          <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

                              <div class="card-header border-0">
                                  <div class="d-flex justify-content-between align-items-center">
                                      <div>
                                          <h5 class="mb-0">Wake SIM Number Tracker</h5>
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
                        <asp:AsyncPostBackTrigger ControlID="btnWakeSimNumber" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showWakeSimNumberLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('wakeSimNumberSpinner');

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
        function disableWakeSimNumberButton() {
            const button = document.getElementById('<%= btnWakeSimNumber.ClientID %>');
            const spinner = document.getElementById('wakeSimNumberSpinner');


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
        function resetWakeSimNumberLoading() {
            const button = document.getElementById('<%= btnWakeSimNumber.ClientID %>');
            const spinner = document.getElementById('wakeSimNumberSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Wake SIM Number';
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


