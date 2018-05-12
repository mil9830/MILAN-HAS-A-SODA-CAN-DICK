<%@ Page Title="" Language="C#" MasterPageFile="~/main.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="iCARS.login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 50%;
            height: 26px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%; height: 100px; background-color: #EEEEEE;" border="0">
        <tr>
            <td style="text-align: center; color: #000000;">
                <div style="text-align: center; font-size: 32px;">Account Creation<br /><hr style="width: 15%;" /></div>
                <table style="width:100%;">
                    <tr>
                        <td style="text-align: right; width: 50%;">Username</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtUser" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">Password</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtPass" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">Confirm Password</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtCPass" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">First Name</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtFName" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">Last Name</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtLName" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">Age</td>
                        <td style="text-align: left; width: 50%;">
                            <asp:TextBox ID="txtAge" runat="server" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right; width: 50%;">&nbsp;</td>
                        <td style="text-align: left; width: 50%;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 300px; text-align: right;">
                                        <asp:Button ID="btnCreate" runat="server" Text="Create" OnClick="btnCreate_Click" />
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
