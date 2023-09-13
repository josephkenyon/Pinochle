import '../../App.css'
import React from 'react';
import ReadyBox from './Readybox/ReadyBox';
import { useSelector } from 'react-redux';
import DisplayedCards from './DisplayedCards/DisplayedCards';

export default function Player({ playerStateName }) {
    const name = useSelector((state) => state.playerState[playerStateName].name)
    const showReady = useSelector((state) => state.playerState[playerStateName].showReady)
    const showLastBid = useSelector((state) => state.playerState[playerStateName].showLastBid)
    const lastBid = useSelector((state) => state.playerState[playerStateName].lastBid)
    const displayedCards = useSelector((state) => state.playerState[playerStateName].displayedCards)

    return (
        <div className="vertical-div">
            <span>{name || "Waiting for player"}</span>
            { showReady ? <ReadyBox playerStateName={playerStateName}/> : null }
            { showLastBid ? <span>{"Last bid: " + (lastBid == -1 ? "Passed" : lastBid)}</span> : null }
            { displayedCards ? <DisplayedCards playerStateName={playerStateName}/> : null }
        </div>
    )
}
