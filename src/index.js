const constants = require('./constants')
const express = require('express');
const app = express();

const server = require('http').createServer(app);
const io = require('socket.io').listen(server);

app.set('port', process.env.PORT || 3000);

let clients = [];

server.listen(app.get('port'), function() {
  console.log('************ SERVER STARTED ************');
});

io.on(constants.SOCKET_CONNECT, (socket) => {

  socket.on(constants.USER_CONNECT, () => handleUserConnect(socket));
  socket.on(constants.USER_INFO, (data) => handleUserInfo(socket, data)); 
  socket.on(constants.USER_OPTIONS, (data) => handleUserOptions(socket, data));
  socket.on(constants.SOCKET_DISCONNECT, () => handleSocketDisconnect(socket));
});

const handleUserConnect = (socket) => {
  console.log('User connected');
  clients.forEach(client => {
    socket.emit(constants.USER_LOGGED_IN, client);
    console.log('User name '+ client.name+ ' is connected');
  });
};

const handleUserInfo = (socket, data) => {
  let currentUser = {
    id: socket.id,
    name: data.name,
    message: data.message
  };
  clients.push(currentUser);
  console.log('Player logged in.' + currentUser.name);
  console.log('Player message.' + currentUser.message);
  socket.emit(constants.USER_LOGGED_IN, currentUser);
  socket.broadcast.emit(constants.USER_LOGGED_IN, currentUser);
}

const handleUserOptions = (socket, data) => {
  const currentUser = findUserByID(socket.id);
  console.log(currentUser);
  console.log(data);
}

const handleSocketDisconnect = (socket) => {
  const currentUser = findUserByID(socket.id);
  if (!clients.length) {
    return; 
  }
  console.log('Player disconnected ' + currentUser.name);
  socket.broadcast.emit(constants.USER_DISCONNECTED, currentUser);
  clients = clients.filter(client => client.name != currentUser.name);
}

const findUserByID = (id) => {
  return clients.find((client) => client.id === id);
}