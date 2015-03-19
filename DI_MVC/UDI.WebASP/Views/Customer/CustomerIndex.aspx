<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="CustomerIndex.aspx.cs" Inherits="UDI.WebASP.Views.Customer.CustomerIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="row">
        <div class="col-md-4">
            <h2>Create Customer</h2>
            
               <div class="input-group">
                <span class="input-group-addon" id="basic-addon1">Contact Name</span>
                <%--<input type="text" class="form-control" placeholder="Username" aria-describedby="basic-addon1">--%>
                   <asp:TextBox ID="txtContactName" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon2">Contact Title</span>
                <asp:TextBox ID="txtContacTitle" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon3">Adress</span>
                <asp:TextBox ID="txtAdress" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon4">City</span>
                <asp:TextBox ID="txtCity" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon5">Region</span>
                <asp:TextBox ID="txtRegion" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon6">Postal Code</span>
                <asp:TextBox ID="txtPostalcode" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon7">Country</span>
                <asp:TextBox ID="txtCountry" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon8">Phone</span>
                <asp:TextBox ID="txtPhone" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            <div class="input-group">
                <span class="input-group-addon" id="basic-addon9">Fax</span>
                <asp:TextBox ID="txtFax" CssClass="form-control" runat="server"></asp:TextBox>
             </div>
            <br />
            

           <asp:Button ID="btnCreat" Text="CREATE" runat="server" CssClass="btn btn-primary" OnClick="btnCreat_Click" />
        </div>
        
    </div>
        
          

    
</asp:Content>
