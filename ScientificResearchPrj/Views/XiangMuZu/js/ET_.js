var ET_={
    onload: function () {
        $('#link_add').bind('click', BS_.appendRow);
        $('#link_remove').bind('click', BS_.remove);

        $('#link_getleaderormembertoselected').bind('click', BS_.getLeaderOrMemberSelected);
        $('#link_removeselectleaderormember').bind('click', BS_.removeSelectedLeaderOrMember);
        $('#link_removeallselectleaderormember').bind('click', BS_.removeAllSelectedLeaderOrMember);

        BS_.onLoad();
       
    }
}