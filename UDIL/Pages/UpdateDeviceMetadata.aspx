<%@ Page Title="UDIL Tester - Update Device Metadata" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="UpdateDeviceMetadata.aspx.cs" Inherits="UDIL.Pages.UpdateDeviceMetadata" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="device-creation">
                <h2 class="section-header"><i class="bi bi-gear"></i> Update Device Metadata</h2>
                <div class="card">
                   
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDmTransactionId" runat="server" AssociatedControlID="dmTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="dmTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDmPrivateKey" runat="server" AssociatedControlID="dmPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="dmPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDmGlobalDeviceId" runat="server" AssociatedControlID="dmGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="dmGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m98999997" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDmRequestDateTime" runat="server" AssociatedControlID="dmRequestDateTime" CssClass="form-label" Text="Request DateTime"></asp:Label>
                                <asp:TextBox ID="dmRequestDateTime" runat="server" Text="2024-08-31 14:30:00" CssClass="form-control" placeholder="2024-08-31 14:30:00" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDmCommunicationMode" runat="server" AssociatedControlID="dmCommunicationMode" CssClass="form-label" Text="Communication Mode"></asp:Label>
                                <asp:DropDownList ID="dmCommunicationMode" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1" Selected="True">1 - GPRS/3G/4G</asp:ListItem>
                                    <asp:ListItem Value="2">2 - RF</asp:ListItem>
                                    <asp:ListItem Value="3">3 - PLC</asp:ListItem>
                                    <asp:ListItem Value="4">4 - Ethernet</asp:ListItem>
                                    <asp:ListItem Value="5">5 - Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDmBidirectionalDevice" runat="server" AssociatedControlID="dmBidirectionalDevice" CssClass="form-label" Text="Bidirectional Device"></asp:Label>
                                <asp:DropDownList ID="dmBidirectionalDevice" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="0" Selected="True">0 - False</asp:ListItem>
                                    <asp:ListItem Value="1">1 - True</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDmCommunicationType" runat="server" AssociatedControlID="dmCommunicationType" CssClass="form-label" Text="Communication Type"></asp:Label>
                                <asp:DropDownList ID="dmCommunicationType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="2" Selected="True">2 - Mode-II / Keep-alive</asp:ListItem>
                                    <asp:ListItem Value="1">1 - Mode-I / Non-Keepalive</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDmInitialCommunicationTime" runat="server" AssociatedControlID="dmInitialCommunicationTime" CssClass="form-label" Text="Initial Communication Time"></asp:Label>
                                <asp:TextBox ID="dmInitialCommunicationTime" runat="server" CssClass="form-control" Text="00:00:00" placeholder="00:00:00" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDmCommunicationInterval" runat="server" AssociatedControlID="dmCommunicationInterval" CssClass="form-label" Text="Communication Interval"></asp:Label>
                                <asp:TextBox ID="dmCommunicationInterval" runat="server" CssClass="form-control" Text="30" placeholder="30" />
                            </div>
                            <div class="col-md-4 mb-3">
                                <asp:Label ID="lblDmMeterType" runat="server" AssociatedControlID="dmMeterType" CssClass="form-label" Text="Meter Type"></asp:Label>
                                <asp:DropDownList ID="dmMeterType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - Normal</asp:ListItem>
                                    <asp:ListItem Value="2" Selected="True">2 - Whole Current</asp:ListItem>
                                    <asp:ListItem Value="3">3 - CTO</asp:ListItem>
                                    <asp:ListItem Value="4">4 - CTPT</asp:ListItem>
                                    <asp:ListItem Value="5">5 - Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblDmPhase" runat="server" AssociatedControlID="dmPhase" CssClass="form-label" Text="Phase"></asp:Label>
                                <asp:DropDownList ID="dmPhase" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="1">1 - Single</asp:ListItem>
                                    <asp:ListItem Value="2">2 - Other</asp:ListItem>
                                    <asp:ListItem Value="3" Selected="True">3 - Three-phase</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnUpdateDeviceMetadata"
                                runat="server"
                                CssClass="btn btn-dark-primary"
                                Text="Update Device Metadata"
                                OnClick="btnUpdateDeviceMetadata_Click"
                                OnClientClick="showUpdateDeviceMetadataLoading(this); return true;" />

                            <span id="updateDeviceMetadataSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="UpdateDeviceMetadata.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblUpdateDeviceMetadataMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
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
                                          <h5 class="mb-0">Update Device Metadata Tracker</h5>
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
                        <asp:AsyncPostBackTrigger ControlID="btnUpdateDeviceMetadata" EventName="Click" />
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
                                                <asp:BoundField DataField="dmdt_meter_type" HeaderText="Meter Type" />
                                                <asp:BoundField DataField="dmdt_bidirectional_device" HeaderText="Bidirectional Device" />
                                                <asp:BoundField DataField="dmdt_communication_mode" HeaderText="Communication Mode" />
                                                <asp:BoundField DataField="dmdt_communication_type" HeaderText="Communication Type" />
                                                <asp:BoundField DataField="dmdt_communication_interval" HeaderText="Communication Interval" />
                                                <asp:BoundField DataField="dmdt_initial_communication_time" HeaderText="Initial Communication Time" />
                                                <asp:BoundField DataField="dmdt_phase" HeaderText="Phase" />
                                                <asp:BoundField DataField="mdi_reset_date" HeaderText="MDI Reset Date" />
                                                <asp:BoundField DataField="mdi_reset_time" HeaderText="MDI Reset Time" />
                                                <asp:BoundField DataField="msim_id" HeaderText="SIM ID" />
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
        function showUpdateDeviceMetadataLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('updateDeviceMetadataSpinner');

            // Show spinner and change text immediately
            if (spinner) {
                spinner.style.display = 'inline-block';
                button.style.opacity = '0.7';
                button.style.cursor = 'not-allowed';
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
        function disableUpdateDeviceMetadataButton() {
            const button = document.getElementById('<%= btnUpdateDeviceMetadata.ClientID %>');
            const spinner = document.getElementById('updateDeviceMetadataSpinner');


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
        function resetUpdateDeviceMetadataLoading() {
            const button = document.getElementById('<%= btnUpdateDeviceMetadata.ClientID %>');
            const spinner = document.getElementById('updateDeviceMetadataSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Update Device Metadata';
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
