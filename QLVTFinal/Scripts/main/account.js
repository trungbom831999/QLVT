var checkPass = function () {
    var pass = $('#password').val();
    var repass = $('#repassword').val();
    if (pass == repass) {
        $("#notiPass").css("display", "none");
        return true;
    }
    else {
        $("#notiPass").css("display", "block");
        return false;
    }
};