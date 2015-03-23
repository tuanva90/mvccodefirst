<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/Site.Master" AutoEventWireup="true" CodeBehind="CategoryIndex.aspx.cs" Inherits="UDI.WebASP.Views.Categories.CategoryIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
           //$('#myModal').modal('hide');

           $(".tb-hover tr").dblclick(function () {
               $this = $(this);
               if ($this.length > 0) {
                   var id = $this.find("td input").val();
                   var _id1 = $this.find("td input")[0].value;
                   var _name = $this.find("td")[1].textContent.trim();
                   var _des = $this.find("td")[2].textContent.trim();
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

            // the each is used when postback is triggered with checked rows

            //.each(function (index, element) {

            //    var checked = $(element).prop('checked');

            //    if (checked == true)

            //        $(element).parents('tr').addClass('lightGreen');

            //});

            // select all click

            $("#cbSelectAll").change(function () {

                var checked = $(this).prop('checked');

                $('.cbSelectRow').prop('checked', checked).trigger('change');

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

       function CloseAlert() {
           $('#myModal').modal('hide');
           return false;
       }
   </script>
    <style type="text/css">
        .tb-hover tr:hover
        {
            background-color:seagreen;
        }

        .left-align
        {
            text-align: left;
        }
    </style>

    <asp:HiddenField runat="server" ID="ListDelteID" />

    <div class="container">
    <div>
        <h2>Category</h2>
    </div>
    <br />
    <asp:GridView ID="CateGrid" runat="server" CellPadding="05" CellSpacing="10" CssClass="tb-hover" AutoGenerateColumns="false" AllowPaging="True" PageSize="5" AllowSorting="True" OnRowDataBound="CateGrid_RowDataBound" OnPageIndexChanged="CateGrid_PageIndexChanged" OnPageIndexChanging="CateGrid_PageIndexChanging">
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <alternatingrowstyle backcolor="#BFE4FF" />
         <Columns>
             <asp:TemplateField>
                 <HeaderTemplate>
                    <input type="checkbox" id="cbSelectAll" />
                 </HeaderTemplate>
                <ItemTemplate>
                    <input type="checkbox" name="ChequeSelected" class="cbSelectRow" value="<%# Eval("CategoryID") %>"></input>
                </ItemTemplate>
             </asp:TemplateField>
            <asp:TemplateField HeaderText="Name" ItemStyle-Width="200">
            <ItemTemplate>               
                <asp:HiddenField ID="aaaaaaaaa" Runat="Server" 
                         Value='<%# Eval("CategoryID") %>' Visible="true"/>
                <asp:Label ID="name_test" runat="server" Text='<%# Eval("CategoryName") %>'/>
            </ItemTemplate>
          </asp:TemplateField> 
            <%--<asp:BoundField HeaderText="ID test" DataField="CategoryID"/>--%>
            <asp:BoundField HeaderText="Name" DataField="CategoryName" ItemStyle-Width="200"/>
             <asp:BoundField HeaderText="Description" DataField="Description" ItemStyle-Width="300"/>
             <asp:TemplateField HeaderText="Image">
                 <ItemTemplate>
                     <asp:Image ID="CategoryImg" runat="server" ImageUrl="~/Images/noImages.jpg" Width="50" Height="50" />
                 </ItemTemplate>
             </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
    <div class="modal-footer left-align">
        <asp:Button ID="btnAdd" class="btn btn-info" runat="server" Text="Add" OnClientClick="btnAddClick(); return false;"></asp:Button>
        <asp:Button ID="btnDelete" class="btn btn-info" runat="server" Text="Delete" OnClick="btnDelete_Click"></asp:Button>
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
                            <asp:Button ID="btnClear" class="btn btn-success" runat="server" Text="Close" OnClientClick="return CloseAlert();"></asp:Button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnSave"/>
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
     
</asp:Content>
