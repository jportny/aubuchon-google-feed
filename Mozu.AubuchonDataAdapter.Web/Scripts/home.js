/// <reference path="_reference.js" />

$(function () {
    $('.tabs a').click(function (e) {

        var currentEl = e.target;
        if (currentEl.classList.contains('active'))
            return;

        var activeTab = $(e.target.parentElement).find('.active');
        $(activeTab).removeClass('active');
        $(currentEl).addClass('active');
        $('#' + $(activeTab).data('tab-id')).hide();
        $('#' + $(currentEl).data('tab-id')).show({
            complete: function (e) {
                if (this.attributes.id.value == "downloadTab") {
                    GetDownloadFileList();
                }
                if (this.attributes.id.value == "closedHolidaysTab") {
                    GetClosedHolidaysList();
                }
            }
        });
    });

    //$(document).ajaxSuccess(function (evt, jqXHR, settings) {
    //    GetClosedHolidaysList();
    //    console.log("REDY");
    //});

    $('#import').click(function (e) {
        ImportLocation();
    });
    $('#locationfile').change(function (e) {
        $('#fileName').val(e.target.files[0].name);
    });
    $('#export').click(function (e) {
        ExportLocation();
    });
    $('#refreshDownloads').click(function (e) {
        $('#fileList').empty();
        GetDownloadFileList();
    });
    $('#addClosedHolidays').click(function (e) {
        AddClosedHolidays();
    });
});
function GetDownloadFileList() {
    $.ajax({
        type: "POST",
        url: baseUrl + "api/file/list?tenantId=" + $("#hiddenTenantId").val(),
        success: function (data) {
            $('#fileList').empty();
            $.each(data, function (i, item) {
                var row = '<tr>';
                row += '<td>' + item.fileName + '</td>' + '<td>' + '<button class="downloadFile" data-filename="' + item.fullName + '">Download</button>' + '</td>';
                row += '</tr>';
                $('#fileList').append(row);
            });
            $('.downloadFile').off('click').click(function (e) {
                alert("Test");
                var url = baseUrl + "api/file/download?tenantId=" + $("#hiddenTenantId").val() + "&fileName=" + $(e.target).data('filename');
                window.open(url);
            });
        }
    });
}
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
function ImportLocation() {

    var selectedFile = $('input[id=locationfile]')[0];
    if (selectedFile == null) {
        alert("No file selected");
        return;
    }
    var data = new FormData();
    data.append("file", selectedFile.files[0]);
    $('#import').text("Loading...");
    $.ajax({
        type: "POST",
        url: baseUrl + "api/import/upload" + "?tenantId=" + $("#hiddenTenantId").val(),
        contentType: false,
        processData: false,
        data: data,
        success: function (result) {
            alert(result);
            console.log(result);
        },
        error: function (xhr, status, error) {
            alert("An error occurred, check console for error");
            console.log(xhr); console.log(error);
        },
        complete: function () { $('#import').text('Import'); }
    })

}
function AddClosedHolidays() {

    holidayName = $('#holidayName').val();
    holidayDate = $('#holidayDate').val();

    if (holidayName == "" || holidayDate == "") {
        alert("You must fill in all of the fields");
        return;
    }
    $.ajax({
        type: "POST",
        url: baseUrl + "api/closedholidays/PostHoliday",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        data: JSON.stringify({ HolidayName: holidayName, HolidayDate: holidayDate, tenantId: $("#hiddenTenantId").val() }),
        headers: {"x-vol-tenant" : $('#hiddenTenantId').val()},
        success: function (o) {
            if (!o.Success) {
                alert((o.Error ? o.Error : 'An error occurred while trying to add the Closed Holiday to database.'));
                return;
            }
            GetClosedHolidaysList();
            $('#holidayName').val("");
            $('#holidayDate').val("");
        },
    });
}

function DeleteClosedHolidays(value) {

    var holidayId = $(value).data().holidayId;
   
    $.ajax({
        type: "DELETE",
        url: baseUrl + "api/closedholidays/deleteholiday",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        headers: { "x-vol-tenant": $('#hiddenTenantId').val() },
        cache: false,
        data: JSON.stringify({ HolidayId: holidayId, HolidayDate: holidayDate, tenantId: $("#hiddenTenantId").val() }),
        success: function (o) {
            if (!o.Success) {
                alert((o.Error ? o.Error : 'An error occurred while trying to remove the Closed Holiday to database.'));
                return;
            }
            GetClosedHolidaysList();
        },
    });
}

function GetClosedHolidaysList() {
    $.ajax({
        type: "GET",
        url: baseUrl + "api/closedholidays/GetHolidays" + "?tenantId=" + $("#hiddenTenantId").val(),
        contentType: false,
        processData: false,
        success: function (o) {
            if (!o.Success) {
                alert((o.Error ? o.Error : 'AnUnable to retrieve data from the server.'));
                return;
            }

            $('#closedHolidaysList').empty();
            $.each(o.data, function (i, item) {
                var row = '<tr>';
                row += '<td>' + item.HolidayName + '(id:' + item.Id + ')</td>';
                row += '<td>' + item.HolidayDate + '</td>';
                row += '<td>' + '<button class="btnClosedHolidaysRemove mz-button primary secondary" onclick="DeleteClosedHolidays(this)" data-holiday-id="' + item.Id + '">Remove</button>' + '</td>';
                row += '</tr>';
                $('#closedHolidaysList').append(row);
            });
        },
    })
}