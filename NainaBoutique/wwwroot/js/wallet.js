$(document).ready(function () {
    loadWalletTable();
});

function loadWalletTable() {
    dataTable = $('#walTbl').DataTable({
        "ajax": { url: '/customer/wallet/getall' },  
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
