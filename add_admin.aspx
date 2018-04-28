<%@ Page Title="" Language="C#" MasterPageFile="~/admin/adMaster.Master" AutoEventWireup="true" CodeBehind="add_admin.aspx.cs" Inherits="iCARS.admin.add_admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_default" runat="server">
    <table style="text-align: center; margin-left: 10%; margin-right: 10%; width: 80%;">
        <tr style="font-weight: bold; text-decoration: underline;">
            <td class="dark">
                <h1>Admin Login</h1>
                <br />
                <table style="margin: auto; width: 100%;">
                    <tr>
                        <td style="text-align: right; width: 50%;">Username:&nbsp;</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtAdUser" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">Password:&nbsp;</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtAdPass" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">&nbsp;</td>
                        <td style="text-align: left; width: 50%;"><div style="width: 301px; text-align: right;"><asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" /></div></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
