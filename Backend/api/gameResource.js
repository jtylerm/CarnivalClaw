
var GameManager = require('./gameManager');

var gameManager = GameManager.getDefaultGameManager();

module.exports = function(app){

    /**
     * Create new room
     */
    app.get('/api/gameroom/new', function (request, response) {

        var width =  parseInt(request.query.width);
        var height =  parseInt(request.query.height);

        gameManager.purgeSilentRooms();
        var gameRoom = gameManager.addNewRoom(width, height);

        var responseEnvelope = new Object();
        responseEnvelope.gameRoom = gameRoom;

        response.send(responseEnvelope);
    });

    /**
     * Retrieve room status (state, time remaining, etc)
     */
    app.get('/api/gameroom', function (request, response) {

        var roomID =  request.query.room_id;

        var gameRoom = gameManager.roomWithID(roomID);

        var responseEnvelope = new Object();
        responseEnvelope.gameRoom = gameRoom;

        response.send(responseEnvelope);
    });

    /**
     * Browser sends game info for room
     */
    app.get('/api/gameroom/update', function (request, response) {

        var updateID =  parseInt(request.query.update_id);
        var roomID =  request.query.room_id;
        var gameState =  parseInt(request.query.game_state);
        var nextGameState =  parseInt(request.query.next_game_state);
        var timeRemaining =  parseInt(request.query.time_remaining);
        var roundID =  request.query.round_id;

        console.log("update_id: " + updateID);

        var gameRoom = gameManager.updateRoom(updateID, roomID, gameState, nextGameState, timeRemaining, roundID);

        console.log(JSON.stringify(gameRoom));

        var responseEnvelope = new Object();
        responseEnvelope.gameRoom = gameRoom;

        response.send(responseEnvelope);
    });

    /**
     * Mobile client posts score for {player, room, round?, timestamp}
     */
    app.get('/api/player/update', function (request, response) {

        var updateID =  parseInt(request.query.update_id);
        var orientationImageIndex =  parseInt(request.query.orientation_image_index);
        var playerID =  parseInt(request.query.player_id);
        var playerSubID =  request.query.player_sub_id;
        var roundID =  request.query.round_id;
        var roundScore = parseInt(request.query.score);

        var gameRoom = gameManager.updatePlayer(updateID, orientationImageIndex, playerID, playerSubID, roundID, roundScore);

        var responseEnvelope = new Object();

        if(gameRoom){
            responseEnvelope.gameState = gameRoom.gameState;
            responseEnvelope.nextGameState = gameRoom.nextGameState;
            responseEnvelope.timeRemaining = gameRoom.timeRemaining;
            responseEnvelope.currentRoundID = gameRoom.currentRoundID;
            responseEnvelope.stageScale = gameRoom.stageScale;
            responseEnvelope.stagePositionZOffset = gameRoom.stagePositionZOffset;
        }
        else {
            responseEnvelope.errorMsg = 'player/update - no game room for specified orientationImageIndex';
        }

        response.send(responseEnvelope);
    });

}