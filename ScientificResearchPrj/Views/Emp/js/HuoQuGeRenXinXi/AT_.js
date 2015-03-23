var AT_ = {
    AjaxPost: function (url, data, success) {
        $.ajax({
            url: url,
            async: false,
            type: "POST",
            dataType: "json",
            data: data,
            success:  success
        });
    }
}