import '../../App.css'
import React from 'react';

const suits = ["♠︎", "♥︎", "♣︎", "♦︎"]
const ranks = ["A", "10", "K", "Q", "J", "9"]

export default function Card({suitIndex, rankIndex, zIndex, small, selected, onClick}) {
    const suit = suits[suitIndex]
    const rank = ranks[rankIndex]

    const colorClass = (suit == "♣︎" || suit == "♠︎") ? "card-black" : "card-red"

    const smallClassClause = small ? "small-card" : "large-card"
    const leftMarginClause = (zIndex > 0) ? (small ? "card-left-margin-small" : "card-left-margin-large") : ""
    const topMarginClause = selected ? (small ? "card-top-margin-small" : "card-top-margin-large") : ""

    return (
        <div className={["card", smallClassClause, colorClass, leftMarginClause, topMarginClause].join(" ")} style={{zIndex: zIndex}} onClick={() => onClick ? onClick() : null}>
            <div className="card-tl">
                <div className={(small ? "card-value-small" : "card-value-large")}>{rank}</div>
                <div className={(small ? "card-suit-small" : "card-value-large")}>{suit}</div>
            </div>
            <div className="card-br">
                <div className={(small ? "card-value-small" : "card-value-large")}>{rank}</div>
                <div className={(small ? "card-suit-small" : "card-value-large")}>{suit}</div>
            </div>
        </div>
    )
}
