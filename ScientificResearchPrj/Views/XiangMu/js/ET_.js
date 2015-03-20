var ET_={
    onload: function () {
        $('#link_add').bind('click', BS_.appendRow);
        $('#link_remove').bind('click', BS_.remove);
        $('#link_edit').bind('click', BS_.edit);

        BS_.onLoad();
       
    }
}