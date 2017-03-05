var GameState = {
    WAITING_NEXT_ROUND: 1,
    IN_ROUND: 2,
    END_OF_ROUND: 3,
    properties: {
        1: {name: "waiting for next round", value: 1, code: "w"},
        2: {name: "in round", value: 2, code: "i"},
        3: {name: "end of round", value: 3, code: "e"}
    }
};


var TIME_BETWEEN_ROUNDS = 15 * 1000;

var ROUND_DURATION = 30 * 1000;



function GameManager() {
    this.gameRoom = null;
    this.gameState = GameState.WAITING_NEXT_ROUND;
    this.timeRemaining = 0;
    this.timeoutID = null;
}

GameManager.prototype.didGetNewGameRoom = function () {
    $('#orientationImage').attr('src', this.gameRoom.orientationImageURL);
};

GameManager.prototype.getNewRoom = function () {
    $.ajax({
        cache: false,
        url: ENDPOINT_GAMEROOM_NEW
    }).done(function (data) {
        gameManager.gameRoom = data.gameRoom;
        gameManager.didGetNewGameRoom();
        gameManager.startMainTimer();
    }).fail(function (error) {
        alert("ajax fail: " + error);
    });
};



GameManager.prototype.updateServer = function () {
    $.ajax({
        cache: false,
        url: ENDPOINT_GAMEROOM_UPDATE,
        data: {
            "room_id":this.gameRoom.id
        }
    }).done(function (data) {
        gameManager.gameRoom = data.gameRoom;
        //didUpdateGameRoom();
    }).fail(function (error) {
        alert("ajax fail: " + error);
    });
};



GameManager.prototype.startMainTimer = function() {
    this.timeRemaining = TIME_BETWEEN_ROUNDS;

    this.timeoutID = setTimeout(this.mainTimerFired, 10);
};

GameManager.prototype.mainTimerFired = function () {
    console.log('main timer fired');

    //this.updateServer();

    gameManager.timeRemaining -= 1000;

    if(gameManager.timeRemaining == 0) {
        if(gameManager.gameState == GameState.WAITING_NEXT_ROUND) {
            gameManager.gameState = GameState.IN_ROUND;

            gameManager.timeRemaining = ROUND_DURATION;
        }
        else if(gameManager.gameState == GameState.IN_ROUND) {
            gameManager.gameState = GameState.WAITING_NEXT_ROUND;

            gameManager.timeRemaining = TIME_BETWEEN_ROUNDS;
        }
    }

    console.log(gameManager.timeRemaining);

    $('#timerValue').text(":" + Math.ceil(gameManager.timeRemaining / 1000));

    gameManager.timeoutID = setTimeout(gameManager.mainTimerFired, 1000);
};




var gameManager = new GameManager();

$(document).ready(function() {
    //gameManager.getNewRoom();
});