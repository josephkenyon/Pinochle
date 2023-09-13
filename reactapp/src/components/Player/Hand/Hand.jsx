import '../../../App.css'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectCard } from '../../../slices/playerState/playerStateSlice';
import Card from '../../Card/Card';

export default function Hand() {
    const dispatch = useDispatch()

    const hand = useSelector((state) => state.playerState.hand)

    let index = 0;
    return (
        <div className="horizontal-div hand-div">
            {hand.map(card => {
                return <Card key={index} suit={card.suit} rank={card.rank} zIndex={index++} small={false} selected={card.selected} onClick={() => dispatch(selectCard(card.id))}/>
            })}
        </div>
    )
}
