'use strict';

var Client = require('azure-iothub').Client;
var Message = require('azure-iot-common').Message;
const http = require("http");

const host = 'localhost';
const port = 80;

// import { Client } from 'azure-iothub';
// import { Message } from 'azure-iot-common';

var connectionString = 'HostName=ih-greenhouse.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=FJY0SAZrDgLksWRY+S23h87B9Jw3EPGO4zkOzyQrKxI=';
var targetDevice = 'smart-detector-1.0';

var serviceClient = Client.fromConnectionString(connectionString);

// [START app]
const express = require('express');
const app = express();

app.get('/makeFanOFF', (req, res) => {
  
  serviceClient.open(function (err) {
    if (err) {
      console.error('Could not connect: ' + err.message);
    } else {
      console.log('Service client connected');
      serviceClient.getFeedbackReceiver(receiveFeedback);
      // var message = new Message("MAKE-FAN-ON");
      var message = new Message("MAKE-FAN-OFF");
      // var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-LIGHTS-OFF");
      // var message = new Message("MAKE-LIGHTS-ON");
      // var message = new Message("MAKE-SPRINKLER-OFF");
      // var message = new Message("MAKE-SPRINKLER-ON");
  
      message.ack = 'full';
      message.messageId = "My Message ID";
      console.log('Sending message: ' + message.getData());
      serviceClient.send(targetDevice, message, printResultFor('send'));
    }
  });
  res.send('Hello! Fan made OFF!');
});

app.get('/makeFanON', (req, res) => {
  serviceClient.open(function (err) {
    if (err) {
      console.error('Could not connect: ' + err.message);
    } else {
      console.log('Service client connected');
      serviceClient.getFeedbackReceiver(receiveFeedback);
      var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-FAN-OFF");
      // var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-LIGHTS-OFF");
      // var message = new Message("MAKE-LIGHTS-ON");
      // var message = new Message("MAKE-SPRINKLER-OFF");
      // var message = new Message("MAKE-SPRINKLER-ON");
  
      message.ack = 'full';
      message.messageId = "My Message ID";
      console.log('Sending message: ' + message.getData());
      serviceClient.send(targetDevice, message, printResultFor('send'));
    }
  });
  res.send('Hello! Fan made ON!');
});

app.get('/makeWaterpumpON', (req, res) => {
  serviceClient.open(function (err) {
    if (err) {
      console.error('Could not connect: ' + err.message);
    } else {
      console.log('Service client connected');
      serviceClient.getFeedbackReceiver(receiveFeedback);
      // var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-FAN-OFF");
      // var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-LIGHTS-OFF");
      // var message = new Message("MAKE-LIGHTS-ON");
      // var message = new Message("MAKE-SPRINKLER-OFF");
      var message = new Message("MAKE-SPRINKLER-ON");
  
      message.ack = 'full';
      message.messageId = "MAKE-SPRINKLER-ON";
      console.log('Sending message: ' + message.getData());
      serviceClient.send(targetDevice, message, printResultFor('send'));
    }
  });
  res.send('Hello! Waterpump made ON!');
});

app.get('/makeWaterpumpOFF', (req, res) => {
  serviceClient.open(function (err) {
    if (err) {
      console.error('Could not connect: ' + err.message);
    } else {
      console.log('Service client connected');
      serviceClient.getFeedbackReceiver(receiveFeedback);
      // var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-FAN-OFF");
      // var message = new Message("MAKE-FAN-ON");
      // var message = new Message("MAKE-LIGHTS-OFF");
      // var message = new Message("MAKE-LIGHTS-ON");
      // var message = new Message("MAKE-SPRINKLER-OFF");
      var message = new Message("MAKE-SPRINKLER-OFF");
  
      message.ack = 'full';
      message.messageId = "MAKE-SPRINKLER-OFF";
      console.log('Sending message: ' + message.getData());
      serviceClient.send(targetDevice, message, printResultFor('send'));
    }
  });
  res.send('Hello! Waterpump made OFF!');
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



