<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="CategoryIndex.aspx.cs" Inherits="UDI.WebASP.Views.Categories.CategoryIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   <script type="text/javascript">
       $(document).ready(function () {
           $(".tb-hover tr").dblclick(function () {
               $this = $(this);
               if ($this.length > 0) {
                   var id = $this.find("td input").val();
                   var _id1 = $this.find("td input")[0].value;
                   var _name = $this.find("td")[1].textContent;
                   var _des = $this.find("td")[2].textContent;
                   //alert(_id1 + _name + _des);
                   var txtCateID = '#' + '<%= txtCateID.ClientID %>';
                   var txtName = '#' + '<%=txtCateName.ClientID%>';
                   var txtDescription = '#' + '<%=txtDescription.ClientID%>';
                   $(txtCateID).val(_id1);
                   $(txtName).val(_name);
                   $(txtDescription).val(_des);
                   $('#myModal').modal('show');
                   
               }
           });
           
       });
       function btnAddClick()
       {
           var txtCateID = '#' + '<%= txtCateID.ClientID %>';
           var txtName = '#' + '<%=txtCateName.ClientID%>';
           var txtDescription = '#' + '<%=txtDescription.ClientID%>';
           $(txtCateID).val("");
           $(txtName).val("");
           $(txtDescription).val("");
           $('#myModal').modal('show');
       }
       function btnClearClick() {
           var txtCateID = '#' + '<%= txtCateID.ClientID %>';
           var txtName = '#' + '<%=txtCateName.ClientID%>';
           var txtDescription = '#' + '<%=txtDescription.ClientID%>';
           $(txtCateID).val("");
           $(txtName).val("");
           $(txtDescription).val("");
       }
   </script>
    <style type="text/css">
        #MainContent_CateGrid tr:hover
        {
            background-color:seagreen;
        }
    </style>
    <asp:GridView ID="CateGrid" runat="server" CellPadding="05" CellSpacing="10" CssClass="tb-hover" AutoGenerateColumns="false">
         <Columns>
            <asp:TemplateField HeaderText="Name">
            <ItemTemplate>               
                <asp:HiddenField ID="aaaaaaaaa" Runat="Server" 
                         Value='<%# Eval("CategoryID") %>' Visible="true"/>
                <asp:Label ID="name_test" runat="server" Text='<%# Eval("CategoryName") %>'/>
            </ItemTemplate>
          </asp:TemplateField> 
            <%--<asp:BoundField HeaderText="ID test" DataField="CategoryID"/>--%>
            <asp:BoundField HeaderText="Name" DataField="CategoryName"/>
             <asp:BoundField HeaderText="Description" DataField="Description"/>
        </Columns>
    </asp:GridView>
    

    <div class="container">
    <div class="btn-group">
        <asp:Button ID="btnAdd" class="btn-info" runat="server" Text="Add" OnClientClick="btnAddClick(); return false;"></asp:Button>
        <asp:Button ID="btnDelete" class="btn-info" runat="server" Text="Delete"></asp:Button>
    </div>
    </div>
    <br />
    <!-- Bootstrap Modal Dialog -->
    <div class="modal fade bs-example-modal-lg" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <table cellspacing="10" cellpadding="10" id="cate">
                                <tr style="display:none"> 
                                    <td>Category ID</td>
                                    <td>
                                        <asp:TextBox ID="txtCateID" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr> 
                                    <td>Category Name</td>
                                    <td>
                                        <asp:TextBox ID="txtCateName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Description</td>
                                    <td><asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Picture</td>
                                    <td>
                                        <asp:FileUpload ID="txtPicture" runat="server" CssClass="form-control"/>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="modal-footer">
                            <%--<asp:Button ID="btnUpdate" class="btn btn-info" runat="server" Text="Save"></asp:Button>--%>
                            <asp:Button ID="btnSave" class="btn btn-primary" runat="server" Text="Save" OnClick="btnSave_Click"></asp:Button>
                            <asp:Button ID="btnClear" class="btn btn-success" runat="server" Text="Clear" OnClientClick="btnClearClick(); return false;"></asp:Button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
   
</asp:Content>
