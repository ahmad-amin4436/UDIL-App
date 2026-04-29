<%@ Page Title="UDIL Tester - Load Shedding Scheduling" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="LoadSheddingScheduling.aspx.cs" Inherits="UDIL.Pages.LoadSheddingScheduling" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="ip-port-update">
                <h2 class="section-header"><i class="bi bi-calendar-x"></i> Load Shedding Scheduling</h2>
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
                                <asp:Label ID="lblTsStartDateTime" runat="server" AssociatedControlID="tsStartDateTime" CssClass="form-label" Text="Start DateTime"></asp:Label>
                                <asp:TextBox ID="tsStartDateTime" runat="server" Text="2026-12-30 11:45:00" CssClass="form-control" placeholder="2026-12-30 11:45:00" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblTsEndDateTime" runat="server" AssociatedControlID="tsEndDateTime" CssClass="form-label" Text="End DateTime"></asp:Label>
                                <asp:TextBox ID="tsEndDateTime" runat="server" Text="2026-12-30 13:00:00" CssClass="form-control" placeholder="2026-12-30 13:00:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-12 mb-3">
                                <h5 class="mb-3">Load Shedding Slabs</h5>
                                
                                <!-- Slab 1 -->
                                <div class="row mb-2">
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab1Time" runat="server" CssClass="form-label" Text="Slab 1 - Action Time"></asp:Label>
                                        <asp:TextBox ID="tsSlab1Time" runat="server" CssClass="form-control" placeholder="12:00:00" Text="12:00:00" />
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab1Relay" runat="server" CssClass="form-label" Text="Slab 1 - Relay Operation (0=OFF, 1=ON)"></asp:Label>
                                        <asp:DropDownList ID="ddlSlab1Relay" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="OFF (0)" Value="0" Selected="True" />
                                            <asp:ListItem Text="ON (1)" Value="1" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <!-- Slab 2 -->
                                <div class="row mb-2">
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab2Time" runat="server" CssClass="form-label" Text="Slab 2 - Action Time"></asp:Label>
                                        <asp:TextBox ID="tsSlab2Time" runat="server" CssClass="form-control" placeholder="12:15:00" Text="12:15:00" />
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab2Relay" runat="server" CssClass="form-label" Text="Slab 2 - Relay Operation (0=OFF, 1=ON)"></asp:Label>
                                        <asp:DropDownList ID="ddlSlab2Relay" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="OFF (0)" Value="0" />
                                            <asp:ListItem Text="ON (1)" Value="1" Selected="True" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <!-- Slab 3 -->
                                <div class="row mb-2">
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab3Time" runat="server" CssClass="form-label" Text="Slab 3 - Action Time"></asp:Label>
                                        <asp:TextBox ID="tsSlab3Time" runat="server" CssClass="form-control" placeholder="12:30:00" Text="12:30:00" />
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab3Relay" runat="server" CssClass="form-label" Text="Slab 3 - Relay Operation (0=OFF, 1=ON)"></asp:Label>
                                        <asp:DropDownList ID="ddlSlab3Relay" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="OFF (0)" Value="0" Selected="True" />
                                            <asp:ListItem Text="ON (1)" Value="1" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <!-- Slab 4 -->
                                <div class="row mb-2">
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab4Time" runat="server" CssClass="form-label" Text="Slab 4 - Action Time"></asp:Label>
                                        <asp:TextBox ID="tsSlab4Time" runat="server" CssClass="form-control" placeholder="12:45:00" Text="12:45:00" />
                                    </div>
                                    <div class="col-md-6">
                                        <asp:Label ID="lblSlab4Relay" runat="server" CssClass="form-label" Text="Slab 4 - Relay Operation (0=OFF, 1=ON)"></asp:Label>
                                        <asp:DropDownList ID="ddlSlab4Relay" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="OFF (0)" Value="0" />
                                            <asp:ListItem Text="ON (1)" Value="1" Selected="True" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                                <small class="form-text text-muted">Enter action times in HH:mm:ss format. Relay operation: 0=OFF, 1=ON</small>
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnLoadSheddingSchedule"
                                runat="server"
                                CssClass="btn btn-dark-primary"
                                Text="Send Load Shedding Schedule"
                                OnClick="btnLoadSheddingSchedule_Click"
                                OnClientClick="showLoadSheddingScheduleLoading(this); return true;" />

                            <span id="loadSheddingScheduleSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="LoadSheddingScheduling.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblLoadSheddingScheduleMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                <!-- Hidden controls for fail reason functionality -->
                <asp:HiddenField ID="hfFailTableName" runat="server" />
                <asp:TextBox ID="txtFailReason" runat="server" Style="display: none;" />

                  <asp:UpdatePanel ID="updTracker" runat="server">
                      <ContentTemplate>

                          <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">

                              <div class="card-header border-0">
                                  <div class="d-flex justify-content-between align-items-center">
                                      <div>
                                          <h5 class="mb-0">Load Shedding Schedule Tracker</h5>
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
                        <asp:AsyncPostBackTrigger ControlID="btnLoadSheddingSchedule" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <!-- Data Tables Section -->
                <asp:UpdatePanel ID="updDataTables" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="pnlDataTables" runat="server" Visible="false" CssClass="mt-4">
                            <div class="card mb-4">
                                <div class="card-header d-flex justify-content-between align-items-center">
                                    <h6 class="mb-0">Meter Visuals</h6>
                                    <div>
                                        <asp:Button ID="btnMeterVisualsPass" runat="server" CssClass="btn btn-success btn-sm" Text="Pass" OnCommand="TableButton_Command" CommandName="Pass" CommandArgument="MeterVisuals" />
                                        <asp:Button ID="btnMeterVisualsFail" runat="server" CssClass="btn btn-danger btn-sm ms-2" Text="Fail" OnCommand="TableButton_Command" CommandName="Fail" CommandArgument="MeterVisuals" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvMeterVisuals" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="false" GridLines="None">
                                            <Columns>
                                                <asp:BoundField DataField="msn" HeaderText="MSN" />
                                                <asp:BoundField DataField="global_device_id" HeaderText="Global Device ID" />
                                                <asp:BoundField DataField="last_command" HeaderText="Last Command" />
                                                <asp:BoundField DataField="last_command_datetime" HeaderText="Last Command DateTime" />
                                                <asp:BoundField DataField="last_command_resp" HeaderText="Last Command Response" />
                                                <asp:BoundField DataField="lsch_datetime" HeaderText="LSS DateTime" />
                                                <asp:BoundField DataField="lsch_start_datetime" HeaderText="Start DateTime" />
                                                <asp:BoundField DataField="lsch_end_datetime" HeaderText="End DateTime" />
                                                <asp:BoundField DataField="lsch_load_shedding_slabs" HeaderText="Slabs" />
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="text-muted">No meter visual records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                    <!-- Remarks Section for Meter Visuals -->
                                    <div id="meterVisualsRemarks" runat="server" class="mt-3" style="display: none;">
                                        <div class="card border-warning">
                                            <div class="card-body">
                                                <h6 class="card-title">Remarks</h6>
                                                <asp:TextBox ID="txtMeterVisualsRemarks" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveMeterVisualsRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1" Text="Save Remarks" OnCommand="SaveRemarks_Command" CommandName="Save" CommandArgument="MeterVisuals" />
                                                    <asp:Button ID="btnCancelMeterVisualsRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2" Text="Cancel" OnCommand="CancelRemarks_Command" CommandName="Cancel" CommandArgument="MeterVisuals" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Communication History Table -->
                            <div class="card mb-4">
                                <div class="card-header d-flex justify-content-between align-items-center">
                                    <h6 class="mb-0">Communication History</h6>
                                    <div>
                                        <asp:Button ID="btnCommunicationHistoryPass" runat="server" CssClass="btn btn-success btn-sm" Text="Pass" OnCommand="TableButton_Command" CommandName="Pass" CommandArgument="CommunicationHistory" />
                                        <asp:Button ID="btnCommunicationHistoryFail" runat="server" CssClass="btn btn-danger btn-sm ms-2" Text="Fail" OnCommand="TableButton_Command" CommandName="Fail" CommandArgument="CommunicationHistory" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvCommunicationHistory" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="false" GridLines="None">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Message Log">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMessageLog" runat="server" Text='<%# Eval("message_log") %>' CssClass="text-break" style="white-space: pre-wrap; font-family: monospace; font-size: 12px;"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="text-muted">No communication history records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                    <!-- Remarks Section for Communication History -->
                                    <div id="commHistoryRemarks" runat="server" class="mt-3" style="display: none;">
                                        <div class="card border-warning">
                                            <div class="card-body">
                                                <h6 class="card-title">Remarks</h6>
                                                <asp:TextBox ID="txtCommHistoryRemarks" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveCommHistoryRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1" Text="Save Remarks" OnCommand="SaveRemarks_Command" CommandName="Save" CommandArgument="CommunicationHistory" />
                                                    <asp:Button ID="btnCancelCommHistoryRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2" Text="Cancel" OnCommand="CancelRemarks_Command" CommandName="Cancel" CommandArgument="CommunicationHistory" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="card mb-4">
                                <div class="card-header d-flex justify-content-between align-items-center">
                                    <h6 class="mb-0">Events</h6>
                                    <div>
                                        <asp:Button ID="btnEventsPass" runat="server" CssClass="btn btn-success btn-sm" Text="Pass" OnCommand="TableButton_Command" CommandName="Pass" CommandArgument="Events" />
                                        <asp:Button ID="btnEventsFail" runat="server" CssClass="btn btn-danger btn-sm ms-2" Text="Fail" OnCommand="TableButton_Command" CommandName="Fail" CommandArgument="Events" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvEvents" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="false" GridLines="None">
                                            <Columns>
                                                <asp:BoundField DataField="id" HeaderText="Event ID" />
                                                <asp:BoundField DataField="event_code" HeaderText="Event Code" />
                                                <asp:BoundField DataField="event_datetime" HeaderText="Event DateTime" />
                                                <asp:BoundField DataField="event_description" HeaderText="Event Description" />
                                                <asp:BoundField DataField="mdc_read_datetime" HeaderText="MDC Read Date Time" />
                                                <asp:BoundField DataField="db_datetime" HeaderText="DB Date Time" />
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="text-muted">No event records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                    <!-- Remarks Section for Events -->
                                    <div id="eventsRemarks" runat="server" class="mt-3" style="display: none;">
                                        <div class="card border-warning">
                                            <div class="card-body">
                                                <h6 class="card-title">Remarks</h6>
                                                <asp:TextBox ID="txtEventsRemarks" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveEventsRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1" Text="Save Remarks" OnCommand="SaveRemarks_Command" CommandName="Save" CommandArgument="Events" />
                                                    <asp:Button ID="btnCancelEventsRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2" Text="Cancel" OnCommand="CancelRemarks_Command" CommandName="Cancel" CommandArgument="Events" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <asp:Timer ID="timerTables" runat="server" Interval="2000" OnTick="timerTables_Tick" Enabled="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showLoadSheddingScheduleLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('loadSheddingScheduleSpinner');

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
        function disableLoadSheddingScheduleButton() {
            const button = document.getElementById('<%= btnLoadSheddingSchedule.ClientID %>');
            const spinner = document.getElementById('loadSheddingScheduleSpinner');


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
        function resetLoadSheddingScheduleLoading() {
            const button = document.getElementById('<%= btnLoadSheddingSchedule.ClientID %>');
            const spinner = document.getElementById('loadSheddingScheduleSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Send Load Shedding Schedule';
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


