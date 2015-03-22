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
                     
                     <div class="form-group">
                         <asp:Button ID="btnCreate" runat="server" Text="Add new Product" CssClass="btn btn-info" data-toggle="modal" data-target="#myModal" OnClick="btnCreate_Click" />
                     </div>
                     
                     <!-- Modal -->
                    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                      <div class="modal-dialog">
                        <div class="modal-content">
                          <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title" id="myModalLabel">Add New Product</h4>
                          </div>
                          <div class="modal-body">
 
                                 <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon1">Product Name</span>
                                    <%--<input type="text" class="form-control" placeholder="Username" aria-describedby="basic-addon1">--%>
                                     <asp:TextBox ID="productName" CssClass="form-control" runat="server"></asp:TextBox>
                                     <asp:RequiredFieldValidator runat="server" ControlToValidate="productName"
                                        CssClass="text-danger" ErrorMessage="The product Name field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon2">Caterogy</span>
                                    <asp:DropDownList ID="DDL2" CssClass="form-control"  runat="server"></asp:DropDownList>
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon3">QuantityPerUnit</span>
                                    <asp:TextBox ID="QuantityPerUnit" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="QuantityPerUnit"
                                        CssClass="text-danger" ErrorMessage="The QuantityPerUnit field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon4">Unit Price</span>
                                    <asp:TextBox ID="UnitPrice" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UnitPrice"
                                        CssClass="text-danger" ErrorMessage="The Unit Price field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon5">Units In Stock</span>
                                    <asp:TextBox ID="UnitsInStock" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UnitsInStock"
                                        CssClass="text-danger" ErrorMessage="The UnitsInStock field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon6">Units On Order</span>
                                    <asp:TextBox ID="UnitsOnOrder" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UnitsOnOrder"
                                        CssClass="text-danger" ErrorMessage="The UnitsOnOrder field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon7">Reorder Level</span>
                                    <asp:TextBox ID="ReorderLevel" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ReorderLevel"
                                        CssClass="text-danger" ErrorMessage="The ReoderLevel field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon8">Quantity</span>
                                    <asp:TextBox ID="Quantity" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Quantity"
                                        CssClass="text-danger" ErrorMessage="The Quantity field is required." />
                                 </div>
                                <br />
                                <asp:CheckBox ID="chkDiscontinue" runat="server" CssClass="form-control"  Checked ="false" Text="Discontinued"/>
                              </div>   
                          <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                            <asp:Button ID="btnCrPro" runat="server" CssClass="btn btn-info" Text="Save Change" OnClick="btnCrPro_Click" />
                          </div>
                        </div>
                      </div>
                    </div>
                     
                             
                </div>  
             </div>
         </div>
          
    </div>

    
</asp:Content>
