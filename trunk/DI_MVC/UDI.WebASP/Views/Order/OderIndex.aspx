<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="OderIndex.aspx.cs" Inherits="UDI.WebASP.Views.Order.OderIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function callModal() {
            $('#myModal').modal('show')
        }
    </script>
    <div class="container">
        <div class="page-header">
            <h1>Order's History of Customer</h1>
         </div>
         <div class="form-group">
      <asp:DataGrid ID="dtgOrder" CssClass="table table-condensed" HeaderStyle-CssClass="Bold" runat="server" OnItemCommand="dtgOrder_ItemCommand">          
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
        <asp:DataGrid ID="dtgOrderDetail" CssClass="table table-condensed" HeaderStyle-CssClass="Bold" runat="server">
            <Columns>
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
   
</asp:Content>
