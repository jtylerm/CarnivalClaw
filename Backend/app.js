var express = require('express');
var app = express();
var bodyParser = require('body-parser')

app.use(bodyParser.urlencoded({ extended: false }));
app.use(bodyParser.json());

//app.use(require('connect').bodyParser());

app.all('*', function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "X-Requested-With");
    next();
});

app.use(function(req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "X-Requested-With");
    next();
});

//include API resources (endpoints)
require('./api/userResource')(app);

require('./api/gameResource')(app);

/**
	Set server to use 'public' as default content directory, with imageIndex.html as default page
*/
var publicOptions = {
  imageIndex: "index.html"
};
app.use('/', express.static('public', publicOptions));

/**
	Start server listening on specified port
*/
var port = process.env.PORT || 3000;
app.listen(port, function () {
  console.log('Carnival Claw app listening on port ' + port);
});

module.exports = app;


