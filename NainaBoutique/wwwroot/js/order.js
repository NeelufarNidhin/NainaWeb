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
            { data: 'orderDate', "autowidth": true },
            { data: 'paymentMethod', "autowidth": true },
            { data: 'orderStatus', "autowidth": true },
            { data: 'orderTotal', "autowidth": true },
            { data: 'paymentStatus', "autowidth": true },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/details?orderId=${data}" class="btn btn-secondary mx-2">View</a>               
                    
                    </div>`
                },
                "autowidth": true
            }
        ]
    });

}




