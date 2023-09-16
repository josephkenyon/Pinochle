import '../../App.css'
import React from 'react';
import ReadyBox from './Readybox/ReadyBox';
import { useSelector } from 'react-redux';
import DisplayedCards from './DisplayedCards/DisplayedCards';

export default function Player({ playerStateName, ally }) {
    const name = useSelector((state) => state.playerState[playerStateName].name)
    const showReady = useSelector((state) => state.playerState[playerStateName].showReady)
    const showLastBid = useSelector((state) => state.playerState[playerStateName].showLastBid)
    const lastBid = useSelector((state) => state.playerState[playerStateName].lastBid)
    const displayedCards = useSelector((state) => state.playerState[playerStateName].displayedCards)
    const teamIndex = useSelector((state) => state.playerState.teamIndex)
    const highlightPlayer = useSelector((state) => state.playerState[playerStateName].highlightPlayer)

    const highlightClassName = highlightPlayer ? 'highlight-player' : ''

    let thisPlayerTeamIndex = 0;
    if ((teamIndex == 1 && ally) || (teamIndex == 0 && !ally)) {
        thisPlayerTeamIndex = 1;
    }

    return (
        <div className="vertical-div">
            <div className='horizontal-div'>
                <div className={highlightClassName + " player-name-div " + ((thisPlayerTeamIndex == 0) ? 'blue-team-div' : 'green-team-div')}>
                    {name || "Waiting for player"}
                </div>
                { showReady ? <ReadyBox playerStateName={playerStateName}/> : null }
                { showLastBid ? <div className="last-bid-div">{"Last bid: " + (lastBid == -1 ? "Passed" : lastBid)}</div> : null }
            </div>
            { displayedCards ? <DisplayedCards playerStateName={playerStateName}/> : null }
        </div>
    )
}
