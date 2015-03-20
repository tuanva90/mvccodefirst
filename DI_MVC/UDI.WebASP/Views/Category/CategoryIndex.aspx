<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="CategoryIndex.aspx.cs" Inherits="UDI.WebASP.Views.Category.CategoryIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   
    <asp:GridView ID="CateGrid" runat="server">
    </asp:GridView>

    <div class="container">
    <div class="btn-group">
        <asp:Button ID="btnSubmit" class="btn-info" runat="server" Text="Submit"          
        OnClick="btnSubmit_Click"></asp:Button>
    </div>
    </div>
    <br />
    <!-- Bootstrap Modal Dialog -->
    <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblModalBody" runat="server" Text=""></asp:Label>
                            <div class="input-group">
                                <span class="input-group-addon" id="basic-addon1">User Name</span>
                                <asp:TextBox ID="txt1" runat="server" CssClass="form-control" ></asp:TextBox>
                                </div>
                            
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Save</button>
                            <button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>                           
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
   
</asp:Content>
