var homeConfig = {
    pageSize: 8,
    pageIndex: 1
}

//load category to select
$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/Materials/LoadCategory",
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            var s = '<option value="0">Tất cả danh mục cha</option>';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].idCategory + '">' + data[i].nameCategory + '</option>';
            }
            $("#searchCategory").html(s);
            loadData();
        }
    });

    $('[data-toggle="tooltip"]').tooltip();
});

//Load Data
var loadData = function () {
    var name = $('#search-input').val();
    var idCategory = parseInt($("#searchCategory option:selected").attr("value"));
    var sortPrice = parseInt($("#sortPrice option:selected").attr("value"));
    if (homeConfig.pageIndex == 0) homeConfig.pageIndex = 1;
    $('#loading').css('display', 'inline-block');
    $('#download-excel').attr("href", `/Materials/Export?name=${name}&idCategory=${idCategory}&sortPrice=${sortPrice}`);
    $.ajax({
        url: '/Materials/LoadData',
        type: 'GET',
        data: {
            name: name,
            idCategory: idCategory,
            sortPrice: sortPrice,
            page: homeConfig.pageIndex,
            pageSize: homeConfig.pageSize
        },
        dataType: 'json',
        success: function (response) {
            var data = response.data;
            var html = '';
            for (var i = 0; i < data.length; i++) {
                if (data[i].nameMaterial === null) data[i].nameMaterial = "Trống";
                if (data[i].price === null) data[i].price = 0.0;
                if (data[i].count === null) data[i].count = 0;
                if (data[i].idCategory === null) data[i].nameCategory = "Trống";
                if (data[i].idSubCategory === null) data[i].nameSubCategory = "Trống";
                html += '<tr class="trMaterial"><td><div class="dropdown float-right"><button type="button" class="btn btn-extension btn-outline-secondary btn-sm dropdown-toggle" data-toggle="dropdown"><i class="fas fa-ellipsis-h"></i></button><div class="dropdown-menu"><a class="dropdown-item" href="#" title="Sửa" data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '"><i class="fas fa-edit"></i> Sửa</a><a class="dropdown-item" href="#" title="Chi tiết" data-toggle="modal" data-target="#detailsModal" data-id="' + data[i].idMaterial + '"><i class="fas fa-info-circle"></i> Chi tiết</a><a class="dropdown-item" href="#" title="Xóa" data-toggle="modal" data-target="#deleteModal" data-id="' + data[i].idMaterial + '"><i class="fas fa-trash-alt"></i> Xóa</a><a class="dropdown-item" href="#" title="Lấy mã QR" data-toggle="modal" data-target="#qrModal" data-id="' + data[i].idMaterial + '"><i class="fas fa-qrcode"></i> Mã QR</a></div></div><ul class="custom-menu"><li data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '" ><i class="fas fa-edit"></i> Sửa</li><li data-toggle="modal" data-target="#detailsModal" data-id="' + data[i].idMaterial + '"><i class="fas fa-info-circle"></i> Xem chi tiết</li><li data-toggle="modal" data-target="#deleteModal" data-id="' + data[i].idMaterial + '"><i class="fas fa-trash-alt"></i> Xóa</li></ul></td><td data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '">' + data[i].nameMaterial + '</td><td data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '">' + data[i].price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</td><td data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '">' + data[i].count + '</td><td data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '">' + data[i].nameCategory + '</td><td data-toggle="modal" data-target="#editModal" data-id="' + data[i].idMaterial + '">' + data[i].nameSubCategory + '</td></tr>';
            }
            $("#tableMaterial").html(html);
            paging(response.total, function () {
                loadData();
            });
            trRightClick();
        }
    });
}

//phan trang
var resetPagination = function () {
    //if ($('.pagination').data("twbs-pagination")) {
    $('.pagination').twbsPagination('destroy');
    //}
}

var paging = function (totalRow, callback) {
    if (totalRow == 0) $('#not-found').css('display', 'block');
    else {
        $('#not-found').css('display', 'none');
        var totalPage = Math.ceil(totalRow / homeConfig.pageSize);
        if (totalPage < homeConfig.pageIndex) homeConfig.pageIndex = totalPage;
        $('#pagination').twbsPagination({
            totalPages: totalPage,
            visiblePages: 10,
            startPage: homeConfig.pageIndex,
            first: "Trang đầu",
            prev: "Trang trước",
            next: "Trang sau",
            last: "Trang cuối",
            onPageClick: function (event, page) {
                homeConfig.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    }
    $('#loading').css('display', 'none');
}

var trRightClick = function () {
    $('.trMaterial').bind("contextmenu", function (event) {
        event.preventDefault();

        $(this).find(".custom-menu").finish().toggle(100).

            // In the right position (the mouse)
            css({
                top: event.pageY + "px",
                left: event.pageX + "px"
            });
    });


    // If the document is clicked somewhere
    $(document).bind("mousedown", function (e) {

        // If the clicked element is not the menu
        if (!$(e.target).parents(".custom-menu").length > 0) {

            // Hide it
            $(".custom-menu").hide(100);
        }
    });

    $(".custom-menu li").click(function () {
        $(".custom-menu").hide(100);
    });
}

/*$(function () {
    $.contextMenu({
        selector: '.trMaterial',
        build: function ($trigger, e) {
            consolde.log(e);
            return {
                callback: function (key, options) {
                    var m = "clicked: " + key;
                    console.log(m);
                },
                items: items
            };
        }
    });
    var items = {
        "edit": {
            name: "Edit",
            icon: "edit"
        },
        "copy": {
            name: "Copy",
            icon: "copy"
        }
    }
});*/

//Create
$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/Materials/LoadCategory",
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            var s = '';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].idCategory + '">' + data[i].nameCategory + '</option>';
            }
            $("#idCategory").html(s);
            $.ajax({
                type: "GET",
                url: "/Materials/LoadSubCategory?id=" + $("#idCategory option:selected").attr("value"),
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var s = '';
                    for (var i = 0; i < data.length; i++) {
                        s += '<option value="' + data[i].idSubCategory + '">' + data[i].nameSubCategory + '</option>';
                    }
                    $("#idSubCategory").html(s);
                }
            });
        }
    });

    $.ajax({
        type: "GET",
        url: "/Materials/LoadListAdmin",
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            var s = '<option value="0">Tạm trống</option>';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].Admin_ID + '">' + data[i].UserName + '</option>';
            }
            $("#idAdmin").html(s);
        }
    });

    $('#idCategory').on('change', function () {
        var id = $("#idCategory option:selected").attr("value");
        $.ajax({
            type: "GET",
            url: "/Materials/LoadSubCategory?id=" + id,
            data: "{}",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                var s = '';
                for (var i = 0; i < data.length; i++) {
                    s += '<option value="' + data[i].idSubCategory + '">' + data[i].nameSubCategory + '</option>';
                }
                $("#idSubCategory").html(s);
            }
        });
    });
});

$('#formCreate').validate({
    rules: {
        nameMaterial: "required",
        idCategory: "required",
        idSubCategory: "required"
    },
    messages: {
        nameMaterial: "Bạn cần nhập tên vật liệu",
        idCategory: "Cần chọn một danh mục",
        idSubCategory: "Cần chọn một danh mục"
    }
})

$("#formCreate").submit(function (event) {
    if ($('#formCreate').valid()) {
        CreateData();
    }
    event.preventDefault();
});

var CreateData = function () {
    var id = parseInt($("#idMaterial").val());
    var name = $("#nameMaterial").val();
    var price = parseFloat($("#price").val());
    var count = parseInt($("#count").val());
    var idSubCategory = parseInt($("#idSubCategory option:selected").attr("value"));
    var idAdmin = parseInt($("#idAdmin option:selected").attr("value"));
    var material = {
        idMaterial: id,
        nameMaterial: name,
        price: price,
        count: count,
        idSubCategory: idSubCategory,
        idAdmin: idAdmin
    }

    $.ajax({
        url: '/Materials/Create',
        data: {
            strMaterial: JSON.stringify(material)
        },
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.status == true) {
                //alert("Success");
                $("#createModal").hide();
                $(".modal-backdrop").hide();
                resetPagination();
                loadData();
                Swal.fire({
                    position: 'top',
                    icon: 'success',
                    title: 'Tạo mới thành công!',
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            else {
                alert(response.Message);
            }
        },
        error: function (err) {
            console.log(err);
        }
    })
}


//Edit modal
$('#editModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    var modal = $(this);
    //modal.find("form").attr("action", `/Materials/Edit/${id}`);
    $("#idMaterial2").attr("value", id);
    $.ajax({
        type: "GET",
        url: "/Materials/LoadCategory",
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            var s = '';
            for (var i = 0; i < data.length; i++) {
                s += '<option value="' + data[i].idCategory + '">' + data[i].nameCategory + '</option>';
            }
            $("#idCategory2").html(s);
            $.ajax({
                type: "GET",
                url: "/Materials/LoadSubCategory?id=" + $("#idCategory2 option:selected").attr("value"),
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var s = '';
                    for (var i = 0; i < data.length; i++) {
                        s += '<option value="' + data[i].idSubCategory + '">' + data[i].nameSubCategory + '</option>';
                    }
                    $("#idSubCategory2").html(s);
                }
            });
        }
    });

    $.ajax({
        url: '/Materials/LoadDetails?id=' + id,
        type: 'GET',
        data: "{}",
        dataType: 'json',
        success: function (data) {
            $("#nameMaterial2").val(data.nameMaterial);
            $("#price2").val(data.price);
            $("#count2").val(data.count);
            $("#idCategory2").val(data.idCategory);
            var idSub = data.idSubCategory;
            var idAd = data.idAdmin;
            $.ajax({
                type: "GET",
                url: "/Materials/LoadSubCategory?id=" + $("#idCategory2 option:selected").attr("value"),
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var s = '';
                    for (var i = 0; i < data.length; i++) {
                        s += '<option value="' + data[i].idSubCategory + '">' + data[i].nameSubCategory + '</option>';
                    }
                    $("#idSubCategory2").html(s);
                    $("#idSubCategory2").val(idSub);
                }
            });
            $.ajax({
                type: "GET",
                url: "/Materials/LoadListAdmin",
                data: "{}",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    var s = '<option value="0">Tạm trống</option>';
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].Admin_ID == idAd) {
                            s += '<option value="' + data[i].Admin_ID + '" selected>' + data[i].UserName + '</option>';
                        }
                        else {
                            s += '<option value="' + data[i].Admin_ID + '">' + data[i].UserName + '</option>';
                        }
                    }
                    $("#idAdmin2").html(s);
                }
            });
        }
    });

    $('#idCategory2').on('change', function () {
        var id = $("#idCategory2 option:selected").attr("value");
        $.ajax({
            type: "GET",
            url: "/Materials/LoadSubCategory?id=" + id,
            data: "{}",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                var s = '';
                for (var i = 0; i < data.length; i++) {
                    s += '<option value="' + data[i].idSubCategory + '">' + data[i].nameSubCategory + '</option>';
                }
                $("#idSubCategory2").html(s);
            }
        });
    });
})

$("#formEdit").submit(function (event) {
    EditData();
    event.preventDefault();
});

var EditData = function () {
    var id = parseInt($("#idMaterial2").val());
    var name = $("#nameMaterial2").val();
    var price = parseFloat($("#price2").val());
    var count = parseInt($("#count2").val());
    var idSubCategory = parseInt($("#idSubCategory2 option:selected").attr("value"));
    var idAdmin = parseInt($("#idAdmin2 option:selected").attr("value"));
    var material = {
        idMaterial: id,
        nameMaterial: name,
        price: price,
        count: count,
        idSubCategory: idSubCategory,
        idAdmin: idAdmin
    }
    //console.log(JSON.stringify(material));

    $.ajax({
        url: '/Materials/Edit',
        data: {
            strMaterial: JSON.stringify(material)
        },
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.status == true) {
                //alert("Success");
                $("#editModal").hide();
                $(".modal-backdrop").hide();
                loadData();
                Swal.fire({
                    position: 'top',
                    icon: 'success',
                    title: 'Sửa thành công!',
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            else {
                alert(response.Message);
            }
        },
        error: function (err) {
            console.log(err);
        }
    })
}

//Details
$('#detailsModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Button that triggered the modal
    var id = button.data('id'); // Extract info from data-* attributes
    var modal = $(this);
    $.ajax({
        url: '/Materials/LoadDetails?id=' + id,
        type: 'GET',
        data: "{}",
        dataType: 'json',
        success: function (data) {
            if (data.nameMaterial === null) data.nameMaterial = "Trống";
            if (data.price === null) data.price = 0.0;
            if (data.count === null) data.count = 0;
            var body = '';
            body += '<dl class="dl-horizontal"><dt>Tên vật tư</dt ><dd>' + data.nameMaterial + '<br>ID vật liệu: ' + data.idMaterial + '</dd><dt>Đơn giá</dt><dd>' + data.price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</dd><dt>Số lượng tồn</dt><dd>' + data.count + '</dd><dt>Danh mục cha</dt><dd>' + data.nameCategory + '<br>ID danh mục cha: ' + data.idCategory + '</dd><dt>Danh mục con</dt><dd>' + data.nameSubCategory + '<br>ID danh mục con: ' + data.idSubCategory + '</dd><dt>Người quản lý</dt><dd>' + data.nameAdmin + '</dd></dl >';
            modal.find(".modal-body").html(body);
        }
    });
})

//Delete
$('#deleteModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    $("#idMaterial3").attr("value", id);
    var modal = $(this);
    $.ajax({
        url: '/Materials/LoadDetails?id=' + id,
        type: 'GET',
        data: "{}",
        dataType: 'json',
        success: function (data) {
            if (data.nameMaterial === null) data.nameMaterial = "Trống";
            if (data.price === null) data.price = 0.0;
            if (data.count === null) data.count = 0;
            var title = '';
            title += 'Bạn chắc chắn xóa <strong class="text-danger">' + data.nameMaterial + '?';
            modal.find('#exampleModalLabel').html(title);
            var body = '';
            body += '<dl class="dl-horizontal"><dt>Tên vật tư</dt ><dd>' + data.nameMaterial + '</dd><dt>Đơn giá</dt><dd>' + data.price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</dd><dt>Số lượng tồn</dt><dd>' + data.count + '</dd><dt>Danh mục cha</dt><dd>' + data.nameCategory + '</dd><dt>Danh mục con</dt><dd>' + data.nameSubCategory + '</dd><dt>Người quản lý</dt><dd>' + data.nameAdmin + '</dd></dl >';
            modal.find(".modal-body").html(body);

            /*var footer = '<form action="/Materials/delete/' + data.idMaterial + '" method="post"><input name="__RequestVerificationToken" type="hidden" value="thb6xoM_ooEhkcyvu9JmbwduLlgpWphqoehUbnQR1oTxwy-Kj5kvBuGKQYzUIl2fDDkp8Q44CJ8UZODjJhCF2yzDSiN4tMnY1aAdayFpyYc1"><input type="submit" value="Xóa" class="btn btn-danger"></form><button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>';
            modal.find('.modal-footer').html(footer);*/
        }
    });
});

$("#deleteBtn").on('click', function () {
    var id = parseInt($("#idMaterial3").val());
    $.ajax({
        url: '/Materials/Delete',
        data: {
            id: id
        },
        type: 'POST',
        dataType: 'json',
        success: function (response) {
            if (response.status == true) {
                //alert("Success");
                $("#deleteModal").hide();
                $(".modal-backdrop").hide();
                resetPagination();
                loadData();
                Swal.fire({
                    position: 'top',
                    icon: 'success',
                    title: 'Xóa thành công!',
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            else {
                alert(response.Message);
            }
        },
        error: function (err) {
            console.log(err);
        }
    })
});

//qr code
$('#qrModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var id = button.data('id');
    var modal = $(this);
    $.ajax({
        url: '/Materials/LoadQRCode?id=' + id,
        type: 'GET',
        data: "{}",
        dataType: 'json',
        success: function (data) {
            $('#qrcodeImg').attr('src', data.qrcode);
            $('#nameMaterial3').text(data.nameMaterial);
            $('#nameAdmin').text(data.nameAdmin);
        }
    });
});

//search
$('#search').on('click', function () {
    resetPagination();
    loadData();
});
/*$('#search-input').keyup(function () {
    $('.pagination').twbsPagination('destroy');
    console.log("up");
    loadData();
});

$('#search-input').keydown(function () {
    $('.pagination').twbsPagination('destroy');
    console.log("down");
    loadData();
});*/

/*$('#searchCategory').change(function () {
    resetPagination();
    loadData();
})*/

