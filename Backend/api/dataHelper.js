
var dbConnection = require('../connection');

var User = require('./user');

var UserScore = require('./userScore');

module.exports = {

    getUserByID: function(userID, callback){
        _getUserByID(userID, callback);
    },
    recordUserScore: function(userID, score){
        _recordUserScore(userID, score);
    },
    getUserHighScores: function(callback){
        _getUserHighScores(callback);
    }
};

function _getUserByID(userID, callback){
    var connection = dbConnection.getConnection();
    connection.connect();

    connection.query('SELECT * FROM ARUser WHERE ID = ? ', [userID], function(err, rows) {

        if(err){
            console.log("_getUserByID error - " + err);
            callback(err, null);
            return;
        }

        var user = new User();
        user.id = rows[0].ID;
        user.subID = rows[0].SubID;
        user.username = rows[0].Username;

        callback(null, user);
    });

    connection.end();
}

function _recordUserScore(userID, score) {
    var connection = dbConnection.getConnection();
    connection.connect();

    var query = "INSERT INTO ScoreRecord(UserID, Score) " +
        " VALUES (?, ?);";

    connection.query(query, [userID, score], function(err) {

        if(err){
            console.log("_recordUserScore error - " + err);
        }

    });

    connection.end();
}

function _getUserHighScores(callback) {
    var connection = dbConnection.getConnection();
    connection.connect();

    var query = "SELECT tAU.ID AS UserID, tAU.Username AS Username, MAX(tSR.Score) AS Score" +
        " FROM ARUser AS tAU " +
        " JOIN ScoreRecord AS tSR ON tSR.UserID = tAU.ID " +
        " GROUP BY tAU.ID, tAU.Username " +
        " ORDER BY Score DESC, CreatedTimestamp ASC" +
        " LIMIT 18;";

    connection.query(query, [], function(err, rows) {

        if(err){
            console.log("_getUserHighScores error - " + err);
            callback(err, null);
            return;
        }

        var userScores = new Array();
        for(var i = 0; i < rows.length; i++){
            var user = new UserScore();
            user.userID = rows[i].UserID;
            user.username = rows[i].Username;
            user.score = rows[i].Score;
            userScores.push(user);
        }

        callback(null, userScores);
    });

    connection.end();
}