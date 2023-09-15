import '../../../App.css'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setShowLog } from '../../../slices/appState/appStateSlice';

export default function GameScoreHeader() {
    const dispatch = useDispatch()

    const showGameLog = useSelector(state => state.appState.showLog)
    const hideGameLog = !showGameLog
    const teamOneName = useSelector(state => state.playerState.teamOneName)
    const teamTwoName = useSelector(state => state.playerState.teamTwoName)
    const teamOneScoreLog = useSelector(state => state.playerState.teamOneScoreLog) //[0,1,2,3,4,5]
    const teamTwoScoreLog = useSelector(state => state.playerState.teamTwoScoreLog) //[0,1,2,3,4,5]

    let scoreSum1 = 0
    teamOneScoreLog.forEach(score => scoreSum1 += +score)

    let scoreSum2 = 0
    teamTwoScoreLog.forEach(score => scoreSum2 += +score)


    return (
        <div className="score-header-div horizontal-div">
            <div className="show-log-button-div">
                {hideGameLog
                    ? <button onClick={() => dispatch(setShowLog(true))}>
                        Show Score Sheet {">>"}
                    </button> : null}
            </div>
            <div className="game-score-div horizontal-div">
                <div className='team-name-div blue-team-div vertical-div'>
                    {`${teamOneName}:`}
                </div>

                <div className='score-sum-div me-5'>
                    {`${scoreSum1}`}
                </div>

                <div className='ms-5 team-name-div green-team-div'>
                    {`${teamTwoName}:`}
                </div>
                <div className='score-sum-div me-4'>
                    {`${scoreSum2}`}
                </div>
            </div>
        </div>
    )
}
