
//connection.js
//=============

var mysql = require('mysql');

module.exports = {
  	getConnection: function() {
		var connection = mysql.createConnection({
		  host     : 'groupargame-dev.cagci70ht9sz.us-east-1.rds.amazonaws.com',
		  user     : 'SUPERARUSER',
		  password : '$850538FdfIDWPVNY9284#',
		  port     : '3306',
		  database : 'GroupARGame'
		});
		return connection;
	}
};