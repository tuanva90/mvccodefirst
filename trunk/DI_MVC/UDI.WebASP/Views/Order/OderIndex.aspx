<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="OderIndex.aspx.cs" Inherits="UDI.WebASP.Views.Order.OderIndex" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function callModal() {
            $('#myModal').modal('show')
        }
    </script>

    <div class="container">
        <div class="form-group">
            <asp:GridView ID="CurOrderGridView"  runat="server" CellPadding="05" CellSpacing="10" CssClass="tb-hover" AutoGenerateColumns="false" AllowPaging="true" PageSize="5" OnPageIndexChanging="CurOrderGridView_SelectedIndexChanging"> 
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <alternatingrowstyle backcolor="#BFE4FF" />
                <Columns>
                    <asp:BoundField DataField="ProductName" HeaderText="Product Name" ItemStyle-Width="200" />
                    <asp:BoundField DataField="Category.CategoryName" HeaderText="Category Name" ItemStyle-Width="200"  />
                    <asp:BoundField DataField="Quantity" HeaderText="Quantity"  ItemStyle-Width="100" />
                    <asp:BoundField DataField="UnitPrice" HeaderText="UnitPrice"  ItemStyle-Width="100" />
                </Columns>
            </asp:GridView>
        </div>

        <div class="form-group">
            <asp:Button ID="AddOrderBtn" class="btn btn-info" runat="server" Text="Add Order" ValidationGroup="None" OnClick="AddOrderBtn_Click"></asp:Button>
        </div>

    <div class="container">
        <div class="page-header">
            <h1>Order's History of Customer</h1>
        </div>
        <div class="form-group">
            <asp:DataGrid ID="dtgOrder" CssClass="table table-bordered" HeaderStyle-CssClass="Bold" runat="server" OnItemCommand="dtgOrder_ItemCommand">
                <Columns>
                    <asp:ButtonColumn CommandName="DetailCmd" Text="Detail"></asp:ButtonColumn>
                </Columns>
                <HeaderStyle CssClass="Bold"></HeaderStyle>
            </asp:DataGrid>
        </div>

        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">Detail of Order</h4>
                    </div>
                    <div class="modal-body">
                        <asp:DataGrid ID="dtgOrderDetail" CssClass="table table-bordered" HeaderStyle-CssClass="Bold" runat="server">
                            <Columns>
                                <asp:BoundColumn DataField="Product.ProductName" HeaderText="Product Name"></asp:BoundColumn>
                                <asp:BoundColumn DataField="UnitPrice" HeaderText="Unit Price"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Quantity" HeaderText="Quantity"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Discount" HeaderText="Discount"></asp:BoundColumn>                               
                            </Columns>
                        </asp:DataGrid>

                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
