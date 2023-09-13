import '../../../App.css'
import React from 'react';
import { useSelector } from 'react-redux';
import ConnectionService from '../../../services/connectionService';

export default function ReadyBox() {
    const isReady = useSelector((state) => state.isReady)

    const declareReady = async () => {
        await ConnectionService.getConnection().invoke("DeclareReady", !isReady)
    }

    return (
        <div className="horizontal-div">
            <label> Ready </label>
            <input
                className="checkbox mt-2 ms-3"
                type="checkbox"
                checked={isReady}
                onChange={() => declareReady()}/>
        </div>
    )
}
