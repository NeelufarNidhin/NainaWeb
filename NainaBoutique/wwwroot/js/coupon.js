var dataTable

$(document).ready(function () {
    loadCouponTable();
});

function loadCouponTable() {
    dataTable = $('#couponTblData').DataTable({
        "ajax": {
            url: '/admin/coupon/getall'
        },
        "columns": [
            { data: 'couponCode', "autowidth": true },
            { data: 'description', "autowidth": true },
            { data: 'validfrom', "autowidth": true },
            { data: 'validTo', "autowidth": true },
            { data: 'discountType', "autowidth": true },
            { data: 'discountAmount', "autowidth": true },
            { data: 'minCartAmount', "autowidth": true },
            { data: 'maxRedeemableAmount', "autowidth": true },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/coupon/edit?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                     <a onclick=DeleteCoupon('/admin/coupon/delete?id=${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },
                "autowidth": true
            }
        ]
    });

}



function DeleteCoupon(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}
