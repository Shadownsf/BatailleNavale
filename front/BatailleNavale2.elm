import Browser
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http

--  Main

main = 
    Browser.sandbox { init = init, update = update, view = view}

--  Model
type alias CaseBoard = 
    {
        value : String
    ,   positionX : Int
    ,   positionY : Int
    }

type alias Model = List CaseBoard

init : Model
init = [    
            (CaseBoard "_" 0 0) 
        ,   (CaseBoard "_" 1 0) 
        ,   (CaseBoard "_" 2 0) 
        ,   (CaseBoard "_" 3 0)
        ,   (CaseBoard "_" 4 0) 
        ,   (CaseBoard "_" 5 0) 
        ,   (CaseBoard "_" 6 0) 
        ,   (CaseBoard "_" 7 0) 
        ,   (CaseBoard "_" 8 0) 
        ,   (CaseBoard "_" 9 0) 
        ,   (CaseBoard "_" 0 0) 
        ,   (CaseBoard "_" 1 0) 
        ,   (CaseBoard "_" 2 0)  
        ,   (CaseBoard "_" 3 0)
        ,   (CaseBoard "_" 4 0) 
        ,   (CaseBoard "_" 5 0) 
        ,   (CaseBoard "_" 6 0) 
        ,   (CaseBoard "_" 7 0) 
        ,   (CaseBoard "_" 8 0) 
        ,   (CaseBoard "_" 9 0) 
        ]

--  Update

type Action = NoOp | CheckCase (Int, Int)

update : Action -> Model -> Model
update action model = 
    case action of
        NoOp -> model
        CheckCase (x, y) -> List.map (updateCase (x,y) ) model 

updateCase : (Int, Int) -> CaseBoard -> CaseBoard
updateCase (x, y) bCase = 
    if bCase.positionX == x && bCase.positionY == y then
        { bCase | value = "X"}
    else
        bCase

getCase : Int -> Model -> Maybe CaseBoard
getCase index board = 
    case List.head (getItem index board) of
        Just cBoard -> Just cBoard
        Nothing -> Nothing

getItem : Int -> List CaseBoard -> List CaseBoard
getItem index board =
    case index of
    0 -> board
    _ -> getItem (index + 1) (List.drop 1 board)

--  View
view : List CaseBoard -> Html Action
view model = 
    -- List.map createHtml (buildBoard 10 model)
    (List.concat (buildBoard 10 model))
    

buildBoard : Int -> Model -> List (Html Action)
buildBoard max board = 
    if max > 0 then
        (buildLine max board) ++ (buildBoard (max - 1) board)
    else
        [ div[][] ]

buildLine : Int -> Model -> List (Html Action)
buildLine max board = 
    if max > 0 then
        case getCase (10 - max) board of
            Just a      ->      [ (renderCase a) ] ++ (buildLine (max - 1) board)
            Nothing     ->      [ div[][] ]
    else
        [ div[][] ] 

renderCase : CaseBoard -> Html Action
renderCase caseBoard = 
    div[]
    [
        button[onClick (CheckCase (caseBoard.positionX, caseBoard.positionY))][text caseBoard.value]
    ]