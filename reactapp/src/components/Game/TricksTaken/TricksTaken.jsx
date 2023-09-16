import '../../../App.css'
import React from 'react';
import { useSelector } from 'react-redux';

export default function TricksTaken() {
    const teamOneTricksTaken = useSelector(state => state.playerState.teamOneTricksTaken)
    const teamTwoTricksTaken = useSelector(state => state.playerState.teamTwoTricksTaken)

    return (
        <div className="game-score-div horizontal-div">
            <div className='team-name-div blue-team-div vertical-div'>
                {`Tricks won:`}
            </div>

            <div className='score-sum-div me-5'>
                {`${teamOneTricksTaken}`}
            </div>

            <div className='ms-5 team-name-div green-team-div'>
                {`Tricks won:`}
            </div>
            <div className='score-sum-div me-4'>
                {`${teamTwoTricksTaken}`}
            </div>
        </div>
    )
}
