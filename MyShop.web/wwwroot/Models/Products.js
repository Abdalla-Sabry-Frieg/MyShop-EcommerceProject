// To make data table run
// 1- make view with no data , data will return as a json
// 2- css & js inject at the layout

// 1st
//var datTable;
//$(document).ready(function () {
//    loaddata();
//});

//function loaddata() {
//    datTable = $('#myTable').DataTable({
//        "ajax": {
//            "url": "/Admin/Product/GetData"
//        },
//        "autoWidth": false,
//        "responsive": true,
//        "columns": [
//            {
//                "data": "image",
//                "render": function (data) {
//                    return
//                    `
//                        <img  src="~data" width="50" height="50" />

//                    `
//                }
//            }, 
//            { "data": "name" },
//            { "data": "description" },
//            { "data": "price" },
//            { "data": "category.name" },
//            { "data": "createdAt" },
//            {
//                "data": "id",
//                "render": function (data) {
//                    return `
//                              <a href="/Admin/Product/Edit/${data}" class="btn btn-success">Edit</a>
//                              <a onClick=Delete("/Admin/Product/DeleteProduct/${data}") class="btn btn-danger">Delete</a>

//                           `
//                }
//            }
//        ]
//    });
//}

// -- 1st

// 2sd

$(document).ready(function () {
    $('#myTable').DataTable({
        "autoWidth": false,
        "responsive": true
    });
});
// -- 2sd

function Delete(id) {

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Admin/Product/DeleteProduct?id=${id}`
            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });

}