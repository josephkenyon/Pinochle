import '../../App.css'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setConnection, setGameName, setPlayerName } from '../../slices/appState/appStateSlice';
import { setAllyState, setCurrentBid, setDisplayedCards, setHand, setHasState, setHighlightPlayer, setIsReady, setLastBid, setLeftOpponentState, setRightOpponentState, setRoundBidResults, setShowBiddingBox,
    setShowCollectButton,
    setShowLastBid, setShowPlayButton, setShowReady, setShowSwapPosition, setShowTricksTaken, setShowTrumpIndicator, setShowTrumpSelection, setTeamIndex, setTeamOneName, setTeamOneScoreLog, setTeamOneTricksTaken, setTeamTwoName, setTeamTwoScoreLog, setTeamTwoTricksTaken, setTrickState } from '../../slices/playerState/playerStateSlice';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import ConnectionService from '../../services/connectionService';
import { toast } from 'react-toastify';

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
            console.error(message)
        })

        connection.on("SendMessage", (messageObject) => {
            const { content, code } = messageObject
            sendMessage(content, code)
        })

        connection.on("UpdatePlayerState", (newState) => {
            console.debug("recieving player state update...")
            console.debug(newState)

            dispatch(setTeamOneName(newState.teamOneScoreList.shift()))
            dispatch(setTeamTwoName(newState.teamTwoScoreList.shift()))
            dispatch(setTeamOneScoreLog(newState.teamOneScoreList))
            dispatch(setTeamTwoScoreLog(newState.teamTwoScoreList))
            dispatch(setRoundBidResults(newState.roundBidResults))
            dispatch(setTeamIndex(newState.teamIndex))
            dispatch(setLastBid(newState.lastBid))
            dispatch(setCurrentBid(newState.currentBid))
            dispatch(setHighlightPlayer(newState.highlightPlayer))
            dispatch(setIsReady(newState.isReady))
            dispatch(setHasState(true))
            dispatch(setShowReady(newState.showReady))
            dispatch(setShowSwapPosition(newState.showSwapPosition))
            dispatch(setShowLastBid(newState.showLastBid))
            dispatch(setShowBiddingBox(newState.showBiddingBox))
            dispatch(setShowTrumpSelection(newState.showTrumpSelection))
            dispatch(setShowTrumpIndicator(newState.showTrumpIndicator))
            dispatch(setShowTricksTaken(newState.showTricksTaken))
            dispatch(setTeamOneTricksTaken(newState.teamOneTricksTaken))
            dispatch(setTeamTwoTricksTaken(newState.teamTwoTricksTaken))
            dispatch(setShowPlayButton(newState.showPlayButton))
            dispatch(setShowCollectButton(newState.showCollectButton))
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

    function sendMessage(message, code) {
        const className = code == 0 ? 'blue-team-div' : code == 1 ? 'green-team-div' : 'error-div'

        const props = {
            position: "top-left",
            autoClose: 3000,
            hideProgressBar: true,
            closeOnClick: true,
            pauseOnHover: false,
            draggable: false,
            className: className + ' message-div',
            icon: '',
            progress: undefined,
            theme: "colored"
        }

        if (code == 0) {
            toast.info(message, props);
        } else if (code == 1) {
            toast.success(message, props);
        } else if (code == 2) {
            toast.error(message, props);
        }
    }

    return (
        <div className="vertical-div entry-div">
            <h1 className="mb-5" id="tabelLabel">Pinochle</h1>
            <input className="entry-input" type="text" value={gameName} placeholder="Enter a game name"
                onChange={event => dispatch(setGameName(event.target.value.toUpperCase()))}/>

            <input className="entry-input" type="text" value={playerName} placeholder="Enter your player name"
                onChange={event => dispatch(setPlayerName(event.target.value))}/>

            <button className="entry-button" onClick={() => joinGame()}>
                Join Game
            </button>
        </div>
    );
}

