var ET_={
    onload: function () {
        $('#link_add').bind('click', BS_.appendRow);
        $('#link_remove').bind('click', BS_.remove);
        $('#link_unselect').bind('click', BS_.unselect);
        
        BS_.onLoad();
       
    }
}