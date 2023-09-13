import '../../App.css'
import React from 'react';
import Game from '../Game/Game';
import Entry from '../Entry/Entry';
import { useSelector } from 'react-redux';

export default function App() {
    const loading = useSelector((state) => state.appState.loading)
    const connection = useSelector((state) => state.appState.connection)
    const errorMessage = useSelector((state) => state.appState.errorMessage)
    const hasState = useSelector((state) => state.playerState.hasState)

    const displayLoading = loading || (connection && !hasState)

    return (
        <div className="vertical-div">
            {
                displayLoading
                    ? <p><em>Loading...</em></p>
                    : connection
                        ? <Game/>
                        : <Entry/>
            }

            {
                errorMessage
                    ? <div className="alert alert-danger" role="error">{errorMessage}</div>
                    : null
            }
        </div>
    )
}
