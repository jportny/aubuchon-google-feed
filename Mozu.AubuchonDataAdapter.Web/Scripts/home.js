/// <reference path="_reference.js" />

$(function () {
    $('.tabs a').click(function (e) {
        var currentEl = e.target;
        if (currentEl.classList.contains('active'))
            return;
        var activeTab = $(e.target.parentElement).find('.active');
        $(activeTab).removeClass('active');
        $(currentEl).addClass('active');
        $('#' + $(activeTab).data('tab-id')).show();
        });
    });
    $('#export').click(function (e) {
        ExportLocation();
    });
});
function ExportLocation() {
    $.ajax({
        type: "POST",
        url: baseUrl + "api/export/" + $("#hiddenTenantId").val(),
        contentType: false,
        processData: false,
        success: function (result) { console.log(result); },
        error: function (xhr, status, error) { console.log(xhr); console.log(error); }
    });
}
