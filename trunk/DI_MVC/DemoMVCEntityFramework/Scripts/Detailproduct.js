function addToCart(proID, num) {
    $.ajax({
        type: "GET",
        url: "http://localhost:5325/Product/AddToCart?proID=" + proID + "&num=" + $('#txtquantity').val(),
        contentType: "text/xml; charset=utf-8",
        success: function (data) {
            if (data == 1) {
                alert("Added success!");
                //document.getElementById("dialogform").close();
                $('#dialogform').dialog("close");
            } else {
                alert("Failed");
            }
        },
        error: function (error) {
            alert(error.state + " " + error.statusText)
        }
    });
};

function CheckChange(item) {
    var row = item.closest("tr");
    var productID = row.find("#item_ProductID").val();
    var curContent = $('#hd_lsProducts').val();

    if (item[0].checked == true) {
        curContent = curContent + "," + productID + " ";
        $('#hd_lsProducts').val(curContent);
    } else {
        curContent = curContent.replace("," + productID + " ", "");
        $('#hd_lsProducts').val(curContent);
    }
};

function DeleteMultiItem() {
    if ($('#hd_lsProducts').val().length == 0) {
        alert("Must select at least 1 item!");
        return;
    }
    $.ajax({
        type: "GET",
        url: "http://localhost:5325/Product/DeleteProducts?lsID=" + $('#hd_lsProducts').val() + "&CategoryID=" + $('#CategoryID').find("option:selected").val(),
        contentType: "text/xml; charset=utf-8",
        success: function (data) {
            $('#tblProduct').html(data);
        },
        error: function (error) {
            alert(error.state + " " + error.statusText)
        }
    });
}