var ET_={
    onload: function () {
        $('#link_add').bind('click', BS_.appendRow);
        $('#link_remove').bind('click', BS_.remove);
        
        $('#link_unselectdept').bind('click', BS_.unSelectDept);
        $('#link_unselecttutor').bind('click', BS_.unSelectTutor);

        $('#link_getstationtoselected').bind('click', BS_.getStationToSelected);
        $('#link_removeselectstation').bind('click', BS_.removeSelectedStation);
        $('#link_removeallselectstation').bind('click', BS_.removeAllSelectedStation);

        BS_.onLoad();
       
    }
}