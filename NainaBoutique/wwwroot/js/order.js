var dataTable;



$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }

});


function loadDataTable(status) {
    dataTable = $('#tblOrderData').DataTable({
       
        "ajax": { url: '/admin/order/getall?status=' + status },
          
        "columns": [
            { data: 'id', "autowidth": true },
            { data: 'name', "autowidth": true },
            { data: 'mobileNumber', "autowidth": true },
            //{ data: 'size', "autowidth": true },
            { data: 'applicationUser.email', "autowidth": true },
            { data: 'orderStatus', "autowidth": true },
            { data: 'orderTotal', "autowidth": true },
            //{ data: 'color', "autowidth": true },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>               
                    
                    </div>`
                },
                "autowidth": true
            }
        ]
    });

}




