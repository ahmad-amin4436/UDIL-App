<%@ Page Title="UDIL Tester - Transaction Status" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TransactionStatus.aspx.cs" Inherits="UDIL.Pages.TransactionStatus" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="transaction-status">
                <h2 class="section-header"><i class="bi bi-activity"></i> Transaction Status</h2>

                <asp:UpdatePanel ID="updTransactionStatus" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <asp:Label ID="lblTsTransactionId" runat="server" AssociatedControlID="tsTransactionId" CssClass="form-label" Text="Transaction ID"></asp:Label>
                                        <asp:DropDownList
                                            ID="tsTransactionId"
                                            runat="server"
                                            CssClass="form-select"
                                            AutoPostBack="true"
                                            OnSelectedIndexChanged="tsTransactionId_SelectedIndexChanged" />
                                    </div>
                                    <div class="col-md-6 mb-3">
                                        <asp:Label ID="lblTsPrivateKey" runat="server" AssociatedControlID="tsPrivateKey" CssClass="form-label" Text="Private Key"></asp:Label>
                                        <asp:TextBox ID="tsPrivateKey" runat="server" CssClass="form-control" placeholder="private key from session" ReadOnly="true" />
                                    </div>
                                </div>

                                <asp:Label ID="lblTransactionStatusMessage" runat="server" CssClass="mt-2 d-block"></asp:Label>
                            </div>
                        </div>

                        <asp:Panel ID="pnlTracker" runat="server" Visible="false" CssClass="card shadow-sm border-0 mt-4">
                            <div class="card-header border-0">
                                <div class="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h5 class="mb-0">Transaction Tracker</h5>
                                        <small class="text-muted">Transaction:
                                            <asp:Label ID="lblTrackerTransactionId" runat="server" />
                                        </small>
                                    </div>
                                    <asp:Label ID="lblStage" runat="server" CssClass="badge bg-info px-3 py-2" />
                                </div>
                            </div>

                            <div class="card-body">
                                <div class="progress mb-4" style="height: 6px;">
                                    <asp:Panel ID="progressBar" runat="server" CssClass="progress-bar bg-primary" Style="width: 0%"></asp:Panel>
                                </div>

                                <div class="d-flex justify-content-between text-center gap-2 flex-wrap">
                                    <asp:Label ID="step0" runat="server" CssClass="tracker-step">Waiting</asp:Label>
                                    <asp:Label ID="step1" runat="server" CssClass="tracker-step">Start</asp:Label>
                                    <asp:Label ID="step2" runat="server" CssClass="tracker-step">Request Sent</asp:Label>
                                    <asp:Label ID="step3" runat="server" CssClass="tracker-step">Connected</asp:Label>
                                    <asp:Label ID="step4" runat="server" CssClass="tracker-step">Command Sent</asp:Label>
                                    <asp:Label ID="step5" runat="server" CssClass="tracker-step">Executed</asp:Label>
                                </div>

                                <div class="mt-4 text-center">
                                    <asp:Label ID="lblStageDescription" runat="server" CssClass="text-muted" />
                                </div>
                            </div>
                        </asp:Panel>

                        <asp:Timer ID="timerTracker" runat="server" Interval="2000" OnTick="timerTracker_Tick" Enabled="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="tsTransactionId" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="timerTracker" EventName="Tick" />
                    </Triggers>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>
</asp:Content>
