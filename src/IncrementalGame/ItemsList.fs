module IncrementalGame.ItemsList


type StatType = | Health | Attack | Defense
(*
0th layer, additive to base stats
-1st layer, always multiplicative
otherwise additive within the same layer, layers are then multiplied together
*)
type ItemStat = {Type: StatType; Layer: int;  Value: float}
type Item =
    { Name: string
      Stats: ItemStat list
      id: int }


let itemsInGame = [
    { Name= "Basic Sword"; Stats = [{Type = Defense; Layer = 0; Value = 1.0 }]; id = 1 }
    { Name= "Basic Shield"; Stats = [{Type = Attack; Layer = 0; Value = 1.0 }]; id = 2 }
]