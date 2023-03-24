'use strict';

const http = require("http");
var fs = require("fs");

const host = 'localhost';
const port = 80;

// [START app]
const express = require('express');
const app = express();

app.get('/makeFanOFF', (req, res) => {
  
  callChangeState(res, (res, states) => {
    console.log("states", states)
    states.isFanON = false;
    changeState(states);
      
    res.json(states);
  });

});

app.get('/getReadings', (req, res) => {
 
  res.send({
    moisture: getRandomValuesInRange(60, 90),
    humidity: getRandomValuesInRange(60, 90),
    temperature: getRandomValuesInRange(22, 45)
  });
});

app.get("/getDevicesStatus", (req, res) => {
  // Reading the file
  fs.readFile("./FileDB/MachineStates.json", 'utf8', function (err, jsonString) {
    if (err) {
      return console.error(err);
    }
    const states = JSON.parse(jsonString)
    console.log("Data read :- " + states.isFanON);
    res.json(states);
  });

});

app.get('/makeFanON', async (req, res) => {

  callChangeState(res, (res, states) => {
    console.log("states", states)
    states.isFanON = true;
    changeState(states);
      
    res.json(states);
  });

});



app.get('/makeSprinklerON', (req, res) => {
 
  callChangeState(res, (res, states) => {
    console.log("states", states)
    states.isSprinklerON = true;
    changeState(states);
      
    res.json(states);
  });

});

app.get('/makeSprinklerOFF', (req, res) => {

  callChangeState(res, (res, states) => {
    console.log("states", states)
    states.isSprinklerON = false;
    changeState(states);
      
    res.json(states);
  });

});

app.get('/makeLight1OFF', (req, res) => {

  callChangeState(res, (res, states) => {
    console.log("states", states)
    states.isLight1ON = false;
    changeState(states);
      
    res.json(states);
  });

});

app.get('/makeLight1ON', (req, res) => {

  callChangeState(res, (res, states) => {
    console.log("states", states)
    states.isLight1ON = true;
    changeState(states);
      
    res.json(states);
  });

});

// Listen to the App Engine-specified port, or 8080 otherwise
const PORT = process.env.PORT || 80;
app.listen(PORT, () => {
  console.log(`Server listening on port ${PORT}...`);
});
// [END app]

function printResultFor(op) {
  return function printResult(err, res) {
    if (err) console.log(op + ' error: ' + err.toString());
    if (res) console.log(op + ' status: ' + res.constructor.name);
  };
}

function receiveFeedback(err, receiver) {
  receiver.on('message', function (msg) {
    console.log('Feedback message:')
    console.log(msg.getData().toString('utf-8'));
  });
}



const getRandomValuesInRange = (max, min) => {
  return  Math.floor(Math.random() * (max - min + 1)) + min;
}

const changeState = (states) => {
  fs.writeFile(
    "./FileDB/MachineStates.json",
    JSON.stringify(states),
    function (err) {
      if (err) {
        return console.error(err);
      }
  
      // If no error the remaining code executes
      console.log(" Finished writing ");
      console.log("Reading the data that's written");
  
      // Reading the file
      fs.readFile("./FileDB/MachineStates.json", function (err, data) {
        if (err) {
          return console.error(err);
        }
        console.log("Data read :: " + data.toString());
      });
    }
  );
}

const  callChangeState = (res, processChangeState) => {
  fs.readFile("./FileDB/MachineStates.json", 'utf8', function (err, jsonString) {
    if (err) {
      return console.error(err);
    }
    var states = JSON.parse(jsonString);
    processChangeState(res, states)
  });
}