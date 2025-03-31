const express = require('express');
const app = express();
const port = 8080; // puerto de pruebas

class Gato{
  constructor(){
    this.init();
  }

  init ()
  {
    this._grid = [0,0,0,0,0,0,0,0,0];
    this._score1 = 0;
    this._score2 = 0;
  }

  getGrid (pos)
  {
    return this._grid[pos];
  }

  getScore (player)
  {
    if (player == 1)
    {
      return this._score1;
    }
    else
    {
      return this._score2;
    }
  }

  scorePlusPlus (player)
  {
    if (player == 1)
    {
      this._score1++;
    }
    else
    {
      this._score2++;
    }
  } 
}

gato = new Gato();
gato.init();

app.get('/', (req, res) => { // http://localhost
  res.send('Hello World!');
});

app.get('/init', (req, res) => { // http://localhost/init
  gato.init();
  res.send('Gato restarted...');
});

app.get('/status/:player', (req, res) => { // http://localhost/status/1
  let strRes = "";
  if (req.params['player'] == "1")
  {
    strRes = "Player 01: "+gato.getScore(1);
  }
  else if (req.params['player'] == "2")
  {
    gato.scorePlusPlus(2);
    strRes = "Player 02:"+gato.getScore(2);
  }
  else
  {
    strRes = "Error";
  }

  res.send(strRes);

});

app.listen(port, () => {
  console.log(`Server init in: ${port}`);
});