
const uuidV1 = require('uuid/v1');

var CronJob = require('cron').CronJob;

var GameRoom = require('./gameRoom');

var dataHelper = require('./dataHelper');

var GameState = {
    UNSPECIFIED: 0,
    WAITING_NEXT_ROUND: 1,
    IN_ROUND: 2,
    END_OF_ROUND: 3,
    properties: {
        0: {name: "unspecified", value: 0, code: "u"},
        1: {name: "waiting for next round", value: 1, code: "w"},
        2: {name: "in round", value: 2, code: "i"},
        3: {name: "end of round", value: 3, code: "e"}
    }
};
if (Object.freeze) Object.freeze(GameState);

var BETWEEN_ROUNDS_DURATION = 15;
var ROUND_DURATION = 45;

var orientationImages = [
    "https://s3.amazonaws.com/groupargame/Images/Orientation/orientationImage0.jpg",
    "https://s3.amazonaws.com/groupargame/Images/Orientation/orientationImage1.jpg",
    "https://s3.amazonaws.com/groupargame/Images/Orientation/orientationImage2.jpg",
    "https://s3.amazonaws.com/groupargame/Images/Orientation/orientationImage3.jpg",
    "https://s3.amazonaws.com/groupargame/Images/Orientation/orientationImage4.jpg",
    "https://s3.amazonaws.com/groupargame/Images/Orientation/orientationImage5.jpg"
];
if (Object.freeze) Object.freeze(orientationImages);

// Constructor
function GameManager() {
    this.rooms = new Array();
    this.highScores = new Array();
}

// prototype methods

GameManager.prototype.addRoomZero = function() {
    var roomZero = this.newRoomWithImageIndex(0);
    if(roomZero == null){
        throw "unable to create room zero";
    }
    else {
        this.rooms.push(roomZero);
        return roomZero;
    }
};

GameManager.prototype.addNewRoom = function(width, height) {
    var imageIndex = this.nextAvailableIndex();
    if(imageIndex >= 0){
        var newRoom = this.newRoomWithImageIndex(imageIndex);
        if(width == 6816 && height == 2240){
            newRoom.stageScale = .5;
            newRoom.stagePositionZOffset = -1;
            console.log("immersion theatre added");
        }
        else if(width == 7337 && height == 1720){
            newRoom.stageScale = .6;
            newRoom.stagePositionZOffset = -1.3;
            console.log("game lab added");
        }
        this.rooms.push(newRoom);
        return newRoom;
    }
    else {
        return null;
    }
};

GameManager.prototype.newRoomWithImageIndex = function(imageIndex) {
    var newRoom = new GameRoom();
    newRoom.id = uuidV1();
    newRoom.imageIndex = imageIndex;
    newRoom.orientationImageURL = orientationImages[newRoom.imageIndex];
    newRoom.updateLastContactDate();
    newRoom.gameState = GameState.UNSPECIFIED;
    newRoom.nextGameState = GameState.UNSPECIFIED;
    return newRoom;
};

GameManager.prototype.nextAvailableIndex = function() {

    if(this.rooms.length >= orientationImages.length){
        //throw "nextAvailableIndex error - no available image imageIndex";
        console.log("nextAvailableIndex - no available image imageIndex")
        return -1;
    }

    var index = 1;
    var available = false;
    while(!available) {
        var found = false;
        for(var i = 1; i < this.rooms.length; i++) {
            var gameRoom = this.rooms[i];
            if(index == gameRoom.imageIndex){
                found = true;
            }
        }
        if(found) {
            index++;
        }
        else {
            available = true;
        }
    }
    return index;
};

GameManager.prototype.purgeSilentRooms = function() {

    //purge all but room zero
    for(var i = this.rooms.length - 1; i >= 1; i--) {
        var gameRoom = this.rooms[i];
        var timeSinceLastContact = Math.abs(new Date() - gameRoom.lastContactDate);
        if(timeSinceLastContact > 15*1000){
            this.rooms.splice(i, 1);
        }
    }
};

GameManager.prototype.roomWithID = function (roomID) {
    for(var i = 0; i < this.rooms.length; i++) {
        var gameRoom = this.rooms[i];
        if(roomID == gameRoom.id){
            return gameRoom;
        }
    }
    return null;
};

GameManager.prototype.roomWithOrientationImageIndex = function (orientationImageIndex) {
    for(var i = 0; i < this.rooms.length; i++) {
        var gameRoom = this.rooms[i];
        if(orientationImageIndex == gameRoom.imageIndex){
            return gameRoom;
        }
    }
    return null;
};

GameManager.prototype.updateRoom = function (updateID, roomID, gameState, nextGameState, timeRemaining, roundID) {
    var gameRoom = this.roomWithID(roomID);
    if(gameRoom){
        if(updateID > gameRoom.updateID){
            if(gameRoom.gameState != gameState) {

                console.log("game state changed : " + gameState + ", " + roundID);
                gameRoom.gameState = gameState;
                gameRoom.nextGameState = nextGameState;
                gameRoom.setRoundID(roundID);
            }
            gameRoom.timeRemaining = timeRemaining;
            gameRoom.updateLastContactDate();
            gameRoom.updateID = updateID;
            gameRoom.highScores = this.highScores;
            console.log("updateRoom " + gameRoom.updateID);
        }
    }
    else {
        //TODO: notify web client that room doesn't exist...new room needed
        console.log("updateRoom - room with ID " + roomID + " not found");
    }

    return gameRoom;
};

GameManager.prototype.updateRoomZero = function () {
    var gameRoom = this.rooms[0];
    //TODO: track time remaining, switch rounds, etc
    if(gameRoom){

        gameRoom.timeRemaining -= 1; //subtract 1 second
        if(gameRoom.timeRemaining <= 0){
            if(gameRoom.gameState == GameState.IN_ROUND){
                gameRoom.gameState = GameState.WAITING_NEXT_ROUND;
                gameRoom.nextGameState = GameState.IN_ROUND;
                gameRoom.setRoundID(uuidV1());
                gameRoom.timeRemaining = BETWEEN_ROUNDS_DURATION;
                console.log("game state changed room ZERO: " + gameRoom.gameState + ", " + gameRoom.getRoundID());
            }
            else {
                gameRoom.gameState = GameState.IN_ROUND;
                gameRoom.nextGameState = GameState.WAITING_NEXT_ROUND;
                gameRoom.setRoundID(uuidV1());
                gameRoom.timeRemaining = ROUND_DURATION;
                console.log("game state changed room ZERO: " + gameRoom.gameState + ", " + gameRoom.getRoundID());
            }
        }

        console.log("updateRoom ZERO timeRemaining=" + gameRoom.timeRemaining);
    }
    else {
        //TODO: notify web client that room doesn't exist...new room needed
        console.log("updateRoomZero - room zero not found");
    }

    return gameRoom;
};

GameManager.prototype.updatePlayer = function (updateID, orientationImageIndex, playerID, playerSubID, roundID, roundScore) {
    var gameRoom = this.roomWithOrientationImageIndex(orientationImageIndex);
    if(gameRoom){

        gameRoom.purgeSilentPlayers();
        var isCurrentRound = (gameRoom.currentRoundID == roundID);
        gameRoom.updatePlayer(updateID, playerID, playerSubID, roundScore, isCurrentRound, this.didReportScore);
    }
    else {
        //TODO: notify web client that room doesn't exist...room no longer running?
    }

    return gameRoom;
};

GameManager.prototype.loadHighScores = function(){
    dataHelper.getUserHighScores(function(err, userScores){
        if(userScores){
            _defaultGameManager.highScores = userScores;
        }
    });
};

GameManager.prototype.didReportScore = function(playerID, username, score){
    dataHelper.recordUserScore(playerID, score);

    if(_defaultGameManager.highScores.length > 0){
        var lastScore = _defaultGameManager.highScores[_defaultGameManager.highScores.length - 1];
        if(score > lastScore.score){
            _defaultGameManager.loadHighScores();
        }
    }
};

//scheduled task

new CronJob('* * * * * *', function() {
    //fires once per second
    //console.log('task per second');

    _defaultGameManager.updateRoomZero();

}, null, true, 'UTC');

var _defaultGameManager = new GameManager();
_defaultGameManager.addRoomZero();
_defaultGameManager.loadHighScores();

function _getDefaultGameManager(){
    return _defaultGameManager;
}

// export the class
module.exports = GameManager;

//export the functions
module.exports = {
    getDefaultGameManager: function() {
        return _getDefaultGameManager();
    }
}
