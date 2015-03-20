var ET_={
    onload: function () {
        CurrentPropertygridDataCount = 2;

        $('#link_xuanzexiangmu').bind('click', BS_.openProjectDialog);
        $('#link_xuanzeketi').bind('click', BS_.openSunjectDialog);
        $('#link_fasong').bind('click', BS_.faSong);
        $('#link_baocun').bind('click', BS_.baoCun);
        $('#link_liuchengtu').bind('click', BS_.liuChengTu);

        $('#link_xuanzexiangmu').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_xuanzeketi').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_fasong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_baocun').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_liuchengtu').bind('mouseover', _CommomOperation.propertygridEndEdit);

        $('#link_unselectproject').bind('click', BS_.unSelectProject);
        $('#link_unselectsubject').bind('click', BS_.unSelectSubject);
         
        BS_.onLoad();
    }
}