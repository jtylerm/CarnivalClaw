
// Constructor
function Player() {

    this.id = null;
    this.subID = null;
    this.updateID = -1; //incremented by client
    this.username = null;
    this.currentRoundScore = 0;
    this.previousRoundScore = 0;
    this.lastContactDate = new Date();
}
// class methods
Player.prototype.updateLastContactDate = function() {
    this.lastContactDate = new Date();
};
// export the class
module.exports = Player;