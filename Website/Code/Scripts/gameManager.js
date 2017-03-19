var GameState = {
    UNSPECIFIED: 0,
    WAITING_NEXT_ROUND: 1,
    IN_ROUND: 2,
    END_OF_ROUND: 3,
    properties: {
        1: {name: "waiting for next round", value: 1, code: "w"},
        2: {name: "in round", value: 2, code: "i"},
        3: {name: "end of round", value: 3, code: "e"}
    }
};


var TIME_BETWEEN_ROUNDS = 10 * 1000;

var ROUND_DURATION = 45 * 1000;



function GameManager() {
    this.gameRoom = null;
    this.gameState = GameState.UNSPECIFIED;
    this.nextGameState = GameState.UNSPECIFIED;
    this.timeRemaining = 0;
    this.timeoutID = null;
    this.updateID = 1;
    this.roundID = null;
}

GameManager.prototype.didGetNewGameRoom = function () {
    $('#orientationImage').attr('src', this.gameRoom.orientationImageURL);
    gameManager.startMainTimer();
};

GameManager.prototype.getNewRoom = function () {
    $.ajax({
        cache: false,
        url: ENDPOINT_GAMEROOM_NEW
    }).done(function (data) {
        gameManager.gameRoom = data.gameRoom;
        gameManager.didGetNewGameRoom();

    }).fail(function (error) {
        console.log("ajax fail: " + error);
    });
};



GameManager.prototype.sendRoomUpdate = function () {
    this.updateID++;

    var dataObj = {
        "update_id":this.updateID,
        "room_id":this.gameRoom.id,
        "game_state":this.gameState,
        "next_game_state":this.nextGameState,
        "time_remaining":this.timeRemaining,
        "round_id":this.roundID
    };

    console.log("sendRoomUpdate " + JSON.stringify(dataObj));

    $.ajax({
        cache: false,
        url: ENDPOINT_GAMEROOM_UPDATE,
        data: dataObj
    }).done(function (data) {
        gameManager.gameRoom = data.gameRoom;
        //didUpdateGameRoom();
    }).fail(function (error) {
        console.log("ajax fail: " + error);
    });
};



GameManager.prototype.startMainTimer = function() {
    this.timeRemaining = TIME_BETWEEN_ROUNDS;

    this.timeoutID = setTimeout(this.mainTimerFired, 1000);
};

GameManager.prototype.mainTimerFired = function () {
    console.log('main timer fired');

    if(gameManager.gameState == GameState.UNSPECIFIED) {
        gameManager.transitionToWaiting();
    }
    else {
        gameManager.timeRemaining -= 1000;

        if(gameManager.timeRemaining <= 0) {
            //time to change states
            if(gameManager.gameState == GameState.WAITING_NEXT_ROUND) {
                gameManager.transitionToInRound();
            }
            else if(gameManager.gameState == GameState.IN_ROUND) {
                gameManager.transitionToWaiting();
            }
        }
    }

    console.log(gameManager.timeRemaining);

    $('#timerValue').text(":" + Math.ceil(gameManager.timeRemaining / 1000));

    gameManager.sendRoomUpdate();

    gameManager.timeoutID = setTimeout(gameManager.mainTimerFired, 1000);
};

GameManager.prototype.transitionToWaiting = function () {
    this.gameState = GameState.WAITING_NEXT_ROUND;
    this.nextGameState = GameState.IN_ROUND;
    this.timeRemaining = TIME_BETWEEN_ROUNDS;
    this.roundID = generateUUID();

    //TODO: update the UI
};

GameManager.prototype.transitionToInRound = function () {
    this.gameState = GameState.IN_ROUND;
    this.nextGameState = GameState.WAITING_NEXT_ROUND;
    this.timeRemaining = ROUND_DURATION;

    //TODO: update the UI
};

var gameManager = new GameManager();

$(document).ready(function() {
    gameManager.getNewRoom();
});

function generateUUID() {
    var d = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = (d + Math.random()*16)%16 | 0;
        d = Math.floor(d/16);
        return (c=='x' ? r : (r&0x3|0x8)).toString(16);
    });
    return uuid;
}
