import '../../App.css'
import React from 'react';
import Game from '../Game/Game';
import Entry from '../Entry/Entry';
import { useSelector } from 'react-redux';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function App() {
    const loading = useSelector((state) => state.appState.loading)
    const connection = useSelector((state) => state.appState.connection)
    const hasState = useSelector((state) => state.playerState.hasState)

    const displayLoading = loading || (connection && !hasState)

    return (
        <div className="vertical-div full-screen-div">
            <ToastContainer
                position="top-left"
                autoClose={5000}
                hideProgressBar
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable={false}
                pauseOnHover={false}
                theme="colored"
            />
            { displayLoading
                    ? <p style={{fontSize: 50}}>Loading...</p>
                    : connection
                        ? <Game/>
                        : <Entry/>
            }
        </div>
    )
}
