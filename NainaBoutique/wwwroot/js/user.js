var dataTable;



$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#userTable').DataTable({
        "ajax": {
            url: '/admin/user/getall'
        },
        "columns": [
            { data: 'userName', "autowidth": true },
            { data: 'email', "autowidth": true },
            { data: 'address', "autowidth": true },
            { data: 'role', "autowidth": true },
            //{ data: 'quantityInStock', "autowidth": true },
            //{ data: 'color', "autowidth": true }
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today= new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                       
                        return `<div class="text-center">            
                     <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor-pointer, width=150px">
                     <i class="bi bi-lock-fill"></i> Block</a>
                    </div>`
                    }

                    else {
                        return `<div class="text-center">            
                     <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor-pointer, width=150px">
                     <i class="bi bi-lock-fill"></i> UnBlock</a>
                    </div>`

                    }
                    
                },
                "autowidth": true
            }
        ]
    });

}


function LockUnlock(id) {
   
            $.ajax({
                type: 'POST',
                url: '/Admin/User/BlockUnblock',
                data: JSON.stringify(id),
                contentType:"application/json",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
 
