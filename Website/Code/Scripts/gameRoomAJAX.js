/**
 * Created by Tyler on 1/29/17.
 */





function getNewRoom() {
    $.ajax({
        cache: false,
        url: ENDPOINT_GAMEROOM_NEW
    }).done(function (data) {
        gameManager.gameRoom = data.gameRoom;
        didUpdateGameRoom();
    }).fail(function (error) {
        alert("ajax fail: " + error);
    });
}

function didUpdateGameRoom() {
    $('#orientationImage').attr('src', gameManager.gameRoom.orientationImageURL);
}