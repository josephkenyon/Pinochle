import '../../App.css'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setConnection, setErrorMessage, setGameName, setPlayerName } from '../../slices/appState/appStateSlice';
import { setAllyState, setCurrentBid, setDisplayedCards, setHand, setHasState, setIsReady, setLastBid, setLeftOpponentState, setRightOpponentState, setRoundBidResults, setShowBiddingBox,
    setShowCollectButton,
    setShowLastBid, setShowPlayButton, setShowReady, setShowSwapPlayerIndex, setShowTrumpSelection, setTeamIndex, setTeamOneName, setTeamOneScoreLog, setTeamTwoName, setTeamTwoScoreLog, setTrickState } from '../../slices/playerState/playerStateSlice';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import ConnectionService from '../../services/connectionService';

export default function Entry() {
    const dispatch = useDispatch()
  
    const gameName = useSelector((state) => state.appState.gameName)
    const playerName = useSelector((state) => state.appState.playerName)

    const joinGame = async () => {
        const connection = new HubConnectionBuilder()
            .withUrl("https://73.216.200.18:7177/game")
            .configureLogging(LogLevel.Information)
            .build();

        connection.on("ErrorMessage", (message) => {
            dispatch(setErrorMessage(message))
        })

        connection.on("UpdatePlayerState", (newState) => {
            console.debug("recieving player state update...")
            console.debug(newState)

            dispatch(setErrorMessage(''))
            dispatch(setTeamOneName(newState.teamOneScoreList.shift()))
            dispatch(setTeamTwoName(newState.teamTwoScoreList.shift()))
            dispatch(setTeamOneScoreLog(newState.teamOneScoreList))
            dispatch(setTeamTwoScoreLog(newState.teamTwoScoreList))
            dispatch(setRoundBidResults(newState.roundBidResults))
            dispatch(setTeamIndex(newState.teamIndex))
            dispatch(setLastBid(newState.lastBid))
            dispatch(setCurrentBid(newState.currentBid))
            dispatch(setIsReady(newState.isReady))
            dispatch(setHasState(true))
            dispatch(setShowReady(newState.showReady))
            dispatch(setShowLastBid(newState.showLastBid))
            dispatch(setShowBiddingBox(newState.showBiddingBox))
            dispatch(setShowTrumpSelection(newState.showTrumpSelection))
            dispatch(setShowPlayButton(newState.showPlayButton))
            dispatch(setShowCollectButton(newState.showCollectButton))
            dispatch(setShowSwapPlayerIndex(newState.showSwapPlayerIndex))
            dispatch(setHand(newState.hand))
            dispatch(setDisplayedCards(newState.displayedCards))
            dispatch(setTrickState(newState.trickState))
            dispatch(setAllyState(newState.allyState))
            dispatch(setLeftOpponentState(newState.leftOpponentState))
            dispatch(setRightOpponentState(newState.rightOpponentState))
        })

        connection.onclose(_ => {
            ConnectionService.setConnection(null)
            dispatch(setConnection(false))
            dispatch(setHasState(false))
        })

        try {
            await connection.start()
        } catch (e) {
            this.errorMessage("Unable to connect to the server.")
            return;
        }

        ConnectionService.setConnection(connection)
        dispatch(setConnection(true))

        try {
            await connection.invoke("JoinGame", { GameName: gameName, PlayerName: playerName })
        } catch (e) {
            this.errorMessage("An error occurred trying to join the game.")
        }
    }


    return (
        <div className="vertical-div entry-div">
            <h1 className="mb-5" id="tabelLabel">Pinochle</h1>
            <input className="entry-input" type="text" value={gameName} placeholder="Enter a game name"
                onChange={event => dispatch(setGameName(event.target.value.toUpperCase()))}/>

            <input className="entry-input" type="text" value={playerName} placeholder="Enter your player name"
                onChange={event => dispatch(setPlayerName(event.target.value))}/>

            <button className="entry-button" value="test" onClick={() => joinGame()}>
                Join Game
            </button>
        </div>
    );
}

