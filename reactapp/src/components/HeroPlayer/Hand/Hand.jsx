import '../../../App.css'
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { selectCard } from '../../../slices/playerState/playerStateSlice';
import Card from '../../Card/Card';
import ConnectionService from '../../../services/connectionService';

export default function Hand() {
    const dispatch = useDispatch()

    const hand = useSelector((state) => state.playerState.hand)

    const onSelectCard = async (id) => {
        console.log(hand.length)
        if (hand.length == 1) {
            await ConnectionService.getConnection().invoke("PlayCard", -1)
        } else {
            dispatch(selectCard(id))
        }
    }

    let index = 0;
    return (
        <div className="horizontal-div hero-hand-div">
            {hand.map(card => {
                return <Card key={index} suitIndex={card.suit} rankIndex={card.rank} zIndex={index++}
                    small={false} selected={hand.length > 1 && card.selected} onClick={() => onSelectCard(card.id)}/>
            })}
        </div>
    )
}
