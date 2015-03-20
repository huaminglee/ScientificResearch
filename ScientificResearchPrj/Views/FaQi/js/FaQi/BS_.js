
    var BS_ = {
        open:function (urls, title) {
            window.parent.addPanel({ 'view': urls, 'title': title });
        },
        reload:function () {
            $('#tTable').treegrid("loading");
            window.location.href = "/FaQi/FaQi";
            $('#tTable').treegrid("loaded");
        }

    }