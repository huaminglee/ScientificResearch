var ET_={
    onload: function () {
        
        CurrentPropertygridDataCount = 18;

        $('#link_fasong').bind('click', BS_.faSong);
        $('#link_baocun').bind('click', BS_.baoCun);
        $('#link_chaosong').bind('click', BS_.chaoSong);
        $('#link_liuchengtu').bind('click', BS_.liuChengTu);

        $('#link_fasong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_baocun').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_chaosong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_liuchengtu').bind('mouseover', _CommomOperation.propertygridEndEdit);

        BS_.onLoad();
    }
}