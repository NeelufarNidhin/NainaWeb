﻿$(document).ready(function () {
    loadSalesTable();
});

function loadSalesTable(startdate,enddate) {
    dataTable = $('#SalesTbl').DataTable({
        "ajax": { url: '/admin/dashboard/getall?startdate=?enddate=?' + startdate + enddate },
        "columns": [
            { data: 'id', "autowidth": true },
            { data: 'walletBalance', "autowidth": true },
            { data: 'orderId', "autowidth": true },
            
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     
                    
                    </div>`
                },
                "autowidth": true
            }
        ]
    });

}



//$(document).ready(function () {
//    $('#openModalButton').click(function () {
//        $('#myModal').modal('show');

//    });

//});


//$(document).ready(function () {
//    // Add a click event handler to the close button inside the modal
//    $('#myModal').on('click', '.close-modal-btn', function () {
//        $('#myModal').modal('hide'); // Hide the modal
//    });
//});
