import '../../App.css'
import React from 'react';
import Player from '../Player/Player';
import HeroPlayer from '../HeroPlayer/HeroPlayer';
import ScoreLog from './GameLog/GameLog';
import Trick from '../HeroPlayer/Trick/Trick';

export default function Game() {

    return (
        <div className='horizontal-div'>
            <div className='score-log-div'>
                <ScoreLog/>
            </div>
            <div className='players-div'>
                <div className='ally-div'>
                    <Player playerStateName={'allyState'} />
                </div>
                <div className="opponents-div">
                    <div className='opponent-div'>
                        <Player playerStateName={'leftOpponentState'} />
                    </div>

                    <Trick/>

                    <div className='opponent-div'>
                        <Player playerStateName={'rightOpponentState'} />
                    </div>
                </div>
                <div className='player-div'>
                    <HeroPlayer/>
                </div>
            </div>
        </div>
    )
}
