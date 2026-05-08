<%@ Page Title="UDIL Tester - LSCH - ODR" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="LSCH.aspx.cs" Inherits="UDIL.Read.LSCH" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="lsch-read">
                <h2 class="section-header"><i class="bi bi-search"></i> LSCH - On Demand Read</h2>
                <div class="card">

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblLschTransactionId" runat="server" AssociatedControlID="lschTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="lschTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblLschPrivateKey" runat="server" AssociatedControlID="lschPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="lschPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblLschGlobalDeviceId" runat="server" AssociatedControlID="lschGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="lschGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m97999989" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblLschType" runat="server" AssociatedControlID="lschType" CssClass="form-label" Text="Type"></asp:Label>
                                <asp:DropDownList ID="lschType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="LSCH" Selected="True">LSCH</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnLschRead"
                                runat="server"
                                CssClass="btn btn-dark-primary"
                                Text="Read LSCH"
                                OnClick="btnLschRead_Click"
                                OnClientClick="showLschLoading(this); return true;" />

                            <span id="lschSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="LSCH.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblLschMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
                    </div>
                 </div>

                <!-- Response Display Section -->
                <asp:UpdatePanel ID="updResponse" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="pnlResponse" runat="server" Visible="false" CssClass="mt-4">
                            <div class="card shadow-sm border-0">
                                <div class="card-header border-0">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <div class="d-flex align-items-center gap-3">
                                            <h5 class="mb-0">LSCH Response</h5>
                                            <asp:Label ID="lblResponseStatus" runat="server" CssClass="badge bg-success px-3 py-2" />
                                        </div>
                                        <div>
                                            <asp:Button ID="btnLschResponsePass" runat="server" CssClass="btn btn-success btn-sm" Text="Pass" OnClick="btnLschResponsePass_Click" />
                                            <asp:Button ID="btnLschResponseFail" runat="server" CssClass="btn btn-danger btn-sm ms-2" Text="Fail" OnClick="btnLschResponseFail_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row mb-3">
                                        <div class="col-md-6">
                                            <strong>Transaction ID:</strong>
                                            <asp:Label ID="lblResponseTransactionId" runat="server" CssClass="ms-2" />
                                        </div>
                                        <div class="col-md-6">
                                            <strong>Message:</strong>
                                            <asp:Label ID="lblResponseMessage" runat="server" CssClass="ms-2" />
                                        </div>
                                    </div>
                                    <hr />
                                    <h6 class="mb-3">LSCH Data</h6>
                                    <div class="row g-3">
                                        <div class="col-md-3">
                                            <div class="card h-100 border-light">
                                                <div class="card-body">
                                                    <small class="text-muted d-block">Global Device ID</small>
                                                    <asp:Label ID="lblRespGlobalDeviceId" runat="server" CssClass="fw-bold" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="card h-100 border-light">
                                                <div class="card-body">
                                                    <small class="text-muted d-block">MSN</small>
                                                    <asp:Label ID="lblRespMsn" runat="server" CssClass="fw-bold" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="card h-100 border-light">
                                                <div class="card-body">
                                                    <small class="text-muted d-block">LSCH DateTime</small>
                                                    <asp:Label ID="lblRespLschDateTime" runat="server" CssClass="fw-bold" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="card h-100 border-light">
                                                <div class="card-body">
                                                    <small class="text-muted d-block">LSCH Start DateTime</small>
                                                    <asp:Label ID="lblRespLschStartDateTime" runat="server" CssClass="fw-bold" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="card h-100 border-light">
                                                <div class="card-body">
                                                    <small class="text-muted d-block">LSCH End DateTime</small>
                                                    <asp:Label ID="lblRespLschEndDateTime" runat="server" CssClass="fw-bold" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- Response Remarks Section -->
                                    <div id="lschResponseRemarks" runat="server" class="mt-3" style="display: none;">
                                        <div class="card border-warning">
                                            <div class="card-body">
                                                <h6 class="card-title">Remarks</h6>
                                                <asp:TextBox ID="txtLschResponseRemarks" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="2" placeholder="Enter remarks..."></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveLschResponseRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1" Text="Save Remarks" OnClick="btnSaveLschResponseRemarks_Click" />
                                                    <asp:Button ID="btnCancelLschResponseRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2" Text="Cancel" OnClick="btnCancelLschResponseRemarks_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnLschRead" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnLschResponsePass" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnLschResponseFail" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveLschResponseRemarks" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelLschResponseRemarks" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <!-- Hidden controls for fail reason functionality -->
                <asp:HiddenField ID="hfFailTableName" runat="server" />
                <asp:TextBox ID="txtFailReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter reason..." style="display: none;" />

                <!-- Data Tables Section -->
                <asp:UpdatePanel ID="updDataTables" runat="server">
                    <ContentTemplate>
                        <asp:Panel ID="pnlDataTables" runat="server" Visible="false" CssClass="mt-4">
                            <h4 class="mb-4">Device Data Tables</h4>

                            <!-- Meter Visuals Table -->
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

                            <!-- Events Table -->
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
                                                <asp:BoundField DataField="id" HeaderText="ID" />
                                                <asp:BoundField DataField="msn" HeaderText="MSN" />
                                                <asp:BoundField DataField="global_device_id" HeaderText="Global Device ID" />
                                                <asp:BoundField DataField="event_code" HeaderText="Event Code" />
                                                <asp:BoundField DataField="event_counter" HeaderText="Event Counter" />
                                                <asp:BoundField DataField="event_description" HeaderText="Event Description" />
                                                <asp:BoundField DataField="event_datetime" HeaderText="Event DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                                <asp:BoundField DataField="mdc_read_datetime" HeaderText="MDC Read DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                                <asp:BoundField DataField="db_datetime" HeaderText="DB DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />

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
                        </asp:Panel>

                        <!-- Timer for refreshing tables -->
                        <asp:Timer ID="timerTables" runat="server" Interval="2000" OnTick="timerTables_Tick" Enabled="false" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>



        </div>
    </div>

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">

    <script>
        function showLschLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('lschSpinner');

            // Show spinner and change text immediately
            if (spinner) {
                spinner.style.display = 'inline-block';
                button.style.opacity = '0.7';
                button.style.cursor = 'not-allowed';
            }

            // Change text
            const originalText = button.value;
            button.value = 'Reading...';

            // store original text
            button.setAttribute('data-original-text', originalText);

            // Don't disable button immediately - let server handle it
            // This ensures the server-side event fires properly
            return true;
        }

        // Function to disable button after successful processing
        function disableLschButton() {
            const button = document.getElementById('<%= btnLschRead.ClientID %>');
            const spinner = document.getElementById('lschSpinner');


            if (button) {
                button.disabled = true;
                button.style.opacity = '0.7';
                button.style.cursor = 'not-allowed';

                // Keep "Reading..." text to show completion
                button.value = 'Processing...';
            }

            if (spinner) {
                spinner.style.display = 'inline-block';
            }
        }

        // Optional reset function
        function resetLschLoading() {
            const button = document.getElementById('<%= btnLschRead.ClientID %>');
            const spinner = document.getElementById('lschSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Read LSCH';
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

