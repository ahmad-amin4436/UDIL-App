<%@ Page Title="UDIL Tester - MBILL - ODR" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="MBILL.aspx.cs" Inherits="UDIL.Read.MBILL" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="mbill-read">
                <h2 class="section-header"><i class="bi bi-search"></i> MBILL - On Demand Read</h2>
                <div class="card">

                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMbillTransactionId" runat="server" AssociatedControlID="mbillTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                <asp:TextBox ID="mbillTransactionId" runat="server" CssClass="form-control" ReadOnly="true" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMbillPrivateKey" runat="server" AssociatedControlID="mbillPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                <asp:TextBox ID="mbillPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" Enabled="false" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMbillGlobalDeviceId" runat="server" AssociatedControlID="mbillGlobalDeviceId" CssClass="form-label" Text="Global Device ID"></asp:Label>
                                <asp:TextBox ID="mbillGlobalDeviceId" runat="server" CssClass="form-control" placeholder="Global Device ID" Text="m97999989" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMbillType" runat="server" AssociatedControlID="mbillType" CssClass="form-label" Text="Type"></asp:Label>
                                <asp:DropDownList ID="mbillType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="MBIL" Selected="True">MBIL</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMbillStartDateTime" runat="server" AssociatedControlID="mbillStartDateTime" CssClass="form-label" Text="Start DateTime"></asp:Label>
                                <asp:TextBox ID="mbillStartDateTime" runat="server" CssClass="form-control" placeholder="YYYY-MM-DD HH:mm:ss" Text="2026-04-11 09:00:00" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <asp:Label ID="lblMbillEndDateTime" runat="server" AssociatedControlID="mbillEndDateTime" CssClass="form-label" Text="End DateTime"></asp:Label>
                                <asp:TextBox ID="mbillEndDateTime" runat="server" CssClass="form-control" placeholder="YYYY-MM-DD HH:mm:ss" Text="2026-05-11 10:00:00" />
                            </div>
                        </div>
                        <div class="position-relative d-inline-block">
                            <asp:Button
                                ID="btnMbillRead"
                                runat="server"
                                CssClass="btn btn-dark-primary"
                                Text="Read MBILL"
                                OnClick="btnMbillRead_Click"
                                OnClientClick="showMbillLoading(this); return true;" />

                            <span id="mbillSpinner"
                                class="position-absolute top-50 start-50 translate-middle"
                                style="display: none;">
                                <span class="spinner-border spinner-border-sm"></span>
                            </span>
                        </div>
                        <a href="MBILL.aspx" class="btn btn-secondary ms-2">Reset</a>
                        <asp:Label ID="lblMbillMessage" runat="server" CssClass="mt-3 d-block"></asp:Label>
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
                                            <h5 class="mb-0">MBILL Response</h5>
                                            <asp:Label ID="lblResponseStatus" runat="server" CssClass="badge bg-success px-3 py-2" />
                                        </div>
                                        <div>
                                            <asp:Button ID="btnMbillResponsePass" runat="server" CssClass="btn btn-success btn-sm" Text="Pass" OnClick="btnMbillResponsePass_Click" />
                                            <asp:Button ID="btnMbillResponseFail" runat="server" CssClass="btn btn-danger btn-sm ms-2" Text="Fail" OnClick="btnMbillResponseFail_Click" />
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
                                    <h6 class="mb-3">MBILL Data</h6>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                
                <!-- Separate UpdatePanel for MBILL Data to prevent scroll reset during timer ticks -->
                <asp:UpdatePanel ID="updMbillData" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <asp:Panel ID="pnlMbillData" runat="server" Visible="false" CssClass="mt-4">
                            <div class="card shadow-sm border-0">
                                <div class="card-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvMbillData" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="false" GridLines="None">
                                            <Columns>
                                                <asp:BoundField DataField="meter_datetime" HeaderText="Meter DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                                <asp:BoundField DataField="global_device_id" HeaderText="Global Device ID" />
                                                <asp:BoundField DataField="msn" HeaderText="MSN" />
                                                <asp:BoundField DataField="mdi_reset_datetime" HeaderText="MDI Reset DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                                <asp:BoundField DataField="reset_count" HeaderText="Reset Count" />
                                                <asp:BoundField DataField="longitude" HeaderText="Longitude" />
                                                <asp:BoundField DataField="latitude" HeaderText="Latitude" />
                                                <asp:BoundField DataField="reading_mode" HeaderText="Reading Mode" />
                                                <asp:BoundField DataField="picture_1" HeaderText="Picture 1" />
                                                <asp:BoundField DataField="picture_2" HeaderText="Picture 2" />
                                                <asp:BoundField DataField="active_energy_abs_t1" HeaderText="Active Energy Abs T1" />
                                                <asp:BoundField DataField="active_energy_abs_t2" HeaderText="Active Energy Abs T2" />
                                                <asp:BoundField DataField="active_energy_abs_t3" HeaderText="Active Energy Abs T3" />
                                                <asp:BoundField DataField="active_energy_abs_t4" HeaderText="Active Energy Abs T4" />
                                                <asp:BoundField DataField="active_energy_abs_tl" HeaderText="Active Energy Abs TL" />
                                                <asp:BoundField DataField="active_energy_pos_t1" HeaderText="Active Energy Pos T1" />
                                                <asp:BoundField DataField="active_energy_pos_t2" HeaderText="Active Energy Pos T2" />
                                                <asp:BoundField DataField="active_energy_pos_t3" HeaderText="Active Energy Pos T3" />
                                                <asp:BoundField DataField="active_energy_pos_t4" HeaderText="Active Energy Pos T4" />
                                                <asp:BoundField DataField="active_energy_pos_tl" HeaderText="Active Energy Pos TL" />
                                                <asp:BoundField DataField="active_energy_neg_t1" HeaderText="Active Energy Neg T1" />
                                                <asp:BoundField DataField="active_energy_neg_t2" HeaderText="Active Energy Neg T2" />
                                                <asp:BoundField DataField="active_energy_neg_t3" HeaderText="Active Energy Neg T3" />
                                                <asp:BoundField DataField="active_energy_neg_t4" HeaderText="Active Energy Neg T4" />
                                                <asp:BoundField DataField="active_energy_neg_tl" HeaderText="Active Energy Neg TL" />
                                                <asp:BoundField DataField="reactive_energy_abs_t1" HeaderText="Reactive Energy Abs T1" />
                                                <asp:BoundField DataField="reactive_energy_abs_t2" HeaderText="Reactive Energy Abs T2" />
                                                <asp:BoundField DataField="reactive_energy_abs_t3" HeaderText="Reactive Energy Abs T3" />
                                                <asp:BoundField DataField="reactive_energy_abs_t4" HeaderText="Reactive Energy Abs T4" />
                                                <asp:BoundField DataField="reactive_energy_abs_tl" HeaderText="Reactive Energy Abs TL" />
                                                <asp:BoundField DataField="reactive_energy_pos_t1" HeaderText="Reactive Energy Pos T1" />
                                                <asp:BoundField DataField="reactive_energy_pos_t2" HeaderText="Reactive Energy Pos T2" />
                                                <asp:BoundField DataField="reactive_energy_pos_t3" HeaderText="Reactive Energy Pos T3" />
                                                <asp:BoundField DataField="reactive_energy_pos_t4" HeaderText="Reactive Energy Pos T4" />
                                                <asp:BoundField DataField="reactive_energy_pos_tl" HeaderText="Reactive Energy Pos TL" />
                                                <asp:BoundField DataField="reactive_energy_neg_t1" HeaderText="Reactive Energy Neg T1" />
                                                <asp:BoundField DataField="reactive_energy_neg_t2" HeaderText="Reactive Energy Neg T2" />
                                                <asp:BoundField DataField="reactive_energy_neg_t3" HeaderText="Reactive Energy Neg T3" />
                                                <asp:BoundField DataField="reactive_energy_neg_t4" HeaderText="Reactive Energy Neg T4" />
                                                <asp:BoundField DataField="reactive_energy_neg_tl" HeaderText="Reactive Energy Neg TL" />
                                                <asp:BoundField DataField="active_mdi_pos_t1" HeaderText="Active MDI Pos T1" />
                                                <asp:BoundField DataField="active_mdi_pos_t2" HeaderText="Active MDI Pos T2" />
                                                <asp:BoundField DataField="active_mdi_pos_t3" HeaderText="Active MDI Pos T3" />
                                                <asp:BoundField DataField="active_mdi_pos_t4" HeaderText="Active MDI Pos T4" />
                                                <asp:BoundField DataField="active_mdi_pos_tl" HeaderText="Active MDI Pos TL" />
                                                <asp:BoundField DataField="active_mdi_neg_t1" HeaderText="Active MDI Neg T1" />
                                                <asp:BoundField DataField="active_mdi_neg_t2" HeaderText="Active MDI Neg T2" />
                                                <asp:BoundField DataField="active_mdi_neg_t3" HeaderText="Active MDI Neg T3" />
                                                <asp:BoundField DataField="active_mdi_neg_t4" HeaderText="Active MDI Neg T4" />
                                                <asp:BoundField DataField="active_mdi_neg_tl" HeaderText="Active MDI Neg TL" />
                                                <asp:BoundField DataField="active_mdi_abs_t1" HeaderText="Active MDI Abs T1" />
                                                <asp:BoundField DataField="active_mdi_abs_t2" HeaderText="Active MDI Abs T2" />
                                                <asp:BoundField DataField="active_mdi_abs_t3" HeaderText="Active MDI Abs T3" />
                                                <asp:BoundField DataField="active_mdi_abs_t4" HeaderText="Active MDI Abs T4" />
                                                <asp:BoundField DataField="active_mdi_abs_tl" HeaderText="Active MDI Abs TL" />
                                                <asp:BoundField DataField="cumulative_mdi_pos_t1" HeaderText="Cumulative MDI Pos T1" />
                                                <asp:BoundField DataField="cumulative_mdi_pos_t2" HeaderText="Cumulative MDI Pos T2" />
                                                <asp:BoundField DataField="cumulative_mdi_pos_t3" HeaderText="Cumulative MDI Pos T3" />
                                                <asp:BoundField DataField="cumulative_mdi_pos_t4" HeaderText="Cumulative MDI Pos T4" />
                                                <asp:BoundField DataField="cumulative_mdi_pos_tl" HeaderText="Cumulative MDI Pos TL" />
                                                <asp:BoundField DataField="cumulative_mdi_neg_t1" HeaderText="Cumulative MDI Neg T1" />
                                                <asp:BoundField DataField="cumulative_mdi_neg_t2" HeaderText="Cumulative MDI Neg T2" />
                                                <asp:BoundField DataField="cumulative_mdi_neg_t3" HeaderText="Cumulative MDI Neg T3" />
                                                <asp:BoundField DataField="cumulative_mdi_neg_t4" HeaderText="Cumulative MDI Neg T4" />
                                                <asp:BoundField DataField="cumulative_mdi_neg_tl" HeaderText="Cumulative MDI Neg TL" />
                                                <asp:BoundField DataField="cumulative_mdi_abs_t1" HeaderText="Cumulative MDI Abs T1" />
                                                <asp:BoundField DataField="cumulative_mdi_abs_t2" HeaderText="Cumulative MDI Abs T2" />
                                                <asp:BoundField DataField="cumulative_mdi_abs_t3" HeaderText="Cumulative MDI Abs T3" />
                                                <asp:BoundField DataField="cumulative_mdi_abs_t4" HeaderText="Cumulative MDI Abs T4" />
                                                <asp:BoundField DataField="cumulative_mdi_abs_tl" HeaderText="Cumulative MDI Abs TL" />
                                                <asp:BoundField DataField="mdc_read_datetime" HeaderText="MDC Read DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                                <asp:BoundField DataField="db_datetime" HeaderText="DB DateTime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                                <asp:BoundField DataField="is_synced" HeaderText="Is Synced" />
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="text-muted">No MBILL data records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                    <!-- Response Remarks Section -->
                                    <div id="mbillResponseRemarks" runat="server" class="mt-3" style="display: none;">
                                        <div class="card border-warning">
                                            <div class="card-body">
                                                <h6 class="card-title">Remarks</h6>
                                                <asp:TextBox ID="txtMbillResponseRemarks" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="2" placeholder="Enter remarks..."></asp:TextBox>
                                                <div class="mt-2">
                                                    <asp:Button ID="btnSaveMbillResponseRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1" Text="Save Remarks" OnClick="btnSaveMbillResponseRemarks_Click" />
                                                    <asp:Button ID="btnCancelMbillResponseRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2" Text="Cancel" OnClick="btnCancelMbillResponseRemarks_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
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
                                                        <details class="comm-history-log">
                                                            <summary class="comm-history-log-summary"><%# UDIL.Shared.CommunicationHistoryGridHelper.GetPreview(Eval("message_log")) %></summary>
                                                            <pre class="comm-history-log-full mb-0"><%# UDIL.Shared.CommunicationHistoryGridHelper.GetFull(Eval("message_log")) %></pre>
                                                        </details>
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
        function showMbillLoading(button) {
            if (!button) return;

            const spinner = document.getElementById('mbillSpinner');

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
        function disableMbillButton() {
            const button = document.getElementById('<%= btnMbillRead.ClientID %>');
            const spinner = document.getElementById('mbillSpinner');

            if (button) {
                button.disabled = true;
                button.style.opacity = '0.7';
                button.style.cursor = 'not-allowed';

                // Keep "Reading..." text to show completion
                button.value = 'Processing...';
            }

            if (spinner) {
                spinner.style.display = 'none';
            }
        }

        // Optional reset function
        function resetMbillLoading() {
            const button = document.getElementById('<%= btnMbillRead.ClientID %>');
            const spinner = document.getElementById('mbillSpinner');

            if (button) {
                button.disabled = false;
                button.style.opacity = '1';
                button.style.cursor = 'pointer';

                const originalText = 'Read MBILL';
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

