import '../../../App.css'
import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import ConnectionService from '../../../services/connectionService';
import NumericInput from 'react-numeric-input';

export default function BiddingBox() {
    const currentBid = useSelector((state) => state.playerState.currentBid)
    const minimumBid = currentBid + 1

    const [myBid, setMyBid] = useState(minimumBid);

    const bid = async (bid) => {
        await ConnectionService.getConnection().invoke("Bid", bid)
    }

    return (
        <div className="vertical-div bidding-div">
            <div className='horizontal-div mb-3 numeric-input-div'>
                <div className='me-5 enter-bid-div'> Enter bid: </div>
                <NumericInput min={minimumBid} max={100} value={myBid} onChange={value => setMyBid(value)}/>
            </div>
            
            <div className='horizontal-div'>
                <button className="button" value="test" onClick={() => bid(-1)}>
                    Pass
                </button>

                <button className="button ms-5" value="test" onClick={() => bid(myBid)}>
                    Submit
                </button>
            </div>
        </div>
    )
}
