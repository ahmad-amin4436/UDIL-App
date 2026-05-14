<%@ Page Title="UDIL Tester - Load Profile Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="load_profile_data.aspx.cs" Inherits="UDIL.EventsDataTables.load_profile_data" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content">
        <div class="container-fluid mt-5">
            <section id="load-profile-data">
                <h2 class="section-header"><i class="bi bi-table"></i> Load Profile Data</h2>

                <div class="card">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <h6 class="mb-0">Load Profile Data</h6>
                        <div>
                            <asp:Button ID="btnLoadProfileDataPass" runat="server" CssClass="btn btn-success btn-sm" Text="Pass" OnCommand="TableButton_Command" CommandName="Pass" CommandArgument="LoadProfileData" UseSubmitBehavior="false" />
                            <asp:Button ID="btnLoadProfileDataFail" runat="server" CssClass="btn btn-danger btn-sm ms-2" Text="Fail" OnCommand="TableButton_Command" CommandName="Fail" CommandArgument="LoadProfileData" UseSubmitBehavior="false" />
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <asp:Label ID="lblSearch" runat="server" AssociatedControlID="txtSearch" CssClass="form-label" Text="Search"></asp:Label>
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search in all columns..." AutoPostBack="true" OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <asp:Label ID="lblClear" runat="server" CssClass="form-label" Text="&nbsp;"></asp:Label>
                                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-secondary w-100" Text="Clear" OnClick="btnClear_Click" UseSubmitBehavior="false" />
                            </div>
                            <div class="col-md-4 text-end">
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
                            <asp:GridView ID="gvLoadProfileData" runat="server" CssClass="table table-striped table-hover table-bordered" 
                                AutoGenerateColumns="true" 
                                GridLines="None" 
                                AllowPaging="true" 
                                PageSize="50" 
                                OnPageIndexChanging="gvLoadProfileData_PageIndexChanging"
                                OnDataBound="gvLoadProfileData_DataBound"
                                HeaderStyle-CssClass="table-dark sticky-header"
                                RowStyle-CssClass="align-middle"
                                AlternatingRowStyle-CssClass="table-light">
                                <EmptyDataTemplate>
                                    <div class="text-center text-muted py-5">
                                        <i class="bi bi-inbox fs-1"></i>
                                        <p class="mt-2">No load profile data found.</p>
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
                                                        <li class='<%# Container.DataItem.ToString() == gvLoadProfileData.PageIndex.ToString() ? "page-item active" : "page-item" %>'>
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
                        <!-- Remarks Section for Load Profile Data -->
                        <div id="loadProfileDataRemarks" runat="server" class="mt-3" style="display: none;">
                            <div class="card border-warning">
                                <div class="card-body">
                                    <h6 class="card-title">Remarks</h6>
                                    <asp:TextBox ID="txtLoadProfileDataRemarks" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="2" placeholder="Enter remarks..." ReadOnly="false"></asp:TextBox>
                                    <div class="mt-2">
                                        <asp:Button ID="btnSaveLoadProfileDataRemarks" runat="server" CssClass="btn btn-dark-primary btn-sm me-1" Text="Save Remarks" OnCommand="SaveRemarks_Command" CommandName="Save" CommandArgument="LoadProfileData" UseSubmitBehavior="false" />
                                        <asp:Button ID="btnCancelLoadProfileDataRemarks" runat="server" CssClass="btn btn-secondary btn-sm ms-2" Text="Cancel" OnCommand="CancelRemarks_Command" CommandName="Cancel" CommandArgument="LoadProfileData" UseSubmitBehavior="false" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>


           
        </div>
    </div>

</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>



