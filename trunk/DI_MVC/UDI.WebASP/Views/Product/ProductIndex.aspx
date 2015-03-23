<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="ProductIndex.aspx.cs" Inherits="UDI.WebASP.Views.Product.ProductIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .tb-hover tr:hover
        {
            background-color:seagreen;
        }

        .left-align
        {
            text-align: left;
        }

        .hiden
        {
            display: none;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            //$('#myModal').modal('hide');

            $(".tb-hover tr").dblclick(function () {
                $this = $(this);
                if ($this.length > 0) {
                    var id = $this.find("td input")[0].value;
                    $('#' + '<%= SelectedItem.ClientID %>').val(id);
                    $('#' + '<%= SubmitBtn.ClientID %>').click();
               }
           });

            $('.cbSelectRow').change(function () {
                var curContent = $('#' + '<%= ListDelteID.ClientID %>').val();
                var CateID = $(this).prop('value');

                // detect if the checkbox is checked
                var checked = $(this).prop('checked');

                // gets the table row indiect parent
                var trParent = $(this).parents('tr');

                // add or remove the css class according to the check state
                if (checked == true) {
                    trParent.addClass('lightGreen')
                    curContent = curContent + "," + CateID + " ";
                    $('#' + '<%= ListDelteID.ClientID %>').val(curContent);
                }
                else {
                    trParent.removeClass('lightGreen');
                    curContent = curContent.replace("," + CateID + " ", "");
                    $('#' + '<%= ListDelteID.ClientID %>').val(curContent);
                }

            });

            // select all click
            $("#cbSelectAll").change(function () {

                var checked = $(this).prop('checked');

                $('.cbSelectRow').prop('checked', checked).trigger('change');

            });
        });

       function CloseAlert() {
           $('#myModal').modal('hide');
           return false;
       }

       function OpenAlert() {
           $('#myModal').modal('show');
           return false;
       }

       function CreateProduct() {
           $('#' + '<%= ProductIDHidenField.ClientID %>').val("");
           OpenAlert();
       }

        function ShowMessage(msg) {
            alert(msg);
        }
   </script>

     <div class="container">
         <asp:HiddenField runat="server" ID="ListDelteID" />
         <asp:HiddenField runat="server" ID="SelectedItem" />
         <div class="rows">
             <div>
                 <div>
                    <h2>Product</h2>
                    <div class="form-group">
                        <asp:DropDownList ID="DropDownList1" CssClass="form-control" runat="server" AutoPostBack="true" Width="20%" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">       
                        </asp:DropDownList>  
                    </div>
                    <br />
                     <div class="form-group">
                         <asp:GridView ID="ProductGridView" runat="server" CellPadding="05" CellSpacing="10" CssClass="tb-hover" AutoGenerateColumns="false" AllowPaging="true" PageSize="5" OnPageIndexChanging="ProductGridView_SelectedIndexChanging"> 
                             <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                             <alternatingrowstyle backcolor="#BFE4FF" />
                             <Columns>
                                <asp:TemplateField ItemStyle-Width="50">
                                    <HeaderTemplate>
                                        <input type="checkbox" id="cbSelectAll" />
                                     </HeaderTemplate>
                                    <ItemTemplate>
                                        <input type="checkbox" name="ChequeSelected" class="cbSelectRow" value="<%# Eval("ProductID") %>"></input>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                <asp:BoundField DataField="Category.CategoryName" HeaderText="Category Name" ItemStyle-Width="200" />
                                <asp:BoundField DataField="QuantityPerUnit" HeaderText="Quantity Per Unit" ItemStyle-Width="100" />
                                 <asp:BoundField DataField="UnitsInStock" HeaderText="Units In Stock" ItemStyle-Width="100" />
                                 <asp:BoundField DataField="UnitsOnOrder" HeaderText="Units On Order" ItemStyle-Width="100" />
                                 <asp:BoundField DataField="ReorderLevel" HeaderText="Reorder Level" ItemStyle-Width="100" />
                                 <asp:BoundField DataField="Discontinued" HeaderText="Discontinued" ItemStyle-Width="100" />
                                 <asp:BoundField DataField="Quantity" HeaderText="Quantity" ItemStyle-Width="50" />
                            </Columns>
                         </asp:GridView>
                     </div>
                     
                     <div class="form-group">
                         <asp:Button ID="btnCreate" runat="server" Text="Add new Product" CssClass="btn btn-info" OnClientClick="CreateProduct(); return false;"/>
                         <asp:Button ID="btnDelete" class="btn btn-info" runat="server" Text="Delete" ValidationGroup="None" OnClick="btnDelete_Click"></asp:Button>
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
                                     <asp:HiddenField ID="ProductIDHidenField" runat="server" />
                                    <span class="input-group-addon" id="basic-addon1">Product Name</span>
                                    <%--<input type="text" class="form-control" placeholder="Username" aria-describedby="basic-addon1">--%>
                                     <asp:TextBox ID="ProductNameTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                     <asp:RequiredFieldValidator runat="server" ControlToValidate="ProductNameTextBox"
                                        CssClass="text-danger" ErrorMessage="The product Name field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon2">Caterogy</span>
                                    <asp:DropDownList ID="CategoriesDDL2" CssClass="form-control"  runat="server"></asp:DropDownList>
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon3">QuantityPerUnit</span>
                                    <asp:TextBox ID="QuantityPerUnitTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="QuantityPerUnitTextBox"
                                        CssClass="text-danger" ErrorMessage="The QuantityPerUnit field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon4">Unit Price</span>
                                    <asp:TextBox ID="UnitPriceTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UnitPriceTextBox"
                                        CssClass="text-danger" ErrorMessage="The Unit Price field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon5">Units In Stock</span>
                                    <asp:TextBox ID="UnitsInStockTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UnitsInStockTextBox"
                                        CssClass="text-danger" ErrorMessage="The UnitsInStock field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon6">Units On Order</span>
                                    <asp:TextBox ID="UnitsOnOrderTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UnitsOnOrderTextBox"
                                        CssClass="text-danger" ErrorMessage="The UnitsOnOrder field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon7">Reorder Level</span>
                                    <asp:TextBox ID="ReorderLevelTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ReorderLevelTextBox"
                                        CssClass="text-danger" ErrorMessage="The ReorderLevel field is required." />
                                 </div>
                                <br />
                                <div class="input-group">
                                    <span class="input-group-addon" id="basic-addon8">Quantity</span>
                                    <asp:TextBox ID="QuantityTextBox" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="QuantityTextBox"
                                        CssClass="text-danger" ErrorMessage="The Quantity field is required." />
                                 </div>
                                <br />
                                <asp:CheckBox ID="DiscontinueCheckBox" runat="server" CssClass="form-control"  Checked ="false" Text="Discontinued"/>
                              </div>   
                            
                            <div class="modal-body">
                                <asp:TextBox ID="OrderQuantityTextBox" runat="server" Width ="50" />
                                <asp:Button ID="AddOrderBtn" runat="server" CssClass="btn" Text="Add To Cart" ValidationGroup="None" OnClick="AddOrderBtn_Click" />
                            </div>

                          <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                            <asp:Button ID="CreateProductButton" runat="server" CssClass="btn btn-info" Text="Save Change" OnClick="btnCrPro_Click" />
                          </div>
                        </div>
                      </div>
                    </div>
                     
                             
                </div>  
             </div>
         </div>
          
    </div>

    <asp:Button ID="SubmitBtn" runat="server" CssClass="hiden" Text="Summit" OnClick="submit_Click" ValidationGroup="None" />

</asp:Content>
