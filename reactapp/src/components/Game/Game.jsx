import { useSelector } from 'react-redux';
import '../../App.css'
import React from 'react';
import HeroPlayer from '../Player/HeroPlayer';

export default function Game() {
    const allyState = useSelector((state) => state.playerState.allyState)
    const leftOpponentState = useSelector((state) => state.playerState.leftOpponentState)
    const rightOpponentState = useSelector((state) => state.playerState.rightOpponentState)

    return (
        <div className='players-div'>
            <div className='ally-div'>
                {renderPlayerState(allyState)}
            </div>
            <div className="opponents-div">
                <div className='opponent-div'>
                    {renderPlayerState(leftOpponentState)}
                </div>

                <div className='opponent-spacer'/>

                <div className='opponent-div'>
                    {renderPlayerState(rightOpponentState)}
                </div>
            </div>
            <div className='player-div'>
                <HeroPlayer/>
            </div>
        </div>
    )
}

function renderPlayerState(playerState) {
    return playerState ?
        <div className="vertical-div">
            <span>{playerState.name || "Waiting for player"}</span>
            {renderCheckBox(playerState)}
            {lastBid(playerState)}
        </div>
    : null
}

function renderCheckBox(playerState) {
    return playerState.showReady
        ? <input
            className="checkbox mt-2"
            type="checkbox"
            readOnly={true}
            checked={playerState.isReady || false}/>
        : null
}

function lastBid(playerState) {
    return playerState.showLastBid
        ? <span>{"Last bid: " + (playerState.lastBid == -1 ? "Passed" : playerState.lastBid)}</span>

        : null
}
