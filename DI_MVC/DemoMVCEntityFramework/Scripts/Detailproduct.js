
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
};

$(document).ready(function () {
    $(".tbproduct tbody tr   ").mouseover(function () {
        $this = $(this);
        $(".tbproduct tbody tr   ").removeClass("_selected");
        $this.addClass("_selected");
    });

    $(".tbproduct tbody tr   ").mouseout(function () {
        $this = $(this);
        $(".tbproduct tbody tr   ").removeClass("_selected");
    });
    $(".tbproduct tbody tr   ").dblclick(function () {
        $this = $(this);
        if ($this.length > 0) {
            var jobNum = $this.find("#item_ProductID").val();
            //alert(jobNum);
            $.ajax({
                type: "GET",
                url: "http://localhost:5325/Product/Details?id=" + jobNum,
                data: jobNum,
                contentType: "text/xml; charset=utf-8",
                success: function (data) {
                    $('#tblpro').html(data);
                },
                error: function (error) {
                    alert(error.state + " " + error.statusText)
                }
            });
            //document.getElementById("dialogform").showModal();
            $('#dialogform').dialog({ width: 500 });
            //window.open('http://localhost:5325/Product/Details?id=' + jobNum, '_self', false);

        }
    });
    $("#dialogform").dblclick(function () {
        //document.getElementById("dialogform").close();
        $('#dialogform').dialog("close");
    });
    

    //$("#btnOrder").click(function () {
    //    var id = document.getElementById("prid").val();
    //    var quantity = document.getElementById("txtquantity").val();
    //    //var id = $("#prid").val();
    //    //var quantity = $("txtquantity").val();
    //    alert(id + " " + quantity);
    //    $.ajax({
    //        type: "POST",
    //        url: "http://localhost:5325/Product/AddOrder",
    //        data: '{"ProductID":' + id + '","quantity":"' + quantity + '"}',
    //        contentType: "text/xml; charset=utf-8",
    //        success: function (data) {
    //            alert("add successfully !");
    //        },
    //        error: function (error) {
    //            alert(error.state + " " + error.statusText)
    //        }
    //    });
    //});
    //function filter(term, _id, cellNr) {
    //    var suche = term.value.toLowerCase();
    //    var table = document.getElementById(_id);
    //    var ele;
    //    for (var r = 1; r < table.rows.length; r++) {
    //        ele = table.rows[r].cells[cellNr].innerHTML.replace(/<[^>]+>/g, "");
    //        if (ele.toLowerCase().indexOf(suche) >= 0)
    //            table.rows[r].style.display = '';
    //        else table.rows[r].style.display = 'none';
    //    }
    //}
    //function filter (phrase, _id)
    //{
    //    var words = phrase.value.toLowerCase().split(" ");
    //    var table = document.getElementById(_id);
    //    var ele;
    //    for (var r = 1; r < table.rows.length; r++) {
    //        ele = table.rows[r].innerHTML.replace(/<[^>]+>/g, "");
    //        var displayStyle = 'none';
    //        for (var i = 0; i < words.length; i++) {
    //            if (ele.toLowerCase().indexOf(words[i]) >= 0)
    //                displayStyle = '';
    //            else {
    //                displayStyle = 'none';
    //                break;
    //            }
    //        }
    //        table.rows[r].style.display = displayStyle;
    //    }
    //};
});
