
const uuidV1 = require('uuid/v1');

var dbConnection = require('../connection');

module.exports = function(app){

	app.get('/api/guestuser/new', function (request, response) {

		var responseEnvelope = new Object();

		var connection = dbConnection.getConnection();
		connection.connect();

		var subID = uuidV1();
		connection.query('INSERT INTO ARUser (SubID, Username, IsGuest, Activated) VALUES (?, UUID(), TRUE, FALSE)', [subID], function(insertErr, insertResult) {
			if (insertErr){
				//throw insertErr;
				console.log("/api/guestuser/new insert error - " + insertErr);
				response.error = insertErr;
				response.send(responseEnvelope);
				connection.end();
			}
			else {
				console.log(insertResult.insertId);
				var username = "Wolfpacker_" + (insertResult.insertId + 100);

				connection.query('UPDATE ARUser SET Username = ?, Activated = TRUE WHERE ID = ?', [username, insertResult.insertId], function(updateErr, updateResult) {
					if (updateErr){
						//throw updateErr;
						console.log("/api/guestuser/new update error - " + updateErr);
						response.error = updateErr;
						response.send(responseEnvelope);
					}
					else {
						responseEnvelope.id = insertResult.insertId;
						responseEnvelope.username = username;
						responseEnvelope.subID = subID;

						response.send(responseEnvelope);

						console.log(JSON.stringify(responseEnvelope));
					}
				});

				connection.end();
			}
		});
	});

	app.post('/api/user', function (request, response) {

		var username = request.body.username;
		var pwd = request.body.pwd;

		console.log("username: " + username);
		console.log("pwd: " + pwd);

		var connection = dbConnection.getConnection();
	  	connection.connect();
	 
		connection.query('SELECT * FROM ARUser WHERE Username = ? AND PwdHash = MD5(?) ', [username, pwd], function(err, rows, fields) {

			var responseEnvelope = new Object();

			if (err){
				//throw err;
				console.log("/api/user error - " + err);
				response.error = err;
				response.send(responseEnvelope);
			}
			else {
				if (rows.length == 1) {
					responseEnvelope.userID = rows[0].ID;
					responseEnvelope.msg = "User authenticated";
				}
				else {
					responseEnvelope.errorMsg = 'Username/password combination not found';
				}

				response.send(responseEnvelope);

				//console.log(JSON.stringify(responseEnvelope));
			}
		});
	 
		connection.end();
	});

}