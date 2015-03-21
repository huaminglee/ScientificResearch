var ET_={
    onload: function () {
        
        CurrentPropertygridDataCount = 16;

        if (CanEdit == 0) {
            $('#link_fasong').linkbutton("disable");
            $('#link_baocun').linkbutton("disable");
            $('#link_chaosong').linkbutton("disable");
        } else {
            $('#link_fasong').bind('click', BS_.faSong);
            $('#link_baocun').bind('click', BS_.baoCun);
            $('#link_chaosong').bind('click', BS_.chaoSong);
        }
        $('#link_liuchengtu').bind('click', BS_.liuChengTu);

        $('#link_fasong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_baocun').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_chaosong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_liuchengtu').bind('mouseover', _CommomOperation.propertygridEndEdit);

        BS_.onLoad();
    }
}