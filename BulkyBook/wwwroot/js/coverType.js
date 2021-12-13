var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/CoverType/GetAll"
        },
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i>
                                </a>
                                <a onclick=Delete("/Admin/CoverType/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-trash-alt"></i>
                                </a>
                            </div>
                            `;
                }, "width": "40%"
            }
        ],
        "language": {
            "emptyTable": "no data found",
            "url": "//cdn.datatables.net/plug-ins/1.11.3/i18n/ru.json"
        }
    });
}
function Delete(url) {
    swal({
        title: "Вы действительно хотите удалить?",
        text: "Вы не сможете вернуть удаленные данные!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}