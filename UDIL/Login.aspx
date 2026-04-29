<%@ Page Title="Login - UDIL Application" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="UDIL.Login" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
   
        <div class="container-fluid mt-5">
            <div class="row justify-content-center">
                <div class="col-lg-8 col-md-10">
                    
                    <div class="card shadow-lg">
                        <div class="row g-0">
                            <div class="col-lg-5">
                                <div class="bg-dark-danger text-white p-5 text-center h-100 d-flex flex-column justify-content-center">
                                  
                                    <h1 class="display-4 fw-bold mb-3">UDIL</h1>
                                    <h2 class="mb-3">Test Suite</h2>
                                    <p class="lead">Universal Data Integration Layer</p>
                                    <div class="mt-auto">
                                        <small>&copy; 2026 UDIL System v1.0</small>
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-7">
                                <div class="card-body p-5">
                                    <div class="text-center mb-4">
                                          <div class="mb-4">
                                        <img src="assets/logo-rm-bg.png" alt="UDIL Logo" class="img-fluid" style="max-height: 80px;" />
                                    </div>
                                        <h3 class="fw-bold">Sign In</h3>
                                        <p class="text-muted">Enter your credentials to access the system</p>
                                    </div>

                                    <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-danger alert-dismissible fade show" role="alert">
                                        <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
                                        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                                    </asp:Panel>

                                    <div class="mb-3">
                                        <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUsername" CssClass="form-label">Username</asp:Label>
                                        <div class="input-group">
                                            <span class="input-group-text"><i class="bi bi-person"></i></span>
                                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter your username"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mb-3">
                                        <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="form-label">Password</asp:Label>
                                        <div class="input-group">
                                            <span class="input-group-text"><i class="bi bi-lock"></i></span>
                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="mb-3 form-check">
                                        <asp:CheckBox ID="chkRememberMe" runat="server" CssClass="form-check-input" />
                                        <asp:Label ID="lblRememberMe" runat="server" AssociatedControlID="chkRememberMe" CssClass="form-check-label">Remember me</asp:Label>
                                    </div>

                                    <div class="d-grid">
                                        <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-dark-primary btn-lg" OnClick="btnLogin_Click" />
                                    </div>

                                    <div class="text-center mt-4">
                                        <small class="text-muted">
                                            <i class="fas fa-shield-alt"></i> 
                                            Secure authentication system
                                        </small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        // Focus on username field when page loads
        document.addEventListener('DOMContentLoaded', function() {
            var usernameField = document.getElementById('<%= txtUsername.ClientID %>');
            if (usernameField) {
                usernameField.focus();
            }
        });

        // Handle Enter key in password field
        var passwordField = document.getElementById('<%= txtPassword.ClientID %>');
        if (passwordField) {
            passwordField.addEventListener('keypress', function(e) {
                if (e.key === 'Enter') {
                    document.getElementById('<%= btnLogin.ClientID %>').click();
                }
            });
        }
    </script>
</asp:Content>
