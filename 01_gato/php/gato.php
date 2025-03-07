<?php

    class Gato{
        public $db="game.db";

        public  $p1, // username
                $p2, // username2
                $actual,
                $round,
                $score1,
                $score2,
                $board; // array

        public function init()
        {
            $this->board = array(0,0,0,0,0,0,0,0,0);
            
            $this->p1="id1";
            $this->p2="id2";
            $this->actual=0;
            $this->round=0;
            $this->score1=0;
            $this->score2=0;

            $this->saveDb();
        }
        
        public function saveDb ()
        {
            $file = fopen($this->db, "w") or die("error");
            $strData = json_encode($this->toJson());
            fwrite($file, $strData);
            fclose($file);
        }

        public function toJson ()
        {
            $data = array(
                "p1"=>$this->p1,
                "p2"=>$this->p2,
                "actual"=>$this->actual,
                "round"=>$this->round,
                "score1"=>$this->score1,
                "score2"=>$this->score2,
                "board"=>$this->board
            );

            return $data;
        }

        public function loadDb ()
        {
            $file = fopen($this->db, "r") or die ("error");
            $strData = fread($file,filesize($this->db));
            $data = json_decode($strData);
            
            $this->p1 = $data->p1;
            $this->p2 = $data->p2;
            $this->actual = $data->actual;
            $this->round = $data->round;
            $this->score1 = $data->score1;
            $this->score2 = $data->score2;
            $this->board = $data->board;
        }

        public function toString()
        {
            echo "".
            "p1:".$this->p1."<br/>".
            "p2:".$this->p2."<br/>".
            "actual:".$this->actual."<br/>".
            "round:".$this->round."<br/>".
            "score1:".$this->score1."<br/>".
            "score2:".$this->score2."<br/>".
            "board:";
            var_dump($this->board);
            
        }

        public function setPlayerId ($player, $id)
        {
            if ($player == 1)
                $this->p1 = $id;
            elseif ($player == 2)
                $this->p2 = $id;
            else
                return false;
            return true;
        }

        public function getPlayer($id)
        {
            if ($id == $this->p1)
                return 1;
            elseif ($id == $this->p2)
                return 2;
            else
                return 0;
        }

        public function getScore ($player)
        {
            switch($player)
            {
                case 1:
                    return $this->score1;
                break;

                case 2:
                    return $this->score2;
                break;

                default:
                    return "-1";
                break;
            }
        }

        public function getStatus ($id)
        {
            $player = $this->getPlayer($id);

            $data = array(
                "actual"=>$this->actual,
                "round"=>$this->round,
                "score".$player=>$this->getScore($player),
                "board"=>$this->board,
            );

            return json_encode($data);
        }

        public function turn($id, $pos) // pos en formato de array unidimensional
        {
            // validar que sea su turno con el ID
            // pos válida

            if ( $this->board[$pos] == 0 ) // pos vacía
            {
                //guardar pos
                $this->board[$pos] = ($this->getPlayer($id) % 2) +1 ;
                return "OK";
            }else{ // error
                return "error";
            }
        }

        public function isWin ()
        {
            // validaciones
        }
    }

    $gato = new Gato();

    if( !empty($_GET["action"]) )
    {
        $action = $_GET["action"];
    }
    else
    {
        $action = 0;
    }

    switch($action)
    {
        case 0: // empty
            echo "empty";
        break;

        case 1:
            $gato->init();
            $gato->loadDb();
            $gato->toString();
        break;

        case 2:
            if( !empty($_GET["id"]) )
            {
                $id = $_GET["id"];
            }
            else
            {
                $id = 0;
            }
            
            $gato->loadDb();
            echo $gato->getStatus($id);
        break;

        case 3: // turn
            if( !empty($_GET["id"]) ) // user
            {
                $id = $_GET["id"];
            }
            else
            {
                $id = 0;
            }
            
            if( !empty($_GET["pos"]) ) // user
            {
                $pos = $_GET["pos"]-1;
            }
            else
            {
                $pos = -1;
            }

            $gato->loadDb();
            echo $gato->turn($id, $pos);
            $gato->saveDb();
        break;

        default:
            echo "No Control";
        break;
    }

?>