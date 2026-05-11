<%@ Page Title="UDIL Tester - Billing Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="billing_data.aspx.cs" Inherits="UDIL.EventsDataTables.billing_data" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="billing-data">
                <h2 class="section-header"><i class="bi bi-table"></i> Billing Data</h2>

                <div class="card">
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-4">
                                <asp:Label ID="lblSearch" runat="server" AssociatedControlID="txtSearch" CssClass="form-label" Text="Search"></asp:Label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search in all columns..."></asp:TextBox>
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-dark-primary" Text="Search" OnClick="btnSearch_Click" UseSubmitBehavior="false" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <asp:Label ID="lblClear" runat="server" CssClass="form-label" Text="&nbsp;"></asp:Label>
                                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-secondary w-100" Text="Clear" OnClick="btnClear_Click" UseSubmitBehavior="false" />
                            </div>
                            <div class="col-md-6 text-end">
                                <asp:Label ID="lblRecordCount" runat="server" CssClass="form-label d-block mb-2" Text=""></asp:Label>
                                <div class="d-flex justify-content-end gap-2">
                                    <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" Style="width: auto;">
                                        <asp:ListItem Value="25" Text="25 per page"></asp:ListItem>
                                        <asp:ListItem Value="50" Text="50 per page" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="100" Text="100 per page"></asp:ListItem>
                                        <asp:ListItem Value="200" Text="200 per page"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="table-responsive" style="max-height: 600px; overflow-y: auto;">
                            <asp:GridView ID="gvBillingData" runat="server" CssClass="table table-striped table-hover table-bordered" 
                                AutoGenerateColumns="true" 
                                GridLines="None" 
                                AllowPaging="true" 
                                PageSize="50" 
                                OnPageIndexChanging="gvBillingData_PageIndexChanging"
                                OnDataBound="gvBillingData_DataBound"
                                HeaderStyle-CssClass="table-dark sticky-header"
                                RowStyle-CssClass="align-middle"
                                AlternatingRowStyle-CssClass="table-light">
                                <EmptyDataTemplate>
                                    <div class="text-center text-muted py-5">
                                        <i class="bi bi-inbox fs-1"></i>
                                        <p class="mt-2">No billing data found.</p>
                                    </div>
                                </EmptyDataTemplate>
                                <PagerTemplate>
                                    <div class="d-flex justify-content-between align-items-center mt-3">
                                        <nav aria-label="Page navigation">
                                            <ul class="pagination pagination-sm mb-0">
                                                <li class="page-item">
                                                    <asp:LinkButton ID="lnkFirst" runat="server" CommandName="Page" CommandArgument="First" 
                                                        CssClass="page-link" OnClick="PageButton_Click">
                                                        <i class="bi bi-chevron-double-left"></i>
                                                    </asp:LinkButton>
                                                </li>
                                                <li class="page-item">
                                                    <asp:LinkButton ID="lnkPrev" runat="server" CommandName="Page" CommandArgument="Prev" 
                                                        CssClass="page-link" OnClick="PageButton_Click">
                                                        <i class="bi bi-chevron-left"></i>
                                                    </asp:LinkButton>
                                                </li>
                                                <asp:Repeater ID="rptPageNumbers" runat="server" OnItemCommand="rptPageNumbers_ItemCommand">
                                                    <ItemTemplate>
                                                        <li class='<%# Container.DataItem.ToString() == gvBillingData.PageIndex.ToString() ? "page-item active" : "page-item" %>'>
                                                            <asp:LinkButton ID="lnkPage" runat="server" CommandName="Page" CommandArgument='<%# Container.DataItem %>' 
                                                                CssClass="page-link"><%# Container.DataItem %></asp:LinkButton>
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <li class="page-item">
                                                    <asp:LinkButton ID="lnkNext" runat="server" CommandName="Page" CommandArgument="Next" 
                                                        CssClass="page-link" OnClick="PageButton_Click">
                                                        <i class="bi bi-chevron-right"></i>
                                                    </asp:LinkButton>
                                                </li>
                                                <li class="page-item">
                                                    <asp:LinkButton ID="lnkLast" runat="server" CommandName="Page" CommandArgument="Last" 
                                                        CssClass="page-link" OnClick="PageButton_Click">
                                                        <i class="bi bi-chevron-double-right"></i>
                                                    </asp:LinkButton>
                                                </li>
                                            </ul>
                                        </nav>
                                        <div class="page-jumper d-flex align-items-center gap-2">
                                            <span class="text-muted small">Go to:</span>
                                            <asp:TextBox ID="txtGoToPage" runat="server" CssClass="form-control form-control-sm" Style="width: 60px;" autocomplete="off"></asp:TextBox>
                                            <asp:Button ID="btnGoToPage" runat="server" CssClass="btn btn-sm btn-outline-secondary" Text="Go" OnClick="btnGoToPage_Click" UseSubmitBehavior="false" />
                                        </div>
                                    </div>
                                </PagerTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </section>


           
        </div>
    </div>

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>



