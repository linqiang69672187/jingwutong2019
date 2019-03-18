$(document).on('click.bs.carousel.data-api', '#bk', function (e) {
    $('.progresshz').show();
    $('#excel').hide();
    $('#bk').attr("disabled",true);
    var data = {
    }
    $.ajax({
        type: "POST",
        url: "../Handle/bakDB.ashx",
        data: data,
        dataType: "json",
        success: function (data) {
            $('.progresshz').hide();
            $('#excel').show();
            $('#excel').attr('href', 'Handle/upload/'+data.data+'.xls');
            $('#bk').attr("disabled", false); 
        },
        error: function (msg) {
            console.debug("错误:ajax");
            $('.progresshz').hide();
            $('#bk').attr("disabled", false);
        }
    });

});
