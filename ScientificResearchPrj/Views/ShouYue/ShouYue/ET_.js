var ET_={
    onload: function () {
       
        CurrentPropertygridDataCount = 21;

        if (CanEdit == 0) {
            $('#link_fasong').linkbutton("disable");
        } else {
            $('#link_fasong').bind('click', BS_.faSong);
        }
        $('#link_liuchengtu').bind('click', BS_.liuChengTu);

        $('#link_fasong').bind('mouseover', _CommomOperation.propertygridEndEdit);
        $('#link_liuchengtu').bind('mouseover', _CommomOperation.propertygridEndEdit);
         
    }
}