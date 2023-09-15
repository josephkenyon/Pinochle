import '../../../App.css'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import ConnectionService from '../../../services/connectionService';
import NumericInput from 'react-numeric-input';
import { setCurrentBid } from '../../../slices/playerState/playerStateSlice';

export default function BiddingBox() {
    const dispatch = useDispatch()
    const lastBid = useSelector((state) => state.playerState.lastBid)
    const currentBid = useSelector((state) => state.playerState.currentBid)

    let minimumBid = lastBid + 1

    const bid = async (bid) => {
        await ConnectionService.getConnection().invoke("Bid", bid)
    }

    return (
        <div className="vertical-div bidding-div">
            <div className='horizontal-div mb-3 numeric-input-div'>
                <div className='me-5 enter-bid-div'> Enter bid: </div>
                <NumericInput min={minimumBid} max={100} value={currentBid} onChange={value => dispatch(setCurrentBid(value))}/>
            </div>
            
            <div className='horizontal-div'>
                <button className="button" value="test" onClick={() => bid(-1)}>
                    Pass
                </button>

                <button className="button ms-5" value="test" onClick={() => bid(currentBid)}>
                    Submit
                </button>
            </div>
        </div>
    )
}
