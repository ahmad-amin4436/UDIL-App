<%@ Page Title="UDIL Tester - Update Major Alarms" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="UpdateMajorAlarms.aspx.cs" Inherits="UDIL.Pages.UpdateMajorAlarms" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="ip-port-update">
                <h2 class="section-header"><i class="bi bi-exclamation-triangle"></i> Update Major Alarms</h2>
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
                            <div class="col-12">
                                <h5 class="mb-3">Major Alarms Configuration</h5>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode1" runat="server" AssociatedControlID="wsEventCode1" CssClass="form-label" Text="Event Code 1"></asp:Label>
                                <asp:TextBox ID="wsEventCode1" runat="server" CssClass="form-control" placeholder="209" Text="209" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription1" runat="server" AssociatedControlID="wsEventDescription1" CssClass="form-label" Text="Event Description 1"></asp:Label>
                                <asp:TextBox ID="wsEventDescription1" runat="server" CssClass="form-control" placeholder="Sim Card Removed" Text="Sim Card Removed" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode2" runat="server" AssociatedControlID="wsEventCode2" CssClass="form-label" Text="Event Code 2"></asp:Label>
                                <asp:TextBox ID="wsEventCode2" runat="server" CssClass="form-control" placeholder="210" Text="210" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription2" runat="server" AssociatedControlID="wsEventDescription2" CssClass="form-label" Text="Event Description 2"></asp:Label>
                                <asp:TextBox ID="wsEventDescription2" runat="server" CssClass="form-control" placeholder="Sim Card Inserted" Text="Sim Card Inserted" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode3" runat="server" AssociatedControlID="wsEventCode3" CssClass="form-label" Text="Event Code 3"></asp:Label>
                                <asp:TextBox ID="wsEventCode3" runat="server" CssClass="form-control" placeholder="101" Text="101" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription3" runat="server" AssociatedControlID="wsEventDescription3" CssClass="form-label" Text="Event Description 3"></asp:Label>
                                <asp:TextBox ID="wsEventDescription3" runat="server" CssClass="form-control" placeholder="MDI RESET" Text="MDI RESET" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode4" runat="server" AssociatedControlID="wsEventCode4" CssClass="form-label" Text="Event Code 4"></asp:Label>
                                <asp:TextBox ID="wsEventCode4" runat="server" CssClass="form-control" placeholder="301" Text="301" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription4" runat="server" AssociatedControlID="wsEventDescription4" CssClass="form-label" Text="Event Description 4"></asp:Label>
                                <asp:TextBox ID="wsEventDescription4" runat="server" CssClass="form-control" placeholder="Optical Port Login" Text="Optical Port Login" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode5" runat="server" AssociatedControlID="wsEventCode5" CssClass="form-label" Text="Event Code 5"></asp:Label>
                                <asp:TextBox ID="wsEventCode5" runat="server" CssClass="form-control" placeholder="202" Text="202" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription5" runat="server" AssociatedControlID="wsEventDescription5" CssClass="form-label" Text="Event Description 5"></asp:Label>
                                <asp:TextBox ID="wsEventDescription5" runat="server" CssClass="form-control" placeholder="Contactor ON" Text="Contactor ON" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode6" runat="server" AssociatedControlID="wsEventCode6" CssClass="form-label" Text="Event Code 6"></asp:Label>
                                <asp:TextBox ID="wsEventCode6" runat="server" CssClass="form-control" placeholder="203" Text="203" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription6" runat="server" AssociatedControlID="wsEventDescription6" CssClass="form-label" Text="Event Description 6"></asp:Label>
                                <asp:TextBox ID="wsEventDescription6" runat="server" CssClass="form-control" placeholder="Contactor Off" Text="Contactor Off" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode7" runat="server" AssociatedControlID="wsEventCode7" CssClass="form-label" Text="Event Code 7"></asp:Label>
                                <asp:TextBox ID="wsEventCode7" runat="server" CssClass="form-control" placeholder="328" Text="328" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription7" runat="server" AssociatedControlID="wsEventDescription7" CssClass="form-label" Text="Event Description 7"></asp:Label>
                                <asp:TextBox ID="wsEventDescription7" runat="server" CssClass="form-control" placeholder="Bi-directional Mode" Text="Bi-directional Mode" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode8" runat="server" AssociatedControlID="wsEventCode8" CssClass="form-label" Text="Event Code 8"></asp:Label>
                                <asp:TextBox ID="wsEventCode8" runat="server" CssClass="form-control" placeholder="206" Text="206" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription8" runat="server" AssociatedControlID="wsEventDescription8" CssClass="form-label" Text="Event Description 8"></asp:Label>
                                <asp:TextBox ID="wsEventDescription8" runat="server" CssClass="form-control" placeholder="Door Open" Text="Door Open" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode9" runat="server" AssociatedControlID="wsEventCode9" CssClass="form-label" Text="Event Code 9"></asp:Label>
                                <asp:TextBox ID="wsEventCode9" runat="server" CssClass="form-control" placeholder="207" Text="207" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription9" runat="server" AssociatedControlID="wsEventDescription9" CssClass="form-label" Text="Event Description 9"></asp:Label>
                                <asp:TextBox ID="wsEventDescription9" runat="server" CssClass="form-control" placeholder="Battery low" Text="Battery low" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventCode10" runat="server" AssociatedControlID="wsEventCode10" CssClass="form-label" Text="Event Code 10"></asp:Label>
                                <asp:TextBox ID="wsEventCode10" runat="server" CssClass="form-control" placeholder="302" Text="302" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblEventDescription10" runat="server" AssociatedControlID="wsEventDescription10" CssClass="form-label" Text="Event Description 10"></asp:Label>
                                <asp:TextBox ID="wsEventDescription10" runat="server" CssClass="form-control" placeholder="Optical Port Login Failure" Text="Optical Port Login Failure" />
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnUpdateMajorAlarms"
                                runat="server"
                                CssClass="btn btn-dark-primary"
                                Text="Update Major Alarms"
                                OnClick="btnUpdateMajorAlarms_Click"
                                OnClientClick="showUpdateMajorAlarmsLoading(this); return true;" />

                            <span id="updateMajorAlarmsSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="UpdateMajorAlarms.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblWakeSimNumberMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                <!-- Hidden controls for fail reason functionality -->
                <asp:HiddenField ID="hfFailTableName" runat="server" />
                <asp:TextBox ID="txtFailReason" runat="server" style="display:none;" />

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
                        <asp:AsyncPostBackTrigger ControlID="btnUpdateMajorAlarms" EventName="Click" />
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
                                        <asp:Button ID="btnMeterVisualsPass" runat="server" CssClass="btn btn-success btn-sm"
                                            Text="Pass" CommandArgument="MeterVisuals" CommandName="Pass" OnCommand="TableButton_Command" />
                                        <asp:Button ID="btnMeterVisualsFail" runat="server" CssClass="btn btn-danger btn-sm ms-2"
                                            Text="Fail" CommandArgument="MeterVisuals" CommandName="Fail" OnCommand="TableButton_Command" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvMeterVisuals" runat="server" CssClass="table table-striped table-hover"
                                            AutoGenerateColumns="false" GridLines="None">
                                            <Columns>
                                                <asp:BoundField DataField="msn" HeaderText="MSN" />
                                                <asp:BoundField DataField="global_device_id" HeaderText="Global Device ID" />
                                                <asp:BoundField DataField="last_command" HeaderText="Last Command" />
                                                <asp:BoundField DataField="last_command_datetime" HeaderText="Last Command DateTime" />
                                                <asp:BoundField DataField="last_command_resp" HeaderText="Last Command Response" />
                                                <asp:BoundField DataField="wsim_datetime" HeaderText="Wake Up Number 1" />
                                                <asp:BoundField DataField="wsim_wakeup_number_1" HeaderText="Wake Up Number 1" />
                                                <asp:BoundField DataField="wsim_wakeup_number_2" HeaderText="Wake Up Number 2" />
                                                <asp:BoundField DataField="wsim_wakeup_number_3" HeaderText="Wake Up Number 3" />
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
                                                <asp:TextBox ID="txtMeterVisualsRemarks" runat="server" CssClass="form-control mb-2"
                                                    TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveMeterVisualsRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1"
                                                        Text="Save Remarks" CommandArgument="MeterVisuals" OnCommand="SaveRemarks_Command" />
                                                    <asp:Button ID="btnCancelMeterVisualsRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2"
                                                        Text="Cancel" CommandArgument="MeterVisuals" OnCommand="CancelRemarks_Command" />
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
                                        <asp:Button ID="btnCommunicationHistoryPass" runat="server" CssClass="btn btn-success btn-sm"
                                            Text="Pass" CommandArgument="CommunicationHistory" CommandName="Pass" OnCommand="TableButton_Command" />
                                        <asp:Button ID="btnCommunicationHistoryFail" runat="server" CssClass="btn btn-danger btn-sm ms-2"
                                            Text="Fail" CommandArgument="CommunicationHistory" CommandName="Fail" OnCommand="TableButton_Command" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvCommunicationHistory" runat="server" CssClass="table table-striped table-hover"
                                            AutoGenerateColumns="false" GridLines="None">
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
                                                <asp:TextBox ID="txtCommHistoryRemarks" runat="server" CssClass="form-control mb-2"
                                                    TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveCommHistoryRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1"
                                                        Text="Save Remarks" CommandArgument="CommunicationHistory" OnCommand="SaveRemarks_Command" />
                                                    <asp:Button ID="btnCancelCommHistoryRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2"
                                                        Text="Cancel" CommandArgument="CommunicationHistory" OnCommand="CancelRemarks_Command" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Events Table -->
                            <div class="card mb-4">
                                <div class="card-header d-flex justify-content-between align-items-center">
                                    <h6 class="mb-0">Events</h6>
                                    <div>
                                        <asp:Button ID="btnEventsPass" runat="server" CssClass="btn btn-success btn-sm"
                                            Text="Pass" CommandArgument="Events" CommandName="Pass" OnCommand="TableButton_Command" />
                                        <asp:Button ID="btnEventsFail" runat="server" CssClass="btn btn-danger btn-sm ms-2"
                                            Text="Fail" CommandArgument="Events" CommandName="Fail" OnCommand="TableButton_Command" />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvEvents" runat="server" CssClass="table table-striped table-hover"
                                            AutoGenerateColumns="false" GridLines="None">
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
                                                <asp:TextBox ID="txtEventsRemarks" runat="server" CssClass="form-control mb-2"
                                                    TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveEventsRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1"
                                                        Text="Save Remarks" CommandArgument="Events" OnCommand="SaveRemarks_Command" />
                                                    <asp:Button ID="btnCancelEventsRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2"
                                                        Text="Cancel" CommandArgument="Events" OnCommand="CancelRemarks_Command" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <asp:Timer ID="timerTables" runat="server" Interval="2000"
                                OnTick="timerTables_Tick" Enabled="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>


           
        </div>
    </div>
    
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function showUpdateMajorAlarmsLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('wakeSimNumberSpinner');

            // Show spinner and change text immediately
            if (spinner) 
            {
                spinner.style.display = 'inline-block';
                button.style.opacity = '0.7';
                button.style.cursor = 'not-allowed';
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
        function disableUpdateMajorAlarmsButton() {
            const button = document.getElementById('<%= btnUpdateMajorAlarms.ClientID %>');
            const spinner = document.getElementById('updateMajorAlarmsSpinner');


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
        function resetUpdateMajorAlarmsLoading() {
            const button = document.getElementById('<%= btnUpdateMajorAlarms.ClientID %>');
            const spinner = document.getElementById('updateMajorAlarmsSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Update Major Alarms';
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


