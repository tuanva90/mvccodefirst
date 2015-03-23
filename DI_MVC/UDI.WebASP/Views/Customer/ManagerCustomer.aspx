<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="ManagerCustomer.aspx.cs" Inherits="UDI.WebASP.Views.Customer.ManagerCustomer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
    <div class="page-header">
            <h1>Manager Customer</h1>
        </div>
    
    <div class="row">
        <div class="col-md-4">
            <div class="panel panel-default">
                                
                <div class="panel-body">
                    <table>
                        <tr>
                            <td>Contact Name</td>
                            <td>
                                <asp:TextBox runat="server" ID="ContactName" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ContactName"
                                CssClass="text-danger" ErrorMessage="The Contact Name field is required." />

                            </td>
                        </tr>
                        <tr>
                            <td>Contact Title</td>
                            <td>
                                <asp:TextBox runat="server" ID="ContactTitle" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ContactTitle"
                                 CssClass="text-danger" ErrorMessage="The Contact Title field is required." />
                            </td>
                        </tr>
                        <tr>
                            <td>Address</td>
                            <td>
                                <asp:TextBox runat="server" ID="Address" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Address"
                                CssClass="text-danger" ErrorMessage="The Address field is required." />
                            </td>
                        </tr>
                        <tr>
                            <td>City</td>
                            <td>
                                <asp:TextBox runat="server" ID="City" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ContactTitle"
                                CssClass="text-danger" ErrorMessage="The City Title field is required." />
                            </td>
                        </tr>
                    </table>              
            </div>
        </div>
        </div>
        

        <div class="col-md-4">
            <div class="panel panel-default">
                                
                <div class="panel-body">
                    <table>
                        <tr>
                            <td>Region</td>
                            <td>
                                <asp:TextBox runat="server" ID="Region" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Region"
                                CssClass="text-danger" ErrorMessage="The Region field is required." />

                            </td>
                        </tr>
                        <tr>
                            <td>Postal Code</td>
                            <td>
                                <asp:TextBox runat="server" ID="PostalCode" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="PostalCode"
                                CssClass="text-danger" ErrorMessage="The Postcal Code field is required." />
                            </td>
                        </tr>
                        <tr>
                            <td>Country</td>
                            <td>
                                <asp:TextBox runat="server" ID="Country" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Country"
                                CssClass="text-danger" ErrorMessage="The Country field is required." />
                            </td>
                        </tr>                   
                    </table>              
            </div>
        </div>
        </div>

        <div class="col-md-4">
            <div class="panel panel-default">
                                
                <div class="panel-body">
                    <table>
                        <tr>
                            <td>Phone</td>
                            <td>
                                <asp:TextBox runat="server" ID="Phone" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Phone"
                                CssClass="text-danger" ErrorMessage="The Phone field is required." />


                            </td>
                        </tr>
                        <tr>
                            <td>Fax</td>
                            <td>
                                <asp:TextBox runat="server" ID="Fax" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Fax"
                                CssClass="text-danger" ErrorMessage="The Fax field is required." />
                            </td>
                        </tr>                       
                    </table>              
            </div>
        </div>
        </div>
        </div>
       
        
    </div>
        
</asp:Content>
