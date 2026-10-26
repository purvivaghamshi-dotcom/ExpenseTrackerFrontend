
<%@ Page Title="Admin Dashboard" Language="C#"
    MasterPageFile="~/Admin/AdminMaster.master"
    AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    Inherits="ExpenseTrackerFrontend.Admin.AdminDashboard" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="page-heading">
        <h2>Admin Dashboard</h2>
        <p>System overview and activity.</p>
    </div>

    <asp:Label
        ID="lblDashboardMessage"
        runat="server"
        ForeColor="#ff657d"
        EnableViewState="false" />

    <div class="stats-grid">

        <div class="stat-card neon-cyan">
            <div class="stat-title">TOTAL USERS</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalUsers"
                    runat="server" Text="0" />
            </div>
        </div>

        <div class="stat-card neon-green">
            <div class="stat-title">TOTAL INCOME</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalIncome"
                    runat="server" Text="₹0" />
            </div>
        </div>

        <div class="stat-card neon-orange">
            <div class="stat-title">TOTAL EXPENSE</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalExpenses"
                    runat="server" Text="₹0" />
            </div>
        </div>

        <div class="stat-card neon-purple">
            <div class="stat-title">TRANSACTIONS</div>
            <div class="stat-value">
                <asp:Label ID="lblTotalTransactions"
                    runat="server" Text="0" />
            </div>
        </div>

    </div>

    <div class="card">
        <h3>Recent System Activity</h3>

        <asp:Label
            ID="lblNoRecentActivity"
            runat="server"
            Text="No recent activity found."
            ForeColor="#aab1bb"
            Visible="false" />

        <asp:Repeater ID="rptRecentActivity" runat="server">

            <HeaderTemplate>
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>User</th>
                            <th>Activity</th>
                            <th>Date</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>

            <ItemTemplate>
                <tr>
                    <td><%#: Eval("UserName") %></td>
                    <td><%#: Eval("Activity") %></td>
                    <td><%#: Eval("ActivityDate", "{0:dd MMM yyyy}") %></td>
                </tr>
            </ItemTemplate>

            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>

        </asp:Repeater>
    </div>

</asp:Content>
