const WebSocket = require('ws');
const clients = [];
const users = [];

class User{
	constructor()
	{
		this._username="No name";
		this._conn=null;
	}

	set username( user )
	{
		this._username = user;
	}

	get username ()
	{
		return this._username;
	}

	set connection( con )
	{
		this._conn = con;
	}

	get connection ()
	{
		return this._conn;
	}

	static findClientByUsername (lst, username)
	{
		lst.forEach(user => {
			if(user.username === username)
			{
				return user;
			}
		});
		return null;
	}
}

const wss = new WebSocket.Server({ port: 8080 },()=>{
    console.log('Server Started');
});

wss.on('connection', function connection(ws) {
	
	console.log('New connenction');
	let user = new User ();
	user.connection = ws;
	users.push(user); // Agregar la conexión (cliente) a la lista
	
    ws.on('open', (data) => {
		console.log('Now Open');
	});

	ws.on('message', (data) => {
		console.log('Data received: %s',data);
		
		//ws.send("The server response: "+data); // Para mandar el mensaje al cliente que lo envió

		let info = data.toString().split('|'); // 200|username // 100| // 300|id|pos

		switch (info[0])
		{
			case '200': // set Username
				user.username = info[1];
				user.connection.send("200|UserName upDated: "+user.username);
			break; 

			case '300': // getList
				let lista = "";
				users.forEach(us => {
					if(us.connection.readyState === WebSocket.OPEN)
					{
						lista = lista + us.username + " - ";
						//us.send(cliente.username + " says: " + data); // si falla, cambiar a: `data.toString()`
					}
				});
				user.connection.send("300|list: "+lista);
			break;

			case '400': // Mandar mensaje directo
				let u=true;

				users.forEach(us => {
					if(us.username === info[1])
					{
						u=false;
						us.connection.send("400|"+info[2]);
					}
				});

				if(u == true){
					user.connection.send("404|User not found");
				}

				break;
			
				case '404': // Mandar mensaje directo
				break;

			default: // broadcast
				// Mandar a todos los clientes conectados el mensaje con el username de quien lo envió
				users.forEach(us => {
					if(us.readyState === WebSocket.OPEN)
					{
						us.send(us.username + " says: " + data); // si falla, cambiar a: `data.toString()`
					}
				});
			break;
		}
	});

	// Al cerrar la conexión, quitar de la lista de clientes
	ws.on('close', () => { 
		let index = users.indexOf(user);
		if(index > -1)
		{
			users.splice(index, 1);
			user.connection.send("UserName disconnected: "+user.username);
		}
	});
});

wss.on('listening',()=>{
   console.log('Now listening on port 8080...');
});