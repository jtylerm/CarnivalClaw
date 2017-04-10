
var Player = require('./player');

var dataHelper = require('./dataHelper');

// Constructor
function GameRoom() {

    this.id = null;
    this.updateID = -1;  //incremented by client
    this.imageIndex = -1;  //TODO: replace with image UUID?
    this.creationDate = new Date();
    this.lastContactDate = this.creationDate;
    this.gameState = null;
    this.nextGameState = null;
    this.timeRemaining = 0;
    this.previousRoundID = null;
    this.currentRoundID = null;
    this.stageScale = 1;
    this.stagePositionZOffset = 0;

    this.players = new Array();
    this.highScores = null;
}

// class methods
GameRoom.prototype.updateLastContactDate = function() {
    this.lastContactDate = new Date();
};

GameRoom.prototype.updatePlayer = function (updateID, playerID, playerSubID, roundScore, isCurrentRound, scoreCallback) {
    var player = this.playerWithID(playerID);
    if(player === null){
        player = new Player();
        player.id = playerID;
        this.players.push(player);
    }

    if(player.subID == null){
        dataHelper.getUserByID(player.id, function(err, user){
            if(user){
                player.subID = user.subID;
                player.username = user.username;
            }
        });
    }
    else if(player.subID == playerSubID) { //check that subIDs match before updating record
        if(updateID > player.updateID){

            if(isCurrentRound){
                player.currentRoundScore = roundScore;
            }
            else {
                player.previousRoundScore = roundScore;
            }

            scoreCallback(player.id, player.username, roundScore);

            player.updateLastContactDate();

            player.updateID = updateID;
        }
    }

    return player;
}

GameRoom.prototype.playerWithID = function (playerID) {
    for(var i = 0; i < this.players.length; i++) {
        var player = this.players[i];
        if(playerID == player.id){
            return player;
        }
    }
    return null;
}

GameRoom.prototype.purgeSilentPlayers = function() {

    for(var i = this.players.length - 1; i >= 0; i--) {
        var player = this.players[i];
        var timeSinceLastContact = Math.abs(new Date() - player.lastContactDate);
        if(timeSinceLastContact > 15*1000){
            this.players.splice(i, 1);
        }
    }
};

GameRoom.prototype.getRoundID = function() {
    return this.currentRoundID;
};

GameRoom.prototype.setRoundID = function(roundID) {
    if(this.currentRoundID != roundID){
        this.previousRoundID = this.currentRoundID;
        this.currentRoundID = roundID;
    }
};

// export the class
module.exports = GameRoom;
