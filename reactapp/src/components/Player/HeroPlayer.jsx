import '../../App.css'
import React from 'react';
import ReadyBox from './Readybox/ReadyBox';
import BiddingBox from './BiddingBox/BiddingBox';
import TrumpSelectionBox from './TrumpSelectionBox/TrumpSelectionBox';
import Hand from './Hand/Hand';
import { useSelector } from 'react-redux';

export default function HeroPlayer() {
    const hasInitialState = useSelector((state) => state.playerState.hasInitialState)
    const showReady = useSelector((state) => state.playerState.showReady)
    const showBiddingBox = useSelector((state) => state.playerState.showBiddingBox)
    const showTrumpSelection = useSelector((state) => state.playerState.showTrumpSelection)

    return (
        <div className='player-div'>
            { hasInitialState ?
                <div className="vertical-div">
                    { showReady ? <ReadyBox/> : null }
                    { showBiddingBox ? <BiddingBox/> : null }
                    { showTrumpSelection ? <TrumpSelectionBox/> : null }
                    { <Hand/> }
                </div>
            : null}
        </div>
    )
}
