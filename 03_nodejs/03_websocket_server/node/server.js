const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
    
    ws.on('open', (data) => {
        console.log('New Connection');
     });

   ws.on('message', (data) => {
      console.log('Data received: %s',data);
      ws.send("The server response: "+data);
   });
});

wss.on('listening',()=>{
   console.log('listening on 8080');
});