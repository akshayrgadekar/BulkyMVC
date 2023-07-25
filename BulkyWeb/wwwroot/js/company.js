﻿var dataTable;
$(document).ready(function () {
    loaddatatable();
});
function loaddatatable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "12%" },
            { data: 'address', "width": "10%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "12%" },
            { data: 'postalCode', "width": "12%" },
            { data: 'phoneNumber', "width": "12%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2"> 
                                <i class="bi bi-pencil-square"></i> Edit</a>
                            <a onClick=Delete('/admin/company/delete?id=${data}') class="btn btn-danger mx-2">
                                <i class="bi bi-pencil-square"></i> Delete
                            </a>
                        </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
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

