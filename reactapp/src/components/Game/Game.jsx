import '../../App.css'
import React from 'react';
import Player from '../Player/Player';
import HeroPlayer from '../HeroPlayer/HeroPlayer';

export default function Game() {

    return (
        <div className='players-div'>
            <div className='ally-div'>
                <Player playerStateName={'allyState'} />
            </div>
            <div className="opponents-div">
                <div className='opponent-div'>
                    <Player playerStateName={'leftOpponentState'} />
                </div>

                <div className='opponent-spacer'/>

                <div className='opponent-div'>
                    <Player playerStateName={'rightOpponentState'} />
                </div>
            </div>
            <div className='player-div'>
                <HeroPlayer/>
            </div>
        </div>
    )
}
