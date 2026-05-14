<%@ Page Title="Dashboard - UDIL Tester" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
   CodeBehind="Dashboard.aspx.cs" Inherits="UDIL.Dashboard" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="main-content  mt-5">
        <div class="container-fluid">
            <section id="dashboard">
                <h2 class="section-header"><i class="bi bi-house-door"></i>Dashboard</h2>
                <div class="row">
                    <div class="col-lg-4 col-md-6 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title"><i class="bi bi-activity"></i>System Status</h5>
                                <p class="card-text">All systems operational.</p>
                                <span class="badge bg-success">Online</span>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title"><i class="bi bi-play-circle"></i>Active Tests</h5>
                                <p class="card-text">5 tests running.</p>
                                <span class="badge bg-warning">In Progress</span>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-4 col-md-6 mb-4">
                        <div class="card">
                            <div class="card-body">
                                <h5 class="card-title"><i class="bi bi-clock-history"></i>Recent Logs</h5>
                                <p class="card-text">Last update: 2026-04-07</p>
                                <span class="badge bg-info">Updated</span>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

           

        </div>
    </div>
    
</asp:Content>

