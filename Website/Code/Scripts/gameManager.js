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


var TIME_BETWEEN_ROUNDS = 20 * 1000;

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

    var url = ENDPOINT_GAMEROOM_NEW + "?";
    url += "width=" + $(window).width();
    url += "&height=" + $(window).height();

    $.ajax({
        cache: false,
        url: url
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

    //console.log("sendRoomUpdate " + JSON.stringify(dataObj));

    $.ajax({
        cache: false,
        url: ENDPOINT_GAMEROOM_UPDATE,
        data: dataObj
    }).done(function (data) {
        gameManager.gameRoom = data.gameRoom;
        console.log("gameRoom " + JSON.stringify(gameManager.gameRoom));
        gameManager.showCurrentScores();
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
    //console.log('main timer fired');

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
    var timeRemainingPrefix = null;
    if(gameManager.gameState == GameState.WAITING_NEXT_ROUND){
        timeRemainingPrefix = "Time Until Next Round: ";
    }
    else if(gameManager.gameState == GameState.IN_ROUND) {
        timeRemainingPrefix = "Time Remaining In Current Round: ";
    }
    $('#timerText').text(timeRemainingPrefix);
    $('#timerValue').text(Math.ceil(gameManager.timeRemaining / 1000));

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

GameManager.prototype.showCurrentScores = function(){
    var PLAYERS_PER_LEADERBOARD = 6;
    var players = this.gameRoom.players;
    var isHighScores = false;

    //DEBUG - test with many players
    // var playerID = 1001;
    // while (players.length < 9) {
    //     var player = new Object();
    //     player.id = playerID++;
    //     player.previousRoundScore = 3000;
    //     player.currentRoundScore = 4000;
    //     players.push(player);
    // }

    if(players.length == 0) {
        isHighScores = true;
    }
    else {
        if(this.gameRoom.gameState == GameState.IN_ROUND) {
            isHighScores = false;
        }
        else {
            if(this.timeRemaining >= 15 * 1000) {
                isHighScores = false;
            }
            else if(this.timeRemaining < 15 * 1000 && this.timeRemaining >= 10 * 1000) {
                isHighScores = true;
            }
            else if(this.timeRemaining < 10 * 1000 && this.timeRemaining >= 5 * 1000) {
                isHighScores = false;
            }
            else if(this.timeRemaining < 5 * 1000){
                isHighScores = true;
            }
        }
    }

    var playerScores = null;
    if(isHighScores){
        playerScores = this.gameRoom.highScores;
        $("#lblHighScoresTitle").text("High Scores");
    }
    else {
        playerScores = players;
        $("#lblHighScoresTitle").text("Current Round Scores");
    }

    var leaderboardDivs = [$("#leaderboard0"), $("#leaderboard1"), $("#leaderboard2")];
    for(var i = 0; i < leaderboardDivs.length; i++){
        if(playerScores.length > i*PLAYERS_PER_LEADERBOARD){
            leaderboardDivs[i].show();
            var table = leaderboardDivs[i].find("table");
            var startIndex = i*PLAYERS_PER_LEADERBOARD;
            var endIndex = Math.min(startIndex + PLAYERS_PER_LEADERBOARD, playerScores.length);
            var playersForLeaderboard = playerScores.slice(startIndex, endIndex);
            populateLeaderboardTableWithPlayers(table, playersForLeaderboard, this.gameRoom.gameState, isHighScores);
        }
        else {
            leaderboardDivs[i].hide();
        }
    }
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

function populateLeaderboardTableWithPlayers(table, players, gameState, isHighScores){
    var tbody = table.find("tbody");
    tbody.empty();

    for(var i = 0; i < players.length; i++){
        var player = players[i];

        var tableRow = $("<tr></tr>");
        tbody.append(tableRow);

        var usernameCol = $("<td></td>");
        tableRow.append(usernameCol);
        //TODO: show username from server
        //usernameCol.text("Wolfpacker_" + player.id);
        usernameCol.text(player.username);

        var scoreCol = $("<td></td>");
        tableRow.append(scoreCol);

        if(isHighScores) {
            scoreCol.text(player.score);
        }
        else {
            if(gameState == GameState.WAITING_NEXT_ROUND){
                scoreCol.text(player.previousRoundScore);
            }
            else {
                scoreCol.text(player.currentRoundScore);
            }
        }
    }

    var extraRowCount = 6 - players.length;
    for(var i = 0; i < extraRowCount; i++){
        var dummyRow = $("<tr></tr>");
        tbody.append(dummyRow);

        var dummyCol = $("<td></td>");
        dummyRow.append(dummyCol);
        dummyCol.html("&nbsp;");

        dummyCol = $("<td></td>");
        dummyRow.append(dummyCol);
        dummyCol.html("&nbsp;");
    }

}