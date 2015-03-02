function addToCart(proID, num) {
    $.ajax({
        type: "GET",
        url: "http://localhost:5325/Product/AddToCart?proID=" + proID + "&num=" + $('#txtquantity').val(),
        contentType: "text/xml; charset=utf-8",
        success: function (data) {
            if (data == 1) {
                alert("Added success!");
                document.getElementById("dialogform").close();
            } else {
                alert("Failed");
            }
        },
        error: function (error) {
            alert(error.state + " " + error.statusText)
        }
    });
};