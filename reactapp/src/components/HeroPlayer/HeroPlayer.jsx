import '../../App.css'
import React from 'react';
import ReadyBox from './Readybox/ReadyBox';
import BiddingBox from './BiddingBox/BiddingBox';
import TrumpSelectionBox from './TrumpSelectionBox/TrumpSelectionBox';
import Hand from './Hand/Hand';
import { useSelector } from 'react-redux';
import PlayCardButton from './PlayCardButton/PlayCardButton';
import CollectTrickButton from './PlayCardButton/CollectTrickButton';

export default function HeroPlayer() {
    const hasState = useSelector((state) => state.playerState.hasState)
    const showReady = useSelector((state) => state.playerState.showReady)
    const showBiddingBox = useSelector((state) => state.playerState.showBiddingBox)
    const showTrumpSelection = useSelector((state) => state.playerState.showTrumpSelection)
    const showPlayButton = useSelector((state) => state.playerState.showPlayButton)
    const showCollectButton = useSelector((state) => state.playerState.showCollectButton)

    return (
        <div className='player-div'>
            { hasState ?
                <div className="vertical-div">
                    { showReady ? <ReadyBox/> : null }
                    { showBiddingBox ? <BiddingBox/> : null }
                    { showTrumpSelection ? <TrumpSelectionBox/> : null }
                    { <Hand/> }
                    { showPlayButton ? <PlayCardButton/> : null }
                    { showCollectButton ? <CollectTrickButton/> : null }
                </div>
            : null}
        </div>
    )
}
