var ET_={
    onload: function () {
       
        CurrentPropertygridDataCount = 21;

        $('#link_fasong').bind('click', BS_.faSong);
        $('#link_liuchengtu').bind('click', BS_.liuChengTu);

        $('#link_fasong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_liuchengtu').bind('mouseover', _CommomOperation.propertygridEndEdit);
         
    }
}