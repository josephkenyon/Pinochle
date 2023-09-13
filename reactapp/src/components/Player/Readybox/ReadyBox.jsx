import '../../../App.css'
import React from 'react';
import { useSelector } from 'react-redux';

export default function ReadyBox({ playerStateName }) {
    const isReady = useSelector((state) => state.playerState[playerStateName].isReady)

    return (
<       input
            className="checkbox mt-2"
            type="checkbox"
            readOnly={true}
            checked={isReady || false}/>
    )
}
