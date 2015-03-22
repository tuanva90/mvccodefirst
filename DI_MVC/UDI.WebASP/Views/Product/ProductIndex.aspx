<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="ProductIndex.aspx.cs" Inherits="UDI.WebASP.Views.Product.ProductIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <div class="container">
         <div class="rows">
             <div class="col-md-2">
                 <div class="page-header">
                    <h2>Product</h2>
                        <div class="form-group">
                           <asp:DropDownList ID="DropDownList1" CssClass="form-control" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="True">       
                            </asp:DropDownList>   
                        </div>
                        <br />
                     <div class="form-group">
                         <asp:DataGrid ID="dtgProduct" CssClass="table table-condensed" HeaderStyle-CssClass="Bold" runat="server"> </asp:DataGrid>
                     </div>        
                </div>  
             </div>
         </div>
          
    </div>

    
</asp:Content>
