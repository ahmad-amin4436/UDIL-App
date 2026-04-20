<%@ Page Title="UDIL Tester - Update TOU" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="UpdateTOU.aspx.cs" Inherits="UDIL.Pages.UpdateTOU" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="update-tou">
                <h2 class="section-header"><i class="bi bi-clock"></i> Update Time of Use (TOU)</h2>
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsTransactionId" runat="server" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="tsTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsPrivateKey" runat="server" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="tsPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsGlobalDeviceId" runat="server" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="tsGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m97999996" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsRequestDateTime" runat="server" CssClass="form-label" Text="Request DateTime"></asp:Label>
                                <asp:TextBox ID="tsRequestDateTime" runat="server" Text="2025-12-04 12:15:00" CssClass="form-control" placeholder="2025-12-04 12:15:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsActivationDateTime" runat="server" CssClass="form-label" Text="Activation DateTime"></asp:Label>
                                <asp:TextBox ID="tsActivationDateTime" runat="server" Text="2026-01-30 11:55:00" CssClass="form-control" placeholder="2026-01-30 11:55:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:Label ID="lblDayProfile" runat="server" CssClass="form-label" Text="Day Profile"></asp:Label>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 1 Name:</label>
                                        <asp:TextBox ID="tsDay1Name" runat="server" CssClass="form-control" Text="d1" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 1 Tariff Slabs:</label>
                                        <asp:TextBox ID="tsDay1Tariff" runat="server" CssClass="form-control" Text="12:25, 12:55" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 2 Name:</label>
                                        <asp:TextBox ID="tsDay2Name" runat="server" CssClass="form-control" Text="d2" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 2 Tariff Slabs:</label>
                                        <asp:TextBox ID="tsDay2Tariff" runat="server" CssClass="form-control" Text="13:25, 13:55" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 3 Name:</label>
                                        <asp:TextBox ID="tsDay3Name" runat="server" CssClass="form-control" Text="d3" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 3 Tariff Slabs:</label>
                                        <asp:TextBox ID="tsDay3Tariff" runat="server" CssClass="form-control" Text="14:25, 14:55" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 4 Name:</label>
                                        <asp:TextBox ID="tsDay4Name" runat="server" CssClass="form-control" Text="d4" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Day 4 Tariff Slabs:</label>
                                        <asp:TextBox ID="tsDay4Tariff" runat="server" CssClass="form-control" Text="15:25, 15:55" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:Label ID="lblWeekProfile" runat="server" CssClass="form-label" Text="Week Profile"></asp:Label>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 1 Name:</label>
                                        <asp:TextBox ID="tsWeek1Name" runat="server" CssClass="form-control" Text="w1" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 1 Days:</label>
                                        <asp:TextBox ID="tsWeek1Days" runat="server" CssClass="form-control" Text="d1, d1, d1, d1, d1, d1, d1" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 2 Name:</label>
                                        <asp:TextBox ID="tsWeek2Name" runat="server" CssClass="form-control" Text="w2" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 2 Days:</label>
                                        <asp:TextBox ID="tsWeek2Days" runat="server" CssClass="form-control" Text="d2, d2, d2, d2, d2, d2" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 3 Name:</label>
                                        <asp:TextBox ID="tsWeek3Name" runat="server" CssClass="form-control" Text="w3" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 3 Days:</label>
                                        <asp:TextBox ID="tsWeek3Days" runat="server" CssClass="form-control" Text="d3, d3, d3, d3, d3, d3" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 4 Name:</label>
                                        <asp:TextBox ID="tsWeek4Name" runat="server" CssClass="form-control" Text="w4" />
                                    </div>
                                    <div class="col-md-6 mb-2">
                                        <label class="form-label">Week 4 Days:</label>
                                        <asp:TextBox ID="tsWeek4Days" runat="server" CssClass="form-control" Text="d4, d4, d4, d4, d4, d4" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:Label ID="lblSeasonProfile" runat="server" CssClass="form-label" Text="Season Profile"></asp:Label>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 1 Name:</label>
                                        <asp:TextBox ID="tsSeason1Name" runat="server" CssClass="form-control" Text="s1" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 1 Week:</label>
                                        <asp:TextBox ID="tsSeason1Week" runat="server" CssClass="form-control" Text="w1" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 1 Start Date:</label>
                                        <asp:TextBox ID="tsSeason1Date" runat="server" CssClass="form-control" Text="01-01" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 2 Name:</label>
                                        <asp:TextBox ID="tsSeason2Name" runat="server" CssClass="form-control" Text="s2" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 2 Week:</label>
                                        <asp:TextBox ID="tsSeason2Week" runat="server" CssClass="form-control" Text="w2" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 2 Start Date:</label>
                                        <asp:TextBox ID="tsSeason2Date" runat="server" CssClass="form-control" Text="01-04" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 3 Name:</label>
                                        <asp:TextBox ID="tsSeason3Name" runat="server" CssClass="form-control" Text="s3" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 3 Week:</label>
                                        <asp:TextBox ID="tsSeason3Week" runat="server" CssClass="form-control" Text="w3" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 3 Start Date:</label>
                                        <asp:TextBox ID="tsSeason3Date" runat="server" CssClass="form-control" Text="01-07" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 4 Name:</label>
                                        <asp:TextBox ID="tsSeason4Name" runat="server" CssClass="form-control" Text="s4" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 4 Week:</label>
                                        <asp:TextBox ID="tsSeason4Week" runat="server" CssClass="form-control" Text="w4" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Season 4 Start Date:</label>
                                        <asp:TextBox ID="tsSeason4Date" runat="server" CssClass="form-control" Text="01-12" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:Label ID="lblHolidayProfile" runat="server" CssClass="form-label" Text="Holiday Profile"></asp:Label>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 1 Name:</label>
                                        <asp:TextBox ID="tsHoliday1Name" runat="server" CssClass="form-control" Text="h1" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 1 Date:</label>
                                        <asp:TextBox ID="tsHoliday1Date" runat="server" CssClass="form-control" Text="24-03" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 1 Day Profile:</label>
                                        <asp:TextBox ID="tsHoliday1Day" runat="server" CssClass="form-control" Text="d1" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 2 Name:</label>
                                        <asp:TextBox ID="tsHoliday2Name" runat="server" CssClass="form-control" Text="h2" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 2 Date:</label>
                                        <asp:TextBox ID="tsHoliday2Date" runat="server" CssClass="form-control" Text="15-08" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 2 Day Profile:</label>
                                        <asp:TextBox ID="tsHoliday2Day" runat="server" CssClass="form-control" Text="d1" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 3 Name:</label>
                                        <asp:TextBox ID="tsHoliday3Name" runat="server" CssClass="form-control" Text="h3" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 3 Date:</label>
                                        <asp:TextBox ID="tsHoliday3Date" runat="server" CssClass="form-control" Text="05-02" />
                                    </div>
                                    <div class="col-md-4 mb-2">
                                        <label class="form-label">Holiday 3 Day Profile:</label>
                                        <asp:TextBox ID="tsHoliday3Day" runat="server" CssClass="form-control" Text="d1" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnUpdateTOU"
                                runat="server"
                                CssClass="btn btn-primary"
                                Text="Update Time of Use"
                                OnClick="btnUpdateTOU_Click"
                                OnClientClick="showUpdateTOULoading(this); return true;" />

                            <span id="updateTouSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="UpdateTOU.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblUpdateTOUMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                  <asp:UpdatePanel ID="updTracker" runat="server">
                      <ContentTemplate>

                          <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

                              <div class="card-header border-0">
                                  <div class="d-flex justify-content-between align-items-center">
                                      <div>
                                          <h5 class="mb-0">Update TOU Tracker</h5>
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
                                      <asp:Label ID="step8" runat="server" CssClass="tracker-step">Events Table</asp:Label>

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
                        <asp:AsyncPostBackTrigger ControlID="btnUpdateTOU" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showUpdateTOULoading(button) {
            if (!button) return;

            const spinner = document.getElementById('updateTouSpinner');

            // Show spinner and change text immediately
            if (spinner) {
                spinner.style.display = 'inline-block';
            }

            // Change text
            const originalText = button.value;
            button.value = 'Updating...';

            // store original text
            button.setAttribute('data-original-text', originalText);

            // Don't disable button immediately - let server handle it
            // This ensures the server-side event fires properly
            return true;
        }

        // Function to disable button after successful processing
        function disableUpdateTOUButton() {
            const button = document.getElementById('<%= btnUpdateTOU.ClientID %>');
            const spinner = document.getElementById('updateTouSpinner');


            if (button) {
                button.disabled = true;
                button.style.opacity = '0.7';
                button.style.cursor = 'not-allowed';

                // Keep "Updating..." text to show completion
                button.value = 'Processing...';
            }

            if (spinner) {
                spinner.style.display = 'inline-block';
            }
        }

        // Optional reset function
        function resetUpdateTOULoading() {
            const button = document.getElementById('<%= btnUpdateTOU.ClientID %>');
            const spinner = document.getElementById('updateTouSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Update Time of Use';
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



